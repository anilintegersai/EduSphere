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
[Route("api/v{version:apiVersion}/question-papers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class QuestionPapersController : ApiControllerBase
{
    private readonly ICrudService<QuestionPaper> _papers;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly IBranchAccessService _branchAccess;

    public QuestionPapersController(
        ICrudService<QuestionPaper> papers,
        ICrudService<Branch> branches,
        ICrudService<Subject> subjects,
        ICrudService<Exam> exams,
        ICrudService<ExamSchedule> schedules,
        IBranchAccessService branchAccess)
    {
        _papers = papers;
        _branches = branches;
        _subjects = subjects;
        _exams = exams;
        _schedules = schedules;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? subjectId,
        [FromQuery] Guid? examId,
        [FromQuery] ApprovalStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _papers.ListAsync(p =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && p.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || p.BranchId == branchId.Value) &&
            (!subjectId.HasValue || p.SubjectId == subjectId.Value) &&
            (!examId.HasValue || p.ExamId == examId.Value) &&
            (!status.HasValue || p.Status == status.Value));

        return Ok(ApiResponse<IEnumerable<QuestionPaperDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _papers.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<QuestionPaperDto>.Fail($"Question paper {id} was not found."))
            : Ok(ApiResponse<QuestionPaperDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionPaperRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _papers.CreateAsync(Apply(new QuestionPaper(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<QuestionPaperDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionPaperRequest request)
    {
        var existing = await _papers.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Question paper {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _papers.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Question paper {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _papers.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Question paper {id} was not found."));

        return await _papers.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Question paper {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateQuestionPaperRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _subjects.GetAsync(request.SubjectId) is null)
            return $"Subject {request.SubjectId} was not found in this tenant.";

        if (request.ExamId is Guid examId)
        {
            var exam = await _exams.GetAsync(examId);
            if (exam is null)
                return $"Exam {examId} was not found in this tenant.";
            if (exam.BranchId != request.BranchId)
                return "Selected exam must belong to the question paper branch.";
        }

        if (request.ExamScheduleId is Guid scheduleId)
        {
            var schedule = await _schedules.GetAsync(scheduleId);
            if (schedule is null)
                return $"Exam schedule {scheduleId} was not found in this tenant.";
            var scheduleExam = await _exams.GetAsync(schedule.ExamId);
            if (scheduleExam is null)
                return "Selected exam schedule has no valid exam.";
            if (scheduleExam.BranchId != request.BranchId)
                return "Selected exam schedule must belong to the question paper branch.";
            if (schedule.SubjectId != request.SubjectId)
                return "Selected exam schedule must use the question paper subject.";
            if (request.ExamId.HasValue && schedule.ExamId != request.ExamId.Value)
                return "Selected exam schedule must belong to the selected exam.";
        }

        return null;
    }

    private static QuestionPaper Apply(QuestionPaper entity, CreateQuestionPaperRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.SubjectId = request.SubjectId;
        entity.ExamId = request.ExamId;
        entity.ExamScheduleId = request.ExamScheduleId;
        entity.Title = request.Title;
        entity.TotalMarks = request.TotalMarks;
        entity.DurationMinutes = request.DurationMinutes;
        entity.Instructions = request.Instructions;
        entity.Source = request.Source;
        entity.Status = request.Status;
        entity.ApprovedByUserId = request.ApprovedByUserId;
        entity.ApprovedOn = request.ApprovedOn;
        return entity;
    }

    private static QuestionPaperDto Map(QuestionPaper e) => new QuestionPaperDto
    {
        BranchId = e.BranchId,
        SubjectId = e.SubjectId,
        ExamId = e.ExamId,
        ExamScheduleId = e.ExamScheduleId,
        Title = e.Title,
        TotalMarks = e.TotalMarks,
        DurationMinutes = e.DurationMinutes,
        Instructions = e.Instructions,
        Source = e.Source,
        Status = e.Status,
        ApprovedByUserId = e.ApprovedByUserId,
        ApprovedOn = e.ApprovedOn
    }.WithMetadata(e);
}
