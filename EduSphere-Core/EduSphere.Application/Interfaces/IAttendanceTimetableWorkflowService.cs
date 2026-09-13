using System.Security.Claims;
using EduSphere.Application.DTOs.Operations;

namespace EduSphere.Application.Interfaces;

public interface IAttendanceTimetableWorkflowService
{
    Task<IReadOnlyList<TimetableSubstitutionDto>> ListSubstitutionsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<TimetableSubstitutionDto>> CreateSubstitutionAsync(
        ClaimsPrincipal actor,
        CreateTimetableSubstitutionRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<TimetableSubstitutionDto>> ReviewSubstitutionAsync(
        ClaimsPrincipal actor,
        Guid substitutionId,
        ReviewTimetableSubstitutionRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimetableConflictDto>> DetectTimetableConflictsAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttendanceCorrectionRequestDto>> ListCorrectionRequestsAsync(
        ClaimsPrincipal actor,
        Guid? attendanceSessionId = null,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AttendanceCorrectionRequestDto>> RequestCorrectionAsync(
        ClaimsPrincipal actor,
        CreateAttendanceCorrectionRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AttendanceCorrectionRequestDto>> ReviewCorrectionAsync(
        ClaimsPrincipal actor,
        Guid correctionRequestId,
        ReviewAttendanceCorrectionRequest request,
        CancellationToken cancellationToken = default);

    Task<AttendanceAnalyticsDto> GetAttendanceAnalyticsAsync(
        ClaimsPrincipal actor,
        DateOnly? from = null,
        DateOnly? to = null,
        Guid? branchId = null,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<int>> QueueAttendanceNotificationsAsync(
        ClaimsPrincipal actor,
        QueueAttendanceNotificationsRequest request,
        CancellationToken cancellationToken = default);
}
