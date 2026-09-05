using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Examinations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Examinations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/grading-schemes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class GradingSchemesController : ApiControllerBase
{
    private readonly ICrudService<GradingScheme> _schemes;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Course> _courses;
    private readonly IBranchAccessService _branchAccess;

    public GradingSchemesController(
        ICrudService<GradingScheme> schemes,
        ICrudService<Branch> branches,
        ICrudService<Course> courses,
        IBranchAccessService branchAccess)
    {
        _schemes = schemes;
        _branches = branches;
        _courses = courses;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? branchId, [FromQuery] Guid? courseId)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _schemes.ListAsync(s =>
            (!_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && (s.BranchId == assignedBranchId.Value || s.BranchId == null))) &&
            (!branchId.HasValue || s.BranchId == branchId.Value) &&
            (!courseId.HasValue || s.CourseId == courseId.Value));

        return Ok(ApiResponse<IEnumerable<GradingSchemeDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _schemes.GetAsync(id);
        return entity is null || !await CanAccessSchemeAsync(entity)
            ? NotFound(ApiResponse<GradingSchemeDto>.Fail($"Grading scheme {id} was not found."))
            : Ok(ApiResponse<GradingSchemeDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradingSchemeRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _schemes.CreateAsync(Apply(new GradingScheme(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<GradingSchemeDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradingSchemeRequest request)
    {
        var existing = await _schemes.GetAsync(id);
        if (existing is null || !await CanManageSchemeAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Grading scheme {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _schemes.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Grading scheme {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _schemes.GetAsync(id);
        if (existing is null || !await CanManageSchemeAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Grading scheme {id} was not found."));

        return await _schemes.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Grading scheme {id} was not found."));
    }

    private async Task<bool> CanAccessSchemeAsync(GradingScheme scheme)
        => !scheme.BranchId.HasValue || await _branchAccess.CanAccessBranchAsync(User, scheme.BranchId.Value);

    private async Task<bool> CanManageSchemeAsync(GradingScheme scheme)
    {
        if (!_branchAccess.IsBranchAdminOnly(User))
            return await CanAccessSchemeAsync(scheme);

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        return assignedBranchId.HasValue && scheme.BranchId == assignedBranchId.Value;
    }

    private async Task<string?> ValidateReferencesAsync(CreateGradingSchemeRequest request)
    {
        if (_branchAccess.IsBranchAdminOnly(User) && !request.BranchId.HasValue)
            return "Branch administrators must create branch-specific grading schemes.";
        if (request.BranchId is Guid branchId)
        {
            if (await _branches.GetAsync(branchId) is null)
                return $"Branch {branchId} was not found in this tenant.";
            if (!await _branchAccess.CanAccessBranchAsync(User, branchId))
                return "You can manage grading schemes only for your assigned branch.";
        }
        if (request.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null)
            return $"Course {courseId} was not found in this tenant.";

        return null;
    }

    private static GradingScheme Apply(GradingScheme entity, CreateGradingSchemeRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.CourseId = request.CourseId;
        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.IsDefault = request.IsDefault;
        entity.Notes = request.Notes;
        return entity;
    }

    private static GradingSchemeDto Map(GradingScheme e) => new GradingSchemeDto
    {
        BranchId = e.BranchId,
        CourseId = e.CourseId,
        Name = e.Name,
        Type = e.Type,
        EffectiveFrom = e.EffectiveFrom,
        IsDefault = e.IsDefault,
        Notes = e.Notes
    }.WithMetadata(e);
}
