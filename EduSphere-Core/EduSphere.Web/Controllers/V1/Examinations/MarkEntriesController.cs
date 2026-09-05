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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Examinations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mark-entries")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class MarkEntriesController : ApiControllerBase
{
    private readonly ICrudService<MarkEntry> _marks;
    private readonly ICrudService<ExamSchedule> _schedules;
    private readonly ICrudService<Exam> _exams;
    private readonly ICrudService<StudentProfile> _students;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchAccessService _branchAccess;

    public MarkEntriesController(
        ICrudService<MarkEntry> marks,
        ICrudService<ExamSchedule> schedules,
        ICrudService<Exam> exams,
        ICrudService<StudentProfile> students,
        UserManager<ApplicationUser> userManager,
        IBranchAccessService branchAccess)
    {
        _marks = marks;
        _schedules = schedules;
        _exams = exams;
        _students = students;
        _userManager = userManager;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? examScheduleId, [FromQuery] Guid? studentProfileId)
    {
        var allowedScheduleIds = await GetAllowedScheduleIdsAsync();
        if (examScheduleId.HasValue && !allowedScheduleIds.Contains(examScheduleId.Value))
            return Forbid();

        var items = await _marks.ListAsync(m =>
            allowedScheduleIds.Contains(m.ExamScheduleId) &&
            (!examScheduleId.HasValue || m.ExamScheduleId == examScheduleId.Value) &&
            (!studentProfileId.HasValue || m.StudentProfileId == studentProfileId.Value));
        return Ok(ApiResponse<IEnumerable<MarkEntryDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _marks.GetAsync(id);
        return entity is null || !await CanAccessMarkAsync(entity)
            ? NotFound(ApiResponse<MarkEntryDto>.Fail($"Mark entry {id} was not found."))
            : Ok(ApiResponse<MarkEntryDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMarkEntryRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _marks.CreateAsync(Apply(new MarkEntry(), request, CurrentUserId()));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<MarkEntryDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMarkEntryRequest request)
    {
        var existing = await _marks.GetAsync(id);
        if (existing is null || !await CanAccessMarkAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Mark entry {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _marks.UpdateAsync(id, e => Apply(e, request, CurrentUserId()));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Mark entry {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _marks.GetAsync(id);
        if (existing is null || !await CanAccessMarkAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Mark entry {id} was not found."));

        return await _marks.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Mark entry {id} was not found."));
    }

    private async Task<HashSet<Guid>> GetAllowedScheduleIdsAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var exams = await _exams.ListAsync(e =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && e.BranchId == assignedBranchId.Value));
        var examIds = exams.Select(e => e.Id).ToHashSet();
        var schedules = await _schedules.ListAsync(s => examIds.Contains(s.ExamId));
        return schedules.Select(s => s.Id).ToHashSet();
    }

    private async Task<bool> CanAccessMarkAsync(MarkEntry mark)
    {
        var schedule = await _schedules.GetAsync(mark.ExamScheduleId);
        var exam = schedule is null ? null : await _exams.GetAsync(schedule.ExamId);
        return exam is not null && await _branchAccess.CanAccessBranchAsync(User, exam.BranchId);
    }

    private async Task<string?> ValidateReferencesAsync(CreateMarkEntryRequest request)
    {
        var schedule = await _schedules.GetAsync(request.ExamScheduleId);
        if (schedule is null)
            return $"Exam schedule {request.ExamScheduleId} was not found in this tenant.";
        var exam = await _exams.GetAsync(schedule.ExamId);
        if (exam is null)
            return "Selected exam schedule has no valid exam.";
        if (!await _branchAccess.CanAccessBranchAsync(User, exam.BranchId))
            return "You can manage marks only for your assigned branch.";
        if (request.MarksObtained > schedule.MaximumMarks)
            return "Marks obtained cannot exceed schedule maximum marks.";

        var student = await _students.GetAsync(request.StudentProfileId);
        if (student is null)
            return $"Student {request.StudentProfileId} was not found in this tenant.";
        if (student.BranchId != exam.BranchId)
            return "Student must belong to the exam branch.";
        if (student.SectionId != schedule.SectionId)
            return "Student must belong to the exam schedule section.";

        return null;
    }

    private Guid? CurrentUserId()
        => Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;

    private static MarkEntry Apply(MarkEntry entity, CreateMarkEntryRequest request, Guid? currentUserId)
    {
        entity.ExamScheduleId = request.ExamScheduleId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.IsAbsent = request.IsAbsent;
        entity.MarksObtained = request.IsAbsent ? 0 : request.MarksObtained;
        entity.Grade = request.Grade;
        entity.GradePoint = request.GradePoint;
        entity.EnteredByUserId = request.EnteredByUserId ?? currentUserId;
        entity.EnteredOn = request.EnteredOn == default ? DateTime.UtcNow : request.EnteredOn;
        entity.Remarks = request.Remarks;
        return entity;
    }

    private static MarkEntryDto Map(MarkEntry e) => new MarkEntryDto
    {
        ExamScheduleId = e.ExamScheduleId,
        StudentProfileId = e.StudentProfileId,
        MarksObtained = e.MarksObtained,
        IsAbsent = e.IsAbsent,
        Grade = e.Grade,
        GradePoint = e.GradePoint,
        EnteredByUserId = e.EnteredByUserId,
        EnteredOn = e.EnteredOn,
        Remarks = e.Remarks
    }.WithMetadata(e);
}
