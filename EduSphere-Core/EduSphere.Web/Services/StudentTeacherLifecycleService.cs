using System.Security.Claims;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class StudentTeacherLifecycleService : IStudentTeacherLifecycleService
{
    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;

    public StudentTeacherLifecycleService(
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
    }

    public async Task<IReadOnlyList<StudentLifecycleEventDto>> ListStudentEventsAsync(
        ClaimsPrincipal actor,
        Guid? studentProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<StudentLifecycleEventDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.StudentLifecycleEvents.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(e => e.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (studentProfileId.HasValue)
            query = query.Where(e => e.StudentProfileId == studentProfileId.Value);
        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var pageSize = Math.Clamp(take, 1, 500);
        var events = await query
            .OrderByDescending(e => e.EffectiveOn)
            .ThenByDescending(e => e.RecordedOn)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return events.Select(Map).ToList();
    }

    public async Task<PeopleOperationResult<StudentLifecycleEventDto>> RecordStudentEventAsync(
        ClaimsPrincipal actor,
        CreateStudentLifecycleEventRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure("Select a tenant before recording lifecycle events.");

        var student = await _dbContext.StudentProfiles
            .FirstOrDefaultAsync(s => s.Id == request.StudentProfileId, cancellationToken);
        if (student is null)
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure("Student profile was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, student.BranchId))
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure("You cannot manage lifecycle events for this student's branch.");

        var targetBranchId = request.ToBranchId ?? student.BranchId;
        if (!await _branchAccess.CanAccessBranchAsync(actor, targetBranchId))
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure("You cannot move the student to the selected branch.");

        var placementResult = await ResolveTargetStudentPlacementAsync(student, request, cancellationToken);
        if (!placementResult.Succeeded || placementResult.Data is null)
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure(placementResult.Errors.ToArray());

        var currentPlacement = await ResolveCurrentStudentPlacementAsync(student, cancellationToken);
        var targetPlacement = placementResult.Data;
        var actorUserId = _branchAccess.GetUserId(actor);

        var lifecycleEvent = new StudentLifecycleEvent
        {
            TenantId = student.TenantId,
            StudentProfileId = student.Id,
            BranchId = targetBranchId,
            EventType = request.EventType,
            FromStatus = student.Status,
            ToStatus = request.ToStatus,
            FromBranchId = student.BranchId,
            ToBranchId = targetBranchId,
            FromAcademicYearId = currentPlacement.AcademicYearId,
            ToAcademicYearId = targetPlacement.AcademicYearId,
            FromCourseId = currentPlacement.CourseId,
            ToCourseId = targetPlacement.CourseId,
            FromBatchId = currentPlacement.BatchId,
            ToBatchId = targetPlacement.BatchId,
            FromSectionId = currentPlacement.SectionId ?? student.SectionId,
            ToSectionId = targetPlacement.SectionId,
            EffectiveOn = request.EffectiveOn,
            RecordedOn = DateTime.UtcNow,
            RecordedByUserId = actorUserId,
            Reason = Normalize(request.Reason),
            Notes = Normalize(request.Notes)
        };

        var enrollmentError = (string?)null;
        var recordedEvent = (StudentLifecycleEventDto?)null;
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            student.Status = request.ToStatus;
            student.BranchId = targetBranchId;
            if (targetPlacement.SectionWasProvided)
                student.SectionId = targetPlacement.SectionId;

            enrollmentError = await ApplyStudentEnrollmentLifecycleAsync(
                student,
                lifecycleEvent,
                request.EventType,
                targetPlacement,
                currentPlacement,
                cancellationToken);
            if (enrollmentError is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            await UpdatePrimaryBranchAssignmentAsync(
                student.TenantId,
                student.UserId,
                targetBranchId,
                request.EffectiveOn,
                "Student branch assignment updated through lifecycle.",
                cancellationToken);

            _dbContext.StudentLifecycleEvents.Add(lifecycleEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            recordedEvent = Map(lifecycleEvent);
        });

        if (enrollmentError is not null)
            return PeopleOperationResult<StudentLifecycleEventDto>.Failure(enrollmentError);

        return PeopleOperationResult<StudentLifecycleEventDto>.Success(
            recordedEvent ?? Map(lifecycleEvent),
            "Student lifecycle event recorded.");
    }

    public async Task<IReadOnlyList<StudentLifecycleRequestDto>> ListStudentRequestsAsync(
        ClaimsPrincipal actor,
        Guid? studentProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<StudentLifecycleRequestDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.StudentLifecycleRequests.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(e => e.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (studentProfileId.HasValue)
            query = query.Where(e => e.StudentProfileId == studentProfileId.Value);
        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var pageSize = Math.Clamp(take, 1, 500);
        var requests = await query
            .OrderBy(e => e.Status == ApprovalStatus.UnderReview ? 0 : 1)
            .ThenByDescending(e => e.RequestedOn)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return requests.Select(Map).ToList();
    }

    public async Task<PeopleOperationResult<StudentLifecycleRequestDto>> CreateStudentRequestAsync(
        ClaimsPrincipal actor,
        CreateStudentLifecycleRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Select a tenant before creating lifecycle requests.");

        var student = await _dbContext.StudentProfiles
            .FirstOrDefaultAsync(s => s.Id == request.StudentProfileId, cancellationToken);
        if (student is null)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Student profile was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, student.BranchId))
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("You cannot create lifecycle requests for this student's branch.");

        var targetBranchId = request.ToBranchId ?? student.BranchId;
        if (!await _branchAccess.CanAccessBranchAsync(actor, targetBranchId))
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("You cannot request movement to the selected branch.");

        var placementResult = await ResolveTargetStudentPlacementAsync(student, request, cancellationToken);
        if (!placementResult.Succeeded)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure(placementResult.Errors.ToArray());

        var hasPending = await _dbContext.StudentLifecycleRequests.AnyAsync(e =>
            e.StudentProfileId == student.Id &&
            e.Status == ApprovalStatus.UnderReview,
            cancellationToken);
        if (hasPending)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("This student already has a lifecycle request under review.");

        var lifecycleRequest = new StudentLifecycleRequest
        {
            TenantId = student.TenantId,
            StudentProfileId = student.Id,
            BranchId = student.BranchId,
            EventType = request.EventType,
            ToStatus = request.ToStatus,
            Status = ApprovalStatus.UnderReview,
            ToBranchId = request.ToBranchId,
            ToAcademicYearId = request.ToAcademicYearId,
            ToCourseId = request.ToCourseId,
            ToBatchId = request.ToBatchId,
            ToSectionId = request.ToSectionId,
            EffectiveOn = request.EffectiveOn,
            RequestedOn = DateTime.UtcNow,
            RequestedByUserId = _branchAccess.GetUserId(actor),
            Reason = Normalize(request.Reason),
            Notes = Normalize(request.Notes)
        };

        _dbContext.StudentLifecycleRequests.Add(lifecycleRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<StudentLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Student lifecycle request submitted for approval.");
    }

    public async Task<PeopleOperationResult<StudentLifecycleRequestDto>> ApproveStudentRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default)
    {
        var lifecycleRequest = await _dbContext.StudentLifecycleRequests
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (lifecycleRequest is null)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Student lifecycle request was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, lifecycleRequest.BranchId))
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("You cannot approve requests for this branch.");

        if (lifecycleRequest.Status != ApprovalStatus.UnderReview)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Only requests under review can be approved.");

        var eventResult = await RecordStudentEventAsync(actor, new CreateStudentLifecycleEventRequest
        {
            StudentProfileId = lifecycleRequest.StudentProfileId,
            EventType = lifecycleRequest.EventType,
            ToStatus = lifecycleRequest.ToStatus,
            ToBranchId = lifecycleRequest.ToBranchId,
            ToAcademicYearId = lifecycleRequest.ToAcademicYearId,
            ToCourseId = lifecycleRequest.ToCourseId,
            ToBatchId = lifecycleRequest.ToBatchId,
            ToSectionId = lifecycleRequest.ToSectionId,
            EffectiveOn = lifecycleRequest.EffectiveOn,
            Reason = lifecycleRequest.Reason,
            Notes = lifecycleRequest.Notes
        }, cancellationToken);

        if (!eventResult.Succeeded || eventResult.Data is null)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure(eventResult.Errors.ToArray());

        lifecycleRequest.Status = ApprovalStatus.Approved;
        lifecycleRequest.DecidedOn = DateTime.UtcNow;
        lifecycleRequest.DecidedByUserId = _branchAccess.GetUserId(actor);
        lifecycleRequest.DecisionNotes = Normalize(decisionNotes);
        lifecycleRequest.AppliedStudentLifecycleEventId = eventResult.Data.Id;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<StudentLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Student lifecycle request approved and applied.");
    }

    public async Task<PeopleOperationResult<StudentLifecycleRequestDto>> RejectStudentRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default)
    {
        var lifecycleRequest = await _dbContext.StudentLifecycleRequests
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (lifecycleRequest is null)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Student lifecycle request was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, lifecycleRequest.BranchId))
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("You cannot reject requests for this branch.");

        if (lifecycleRequest.Status != ApprovalStatus.UnderReview)
            return PeopleOperationResult<StudentLifecycleRequestDto>.Failure("Only requests under review can be rejected.");

        lifecycleRequest.Status = ApprovalStatus.Rejected;
        lifecycleRequest.DecidedOn = DateTime.UtcNow;
        lifecycleRequest.DecidedByUserId = _branchAccess.GetUserId(actor);
        lifecycleRequest.DecisionNotes = Normalize(decisionNotes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<StudentLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Student lifecycle request rejected.");
    }

    public async Task<IReadOnlyList<StudentAlumniRecordDto>> ListAlumniRecordsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<StudentAlumniRecordDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.StudentAlumniRecords.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(e => e.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var pageSize = Math.Clamp(take, 1, 500);
        var records = await query
            .OrderByDescending(e => e.GraduationDate)
            .ThenBy(e => e.AlumniNumber)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return records.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<TeacherLifecycleEventDto>> ListTeacherEventsAsync(
        ClaimsPrincipal actor,
        Guid? teacherProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<TeacherLifecycleEventDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.TeacherLifecycleEvents.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(e => e.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (teacherProfileId.HasValue)
            query = query.Where(e => e.TeacherProfileId == teacherProfileId.Value);
        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var pageSize = Math.Clamp(take, 1, 500);
        var events = await query
            .OrderByDescending(e => e.EffectiveOn)
            .ThenByDescending(e => e.RecordedOn)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return events.Select(Map).ToList();
    }

    public async Task<PeopleOperationResult<TeacherLifecycleEventDto>> RecordTeacherEventAsync(
        ClaimsPrincipal actor,
        CreateTeacherLifecycleEventRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return PeopleOperationResult<TeacherLifecycleEventDto>.Failure("Select a tenant before recording lifecycle events.");

        var teacher = await _dbContext.TeacherProfiles
            .FirstOrDefaultAsync(t => t.Id == request.TeacherProfileId, cancellationToken);
        if (teacher is null)
            return PeopleOperationResult<TeacherLifecycleEventDto>.Failure("Teacher profile was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, teacher.BranchId))
            return PeopleOperationResult<TeacherLifecycleEventDto>.Failure("You cannot manage lifecycle events for this teacher's branch.");

        var targetBranchId = request.ToBranchId ?? teacher.BranchId;
        if (!await _branchAccess.CanAccessBranchAsync(actor, targetBranchId))
            return PeopleOperationResult<TeacherLifecycleEventDto>.Failure("You cannot move the teacher to the selected branch.");

        var validationError = await ValidateTeacherTargetsAsync(request, targetBranchId, cancellationToken);
        if (validationError is not null)
            return PeopleOperationResult<TeacherLifecycleEventDto>.Failure(validationError);

        var actorUserId = _branchAccess.GetUserId(actor);
        var lifecycleEvent = new TeacherLifecycleEvent
        {
            TenantId = teacher.TenantId,
            TeacherProfileId = teacher.Id,
            BranchId = targetBranchId,
            EventType = request.EventType,
            FromStatus = teacher.Status,
            ToStatus = request.ToStatus,
            FromBranchId = teacher.BranchId,
            ToBranchId = targetBranchId,
            FromDepartmentId = teacher.DepartmentId,
            ToDepartmentId = request.ToDepartmentId ?? teacher.DepartmentId,
            EffectiveOn = request.EffectiveOn,
            RecordedOn = DateTime.UtcNow,
            RecordedByUserId = actorUserId,
            Reason = Normalize(request.Reason),
            Notes = Normalize(request.Notes)
        };

        var recordedEvent = (TeacherLifecycleEventDto?)null;
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            teacher.Status = request.ToStatus;
            teacher.BranchId = targetBranchId;
            if (request.ToDepartmentId.HasValue ||
                request.EventType is TeacherLifecycleEventType.DepartmentTransfer or TeacherLifecycleEventType.Onboarded)
                teacher.DepartmentId = request.ToDepartmentId;

            await UpdatePrimaryBranchAssignmentAsync(
                teacher.TenantId,
                teacher.UserId,
                targetBranchId,
                request.EffectiveOn,
                "Teacher branch assignment updated through lifecycle.",
                cancellationToken);

            _dbContext.TeacherLifecycleEvents.Add(lifecycleEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            recordedEvent = Map(lifecycleEvent);
        });

        return PeopleOperationResult<TeacherLifecycleEventDto>.Success(
            recordedEvent ?? Map(lifecycleEvent),
            "Teacher lifecycle event recorded.");
    }

    public async Task<IReadOnlyList<TeacherLifecycleRequestDto>> ListTeacherRequestsAsync(
        ClaimsPrincipal actor,
        Guid? teacherProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<TeacherLifecycleRequestDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.TeacherLifecycleRequests.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(e => e.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (teacherProfileId.HasValue)
            query = query.Where(e => e.TeacherProfileId == teacherProfileId.Value);
        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        var pageSize = Math.Clamp(take, 1, 500);
        var requests = await query
            .OrderBy(e => e.Status == ApprovalStatus.UnderReview ? 0 : 1)
            .ThenByDescending(e => e.RequestedOn)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return requests.Select(Map).ToList();
    }

    public async Task<PeopleOperationResult<TeacherLifecycleRequestDto>> CreateTeacherRequestAsync(
        ClaimsPrincipal actor,
        CreateTeacherLifecycleRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Select a tenant before creating lifecycle requests.");

        var teacher = await _dbContext.TeacherProfiles
            .FirstOrDefaultAsync(t => t.Id == request.TeacherProfileId, cancellationToken);
        if (teacher is null)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Teacher profile was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, teacher.BranchId))
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("You cannot create lifecycle requests for this teacher's branch.");

        var targetBranchId = request.ToBranchId ?? teacher.BranchId;
        if (!await _branchAccess.CanAccessBranchAsync(actor, targetBranchId))
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("You cannot request movement to the selected branch.");

        var validationError = await ValidateTeacherTargetsAsync(request, targetBranchId, cancellationToken);
        if (validationError is not null)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure(validationError);

        var hasPending = await _dbContext.TeacherLifecycleRequests.AnyAsync(e =>
            e.TeacherProfileId == teacher.Id &&
            e.Status == ApprovalStatus.UnderReview,
            cancellationToken);
        if (hasPending)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("This teacher already has a lifecycle request under review.");

        var lifecycleRequest = new TeacherLifecycleRequest
        {
            TenantId = teacher.TenantId,
            TeacherProfileId = teacher.Id,
            BranchId = teacher.BranchId,
            EventType = request.EventType,
            ToStatus = request.ToStatus,
            Status = ApprovalStatus.UnderReview,
            ToBranchId = request.ToBranchId,
            ToDepartmentId = request.ToDepartmentId,
            EffectiveOn = request.EffectiveOn,
            RequestedOn = DateTime.UtcNow,
            RequestedByUserId = _branchAccess.GetUserId(actor),
            Reason = Normalize(request.Reason),
            Notes = Normalize(request.Notes)
        };

        _dbContext.TeacherLifecycleRequests.Add(lifecycleRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<TeacherLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Teacher lifecycle request submitted for approval.");
    }

    public async Task<PeopleOperationResult<TeacherLifecycleRequestDto>> ApproveTeacherRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default)
    {
        var lifecycleRequest = await _dbContext.TeacherLifecycleRequests
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (lifecycleRequest is null)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Teacher lifecycle request was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, lifecycleRequest.BranchId))
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("You cannot approve requests for this branch.");

        if (lifecycleRequest.Status != ApprovalStatus.UnderReview)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Only requests under review can be approved.");

        var eventResult = await RecordTeacherEventAsync(actor, new CreateTeacherLifecycleEventRequest
        {
            TeacherProfileId = lifecycleRequest.TeacherProfileId,
            EventType = lifecycleRequest.EventType,
            ToStatus = lifecycleRequest.ToStatus,
            ToBranchId = lifecycleRequest.ToBranchId,
            ToDepartmentId = lifecycleRequest.ToDepartmentId,
            EffectiveOn = lifecycleRequest.EffectiveOn,
            Reason = lifecycleRequest.Reason,
            Notes = lifecycleRequest.Notes
        }, cancellationToken);

        if (!eventResult.Succeeded || eventResult.Data is null)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure(eventResult.Errors.ToArray());

        lifecycleRequest.Status = ApprovalStatus.Approved;
        lifecycleRequest.DecidedOn = DateTime.UtcNow;
        lifecycleRequest.DecidedByUserId = _branchAccess.GetUserId(actor);
        lifecycleRequest.DecisionNotes = Normalize(decisionNotes);
        lifecycleRequest.AppliedTeacherLifecycleEventId = eventResult.Data.Id;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<TeacherLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Teacher lifecycle request approved and applied.");
    }

    public async Task<PeopleOperationResult<TeacherLifecycleRequestDto>> RejectTeacherRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default)
    {
        var lifecycleRequest = await _dbContext.TeacherLifecycleRequests
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (lifecycleRequest is null)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Teacher lifecycle request was not found.");

        if (!await _branchAccess.CanAccessBranchAsync(actor, lifecycleRequest.BranchId))
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("You cannot reject requests for this branch.");

        if (lifecycleRequest.Status != ApprovalStatus.UnderReview)
            return PeopleOperationResult<TeacherLifecycleRequestDto>.Failure("Only requests under review can be rejected.");

        lifecycleRequest.Status = ApprovalStatus.Rejected;
        lifecycleRequest.DecidedOn = DateTime.UtcNow;
        lifecycleRequest.DecidedByUserId = _branchAccess.GetUserId(actor);
        lifecycleRequest.DecisionNotes = Normalize(decisionNotes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return PeopleOperationResult<TeacherLifecycleRequestDto>.Success(
            Map(lifecycleRequest),
            "Teacher lifecycle request rejected.");
    }

    private async Task<PeopleOperationResult<StudentPlacement>> ResolveTargetStudentPlacementAsync(
        StudentProfile student,
        CreateStudentLifecycleEventRequest request,
        CancellationToken cancellationToken)
    {
        var targetBranchId = request.ToBranchId ?? student.BranchId;
        if (!await _dbContext.Branches.AnyAsync(b => b.Id == targetBranchId, cancellationToken))
            return PeopleOperationResult<StudentPlacement>.Failure("Selected target branch was not found.");

        var targetAcademicYearId = request.ToAcademicYearId;
        var targetCourseId = request.ToCourseId;
        var targetBatchId = request.ToBatchId;
        var targetSectionId = request.ToSectionId;

        if (targetSectionId.HasValue)
        {
            var section = await _dbContext.Sections
                .Include(s => s.Batch)
                .FirstOrDefaultAsync(s => s.Id == targetSectionId.Value, cancellationToken);
            if (section is null)
                return PeopleOperationResult<StudentPlacement>.Failure("Selected target section was not found.");

            targetBatchId ??= section.BatchId;
            if (section.Batch is not null)
            {
                targetCourseId ??= section.Batch.CourseId;
                targetAcademicYearId ??= section.Batch.AcademicYearId;
            }
        }

        if (targetBatchId.HasValue)
        {
            var batch = await _dbContext.Batches
                .FirstOrDefaultAsync(b => b.Id == targetBatchId.Value, cancellationToken);
            if (batch is null)
                return PeopleOperationResult<StudentPlacement>.Failure("Selected target batch was not found.");

            targetCourseId ??= batch.CourseId;
            targetAcademicYearId ??= batch.AcademicYearId;
        }

        if (targetCourseId.HasValue &&
            !await _dbContext.Courses.AnyAsync(c => c.Id == targetCourseId.Value, cancellationToken))
            return PeopleOperationResult<StudentPlacement>.Failure("Selected target course was not found.");

        if (targetAcademicYearId.HasValue &&
            !await _dbContext.AcademicYears.AnyAsync(a => a.Id == targetAcademicYearId.Value, cancellationToken))
            return PeopleOperationResult<StudentPlacement>.Failure("Selected target academic year was not found.");

        if (request.EventType == StudentLifecycleEventType.SectionTransfer && !targetSectionId.HasValue)
            return PeopleOperationResult<StudentPlacement>.Failure("Section transfer requires a target section.");

        if (request.EventType == StudentLifecycleEventType.BranchTransfer && request.ToBranchId is null)
            return PeopleOperationResult<StudentPlacement>.Failure("Branch transfer requires a target branch.");

        if (request.EventType is StudentLifecycleEventType.Promoted or StudentLifecycleEventType.ReAdmitted &&
            (!targetAcademicYearId.HasValue || !targetCourseId.HasValue || !targetBatchId.HasValue))
            return PeopleOperationResult<StudentPlacement>.Failure("Promotion and re-admission require a target academic year, course, and batch.");

        return PeopleOperationResult<StudentPlacement>.Success(new StudentPlacement(
            targetBranchId,
            targetAcademicYearId,
            targetCourseId,
            targetBatchId,
            targetSectionId,
            request.ToSectionId.HasValue));
    }

    private async Task<StudentPlacement> ResolveCurrentStudentPlacementAsync(
        StudentProfile student,
        CancellationToken cancellationToken)
    {
        var activeEnrollment = await _dbContext.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentProfileId == student.Id && e.Status == EnrollmentStatus.Active)
            .OrderByDescending(e => e.EnrollmentDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeEnrollment is not null)
        {
            return new StudentPlacement(
                activeEnrollment.BranchId,
                activeEnrollment.AcademicYearId,
                activeEnrollment.CourseId,
                activeEnrollment.BatchId,
                activeEnrollment.SectionId,
                activeEnrollment.SectionId.HasValue);
        }

        if (student.SectionId is Guid sectionId)
        {
            var section = await _dbContext.Sections
                .AsNoTracking()
                .Include(s => s.Batch)
                .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
            if (section?.Batch is not null)
            {
                return new StudentPlacement(
                    student.BranchId,
                    section.Batch.AcademicYearId,
                    section.Batch.CourseId,
                    section.BatchId,
                    section.Id,
                    true);
            }
        }

        return new StudentPlacement(student.BranchId, null, null, null, student.SectionId, student.SectionId.HasValue);
    }

    private async Task<string?> ApplyStudentEnrollmentLifecycleAsync(
        StudentProfile student,
        StudentLifecycleEvent lifecycleEvent,
        StudentLifecycleEventType eventType,
        StudentPlacement targetPlacement,
        StudentPlacement currentPlacement,
        CancellationToken cancellationToken)
    {
        var activeEnrollments = await _dbContext.Enrollments
            .Where(e => e.StudentProfileId == student.Id && e.Status == EnrollmentStatus.Active)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync(cancellationToken);

        if (eventType is StudentLifecycleEventType.Promoted or StudentLifecycleEventType.ReAdmitted)
        {
            if (!targetPlacement.AcademicYearId.HasValue ||
                !targetPlacement.CourseId.HasValue ||
                !targetPlacement.BatchId.HasValue)
                return "Promotion and re-admission require a target academic year, course, and batch.";

            foreach (var enrollment in activeEnrollments)
            {
                enrollment.Status = eventType == StudentLifecycleEventType.Promoted
                    ? EnrollmentStatus.Promoted
                    : EnrollmentStatus.Completed;
            }

            var alreadyEnrolled = await _dbContext.Enrollments.AnyAsync(e =>
                e.StudentProfileId == student.Id &&
                e.AcademicYearId == targetPlacement.AcademicYearId.Value,
                cancellationToken);
            if (!alreadyEnrolled)
            {
                _dbContext.Enrollments.Add(new Enrollment
                {
                    TenantId = student.TenantId,
                    StudentProfileId = student.Id,
                    BranchId = targetPlacement.BranchId,
                    AcademicYearId = targetPlacement.AcademicYearId.Value,
                    CourseId = targetPlacement.CourseId.Value,
                    BatchId = targetPlacement.BatchId.Value,
                    SectionId = targetPlacement.SectionId,
                    EnrollmentNumber = await NextEnrollmentNumberAsync(student.TenantId, cancellationToken),
                    EnrollmentDate = lifecycleEvent.EffectiveOn,
                    Status = EnrollmentStatus.Active,
                    Notes = eventType == StudentLifecycleEventType.Promoted
                        ? "Created from student lifecycle promotion."
                        : "Created from student lifecycle re-admission."
                });
            }

            if (eventType == StudentLifecycleEventType.Promoted &&
                currentPlacement.AcademicYearId.HasValue &&
                targetPlacement.AcademicYearId.HasValue &&
                targetPlacement.SectionId.HasValue)
            {
                _dbContext.PromotionRecords.Add(new PromotionRecord
                {
                    TenantId = student.TenantId,
                    StudentProfileId = student.Id,
                    FromAcademicYearId = currentPlacement.AcademicYearId.Value,
                    ToAcademicYearId = targetPlacement.AcademicYearId.Value,
                    FromSectionId = currentPlacement.SectionId,
                    ToSectionId = targetPlacement.SectionId.Value,
                    PromotionDate = lifecycleEvent.EffectiveOn,
                    Notes = lifecycleEvent.Notes ?? lifecycleEvent.Reason
                });
            }

            return null;
        }

        if (eventType is StudentLifecycleEventType.SectionTransfer or StudentLifecycleEventType.BranchTransfer)
        {
            foreach (var enrollment in activeEnrollments)
            {
                enrollment.BranchId = targetPlacement.BranchId;
                if (targetPlacement.SectionWasProvided)
                    enrollment.SectionId = targetPlacement.SectionId;
                if (targetPlacement.AcademicYearId.HasValue)
                    enrollment.AcademicYearId = targetPlacement.AcademicYearId.Value;
                if (targetPlacement.CourseId.HasValue)
                    enrollment.CourseId = targetPlacement.CourseId.Value;
                if (targetPlacement.BatchId.HasValue)
                    enrollment.BatchId = targetPlacement.BatchId.Value;
            }
        }

        if (eventType is StudentLifecycleEventType.Withdrawn or StudentLifecycleEventType.Deactivated)
        {
            foreach (var enrollment in activeEnrollments)
                enrollment.Status = EnrollmentStatus.Withdrawn;
        }

        if (eventType == StudentLifecycleEventType.Graduated)
        {
            foreach (var enrollment in activeEnrollments)
                enrollment.Status = EnrollmentStatus.Completed;

            var hasAlumniRecord = await _dbContext.StudentAlumniRecords.AnyAsync(e =>
                e.StudentProfileId == student.Id,
                cancellationToken);
            if (!hasAlumniRecord)
            {
                _dbContext.StudentAlumniRecords.Add(new StudentAlumniRecord
                {
                    TenantId = student.TenantId,
                    StudentProfileId = student.Id,
                    BranchId = targetPlacement.BranchId,
                    AcademicYearId = currentPlacement.AcademicYearId ?? targetPlacement.AcademicYearId,
                    CourseId = currentPlacement.CourseId ?? targetPlacement.CourseId,
                    BatchId = currentPlacement.BatchId ?? targetPlacement.BatchId,
                    AlumniNumber = await NextAlumniNumberAsync(student.TenantId, cancellationToken),
                    GraduationDate = lifecycleEvent.EffectiveOn,
                    ContactEmail = student.Email,
                    ContactPhone = student.PhoneNumber,
                    Notes = lifecycleEvent.Notes ?? lifecycleEvent.Reason
                });
            }
        }

        return null;
    }

    private async Task<string?> ValidateTeacherTargetsAsync(
        CreateTeacherLifecycleEventRequest request,
        Guid targetBranchId,
        CancellationToken cancellationToken)
    {
        if (!await _dbContext.Branches.AnyAsync(b => b.Id == targetBranchId, cancellationToken))
            return "Selected target branch was not found.";

        if (request.EventType == TeacherLifecycleEventType.BranchTransfer && request.ToBranchId is null)
            return "Branch transfer requires a target branch.";

        if (request.EventType == TeacherLifecycleEventType.DepartmentTransfer && request.ToDepartmentId is null)
            return "Department transfer requires a target department.";

        if (request.ToDepartmentId.HasValue &&
            !await _dbContext.Departments.AnyAsync(d => d.Id == request.ToDepartmentId.Value, cancellationToken))
            return "Selected target department was not found.";

        return null;
    }

    private async Task UpdatePrimaryBranchAssignmentAsync(
        Guid tenantId,
        Guid? userId,
        Guid branchId,
        DateOnly effectiveFrom,
        string notes,
        CancellationToken cancellationToken)
    {
        if (!userId.HasValue)
            return;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user is not null)
            user.BranchId = branchId;

        var assignments = await _dbContext.UserBranchAssignments
            .IgnoreQueryFilters()
            .Where(a => a.TenantId == tenantId && a.UserId == userId.Value && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var assignment in assignments.Where(a => a.IsActive && a.BranchId != branchId))
        {
            assignment.IsActive = false;
            assignment.IsPrimary = false;
            assignment.EffectiveUntil = effectiveFrom.AddDays(-1);
        }

        var targetAssignment = assignments.FirstOrDefault(a => a.BranchId == branchId);
        if (targetAssignment is null)
        {
            _dbContext.UserBranchAssignments.Add(new UserBranchAssignment
            {
                TenantId = tenantId,
                UserId = userId.Value,
                BranchId = branchId,
                IsPrimary = true,
                IsActive = true,
                EffectiveFrom = effectiveFrom,
                Notes = notes
            });
        }
        else
        {
            targetAssignment.IsPrimary = true;
            targetAssignment.IsActive = true;
            targetAssignment.EffectiveFrom = effectiveFrom;
            targetAssignment.EffectiveUntil = null;
            targetAssignment.Notes = notes;
        }
    }

    private async Task<string> NextEnrollmentNumberAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        for (var i = 1; i < 100_000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var number = $"ENR-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.Enrollments
                    .IgnoreQueryFilters()
                    .AnyAsync(e => e.TenantId == tenantId && e.EnrollmentNumber == number, cancellationToken))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique enrollment number.");
    }

    private async Task<string> NextAlumniNumberAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        for (var i = 1; i < 100_000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var number = $"ALU-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.StudentAlumniRecords
                    .IgnoreQueryFilters()
                    .AnyAsync(e => e.TenantId == tenantId && e.AlumniNumber == number, cancellationToken))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique alumni number.");
    }

    private static StudentLifecycleEventDto Map(StudentLifecycleEvent e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        IsDeleted = e.IsDeleted,
        CreatedBy = e.CreatedBy,
        CreatedOn = e.CreatedOn,
        ModifiedBy = e.ModifiedBy,
        ModifiedOn = e.ModifiedOn,
        DeletedBy = e.DeletedBy,
        DeletedOn = e.DeletedOn,
        ConcurrencyToken = e.ConcurrencyToken,
        StudentProfileId = e.StudentProfileId,
        BranchId = e.BranchId,
        EventType = e.EventType,
        FromStatus = e.FromStatus,
        ToStatus = e.ToStatus,
        FromBranchId = e.FromBranchId,
        ToBranchId = e.ToBranchId,
        FromAcademicYearId = e.FromAcademicYearId,
        ToAcademicYearId = e.ToAcademicYearId,
        FromCourseId = e.FromCourseId,
        ToCourseId = e.ToCourseId,
        FromBatchId = e.FromBatchId,
        ToBatchId = e.ToBatchId,
        FromSectionId = e.FromSectionId,
        ToSectionId = e.ToSectionId,
        EffectiveOn = e.EffectiveOn,
        RecordedOn = e.RecordedOn,
        RecordedByUserId = e.RecordedByUserId,
        Reason = e.Reason,
        Notes = e.Notes
    };

    private static StudentLifecycleRequestDto Map(StudentLifecycleRequest e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        IsDeleted = e.IsDeleted,
        CreatedBy = e.CreatedBy,
        CreatedOn = e.CreatedOn,
        ModifiedBy = e.ModifiedBy,
        ModifiedOn = e.ModifiedOn,
        DeletedBy = e.DeletedBy,
        DeletedOn = e.DeletedOn,
        ConcurrencyToken = e.ConcurrencyToken,
        StudentProfileId = e.StudentProfileId,
        BranchId = e.BranchId,
        EventType = e.EventType,
        ToStatus = e.ToStatus,
        Status = e.Status,
        ToBranchId = e.ToBranchId,
        ToAcademicYearId = e.ToAcademicYearId,
        ToCourseId = e.ToCourseId,
        ToBatchId = e.ToBatchId,
        ToSectionId = e.ToSectionId,
        EffectiveOn = e.EffectiveOn,
        RequestedOn = e.RequestedOn,
        RequestedByUserId = e.RequestedByUserId,
        DecidedOn = e.DecidedOn,
        DecidedByUserId = e.DecidedByUserId,
        AppliedStudentLifecycleEventId = e.AppliedStudentLifecycleEventId,
        Reason = e.Reason,
        Notes = e.Notes,
        DecisionNotes = e.DecisionNotes
    };

    private static StudentAlumniRecordDto Map(StudentAlumniRecord e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        IsDeleted = e.IsDeleted,
        CreatedBy = e.CreatedBy,
        CreatedOn = e.CreatedOn,
        ModifiedBy = e.ModifiedBy,
        ModifiedOn = e.ModifiedOn,
        DeletedBy = e.DeletedBy,
        DeletedOn = e.DeletedOn,
        ConcurrencyToken = e.ConcurrencyToken,
        StudentProfileId = e.StudentProfileId,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        AlumniNumber = e.AlumniNumber,
        GraduationDate = e.GraduationDate,
        ContactEmail = e.ContactEmail,
        ContactPhone = e.ContactPhone,
        HigherEducation = e.HigherEducation,
        EmployerOrInstitution = e.EmployerOrInstitution,
        Notes = e.Notes
    };

    private static TeacherLifecycleEventDto Map(TeacherLifecycleEvent e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        IsDeleted = e.IsDeleted,
        CreatedBy = e.CreatedBy,
        CreatedOn = e.CreatedOn,
        ModifiedBy = e.ModifiedBy,
        ModifiedOn = e.ModifiedOn,
        DeletedBy = e.DeletedBy,
        DeletedOn = e.DeletedOn,
        ConcurrencyToken = e.ConcurrencyToken,
        TeacherProfileId = e.TeacherProfileId,
        BranchId = e.BranchId,
        EventType = e.EventType,
        FromStatus = e.FromStatus,
        ToStatus = e.ToStatus,
        FromBranchId = e.FromBranchId,
        ToBranchId = e.ToBranchId,
        FromDepartmentId = e.FromDepartmentId,
        ToDepartmentId = e.ToDepartmentId,
        EffectiveOn = e.EffectiveOn,
        RecordedOn = e.RecordedOn,
        RecordedByUserId = e.RecordedByUserId,
        Reason = e.Reason,
        Notes = e.Notes
    };

    private static TeacherLifecycleRequestDto Map(TeacherLifecycleRequest e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        IsDeleted = e.IsDeleted,
        CreatedBy = e.CreatedBy,
        CreatedOn = e.CreatedOn,
        ModifiedBy = e.ModifiedBy,
        ModifiedOn = e.ModifiedOn,
        DeletedBy = e.DeletedBy,
        DeletedOn = e.DeletedOn,
        ConcurrencyToken = e.ConcurrencyToken,
        TeacherProfileId = e.TeacherProfileId,
        BranchId = e.BranchId,
        EventType = e.EventType,
        ToStatus = e.ToStatus,
        Status = e.Status,
        ToBranchId = e.ToBranchId,
        ToDepartmentId = e.ToDepartmentId,
        EffectiveOn = e.EffectiveOn,
        RequestedOn = e.RequestedOn,
        RequestedByUserId = e.RequestedByUserId,
        DecidedOn = e.DecidedOn,
        DecidedByUserId = e.DecidedByUserId,
        AppliedTeacherLifecycleEventId = e.AppliedTeacherLifecycleEventId,
        Reason = e.Reason,
        Notes = e.Notes,
        DecisionNotes = e.DecisionNotes
    };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record StudentPlacement(
        Guid BranchId,
        Guid? AcademicYearId,
        Guid? CourseId,
        Guid? BatchId,
        Guid? SectionId,
        bool SectionWasProvided);
}
