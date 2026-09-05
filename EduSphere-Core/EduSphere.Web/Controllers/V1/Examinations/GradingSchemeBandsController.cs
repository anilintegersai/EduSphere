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
[Route("api/v{version:apiVersion}/grading-scheme-bands")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class GradingSchemeBandsController : ApiControllerBase
{
    private readonly ICrudService<GradingSchemeBand> _bands;
    private readonly ICrudService<GradingScheme> _schemes;
    private readonly IBranchAccessService _branchAccess;

    public GradingSchemeBandsController(
        ICrudService<GradingSchemeBand> bands,
        ICrudService<GradingScheme> schemes,
        IBranchAccessService branchAccess)
    {
        _bands = bands;
        _schemes = schemes;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? gradingSchemeId)
    {
        var allowedSchemeIds = await GetAllowedSchemeIdsAsync();
        if (gradingSchemeId.HasValue && !allowedSchemeIds.Contains(gradingSchemeId.Value))
            return Forbid();

        var items = await _bands.ListAsync(b =>
            allowedSchemeIds.Contains(b.GradingSchemeId) &&
            (!gradingSchemeId.HasValue || b.GradingSchemeId == gradingSchemeId.Value));
        return Ok(ApiResponse<IEnumerable<GradingSchemeBandDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _bands.GetAsync(id);
        return entity is null || !await CanAccessBandAsync(entity)
            ? NotFound(ApiResponse<GradingSchemeBandDto>.Fail($"Grading scheme band {id} was not found."))
            : Ok(ApiResponse<GradingSchemeBandDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradingSchemeBandRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _bands.CreateAsync(Apply(new GradingSchemeBand(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<GradingSchemeBandDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradingSchemeBandRequest request)
    {
        var existing = await _bands.GetAsync(id);
        if (existing is null || !await CanManageBandAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Grading scheme band {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _bands.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Grading scheme band {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _bands.GetAsync(id);
        if (existing is null || !await CanManageBandAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Grading scheme band {id} was not found."));

        return await _bands.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Grading scheme band {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedSchemeIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var schemes = await _schemes.ListAsync(s =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && (s.BranchId == assignedBranchId.Value || s.BranchId == null)));
        return schemes.Select(s => s.Id).ToHashSet();
    }

    private async Task<bool> CanAccessBandAsync(GradingSchemeBand band)
    {
        var scheme = await _schemes.GetAsync(band.GradingSchemeId);
        return scheme is not null &&
               (!scheme.BranchId.HasValue || await _branchAccess.CanAccessBranchAsync(User, scheme.BranchId.Value));
    }

    private async Task<bool> CanManageBandAsync(GradingSchemeBand band)
    {
        var scheme = await _schemes.GetAsync(band.GradingSchemeId);
        if (scheme is null)
            return false;

        if (!_branchAccess.IsBranchAdminOnly(User))
            return !scheme.BranchId.HasValue || await _branchAccess.CanAccessBranchAsync(User, scheme.BranchId.Value);

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        return assignedBranchId.HasValue && scheme.BranchId == assignedBranchId.Value;
    }

    private async Task<string?> ValidateReferencesAsync(CreateGradingSchemeBandRequest request)
    {
        var scheme = await _schemes.GetAsync(request.GradingSchemeId);
        if (scheme is null)
            return $"Grading scheme {request.GradingSchemeId} was not found in this tenant.";
        if (!await CanManageBandAsync(new GradingSchemeBand { GradingSchemeId = scheme.Id }))
            return "You can manage grading bands only for your assigned branch.";
        return null;
    }

    private static GradingSchemeBand Apply(GradingSchemeBand entity, CreateGradingSchemeBandRequest request)
    {
        entity.GradingSchemeId = request.GradingSchemeId;
        entity.Grade = request.Grade;
        entity.MinimumPercentage = request.MinimumPercentage;
        entity.MaximumPercentage = request.MaximumPercentage;
        entity.GradePoint = request.GradePoint;
        entity.SortOrder = request.SortOrder;
        entity.Remarks = request.Remarks;
        return entity;
    }

    private static GradingSchemeBandDto Map(GradingSchemeBand e) => new GradingSchemeBandDto
    {
        GradingSchemeId = e.GradingSchemeId,
        Grade = e.Grade,
        MinimumPercentage = e.MinimumPercentage,
        MaximumPercentage = e.MaximumPercentage,
        GradePoint = e.GradePoint,
        SortOrder = e.SortOrder,
        Remarks = e.Remarks
    }.WithMetadata(e);
}
