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
[Route("api/v{version:apiVersion}/exam-schedules")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class ExamSchedulesController : ApiControllerBase
{
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Room> _rooms;
    private readonly IBranchAccessService _branchAccess;

    public ExamSchedulesController(
        ICrudService<ExamSchedule> schedules,
        ICrudService<Exam> exams,
        ICrudService<Section> sections,
        ICrudService<Subject> subjects,
        ICrudService<TeacherProfile> teachers,
        ICrudService<Room> rooms,
        IBranchAccessService branchAccess)
    {
        _schedules = schedules;
        _exams = exams;
        _sections = sections;
        _subjects = subjects;
        _teachers = teachers;
        _rooms = rooms;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? examId,
        [FromQuery] Guid? sectionId,
        [FromQuery] Guid? subjectId,
        [FromQuery] DateOnly? examDate)
    {
        var allowedExamIds = await GetAllowedExamIdsAsync();
        if (examId.HasValue && !allowedExamIds.Contains(examId.Value))
            return Forbid();

        var items = await _schedules.ListAsync(s =>
            (!_branchAccess.IsBranchAdminOnly(User) || allowedExamIds.Contains(s.ExamId)) &&
            (!examId.HasValue || s.ExamId == examId.Value) &&
            (!sectionId.HasValue || s.SectionId == sectionId.Value) &&
            (!subjectId.HasValue || s.SubjectId == subjectId.Value) &&
            (!examDate.HasValue || s.ExamDate == examDate.Value));

        return Ok(ApiResponse<IEnumerable<ExamScheduleDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _schedules.GetAsync(id);
        if (entity is null || !await CanAccessScheduleAsync(entity))
            return NotFound(ApiResponse<ExamScheduleDto>.Fail($"Exam schedule {id} was not found."));

        return Ok(ApiResponse<ExamScheduleDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamScheduleRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _schedules.CreateAsync(Apply(new ExamSchedule(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ExamScheduleDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExamScheduleRequest request)
    {
        var existing = await _schedules.GetAsync(id);
        if (existing is null || !await CanAccessScheduleAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Exam schedule {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _schedules.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Exam schedule {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _schedules.GetAsync(id);
        if (existing is null || !await CanAccessScheduleAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Exam schedule {id} was not found."));

        return await _schedules.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Exam schedule {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedExamIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var exams = await _exams.ListAsync(e =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && e.BranchId == assignedBranchId.Value));
        return exams.Select(e => e.Id).ToHashSet();
    }

    private async Task<bool> CanAccessScheduleAsync(ExamSchedule schedule)
    {
        var exam = await _exams.GetAsync(schedule.ExamId);
        return exam is not null && await _branchAccess.CanAccessBranchAsync(User, exam.BranchId);
    }

    private async Task<string?> ValidateReferencesAsync(CreateExamScheduleRequest request)
    {
        var exam = await _exams.GetAsync(request.ExamId);
        if (exam is null)
            return $"Exam {request.ExamId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, exam.BranchId))
            return "You can manage exam schedules only for your assigned branch.";
        if (request.ExamDate < exam.StartDate || request.ExamDate > exam.EndDate)
            return "Exam date must fall inside the selected exam date range.";
        if (await _sections.GetAsync(request.SectionId) is null)
            return $"Section {request.SectionId} was not found in this tenant.";
        if (await _subjects.GetAsync(request.SubjectId) is null)
            return $"Subject {request.SubjectId} was not found in this tenant.";

        if (request.TeacherProfileId is Guid teacherId)
        {
            var teacher = await _teachers.GetAsync(teacherId);
            if (teacher is null)
                return $"Teacher {teacherId} was not found in this tenant.";
            if (teacher.BranchId != exam.BranchId)
                return "Selected teacher must belong to the exam branch.";
        }

        if (request.RoomId is Guid roomId)
        {
            var room = await _rooms.GetAsync(roomId);
            if (room is null)
                return $"Room {roomId} was not found in this tenant.";
            if (room.BranchId != exam.BranchId)
                return "Selected room must belong to the exam branch.";
        }

        return null;
    }

    private static ExamSchedule Apply(ExamSchedule entity, CreateExamScheduleRequest request)
    {
        entity.ExamId = request.ExamId;
        entity.SectionId = request.SectionId;
        entity.SubjectId = request.SubjectId;
        entity.TeacherProfileId = request.TeacherProfileId;
        entity.RoomId = request.RoomId;
        entity.ExamDate = request.ExamDate;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.DurationMinutes = request.DurationMinutes;
        entity.MaximumMarks = request.MaximumMarks;
        entity.PassingMarks = request.PassingMarks;
        entity.Status = request.Status;
        entity.SeatingPlanNotes = request.SeatingPlanNotes;
        entity.Instructions = request.Instructions;
        return entity;
    }

    private static ExamScheduleDto Map(ExamSchedule e) => new ExamScheduleDto
    {
        ExamId = e.ExamId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        TeacherProfileId = e.TeacherProfileId,
        RoomId = e.RoomId,
        ExamDate = e.ExamDate,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        DurationMinutes = e.DurationMinutes,
        MaximumMarks = e.MaximumMarks,
        PassingMarks = e.PassingMarks,
        Status = e.Status,
        SeatingPlanNotes = e.SeatingPlanNotes,
        Instructions = e.Instructions
    }.WithMetadata(e);
}
