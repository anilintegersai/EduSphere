using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class AcademicPlanningTests
{
    private static (TenantDbContext Db, TenantContext Tenant) CreateDb()
    {
        var tenant = new TenantContext(); tenant.SetTenant(Guid.NewGuid(), "academic-planning-test");
        return (new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new TestUser()), tenant);
    }

    [Fact]
    public async Task GeneratorHonorsUnavailableTeacherWindowAndBuildsDraft()
    {
        var (db, _) = CreateDb(); await using var scope = db;
        var branch = new Branch { Name = "Central Campus", Code = "CC" };
        var year = new AcademicYear { Name = "2026-27", StartDate = new(2026, 6, 1), EndDate = new(2027, 5, 31) };
        var department = new Department { Code = "SCI", Name = "Science" };
        db.AddRange(branch, year, department); await db.SaveChangesAsync();
        var course = new Course { Code = "BSC", Name = "BSc", DepartmentId = department.Id };
        db.Add(course); await db.SaveChangesAsync();
        var batch = new Batch { Name = "BSc 2026", CourseId = course.Id, AcademicYearId = year.Id, Capacity = 30 };
        var subject = new Subject { Code = "PHY101", Name = "Physics", CourseId = course.Id };
        var teacher = new TeacherProfile { BranchId = branch.Id, EmployeeNumber = "T-1", FirstName = "Anita", LastName = "Rao" };
        db.AddRange(batch, subject, teacher); await db.SaveChangesAsync();
        var section = new Section { Name = "A", BatchId = batch.Id, Capacity = 25 };
        var monday = new TimeSlot { BranchId = branch.Id, Name = "Monday P1", DayOfWeek = DayOfWeek.Monday, PeriodNumber = 1, StartsAt = new(9, 0), EndsAt = new(10, 0) };
        var tuesday = new TimeSlot { BranchId = branch.Id, Name = "Tuesday P1", DayOfWeek = DayOfWeek.Tuesday, PeriodNumber = 1, StartsAt = new(9, 0), EndsAt = new(10, 0) };
        var room = new Room { BranchId = branch.Id, Code = "R1", Name = "Room 1", Capacity = 30 };
        db.AddRange(section, monday, tuesday, room); await db.SaveChangesAsync();
        var template = new TimetableTemplate { BranchId = branch.Id, Name = "Standard", Items = [new TimetableTemplateItem { SubjectId = subject.Id, PeriodsPerWeek = 1, MaximumPeriodsPerDay = 1 }] };
        db.AddRange(template, new TeacherSubjectAssignment { TeacherProfileId = teacher.Id, SubjectId = subject.Id, SectionId = section.Id, IsPrimary = true }, new TeacherAvailabilityRule { BranchId = branch.Id, TeacherProfileId = teacher.Id, DayOfWeek = DayOfWeek.Monday, StartsAt = new(8, 0), EndsAt = new(12, 0), RuleType = AvailabilityRuleType.Unavailable });
        await db.SaveChangesAsync();

        var service = new AcademicPlanningService(db);
        var result = await service.GenerateTimetableAsync(new TimetableGenerationRequest(branch.Id, year.Id, section.Id, template.Id, "Generated", new(2026, 6, 1), null));

        Assert.True(result.Succeeded); Assert.Equal(1, result.ScheduledPeriods); Assert.Equal(0, result.UnscheduledPeriods);
        var entry = await db.TimetableEntries.Include(x => x.TimeSlot).SingleAsync();
        Assert.Equal(DayOfWeek.Tuesday, entry.TimeSlot!.DayOfWeek);
        Assert.Equal(TimetableStatus.Draft, (await db.Timetables.SingleAsync()).Status);
    }

    [Fact]
    public async Task RolloverShiftsTermsAndCalendarIntoTargetYear()
    {
        var (db, _) = CreateDb(); await using var scope = db;
        var source = new AcademicYear { Name = "2025-26", StartDate = new(2025, 6, 1), EndDate = new(2026, 5, 31) };
        var target = new AcademicYear { Name = "2026-27", StartDate = new(2026, 6, 1), EndDate = new(2027, 5, 31) };
        db.AddRange(source, target); await db.SaveChangesAsync();
        var term = new AcademicTerm { AcademicYearId = source.Id, Name = "Semester 1", Type = AcademicTermType.Semester, Sequence = 1, StartDate = new(2025, 6, 1), EndDate = new(2025, 11, 30) };
        db.Add(term); await db.SaveChangesAsync();
        db.Add(new AcademicCalendarEvent { AcademicYearId = source.Id, AcademicTermId = term.Id, Title = "Foundation Day", EventType = AcademicCalendarEventType.Holiday, StartsOn = new(2025, 8, 12), EndsOn = new(2025, 8, 12) }); await db.SaveChangesAsync();

        var result = await new AcademicPlanningService(db).RolloverAsync(new AcademicRolloverRequest(source.Id, target.Id, true, false));

        Assert.Equal(1, result.TermsCopied); Assert.Equal(1, result.CalendarEventsCopied);
        var copiedTerm = await db.AcademicTerms.SingleAsync(x => x.AcademicYearId == target.Id);
        Assert.Equal(new DateOnly(2026, 6, 1), copiedTerm.StartDate);
        Assert.Equal(copiedTerm.Id, (await db.AcademicCalendarEvents.SingleAsync(x => x.AcademicYearId == target.Id)).AcademicTermId);
    }

    private sealed class TestUser : ICurrentUserContext { public string? UserId => "academic-planning-tests"; }
}
