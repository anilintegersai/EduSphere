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

    [HttpGet("students/requests")]
    public async Task<IActionResult> GetStudentRequests(
        [FromQuery] Guid? studentProfileId,
        [FromQuery] Guid? branchId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var requests = await _lifecycle.ListStudentRequestsAsync(
            User,
            studentProfileId,
            branchId,
            take,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<StudentLifecycleRequestDto>>.Ok(requests));
    }

    [HttpPost("students/requests")]
    public async Task<IActionResult> CreateStudentRequest(
        [FromBody] CreateStudentLifecycleRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.CreateStudentRequestAsync(User, request, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<StudentLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("students/requests/{id:guid}/approve")]
    public async Task<IActionResult> ApproveStudentRequest(
        Guid id,
        [FromBody] DecideStudentLifecycleRequestRequest? request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.ApproveStudentRequestAsync(User, id, request?.DecisionNotes, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<StudentLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("students/requests/{id:guid}/reject")]
    public async Task<IActionResult> RejectStudentRequest(
        Guid id,
        [FromBody] DecideStudentLifecycleRequestRequest? request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.RejectStudentRequestAsync(User, id, request?.DecisionNotes, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<StudentLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("students/alumni")]
    public async Task<IActionResult> GetAlumniRecords(
        [FromQuery] Guid? branchId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var records = await _lifecycle.ListAlumniRecordsAsync(User, branchId, take, cancellationToken);
        return Ok(ApiResponse<IEnumerable<StudentAlumniRecordDto>>.Ok(records));
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

    [HttpGet("teachers/requests")]
    public async Task<IActionResult> GetTeacherRequests(
        [FromQuery] Guid? teacherProfileId,
        [FromQuery] Guid? branchId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        var requests = await _lifecycle.ListTeacherRequestsAsync(
            User,
            teacherProfileId,
            branchId,
            take,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<TeacherLifecycleRequestDto>>.Ok(requests));
    }

    [HttpPost("teachers/requests")]
    public async Task<IActionResult> CreateTeacherRequest(
        [FromBody] CreateTeacherLifecycleRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.CreateTeacherRequestAsync(User, request, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<TeacherLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("teachers/requests/{id:guid}/approve")]
    public async Task<IActionResult> ApproveTeacherRequest(
        Guid id,
        [FromBody] DecideTeacherLifecycleRequestRequest? request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.ApproveTeacherRequestAsync(User, id, request?.DecisionNotes, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<TeacherLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("teachers/requests/{id:guid}/reject")]
    public async Task<IActionResult> RejectTeacherRequest(
        Guid id,
        [FromBody] DecideTeacherLifecycleRequestRequest? request,
        CancellationToken cancellationToken = default)
    {
        var result = await _lifecycle.RejectTeacherRequestAsync(User, id, request?.DecisionNotes, cancellationToken);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<TeacherLifecycleRequestDto>.Ok(result.Data))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
