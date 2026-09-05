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
[Route("api/v{version:apiVersion}/question-bank-items")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class QuestionBankItemsController : ApiControllerBase
{
    private readonly ICrudService<QuestionBankItem> _questions;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<SyllabusUnit> _units;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly IBranchAccessService _branchAccess;

    public QuestionBankItemsController(
        ICrudService<QuestionBankItem> questions,
        ICrudService<Branch> branches,
        ICrudService<Subject> subjects,
        ICrudService<SyllabusUnit> units,
        ICrudService<TeacherProfile> teachers,
        IBranchAccessService branchAccess)
    {
        _questions = questions;
        _branches = branches;
        _subjects = subjects;
        _units = units;
        _teachers = teachers;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? subjectId,
        [FromQuery] Guid? syllabusUnitId,
        [FromQuery] QuestionDifficulty? difficulty,
        [FromQuery] BloomLevel? bloomLevel)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _questions.ListAsync(q =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && q.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || q.BranchId == branchId.Value) &&
            (!subjectId.HasValue || q.SubjectId == subjectId.Value) &&
            (!syllabusUnitId.HasValue || q.SyllabusUnitId == syllabusUnitId.Value) &&
            (!difficulty.HasValue || q.Difficulty == difficulty.Value) &&
            (!bloomLevel.HasValue || q.BloomLevel == bloomLevel.Value));

        return Ok(ApiResponse<IEnumerable<QuestionBankItemDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _questions.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<QuestionBankItemDto>.Fail($"Question bank item {id} was not found."))
            : Ok(ApiResponse<QuestionBankItemDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionBankItemRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _questions.CreateAsync(Apply(new QuestionBankItem(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<QuestionBankItemDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionBankItemRequest request)
    {
        var existing = await _questions.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Question bank item {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _questions.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Question bank item {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _questions.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Question bank item {id} was not found."));

        return await _questions.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Question bank item {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateQuestionBankItemRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _subjects.GetAsync(request.SubjectId) is null)
            return $"Subject {request.SubjectId} was not found in this tenant.";

        if (request.SyllabusUnitId is Guid unitId)
        {
            var unit = await _units.GetAsync(unitId);
            if (unit is null)
                return $"Syllabus unit {unitId} was not found in this tenant.";
            if (unit.SubjectId != request.SubjectId)
                return "Selected syllabus unit must belong to the selected subject.";
        }

        if (request.AuthorTeacherProfileId is Guid teacherId)
        {
            var teacher = await _teachers.GetAsync(teacherId);
            if (teacher is null)
                return $"Teacher {teacherId} was not found in this tenant.";
            if (teacher.BranchId != request.BranchId)
                return "Question author must belong to the selected branch.";
        }

        return null;
    }

    private static QuestionBankItem Apply(QuestionBankItem entity, CreateQuestionBankItemRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.SubjectId = request.SubjectId;
        entity.SyllabusUnitId = request.SyllabusUnitId;
        entity.AuthorTeacherProfileId = request.AuthorTeacherProfileId;
        entity.QuestionType = request.QuestionType;
        entity.Difficulty = request.Difficulty;
        entity.BloomLevel = request.BloomLevel;
        entity.Source = request.Source;
        entity.Marks = request.Marks;
        entity.QuestionText = request.QuestionText;
        entity.ExpectedAnswer = request.ExpectedAnswer;
        entity.Tags = request.Tags;
        entity.ApprovalStatus = request.ApprovalStatus;
        entity.ApprovedByUserId = request.ApprovedByUserId;
        entity.ApprovedOn = request.ApprovedOn;
        return entity;
    }

    private static QuestionBankItemDto Map(QuestionBankItem e) => new QuestionBankItemDto
    {
        BranchId = e.BranchId,
        SubjectId = e.SubjectId,
        SyllabusUnitId = e.SyllabusUnitId,
        AuthorTeacherProfileId = e.AuthorTeacherProfileId,
        QuestionType = e.QuestionType,
        Difficulty = e.Difficulty,
        BloomLevel = e.BloomLevel,
        Source = e.Source,
        Marks = e.Marks,
        QuestionText = e.QuestionText,
        ExpectedAnswer = e.ExpectedAnswer,
        Tags = e.Tags,
        ApprovalStatus = e.ApprovalStatus,
        ApprovedByUserId = e.ApprovedByUserId,
        ApprovedOn = e.ApprovedOn
    }.WithMetadata(e);
}
