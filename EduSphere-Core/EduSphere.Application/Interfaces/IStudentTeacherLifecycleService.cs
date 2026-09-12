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

    Task<PeopleOperationResult<TeacherLifecycleEventDto>> RecordTeacherEventAsync(
        ClaimsPrincipal actor,
        CreateTeacherLifecycleEventRequest request,
        CancellationToken cancellationToken = default);
}
