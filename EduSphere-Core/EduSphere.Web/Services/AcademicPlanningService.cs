using System.Text.Json;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AcademicPlanningService(TenantDbContext db) : IAcademicPlanningService
{
    public async Task<TimetableGenerationResult> GenerateTimetableAsync(TimetableGenerationRequest request, CancellationToken cancellationToken = default)
    {
        var started = DateTime.UtcNow;
        var run = new TimetableGenerationRun
        {
            BranchId = request.BranchId, AcademicYearId = request.AcademicYearId, SectionId = request.SectionId,
            TimetableTemplateId = request.TemplateId, Status = TimetableGenerationStatus.Draft, StartedOn = started
        };
        db.TimetableGenerationRuns.Add(run);

        var template = await db.TimetableTemplates.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == request.TemplateId, cancellationToken);
        var section = await db.Sections.Include(x => x.Batch).SingleOrDefaultAsync(x => x.Id == request.SectionId, cancellationToken);
        var yearExists = await db.AcademicYears.AnyAsync(x => x.Id == request.AcademicYearId, cancellationToken);
        if (template is null || template.BranchId != request.BranchId || section is null || !yearExists)
            return await FailAsync(run, "The branch, academic year, section or timetable template is invalid.", cancellationToken);

        var slots = await db.TimeSlots.Where(x => x.BranchId == request.BranchId && !x.IsBreak)
            .OrderBy(x => x.DayOfWeek).ThenBy(x => x.PeriodNumber).ToListAsync(cancellationToken);
        var rooms = await db.Rooms.Where(x => x.BranchId == request.BranchId && x.IsActive && x.Capacity >= section.Capacity)
            .OrderBy(x => x.Capacity).ToListAsync(cancellationToken);
        var assignments = await db.TeacherSubjectAssignments.Include(x => x.TeacherProfile)
            .Where(x => x.TeacherProfile != null && x.TeacherProfile.BranchId == request.BranchId &&
                        (!x.SectionId.HasValue || x.SectionId == request.SectionId) &&
                        (!x.EffectiveFrom.HasValue || x.EffectiveFrom <= request.EffectiveFrom) &&
                        (!x.EffectiveUntil.HasValue || x.EffectiveUntil >= request.EffectiveFrom))
            .OrderByDescending(x => x.SectionId.HasValue).ThenByDescending(x => x.IsPrimary).ToListAsync(cancellationToken);
        var availability = await db.TeacherAvailabilityRules.Where(x => x.BranchId == request.BranchId &&
            (!x.EffectiveFrom.HasValue || x.EffectiveFrom <= request.EffectiveFrom) &&
            (!x.EffectiveTo.HasValue || x.EffectiveTo >= request.EffectiveFrom)).ToListAsync(cancellationToken);
        var occupied = await db.TimetableEntries.Include(x => x.Timetable).Include(x => x.TimeSlot)
            .Where(x => x.Timetable != null && x.Timetable.BranchId == request.BranchId && x.Timetable.Status != TimetableStatus.Archived)
            .ToListAsync(cancellationToken);

        var timetable = new Timetable
        {
            BranchId = request.BranchId, AcademicYearId = request.AcademicYearId, SectionId = request.SectionId,
            Name = request.Name.Trim(), EffectiveFrom = request.EffectiveFrom, EffectiveTo = request.EffectiveTo, Status = TimetableStatus.Draft
        };
        db.Timetables.Add(timetable);
        var diagnostics = new List<string>();
        var generated = new List<TimetableEntry>();
        var dailySubjectCounts = new Dictionary<(Guid SubjectId, DayOfWeek Day), int>();

        foreach (var item in template.Items.OrderByDescending(x => x.PeriodsPerWeek))
        {
            for (var occurrence = 0; occurrence < item.PeriodsPerWeek; occurrence++)
            {
                var teacher = assignments.FirstOrDefault(x => x.SubjectId == item.SubjectId &&
                    slots.Any(slot => CanPlace(slot, x.TeacherProfileId, item, rooms, generated, occupied, availability, timetable, dailySubjectCounts)))?.TeacherProfile;
                if (teacher is null)
                {
                    diagnostics.Add($"No available assigned teacher/slot for subject {item.SubjectId} occurrence {occurrence + 1}.");
                    continue;
                }

                var placement = slots.Select(slot => new
                    {
                        Slot = slot,
                        Room = SelectRoom(item, rooms, slot, teacher.Id, generated, occupied, timetable),
                        Preferred = availability.Any(a => a.TeacherProfileId == teacher.Id && a.DayOfWeek == slot.DayOfWeek && a.RuleType == AvailabilityRuleType.Preferred && Overlaps(a.StartsAt, a.EndsAt, slot.StartsAt, slot.EndsAt))
                    })
                    .Where(x => x.Room is not null && CanPlace(x.Slot, teacher.Id, item, rooms, generated, occupied, availability, timetable, dailySubjectCounts))
                    .OrderByDescending(x => x.Preferred).ThenBy(x => dailySubjectCounts.GetValueOrDefault((item.SubjectId, x.Slot.DayOfWeek)))
                    .ThenBy(x => x.Slot.DayOfWeek).ThenBy(x => x.Slot.PeriodNumber).FirstOrDefault();
                if (placement is null)
                {
                    diagnostics.Add($"Constraints prevented placement for subject {item.SubjectId} occurrence {occurrence + 1}.");
                    continue;
                }

                generated.Add(new TimetableEntry { Timetable = timetable, SectionId = request.SectionId, SubjectId = item.SubjectId, TeacherProfileId = teacher.Id, TimeSlotId = placement.Slot.Id, RoomId = placement.Room!.Id, Notes = "Generated from timetable template" });
                dailySubjectCounts[(item.SubjectId, placement.Slot.DayOfWeek)] = dailySubjectCounts.GetValueOrDefault((item.SubjectId, placement.Slot.DayOfWeek)) + 1;
            }
        }

        var required = template.Items.Sum(x => x.PeriodsPerWeek);
        run.Status = TimetableGenerationStatus.Generated;
        run.CompletedOn = DateTime.UtcNow;
        run.ScheduledPeriods = generated.Count;
        run.UnscheduledPeriods = required - generated.Count;
        run.Diagnostics = JsonSerializer.Serialize(diagnostics);
        run.GeneratedTimetable = timetable;
        db.TimetableEntries.AddRange(generated);
        await db.SaveChangesAsync(cancellationToken);
        return new(true, run.Id, timetable.Id, run.ScheduledPeriods, run.UnscheduledPeriods, diagnostics);
    }

    public async Task<AcademicRolloverResult> RolloverAsync(AcademicRolloverRequest request, CancellationToken cancellationToken = default)
    {
        var source = await db.AcademicYears.SingleAsync(x => x.Id == request.SourceAcademicYearId, cancellationToken);
        var target = await db.AcademicYears.SingleAsync(x => x.Id == request.TargetAcademicYearId, cancellationToken);
        var offset = target.StartDate.DayNumber - source.StartDate.DayNumber;
        var terms = await db.AcademicTerms.Where(x => x.AcademicYearId == source.Id).OrderBy(x => x.Sequence).ToListAsync(cancellationToken);
        var existingSequences = await db.AcademicTerms.Where(x => x.AcademicYearId == target.Id).Select(x => x.Sequence).ToListAsync(cancellationToken);
        var termMap = new Dictionary<Guid, AcademicTerm>();
        foreach (var term in terms.Where(x => !existingSequences.Contains(x.Sequence)))
        {
            var copy = new AcademicTerm { AcademicYearId = target.Id, Name = term.Name, Type = term.Type, Sequence = term.Sequence, StartDate = term.StartDate.AddDays(offset), EndDate = term.EndDate.AddDays(offset) };
            db.AcademicTerms.Add(copy); termMap[term.Id] = copy;
        }
        await db.SaveChangesAsync(cancellationToken);

        var eventCount = 0;
        if (request.CopyCalendar)
        {
            var events = await db.AcademicCalendarEvents.Where(x => x.AcademicYearId == source.Id).ToListAsync(cancellationToken);
            foreach (var item in events)
            {
                db.AcademicCalendarEvents.Add(new AcademicCalendarEvent { AcademicYearId = target.Id, BranchId = item.BranchId, AcademicTermId = item.AcademicTermId.HasValue && termMap.TryGetValue(item.AcademicTermId.Value, out var mapped) ? mapped.Id : null, Title = item.Title, Description = item.Description, EventType = item.EventType, StartsOn = item.StartsOn.AddDays(offset), EndsOn = item.EndsOn.AddDays(offset), IsInstructionalDay = item.IsInstructionalDay });
                eventCount++;
            }
        }

        var templateCount = 0;
        if (request.CopyTemplates)
        {
            var templates = await db.TimetableTemplates.Include(x => x.Items).ToListAsync(cancellationToken);
            foreach (var template in templates)
            {
                var name = $"{template.Name} - {target.Name}";
                if (await db.TimetableTemplates.AnyAsync(x => x.BranchId == template.BranchId && x.Name == name, cancellationToken)) continue;
                db.TimetableTemplates.Add(new TimetableTemplate { BranchId = template.BranchId, Name = name, Description = $"Rolled over from {source.Name}", IsActive = template.IsActive, Items = template.Items.Select(x => new TimetableTemplateItem { SubjectId = x.SubjectId, PeriodsPerWeek = x.PeriodsPerWeek, MaximumPeriodsPerDay = x.MaximumPeriodsPerDay, PreferredRoomId = x.PreferredRoomId, RequireConsecutivePeriods = x.RequireConsecutivePeriods }).ToList() });
                templateCount++;
            }
        }
        await db.SaveChangesAsync(cancellationToken);
        return new(termMap.Count, eventCount, templateCount);
    }

    private async Task<TimetableGenerationResult> FailAsync(TimetableGenerationRun run, string reason, CancellationToken cancellationToken)
    {
        run.Status = TimetableGenerationStatus.Failed; run.CompletedOn = DateTime.UtcNow; run.Diagnostics = JsonSerializer.Serialize(new[] { reason });
        await db.SaveChangesAsync(cancellationToken);
        return new(false, run.Id, null, 0, 0, new[] { reason });
    }

    private static bool CanPlace(TimeSlot slot, Guid teacherId, TimetableTemplateItem item, IReadOnlyList<Room> rooms, IReadOnlyList<TimetableEntry> generated, IReadOnlyList<TimetableEntry> occupied, IReadOnlyList<TeacherAvailabilityRule> availability, Timetable timetable, IReadOnlyDictionary<(Guid, DayOfWeek), int> dailyCounts)
    {
        if (dailyCounts.GetValueOrDefault((item.SubjectId, slot.DayOfWeek)) >= Math.Max(1, item.MaximumPeriodsPerDay)) return false;
        if (availability.Any(a => a.TeacherProfileId == teacherId && a.DayOfWeek == slot.DayOfWeek && a.RuleType == AvailabilityRuleType.Unavailable && Overlaps(a.StartsAt, a.EndsAt, slot.StartsAt, slot.EndsAt))) return false;
        if (generated.Any(x => x.TimeSlotId == slot.Id || (x.TeacherProfileId == teacherId && x.TimeSlotId == slot.Id))) return false;
        if (occupied.Any(x => x.TeacherProfileId == teacherId && x.TimeSlotId == slot.Id && DateRangesOverlap(x.Timetable!, timetable))) return false;
        return rooms.Count > 0;
    }

    private static Room? SelectRoom(TimetableTemplateItem item, IReadOnlyList<Room> rooms, TimeSlot slot, Guid teacherId, IReadOnlyList<TimetableEntry> generated, IReadOnlyList<TimetableEntry> occupied, Timetable timetable)
        => rooms.OrderByDescending(x => x.Id == item.PreferredRoomId).FirstOrDefault(room =>
            !generated.Any(x => x.RoomId == room.Id && x.TimeSlotId == slot.Id) &&
            !occupied.Any(x => x.RoomId == room.Id && x.TimeSlotId == slot.Id && DateRangesOverlap(x.Timetable!, timetable)));

    private static bool Overlaps(TimeOnly aStart, TimeOnly aEnd, TimeOnly bStart, TimeOnly bEnd) => aStart < bEnd && bStart < aEnd;
    private static bool DateRangesOverlap(Timetable left, Timetable right) => left.EffectiveFrom <= (right.EffectiveTo ?? DateOnly.MaxValue) && right.EffectiveFrom <= (left.EffectiveTo ?? DateOnly.MaxValue);
}
