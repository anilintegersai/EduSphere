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
[Route("api/v{version:apiVersion}/question-paper-questions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class QuestionPaperQuestionsController : ApiControllerBase
{
    private readonly ICrudService<QuestionPaperQuestion> _questions;
    private readonly ICrudService<QuestionPaperSection> _sections;
    private readonly ICrudService<QuestionPaper> _papers;
    private readonly ICrudService<QuestionBankItem> _bankItems;
    private readonly IBranchAccessService _branchAccess;

    public QuestionPaperQuestionsController(
        ICrudService<QuestionPaperQuestion> questions,
        ICrudService<QuestionPaperSection> sections,
        ICrudService<QuestionPaper> papers,
        ICrudService<QuestionBankItem> bankItems,
        IBranchAccessService branchAccess)
    {
        _questions = questions;
        _sections = sections;
        _papers = papers;
        _bankItems = bankItems;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? questionPaperSectionId)
    {
        var allowedSectionIds = await GetAllowedSectionIdsAsync();
        if (questionPaperSectionId.HasValue && !allowedSectionIds.Contains(questionPaperSectionId.Value))
            return Forbid();

        var items = await _questions.ListAsync(q =>
            allowedSectionIds.Contains(q.QuestionPaperSectionId) &&
            (!questionPaperSectionId.HasValue || q.QuestionPaperSectionId == questionPaperSectionId.Value));
        return Ok(ApiResponse<IEnumerable<QuestionPaperQuestionDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _questions.GetAsync(id);
        return entity is null || !await CanAccessQuestionAsync(entity)
            ? NotFound(ApiResponse<QuestionPaperQuestionDto>.Fail($"Question paper question {id} was not found."))
            : Ok(ApiResponse<QuestionPaperQuestionDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionPaperQuestionRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _questions.CreateAsync(Apply(new QuestionPaperQuestion(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<QuestionPaperQuestionDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionPaperQuestionRequest request)
    {
        var existing = await _questions.GetAsync(id);
        if (existing is null || !await CanAccessQuestionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper question {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _questions.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Question paper question {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _questions.GetAsync(id);
        if (existing is null || !await CanAccessQuestionAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Question paper question {id} was not found."));

        return await _questions.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Question paper question {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedSectionIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var papers = await _papers.ListAsync(p =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && p.BranchId == assignedBranchId.Value));
        var paperIds = papers.Select(p => p.Id).ToHashSet();
        var sections = await _sections.ListAsync(s => paperIds.Contains(s.QuestionPaperId));
        return sections.Select(s => s.Id).ToHashSet();
    }

    private async Task<QuestionPaper?> GetPaperForSectionAsync(Guid sectionId)
    {
        var section = await _sections.GetAsync(sectionId);
        return section is null ? null : await _papers.GetAsync(section.QuestionPaperId);
    }

    private async Task<bool> CanAccessQuestionAsync(QuestionPaperQuestion question)
    {
        var paper = await GetPaperForSectionAsync(question.QuestionPaperSectionId);
        return paper is not null && await _branchAccess.CanAccessBranchAsync(User, paper.BranchId);
    }

    private async Task<string?> ValidateReferencesAsync(CreateQuestionPaperQuestionRequest request)
    {
        var section = await _sections.GetAsync(request.QuestionPaperSectionId);
        if (section is null)
            return $"Question paper section {request.QuestionPaperSectionId} was not found in this tenant.";

        var paper = await _papers.GetAsync(section.QuestionPaperId);
        if (paper is null)
            return "Selected question paper section has no valid paper.";
        if (!await _branchAccess.CanAccessBranchAsync(User, paper.BranchId))
            return "You can manage question paper questions only for your assigned branch.";

        if (request.QuestionBankItemId is Guid bankItemId)
        {
            var bankItem = await _bankItems.GetAsync(bankItemId);
            if (bankItem is null)
                return $"Question bank item {bankItemId} was not found in this tenant.";
            if (bankItem.BranchId != paper.BranchId)
                return "Question bank item must belong to the same branch as the paper.";
            if (bankItem.SubjectId != paper.SubjectId)
                return "Question bank item must belong to the same subject as the paper.";
        }

        return null;
    }

    private static QuestionPaperQuestion Apply(QuestionPaperQuestion entity, CreateQuestionPaperQuestionRequest request)
    {
        entity.QuestionPaperSectionId = request.QuestionPaperSectionId;
        entity.QuestionBankItemId = request.QuestionBankItemId;
        entity.SortOrder = request.SortOrder;
        entity.QuestionType = request.QuestionType;
        entity.Difficulty = request.Difficulty;
        entity.BloomLevel = request.BloomLevel;
        entity.Marks = request.Marks;
        entity.QuestionText = request.QuestionText;
        entity.SolutionText = request.SolutionText;
        return entity;
    }

    private static QuestionPaperQuestionDto Map(QuestionPaperQuestion e) => new QuestionPaperQuestionDto
    {
        QuestionPaperSectionId = e.QuestionPaperSectionId,
        QuestionBankItemId = e.QuestionBankItemId,
        SortOrder = e.SortOrder,
        QuestionType = e.QuestionType,
        Difficulty = e.Difficulty,
        BloomLevel = e.BloomLevel,
        Marks = e.Marks,
        QuestionText = e.QuestionText,
        SolutionText = e.SolutionText
    }.WithMetadata(e);
}
