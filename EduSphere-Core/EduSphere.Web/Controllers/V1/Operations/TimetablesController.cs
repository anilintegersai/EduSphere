using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/timetables")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class TimetablesController : ApiControllerBase
{
    private readonly ICrudService<Timetable> _timetables;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Section> _sections;
    private readonly IBranchAccessService _branchAccess;

    public TimetablesController(
        ICrudService<Timetable> timetables,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Section> sections,
        IBranchAccessService branchAccess)
    {
        _timetables = timetables;
        _branches = branches;
        _academicYears = academicYears;
        _sections = sections;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? sectionId, [FromQuery] TimetableStatus? status)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _timetables.ListAsync(t =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)) &&
            (!sectionId.HasValue || t.SectionId == sectionId.Value) &&
            (!status.HasValue || t.Status == status.Value));
        return Ok(ApiResponse<IEnumerable<TimetableDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _timetables.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<TimetableDto>.Fail($"Timetable {id} was not found."))
            : Ok(ApiResponse<TimetableDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimetableRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _timetables.CreateAsync(Apply(new Timetable(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TimetableDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimetableRequest request)
    {
        var existing = await _timetables.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Timetable {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _timetables.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Timetable {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _timetables.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Timetable {id} was not found."));

        return await _timetables.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Timetable {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateTimetableRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _academicYears.GetAsync(request.AcademicYearId) is null)
            return $"Academic year {request.AcademicYearId} was not found in this tenant.";
        if (await _sections.GetAsync(request.SectionId) is null)
            return $"Section {request.SectionId} was not found in this tenant.";
        return null;
    }

    private static Timetable Apply(Timetable entity, CreateTimetableRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.SectionId = request.SectionId;
        entity.Name = request.Name;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.Status = request.Status;
        return entity;
    }

    private static TimetableDto Map(Timetable e) => new TimetableDto
    {
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        SectionId = e.SectionId,
        Name = e.Name,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo,
        Status = e.Status
    }.WithMetadata(e);
}
