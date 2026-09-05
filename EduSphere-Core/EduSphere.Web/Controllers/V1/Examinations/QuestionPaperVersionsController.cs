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
[Route("api/v{version:apiVersion}/question-paper-versions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class QuestionPaperVersionsController : ApiControllerBase
{
    private readonly ICrudService<QuestionPaperVersion> _versions;
    private readonly ICrudService<QuestionPaper> _papers;
    private readonly IBranchAccessService _branchAccess;

    public QuestionPaperVersionsController(
        ICrudService<QuestionPaperVersion> versions,
        ICrudService<QuestionPaper> papers,
        IBranchAccessService branchAccess)
    {
        _versions = versions;
        _papers = papers;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? questionPaperId)
    {
        var allowedPaperIds = await GetAllowedPaperIdsAsync();
        if (questionPaperId.HasValue && !allowedPaperIds.Contains(questionPaperId.Value))
            return Forbid();

        var items = await _versions.ListAsync(v =>
            allowedPaperIds.Contains(v.QuestionPaperId) &&
            (!questionPaperId.HasValue || v.QuestionPaperId == questionPaperId.Value));
        return Ok(ApiResponse<IEnumerable<QuestionPaperVersionDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _versions.GetAsync(id);
        return entity is null || !await CanAccessVersionAsync(entity)
            ? NotFound(ApiResponse<QuestionPaperVersionDto>.Fail($"Question paper version {id} was not found."))
            : Ok(ApiResponse<QuestionPaperVersionDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionPaperVersionRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _versions.CreateAsync(Apply(new QuestionPaperVersion(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<QuestionPaperVersionDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionPaperVersionRequest request)
    {
        var existing = await _versions.GetAsync(id);
        if (existing is null || !await CanAccessVersionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper version {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _versions.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Question paper version {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _versions.GetAsync(id);
        if (existing is null || !await CanAccessVersionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper version {id} was not found."));

        return await _versions.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Question paper version {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedPaperIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var papers = await _papers.ListAsync(p =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && p.BranchId == assignedBranchId.Value));
        return papers.Select(p => p.Id).ToHashSet();
    }

    private async Task<bool> CanAccessVersionAsync(QuestionPaperVersion version)
    {
        var paper = await _papers.GetAsync(version.QuestionPaperId);
        return paper is not null && await _branchAccess.CanAccessBranchAsync(User, paper.BranchId);
    }

    private async Task<string?> ValidateReferencesAsync(CreateQuestionPaperVersionRequest request)
    {
        var paper = await _papers.GetAsync(request.QuestionPaperId);
        if (paper is null)
            return $"Question paper {request.QuestionPaperId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, paper.BranchId))
            return "You can manage question paper versions only for your assigned branch.";
        return null;
    }

    private static QuestionPaperVersion Apply(QuestionPaperVersion entity, CreateQuestionPaperVersionRequest request)
    {
        entity.QuestionPaperId = request.QuestionPaperId;
        entity.VersionNumber = request.VersionNumber;
        entity.Source = request.Source;
        entity.Status = request.Status;
        entity.CreatedByUserId = request.CreatedByUserId;
        entity.StudentContentJson = request.StudentContentJson;
        entity.SolutionContentJson = request.SolutionContentJson;
        entity.Notes = request.Notes;
        return entity;
    }

    private static QuestionPaperVersionDto Map(QuestionPaperVersion e) => new QuestionPaperVersionDto
    {
        QuestionPaperId = e.QuestionPaperId,
        VersionNumber = e.VersionNumber,
        Source = e.Source,
        Status = e.Status,
        CreatedByUserId = e.CreatedByUserId,
        StudentContentJson = e.StudentContentJson,
        SolutionContentJson = e.SolutionContentJson,
        Notes = e.Notes
    }.WithMetadata(e);
}
