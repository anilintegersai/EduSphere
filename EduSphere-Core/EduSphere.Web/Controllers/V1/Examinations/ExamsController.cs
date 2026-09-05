using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Examinations;
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

namespace EduSphere.Web.Controllers.V1.Examinations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/exams")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class ExamsController : ApiControllerBase
{
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly IBranchAccessService _branchAccess;

    public ExamsController(
        ICrudService<Exam> exams,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        IBranchAccessService branchAccess)
    {
        _exams = exams;
        _branches = branches;
        _years = years;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? academicYearId,
        [FromQuery] ExamStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _exams.ListAsync(e =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && e.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || e.BranchId == branchId.Value) &&
            (!academicYearId.HasValue || e.AcademicYearId == academicYearId.Value) &&
            (!status.HasValue || e.Status == status.Value));

        return Ok(ApiResponse<IEnumerable<ExamDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _exams.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<ExamDto>.Fail($"Exam {id} was not found."))
            : Ok(ApiResponse<ExamDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _exams.CreateAsync(Apply(new Exam(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ExamDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExamRequest request)
    {
        var existing = await _exams.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Exam {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _exams.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Exam {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _exams.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Exam {id} was not found."));

        return await _exams.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Exam {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateExamRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _years.GetAsync(request.AcademicYearId) is null)
            return $"Academic year {request.AcademicYearId} was not found in this tenant.";
        return null;
    }

    private static Exam Apply(Exam entity, CreateExamRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.WeightagePercentage = request.WeightagePercentage;
        entity.Status = request.Status;
        entity.Instructions = request.Instructions;
        return entity;
    }

    private static ExamDto Map(Exam e) => new ExamDto
    {
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        Name = e.Name,
        Type = e.Type,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        WeightagePercentage = e.WeightagePercentage,
        Status = e.Status,
        Instructions = e.Instructions
    }.WithMetadata(e);
}
