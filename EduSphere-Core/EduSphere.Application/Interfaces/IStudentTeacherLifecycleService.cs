using System.Security.Claims;
using EduSphere.Application.DTOs.People;

namespace EduSphere.Application.Interfaces;

public interface IStudentTeacherLifecycleService
{
    Task<IReadOnlyList<StudentLifecycleEventDto>> ListStudentEventsAsync(
        ClaimsPrincipal actor,
        Guid? studentProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentLifecycleRequestDto>> ListStudentRequestsAsync(
        ClaimsPrincipal actor,
        Guid? studentProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<StudentLifecycleRequestDto>> CreateStudentRequestAsync(
        ClaimsPrincipal actor,
        CreateStudentLifecycleRequestRequest request,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<StudentLifecycleRequestDto>> ApproveStudentRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<StudentLifecycleRequestDto>> RejectStudentRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentAlumniRecordDto>> ListAlumniRecordsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<StudentLifecycleEventDto>> RecordStudentEventAsync(
        ClaimsPrincipal actor,
        CreateStudentLifecycleEventRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TeacherLifecycleEventDto>> ListTeacherEventsAsync(
        ClaimsPrincipal actor,
        Guid? teacherProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TeacherLifecycleRequestDto>> ListTeacherRequestsAsync(
        ClaimsPrincipal actor,
        Guid? teacherProfileId = null,
        Guid? branchId = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<TeacherLifecycleRequestDto>> CreateTeacherRequestAsync(
        ClaimsPrincipal actor,
        CreateTeacherLifecycleRequestRequest request,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<TeacherLifecycleRequestDto>> ApproveTeacherRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<TeacherLifecycleRequestDto>> RejectTeacherRequestAsync(
        ClaimsPrincipal actor,
        Guid requestId,
        string? decisionNotes = null,
        CancellationToken cancellationToken = default);

    Task<PeopleOperationResult<TeacherLifecycleEventDto>> RecordTeacherEventAsync(
        ClaimsPrincipal actor,
        CreateTeacherLifecycleEventRequest request,
        CancellationToken cancellationToken = default);
}
