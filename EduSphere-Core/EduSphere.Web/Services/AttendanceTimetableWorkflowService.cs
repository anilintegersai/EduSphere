using System.Security.Claims;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Controllers.V1.Operations;
using EduSphere.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AttendanceTimetableWorkflowService : IAttendanceTimetableWorkflowService
{
    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;

    public AttendanceTimetableWorkflowService(
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
    }

    public async Task<IReadOnlyList<TimetableSubstitutionDto>> ListSubstitutionsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return Array.Empty<TimetableSubstitutionDto>();
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<TimetableSubstitutionDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.TimetableSubstitutions.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
            query = assignedBranchId.HasValue
                ? query.Where(s => s.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);
        if (from.HasValue)
            query = query.Where(s => s.SubstitutionDate >= from.Value);
        if (to.HasValue)
            query = query.Where(s => s.SubstitutionDate <= to.Value);

        var substitutions = await query
            .OrderByDescending(s => s.SubstitutionDate)
            .ThenByDescending(s => s.RequestedOn)
            .ToListAsync(cancellationToken);

        return substitutions.Select(Map).ToList();
    }

    public async Task<OperationsWorkflowResult<TimetableSubstitutionDto>> CreateSubstitutionAsync(
        ClaimsPrincipal actor,
        CreateTimetableSubstitutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.TimetableEntries
            .Include(e => e.Timetable)
            .Include(e => e.TimeSlot)
            .FirstOrDefaultAsync(e => e.Id == request.TimetableEntryId, cancellationToken);
        if (entry?.Timetable is null || entry.TimeSlot is null)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Timetable entry was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, entry.Timetable.BranchId))
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("You cannot request relief teachers for this branch.");
        if (entry.TeacherProfileId != request.OriginalTeacherProfileId)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Original teacher must match the timetable entry teacher.");
        if (request.OriginalTeacherProfileId == request.SubstituteTeacherProfileId)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Relief teacher must be different from the original teacher.");
        if (request.SubstitutionDate.DayOfWeek != entry.TimeSlot.DayOfWeek)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Substitution date must match the timetable entry day.");

        var originalTeacher = await _dbContext.TeacherProfiles.FindAsync(new object?[] { request.OriginalTeacherProfileId }, cancellationToken);
        var substituteTeacher = await _dbContext.TeacherProfiles.FindAsync(new object?[] { request.SubstituteTeacherProfileId }, cancellationToken);
        if (originalTeacher is null || substituteTeacher is null)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Selected teacher profile was not found.");
        if (substituteTeacher.BranchId != entry.Timetable.BranchId)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Relief teacher must belong to the timetable branch.");

        var duplicate = await _dbContext.TimetableSubstitutions.AnyAsync(s =>
            s.TimetableEntryId == entry.Id &&
            s.SubstitutionDate == request.SubstitutionDate &&
            (s.Status == ApprovalStatus.UnderReview || s.Status == ApprovalStatus.Approved),
            cancellationToken);
        if (duplicate)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("A relief teacher request already exists for this class and date.");

        if (await HasTeacherConflictAsync(
                entry.Timetable.BranchId,
                request.SubstituteTeacherProfileId,
                entry.TimeSlot,
                request.SubstitutionDate,
                entry.Id,
                cancellationToken))
        {
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Relief teacher already has a class or approved substitution in this time slot.");
        }

        var substitution = new TimetableSubstitution
        {
            BranchId = entry.Timetable.BranchId,
            TimetableEntryId = entry.Id,
            SubstitutionDate = request.SubstitutionDate,
            OriginalTeacherProfileId = request.OriginalTeacherProfileId,
            SubstituteTeacherProfileId = request.SubstituteTeacherProfileId,
            RequestedByUserId = _branchAccess.GetUserId(actor),
            RequestedOn = DateTime.UtcNow,
            Status = ApprovalStatus.UnderReview,
            Reason = Normalize(request.Reason)
        };

        _dbContext.TimetableSubstitutions.Add(substitution);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<TimetableSubstitutionDto>.Success(Map(substitution), "Relief teacher request submitted.");
    }

    public async Task<OperationsWorkflowResult<TimetableSubstitutionDto>> ReviewSubstitutionAsync(
        ClaimsPrincipal actor,
        Guid substitutionId,
        ReviewTimetableSubstitutionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Status is not (ApprovalStatus.Approved or ApprovalStatus.Rejected))
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Substitution requests can only be approved or rejected.");

        var substitution = await _dbContext.TimetableSubstitutions
            .Include(s => s.TimetableEntry)
                .ThenInclude(e => e!.Timetable)
            .Include(s => s.TimetableEntry)
                .ThenInclude(e => e!.TimeSlot)
            .FirstOrDefaultAsync(s => s.Id == substitutionId, cancellationToken);
        if (substitution?.TimetableEntry?.Timetable is null || substitution.TimetableEntry.TimeSlot is null)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Relief teacher request was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, substitution.BranchId))
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("You cannot review relief teacher requests for this branch.");
        if (substitution.Status is ApprovalStatus.Approved or ApprovalStatus.Rejected)
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("This relief teacher request has already been reviewed.");

        if (request.Status == ApprovalStatus.Approved &&
            await HasTeacherConflictAsync(
                substitution.BranchId,
                substitution.SubstituteTeacherProfileId,
                substitution.TimetableEntry.TimeSlot,
                substitution.SubstitutionDate,
                substitution.TimetableEntryId,
                cancellationToken))
        {
            return OperationsWorkflowResult<TimetableSubstitutionDto>.Failure("Relief teacher now has a conflicting class or substitution.");
        }

        substitution.Status = request.Status;
        substitution.ReviewedByUserId = _branchAccess.GetUserId(actor);
        substitution.ReviewedOn = DateTime.UtcNow;
        substitution.ReviewNotes = Normalize(request.ReviewNotes);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<TimetableSubstitutionDto>.Success(Map(substitution), request.Status == ApprovalStatus.Approved ? "Relief teacher approved." : "Relief teacher rejected.");
    }

    public async Task<IReadOnlyList<TimetableConflictDto>> DetectTimetableConflictsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return Array.Empty<TimetableConflictDto>();
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<TimetableConflictDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.TimetableEntries
            .AsNoTracking()
            .Include(e => e.Timetable)
            .Include(e => e.TimeSlot)
            .Include(e => e.Section)
            .Include(e => e.TeacherProfile)
            .Include(e => e.Room)
            .Where(e => e.Timetable != null &&
                        e.TimeSlot != null &&
                        e.Timetable.Status != TimetableStatus.Archived);

        if (_branchAccess.IsBranchAdminOnly(actor))
            query = assignedBranchId.HasValue
                ? query.Where(e => e.Timetable!.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);

        if (branchId.HasValue)
            query = query.Where(e => e.Timetable!.BranchId == branchId.Value);

        var entries = await query
            .OrderBy(e => e.Timetable!.Name)
            .ThenBy(e => e.TimeSlot!.DayOfWeek)
            .ThenBy(e => e.TimeSlot!.StartsAt)
            .ToListAsync(cancellationToken);

        var conflicts = new List<TimetableConflictDto>();
        for (var i = 0; i < entries.Count; i++)
        {
            for (var j = i + 1; j < entries.Count; j++)
            {
                var left = entries[i];
                var right = entries[j];
                if (left.Timetable is null || right.Timetable is null || left.TimeSlot is null || right.TimeSlot is null)
                    continue;
                if (left.Timetable.BranchId != right.Timetable.BranchId)
                    continue;
                if (!DateRangesOverlap(left.Timetable.EffectiveFrom, left.Timetable.EffectiveTo, right.Timetable.EffectiveFrom, right.Timetable.EffectiveTo))
                    continue;
                if (!TimeSlotsOverlap(left.TimeSlot, right.TimeSlot))
                    continue;

                if (left.SectionId == right.SectionId)
                    conflicts.Add(CreateConflict("Section", left, right, $"{SectionName(left)} has overlapping classes in {SlotName(left.TimeSlot)}."));
                if (left.TeacherProfileId == right.TeacherProfileId)
                    conflicts.Add(CreateConflict("Teacher", left, right, $"{TeacherName(left)} is assigned to overlapping classes in {SlotName(left.TimeSlot)}."));
                if (left.RoomId.HasValue && left.RoomId == right.RoomId)
                    conflicts.Add(CreateConflict("Room", left, right, $"{RoomName(left)} is assigned to overlapping classes in {SlotName(left.TimeSlot)}."));
            }
        }

        return conflicts;
    }

    public async Task<IReadOnlyList<AttendanceCorrectionRequestDto>> ListCorrectionRequestsAsync(
        ClaimsPrincipal actor,
        Guid? attendanceSessionId = null,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return Array.Empty<AttendanceCorrectionRequestDto>();

        var query = _dbContext.AttendanceCorrectionRequests
            .AsNoTracking()
            .Include(c => c.AttendanceSession)
            .AsQueryable();

        if (attendanceSessionId.HasValue)
            query = query.Where(c => c.AttendanceSessionId == attendanceSessionId.Value);

        var requests = await query
            .OrderByDescending(c => c.RequestedOn)
            .ToListAsync(cancellationToken);

        var result = new List<AttendanceCorrectionRequestDto>();
        foreach (var request in requests)
        {
            if (request.AttendanceSession is not null &&
                await _branchAccess.CanAccessBranchAsync(actor, request.AttendanceSession.BranchId))
            {
                result.Add(Map(request));
            }
        }

        return result;
    }

    public async Task<OperationsWorkflowResult<AttendanceCorrectionRequestDto>> RequestCorrectionAsync(
        ClaimsPrincipal actor,
        CreateAttendanceCorrectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.AttendanceRecords
            .Include(r => r.AttendanceSession)
            .FirstOrDefaultAsync(r => r.Id == request.AttendanceRecordId, cancellationToken);
        if (record?.AttendanceSession is null)
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("Attendance record was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, record.AttendanceSession.BranchId))
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("You cannot request corrections for this branch.");
        if (record.Status == request.RequestedStatus)
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("Requested status is already the current status.");

        var duplicate = await _dbContext.AttendanceCorrectionRequests.AnyAsync(c =>
            c.AttendanceRecordId == record.Id &&
            c.Status == ApprovalStatus.UnderReview,
            cancellationToken);
        if (duplicate)
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("A pending correction already exists for this record.");

        var correction = new AttendanceCorrectionRequest
        {
            AttendanceRecordId = record.Id,
            AttendanceSessionId = record.AttendanceSessionId,
            StudentProfileId = record.StudentProfileId,
            CurrentStatus = record.Status,
            RequestedStatus = request.RequestedStatus,
            RequestedByUserId = _branchAccess.GetUserId(actor),
            RequestedOn = DateTime.UtcNow,
            Status = ApprovalStatus.UnderReview,
            Reason = Normalize(request.Reason)
        };

        _dbContext.AttendanceCorrectionRequests.Add(correction);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Success(Map(correction), "Attendance correction request submitted.");
    }

    public async Task<OperationsWorkflowResult<AttendanceCorrectionRequestDto>> ReviewCorrectionAsync(
        ClaimsPrincipal actor,
        Guid correctionRequestId,
        ReviewAttendanceCorrectionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Status is not (ApprovalStatus.Approved or ApprovalStatus.Rejected))
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("Correction requests can only be approved or rejected.");

        var correction = await _dbContext.AttendanceCorrectionRequests
            .Include(c => c.AttendanceRecord)
            .Include(c => c.AttendanceSession)
            .FirstOrDefaultAsync(c => c.Id == correctionRequestId, cancellationToken);
        if (correction?.AttendanceRecord is null || correction.AttendanceSession is null)
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("Attendance correction request was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, correction.AttendanceSession.BranchId))
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("You cannot review corrections for this branch.");
        if (correction.Status is ApprovalStatus.Approved or ApprovalStatus.Rejected)
            return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Failure("This correction request has already been reviewed.");

        correction.Status = request.Status;
        correction.ReviewedByUserId = _branchAccess.GetUserId(actor);
        correction.ReviewedOn = DateTime.UtcNow;
        correction.ReviewNotes = Normalize(request.ReviewNotes);

        if (request.Status == ApprovalStatus.Approved)
        {
            correction.AttendanceRecord.Status = correction.RequestedStatus;
            correction.AttendanceRecord.MarkedByUserId = correction.ReviewedByUserId;
            correction.AttendanceRecord.MarkedOn = DateTime.UtcNow;
            correction.AttendanceRecord.Remarks = BuildCorrectionRemark(correction.AttendanceRecord.Remarks, correction.ReviewNotes);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<AttendanceCorrectionRequestDto>.Success(Map(correction), request.Status == ApprovalStatus.Approved ? "Correction approved." : "Correction rejected.");
    }

    public async Task<AttendanceAnalyticsDto> GetAttendanceAnalyticsAsync(
        ClaimsPrincipal actor,
        DateOnly? from = null,
        DateOnly? to = null,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = from ?? today.AddDays(-30);
        var end = to ?? today;

        if (!_tenantContext.HasTenant ||
            (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value)))
        {
            return new AttendanceAnalyticsDto { From = start, To = end };
        }

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var sessionsQuery = _dbContext.AttendanceSessions
            .AsNoTracking()
            .Where(s => s.AttendanceDate >= start && s.AttendanceDate <= end);

        if (_branchAccess.IsBranchAdminOnly(actor))
            sessionsQuery = assignedBranchId.HasValue
                ? sessionsQuery.Where(s => s.BranchId == assignedBranchId.Value)
                : sessionsQuery.Where(_ => false);

        if (branchId.HasValue)
            sessionsQuery = sessionsQuery.Where(s => s.BranchId == branchId.Value);

        var sessions = await sessionsQuery.ToListAsync(cancellationToken);
        var sessionIds = sessions.Select(s => s.Id).ToHashSet();
        var records = await _dbContext.AttendanceRecords
            .AsNoTracking()
            .Include(r => r.StudentProfile)
            .Where(r => sessionIds.Contains(r.AttendanceSessionId))
            .ToListAsync(cancellationToken);

        return BuildAnalytics(start, end, sessions.Count, records);
    }

    public async Task<OperationsWorkflowResult<int>> QueueAttendanceNotificationsAsync(
        ClaimsPrincipal actor,
        QueueAttendanceNotificationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.AttendanceSessions
            .Include(s => s.Section)
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(s => s.Id == request.AttendanceSessionId, cancellationToken);
        if (session is null)
            return OperationsWorkflowResult<int>.Failure("Attendance session was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, session.BranchId))
            return OperationsWorkflowResult<int>.Failure("You cannot queue attendance notifications for this branch.");

        var records = await _dbContext.AttendanceRecords
            .Include(r => r.StudentProfile)
                .ThenInclude(s => s!.Guardians)
            .Where(r => r.AttendanceSessionId == session.Id)
            .OrderBy(r => r.StudentProfile!.FirstName)
            .ThenBy(r => r.StudentProfile!.LastName)
            .ToListAsync(cancellationToken);

        if (request.OnlyExceptions)
            records = records.Where(r => r.Status != AttendanceStatus.Present).ToList();

        var queuedRecipientCount = 0;
        var subject = Normalize(request.Subject) ?? $"Attendance update for {session.AttendanceDate:yyyy-MM-dd}";
        foreach (var record in records)
        {
            if (record.StudentProfile is null)
                continue;

            var recipients = BuildNotificationRecipients(record.StudentProfile, session.BranchId, request.Channel).ToList();
            if (recipients.Count == 0)
                continue;

            var message = new NotificationMessage
            {
                BranchId = session.BranchId,
                Channel = request.Channel,
                Subject = subject,
                Body = BuildAttendanceNotificationBody(record, session),
                Status = NotificationStatus.Queued,
                ScheduledOn = DateTime.UtcNow,
                CreatedForUserId = _branchAccess.GetUserId(actor),
                Recipients = recipients
            };

            _dbContext.NotificationMessages.Add(message);
            queuedRecipientCount += recipients.Count;
        }

        if (queuedRecipientCount == 0)
            return OperationsWorkflowResult<int>.Failure("No eligible attendance notification recipients were found.");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<int>.Success(queuedRecipientCount, $"{queuedRecipientCount} attendance notification recipient(s) queued.");
    }

    private async Task<bool> HasTeacherConflictAsync(
        Guid branchId,
        Guid teacherProfileId,
        TimeSlot slot,
        DateOnly substitutionDate,
        Guid excludingEntryId,
        CancellationToken cancellationToken)
    {
        var entries = await _dbContext.TimetableEntries
            .AsNoTracking()
            .Include(e => e.Timetable)
            .Include(e => e.TimeSlot)
            .Where(e => e.Id != excludingEntryId &&
                        e.TeacherProfileId == teacherProfileId &&
                        e.Timetable != null &&
                        e.Timetable.BranchId == branchId &&
                        e.Timetable.Status != TimetableStatus.Archived &&
                        e.TimeSlot != null &&
                        e.TimeSlot.DayOfWeek == substitutionDate.DayOfWeek)
            .ToListAsync(cancellationToken);

        if (entries.Any(e =>
                e.Timetable is not null &&
                e.TimeSlot is not null &&
                DateWithin(e.Timetable.EffectiveFrom, e.Timetable.EffectiveTo, substitutionDate) &&
                TimeSlotsOverlap(slot, e.TimeSlot)))
        {
            return true;
        }

        var substitutions = await _dbContext.TimetableSubstitutions
            .AsNoTracking()
            .Include(s => s.TimetableEntry)
                .ThenInclude(e => e!.TimeSlot)
            .Where(s => s.BranchId == branchId &&
                        s.TimetableEntryId != excludingEntryId &&
                        s.SubstitutionDate == substitutionDate &&
                        s.SubstituteTeacherProfileId == teacherProfileId &&
                        s.Status == ApprovalStatus.Approved)
            .ToListAsync(cancellationToken);

        return substitutions.Any(s => s.TimetableEntry?.TimeSlot is not null && TimeSlotsOverlap(slot, s.TimetableEntry.TimeSlot));
    }

    private static AttendanceAnalyticsDto BuildAnalytics(
        DateOnly from,
        DateOnly to,
        int sessionCount,
        IReadOnlyList<AttendanceRecord> records)
    {
        var totalWeight = records.Sum(r => AttendanceWeight(r.Status));
        var totalRecords = records.Count;
        var studentSummaries = records
            .GroupBy(r => r.StudentProfileId)
            .Select(g =>
            {
                var first = g.First();
                var count = g.Count();
                return new StudentAttendanceSummaryDto
                {
                    StudentProfileId = g.Key,
                    StudentName = first.StudentProfile is null ? "Student" : $"{first.StudentProfile.FirstName} {first.StudentProfile.LastName}",
                    TotalRecords = count,
                    PresentRecords = g.Count(r => r.Status == AttendanceStatus.Present),
                    AbsentRecords = g.Count(r => r.Status == AttendanceStatus.Absent),
                    LateRecords = g.Count(r => r.Status == AttendanceStatus.Late),
                    ExcusedRecords = g.Count(r => r.Status is AttendanceStatus.Excused or AttendanceStatus.Leave),
                    AttendancePercentage = Percent(g.Sum(r => AttendanceWeight(r.Status)), count)
                };
            })
            .OrderBy(s => s.AttendancePercentage)
            .ThenBy(s => s.StudentName)
            .Take(10)
            .ToList();

        return new AttendanceAnalyticsDto
        {
            From = from,
            To = to,
            SessionCount = sessionCount,
            TotalRecords = totalRecords,
            PresentRecords = records.Count(r => r.Status == AttendanceStatus.Present),
            AbsentRecords = records.Count(r => r.Status == AttendanceStatus.Absent),
            LateRecords = records.Count(r => r.Status == AttendanceStatus.Late),
            AttendancePercentage = Percent(totalWeight, totalRecords),
            Students = studentSummaries
        };
    }

    private static IEnumerable<NotificationRecipient> BuildNotificationRecipients(
        StudentProfile student,
        Guid branchId,
        CommunicationChannel channel)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var studentAddress = channel == CommunicationChannel.Email ? student.Email : student.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(studentAddress) && seen.Add(studentAddress.Trim()))
        {
            yield return new NotificationRecipient
            {
                BranchId = branchId,
                UserId = student.UserId,
                DisplayName = $"{student.FirstName} {student.LastName}",
                DestinationAddress = studentAddress.Trim(),
                Status = NotificationStatus.Queued
            };
        }

        foreach (var guardian in student.Guardians.OrderByDescending(g => g.IsPrimary).ThenBy(g => g.FullName))
        {
            var address = channel == CommunicationChannel.Email ? guardian.Email : guardian.PhoneNumber;
            if (string.IsNullOrWhiteSpace(address) || !seen.Add(address.Trim()))
                continue;

            yield return new NotificationRecipient
            {
                BranchId = branchId,
                UserId = guardian.ParentUserId,
                DisplayName = guardian.FullName,
                DestinationAddress = address.Trim(),
                Status = NotificationStatus.Queued
            };
        }
    }

    private static string BuildAttendanceNotificationBody(AttendanceRecord record, AttendanceSession session)
    {
        var studentName = record.StudentProfile is null
            ? "the student"
            : $"{record.StudentProfile.FirstName} {record.StudentProfile.LastName}";
        var subject = session.Subject?.Name ?? "Daily attendance";
        var section = session.Section?.Name ?? "section";
        var period = session.PeriodNumber.HasValue ? $" Period {session.PeriodNumber.Value}" : string.Empty;

        return $"{studentName}'s attendance for {session.AttendanceDate:yyyy-MM-dd} ({section}, {subject}{period}) is marked as {record.Status}.";
    }

    private static string? BuildCorrectionRemark(string? existing, string? reviewNotes)
    {
        var suffix = string.IsNullOrWhiteSpace(reviewNotes)
            ? "Attendance corrected after approval."
            : $"Attendance corrected after approval: {reviewNotes.Trim()}";

        if (string.IsNullOrWhiteSpace(existing))
            return suffix;

        var value = $"{existing.Trim()} | {suffix}";
        return value.Length <= 500 ? value : value[..500];
    }

    private static TimetableConflictDto CreateConflict(string scope, TimetableEntry left, TimetableEntry right, string description)
        => new()
        {
            Severity = "Conflict",
            Scope = scope,
            TimetableId = left.TimetableId,
            TimetableName = left.Timetable?.Name ?? "Timetable",
            TimeSlotId = left.TimeSlotId,
            TimeSlotLabel = left.TimeSlot is null ? "Slot" : SlotName(left.TimeSlot),
            Description = $"{description} Conflict: {TimetableEntryLabel(left)} and {TimetableEntryLabel(right)}."
        };

    private static bool DateRangesOverlap(DateOnly leftStart, DateOnly? leftEnd, DateOnly rightStart, DateOnly? rightEnd)
    {
        var leftTo = leftEnd ?? DateOnly.MaxValue;
        var rightTo = rightEnd ?? DateOnly.MaxValue;
        return leftStart <= rightTo && rightStart <= leftTo;
    }

    private static bool DateWithin(DateOnly start, DateOnly? end, DateOnly value)
        => start <= value && (!end.HasValue || end.Value >= value);

    private static bool TimeSlotsOverlap(TimeSlot left, TimeSlot right)
        => left.DayOfWeek == right.DayOfWeek && left.StartsAt < right.EndsAt && right.StartsAt < left.EndsAt;

    private static decimal AttendanceWeight(AttendanceStatus status)
        => status switch
        {
            AttendanceStatus.Absent => 0m,
            AttendanceStatus.HalfDay => 0.5m,
            _ => 1m
        };

    private static decimal Percent(decimal numerator, int denominator)
        => denominator == 0 ? 0m : Math.Round(numerator * 100m / denominator, 2);

    private static string TimetableEntryLabel(TimetableEntry entry)
        => $"{entry.Timetable?.Name ?? "Timetable"} / {SectionName(entry)} / {TeacherName(entry)}";

    private static string SectionName(TimetableEntry entry)
        => entry.Section?.Name ?? "Section";

    private static string TeacherName(TimetableEntry entry)
        => entry.TeacherProfile is null ? "Teacher" : $"{entry.TeacherProfile.FirstName} {entry.TeacherProfile.LastName}";

    private static string RoomName(TimetableEntry entry)
        => entry.Room?.Name ?? "Room";

    private static string SlotName(TimeSlot slot)
        => $"{slot.DayOfWeek} P{slot.PeriodNumber} {slot.StartsAt:HH\\:mm}-{slot.EndsAt:HH\\:mm}";

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static TimetableSubstitutionDto Map(TimetableSubstitution entity) => new TimetableSubstitutionDto
    {
        BranchId = entity.BranchId,
        TimetableEntryId = entity.TimetableEntryId,
        SubstitutionDate = entity.SubstitutionDate,
        OriginalTeacherProfileId = entity.OriginalTeacherProfileId,
        SubstituteTeacherProfileId = entity.SubstituteTeacherProfileId,
        RequestedByUserId = entity.RequestedByUserId,
        RequestedOn = entity.RequestedOn,
        ReviewedByUserId = entity.ReviewedByUserId,
        ReviewedOn = entity.ReviewedOn,
        Status = entity.Status,
        Reason = entity.Reason,
        ReviewNotes = entity.ReviewNotes
    }.WithMetadata(entity);

    private static AttendanceCorrectionRequestDto Map(AttendanceCorrectionRequest entity) => new AttendanceCorrectionRequestDto
    {
        AttendanceRecordId = entity.AttendanceRecordId,
        AttendanceSessionId = entity.AttendanceSessionId,
        StudentProfileId = entity.StudentProfileId,
        CurrentStatus = entity.CurrentStatus,
        RequestedStatus = entity.RequestedStatus,
        RequestedByUserId = entity.RequestedByUserId,
        RequestedOn = entity.RequestedOn,
        ReviewedByUserId = entity.ReviewedByUserId,
        ReviewedOn = entity.ReviewedOn,
        Status = entity.Status,
        Reason = entity.Reason,
        ReviewNotes = entity.ReviewNotes
    }.WithMetadata(entity);
}
