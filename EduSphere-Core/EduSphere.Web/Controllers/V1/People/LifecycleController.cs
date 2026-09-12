using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.People;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/people/lifecycle")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class LifecycleController : ApiControllerBase
{
    private readonly IStudentTeacherLifecycleService _lifecycle;

    public LifecycleController(IStudentTeacherLifecycleService lifecycle)
    {
        _lifecycle = lifecycle;
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudentEvents(
        [FromQuery] Guid? studentProfileId,
        [FromQuery] Guid? branchId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var events = await _lifecycle.ListStudentEventsAsync(
            User,
            studentProfileId,
            branchId,
            take,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<StudentLifecycleEventDto>>.Ok(events));
    }

    [HttpPost("students")]
    public async Task<IActionResult> RecordStudentEvent(
        [FromBody] CreateStudentLifecycleEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.RecordStudentEventAsync(User, request, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<StudentLifecycleEventDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeacherEvents(
        [FromQuery] Guid? teacherProfileId,
        [FromQuery] Guid? branchId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var events = await _lifecycle.ListTeacherEventsAsync(
            User,
            teacherProfileId,
            branchId,
            take,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<TeacherLifecycleEventDto>>.Ok(events));
    }

    [HttpPost("teachers")]
    public async Task<IActionResult> RecordTeacherEvent(
        [FromBody] CreateTeacherLifecycleEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.RecordTeacherEventAsync(User, request, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<TeacherLifecycleEventDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
