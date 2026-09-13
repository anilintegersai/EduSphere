using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/attendance-workflow")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.AttendanceMarker)]
[RequireTenant]
public class AttendanceWorkflowController : ApiControllerBase
{
    private readonly IAttendanceTimetableWorkflowService _workflow;

    public AttendanceWorkflowController(IAttendanceTimetableWorkflowService workflow)
    {
        _workflow = workflow;
    }

    [HttpGet("corrections")]
    public async Task<IActionResult> GetCorrections([FromQuery] Guid? attendanceSessionId)
        => Ok(ApiResponse<IEnumerable<AttendanceCorrectionRequestDto>>.Ok(
            await _workflow.ListCorrectionRequestsAsync(User, attendanceSessionId)));

    [HttpPost("corrections")]
    public async Task<IActionResult> RequestCorrection([FromBody] CreateAttendanceCorrectionRequest request)
    {
        var result = await _workflow.RequestCorrectionAsync(User, request);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<AttendanceCorrectionRequestDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("corrections/{id:guid}/review")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
    public async Task<IActionResult> ReviewCorrection(Guid id, [FromBody] ReviewAttendanceCorrectionRequest request)
    {
        var result = await _workflow.ReviewCorrectionAsync(User, id, request);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AttendanceCorrectionRequestDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] Guid? branchId)
        => Ok(ApiResponse<AttendanceAnalyticsDto>.Ok(await _workflow.GetAttendanceAnalyticsAsync(User, from, to, branchId)));

    [HttpPost("notifications")]
    public async Task<IActionResult> QueueNotifications([FromBody] QueueAttendanceNotificationsRequest request)
    {
        var result = await _workflow.QueueAttendanceNotificationsAsync(User, request);
        return result.Succeeded
            ? Ok(ApiResponse<int>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
