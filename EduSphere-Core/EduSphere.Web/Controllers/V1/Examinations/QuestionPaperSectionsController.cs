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
[Route("api/v{version:apiVersion}/question-paper-sections")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class QuestionPaperSectionsController : ApiControllerBase
{
    private readonly ICrudService<QuestionPaperSection> _sections;
    private readonly ICrudService<QuestionPaper> _papers;
    private readonly IBranchAccessService _branchAccess;

    public QuestionPaperSectionsController(
        ICrudService<QuestionPaperSection> sections,
        ICrudService<QuestionPaper> papers,
        IBranchAccessService branchAccess)
    {
        _sections = sections;
        _papers = papers;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? questionPaperId)
    {
        var allowedPaperIds = await GetAllowedPaperIdsAsync();
        if (questionPaperId.HasValue && !allowedPaperIds.Contains(questionPaperId.Value))
            return Forbid();

        var items = await _sections.ListAsync(s =>
            allowedPaperIds.Contains(s.QuestionPaperId) &&
            (!questionPaperId.HasValue || s.QuestionPaperId == questionPaperId.Value));
        return Ok(ApiResponse<IEnumerable<QuestionPaperSectionDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _sections.GetAsync(id);
        return entity is null || !await CanAccessSectionAsync(entity)
            ? NotFound(ApiResponse<QuestionPaperSectionDto>.Fail($"Question paper section {id} was not found."))
            : Ok(ApiResponse<QuestionPaperSectionDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionPaperSectionRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _sections.CreateAsync(Apply(new QuestionPaperSection(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<QuestionPaperSectionDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionPaperSectionRequest request)
    {
        var existing = await _sections.GetAsync(id);
        if (existing is null || !await CanAccessSectionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper section {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _sections.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Question paper section {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _sections.GetAsync(id);
        if (existing is null || !await CanAccessSectionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper section {id} was not found."));

        return await _sections.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Question paper section {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedPaperIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var papers = await _papers.ListAsync(p =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && p.BranchId == assignedBranchId.Value));
        return papers.Select(p => p.Id).ToHashSet();
    }

    private async Task<bool> CanAccessSectionAsync(QuestionPaperSection section)
    {
        var paper = await _papers.GetAsync(section.QuestionPaperId);
        return paper is not null && await _branchAccess.CanAccessBranchAsync(User, paper.BranchId);
    }

    private async Task<string?> ValidateReferencesAsync(CreateQuestionPaperSectionRequest request)
    {
        var paper = await _papers.GetAsync(request.QuestionPaperId);
        if (paper is null)
            return $"Question paper {request.QuestionPaperId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, paper.BranchId))
            return "You can manage question paper sections only for your assigned branch.";
        return null;
    }

    private static QuestionPaperSection Apply(QuestionPaperSection entity, CreateQuestionPaperSectionRequest request)
    {
        entity.QuestionPaperId = request.QuestionPaperId;
        entity.Code = request.Code;
        entity.Title = request.Title;
        entity.SortOrder = request.SortOrder;
        entity.Marks = request.Marks;
        entity.Instructions = request.Instructions;
        return entity;
    }

    private static QuestionPaperSectionDto Map(QuestionPaperSection e) => new QuestionPaperSectionDto
    {
        QuestionPaperId = e.QuestionPaperId,
        Code = e.Code,
        Title = e.Title,
        SortOrder = e.SortOrder,
        Marks = e.Marks,
        Instructions = e.Instructions
    }.WithMetadata(e);
}
