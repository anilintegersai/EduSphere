using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/attendance-sessions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.AttendanceMarker)]
[RequireTenant]
public class AttendanceSessionsController : ApiControllerBase
{
    private readonly ICrudService<AttendanceSession> _sessions;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<TimetableEntry> _timetableEntries;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<TeacherSubjectAssignment> _teacherAssignments;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchAccessService _branchAccess;

    public AttendanceSessionsController(
        ICrudService<AttendanceSession> sessions,
        ICrudService<Branch> branches,
        ICrudService<Section> sections,
        ICrudService<Subject> subjects,
        ICrudService<TimetableEntry> timetableEntries,
        ICrudService<TeacherProfile> teachers,
        ICrudService<TeacherSubjectAssignment> teacherAssignments,
        UserManager<ApplicationUser> userManager,
        IBranchAccessService branchAccess)
    {
        _sessions = sessions;
        _branches = branches;
        _sections = sections;
        _subjects = subjects;
        _timetableEntries = timetableEntries;
        _teachers = teachers;
        _teacherAssignments = teacherAssignments;
        _userManager = userManager;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? sectionId,
        [FromQuery] Guid? subjectId,
        [FromQuery] DateOnly? attendanceDate,
        [FromQuery] AttendanceSessionStatus? status)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _sessions.ListAsync(s =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)) &&
            (!sectionId.HasValue || s.SectionId == sectionId.Value) &&
            (!subjectId.HasValue || s.SubjectId == subjectId.Value) &&
            (!attendanceDate.HasValue || s.AttendanceDate == attendanceDate.Value) &&
            (!status.HasValue || s.Status == status.Value));
        return Ok(ApiResponse<IEnumerable<AttendanceSessionDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _sessions.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<AttendanceSessionDto>.Fail($"Attendance session {id} was not found."))
            : Ok(ApiResponse<AttendanceSessionDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceSessionRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        if (!await CanTeacherUseSectionAsync(request.SectionId, request.SubjectId))
            return Forbid();

        var created = await _sessions.CreateAsync(Apply(new AttendanceSession(), request, CurrentUserId()));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AttendanceSessionDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceSessionRequest request)
    {
        var existing = await _sessions.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Attendance session {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        if (!await CanTeacherUseSectionAsync(request.SectionId, request.SubjectId))
            return Forbid();

        var updated = await _sessions.UpdateAsync(id, e => Apply(e, request, CurrentUserId()));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Attendance session {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _sessions.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Attendance session {id} was not found."));

        return await _sessions.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Attendance session {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateAttendanceSessionRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _sections.GetAsync(request.SectionId) is null)
            return $"Section {request.SectionId} was not found in this tenant.";
        if (request.SubjectId is Guid subjectId && await _subjects.GetAsync(subjectId) is null)
            return $"Subject {subjectId} was not found in this tenant.";
        if (request.TimetableEntryId is Guid entryId && await _timetableEntries.GetAsync(entryId) is null)
            return $"Timetable entry {entryId} was not found in this tenant.";
        return null;
    }

    private async Task<bool> CanTeacherUseSectionAsync(Guid sectionId, Guid? subjectId)
    {
        if (!IsTeacherOnly())
            return true;

        if (!Guid.TryParse(_userManager.GetUserId(User), out var userId))
            return false;

        var teacher = (await _teachers.ListAsync(t => t.UserId == userId)).FirstOrDefault();
        if (teacher is null)
            return false;

        var assignments = await _teacherAssignments.ListAsync(a => a.TeacherProfileId == teacher.Id);
        if (assignments.Any(a =>
            (a.SectionId == sectionId || !a.SectionId.HasValue) &&
            (!subjectId.HasValue || a.SubjectId == subjectId.Value)))
        {
            return true;
        }

        return (await _sections.ListAsync(s => s.Id == sectionId && s.ClassTeacherUserId == userId)).Any();
    }

    private bool IsTeacherOnly()
        => User.IsInRole(Roles.Teacher) &&
           !User.IsInRole(Roles.SuperAdmin) &&
           !User.IsInRole(Roles.TenantAdmin) &&
           !User.IsInRole(Roles.BranchAdmin) &&
           !User.IsInRole(Roles.Principal);

    private Guid? CurrentUserId()
        => Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;

    private static AttendanceSession Apply(AttendanceSession entity, CreateAttendanceSessionRequest request, Guid? currentUserId)
    {
        entity.BranchId = request.BranchId;
        entity.SectionId = request.SectionId;
        entity.SubjectId = request.SubjectId;
        entity.TimetableEntryId = request.TimetableEntryId;
        entity.AttendanceDate = request.AttendanceDate;
        entity.SessionType = request.SessionType;
        entity.PeriodNumber = request.PeriodNumber;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.MarkedByUserId = request.MarkedByUserId ?? currentUserId;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }

    private static AttendanceSessionDto Map(AttendanceSession e) => new AttendanceSessionDto
    {
        BranchId = e.BranchId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        TimetableEntryId = e.TimetableEntryId,
        AttendanceDate = e.AttendanceDate,
        SessionType = e.SessionType,
        PeriodNumber = e.PeriodNumber,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        MarkedByUserId = e.MarkedByUserId,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);
}
