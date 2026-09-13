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
[Route("api/v{version:apiVersion}/timetable-workflow")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class TimetableWorkflowController : ApiControllerBase
{
    private readonly IAttendanceTimetableWorkflowService _workflow;

    public TimetableWorkflowController(IAttendanceTimetableWorkflowService workflow)
    {
        _workflow = workflow;
    }

    [HttpGet("conflicts")]
    public async Task<IActionResult> GetConflicts([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<TimetableConflictDto>>.Ok(
            await _workflow.DetectTimetableConflictsAsync(User, branchId)));

    [HttpGet("substitutions")]
    public async Task<IActionResult> GetSubstitutions([FromQuery] Guid? branchId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        => Ok(ApiResponse<IEnumerable<TimetableSubstitutionDto>>.Ok(
            await _workflow.ListSubstitutionsAsync(User, branchId, from, to)));

    [HttpPost("substitutions")]
    public async Task<IActionResult> CreateSubstitution([FromBody] CreateTimetableSubstitutionRequest request)
    {
        var result = await _workflow.CreateSubstitutionAsync(User, request);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<TimetableSubstitutionDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("substitutions/{id:guid}/review")]
    public async Task<IActionResult> ReviewSubstitution(Guid id, [FromBody] ReviewTimetableSubstitutionRequest request)
    {
        var result = await _workflow.ReviewSubstitutionAsync(User, id, request);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<TimetableSubstitutionDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
