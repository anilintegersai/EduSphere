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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/attendance-records")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.AttendanceMarker)]
[RequireTenant]
public class AttendanceRecordsController : ApiControllerBase
{
    private readonly ICrudService<AttendanceRecord> _records;
    private readonly ICrudService<AttendanceSession> _sessions;
    private readonly ICrudService<StudentProfile> _students;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchAccessService _branchAccess;

    public AttendanceRecordsController(
        ICrudService<AttendanceRecord> records,
        ICrudService<AttendanceSession> sessions,
        ICrudService<StudentProfile> students,
        UserManager<ApplicationUser> userManager,
        IBranchAccessService branchAccess)
    {
        _records = records;
        _sessions = sessions;
        _students = students;
        _userManager = userManager;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? attendanceSessionId, [FromQuery] Guid? studentProfileId)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var allowedSessions = await _sessions.ListAsync(s =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value));
        var allowedSessionIds = allowedSessions.Select(s => s.Id).ToHashSet();

        if (attendanceSessionId.HasValue && !allowedSessionIds.Contains(attendanceSessionId.Value))
            return Forbid();

        var items = await _records.ListAsync(r =>
            (!_branchAccess.IsBranchAdminOnly(User) || allowedSessionIds.Contains(r.AttendanceSessionId)) &&
            (!attendanceSessionId.HasValue || r.AttendanceSessionId == attendanceSessionId.Value) &&
            (!studentProfileId.HasValue || r.StudentProfileId == studentProfileId.Value));
        return Ok(ApiResponse<IEnumerable<AttendanceRecordDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _records.GetAsync(id);
        var session = entity is null ? null : await _sessions.GetAsync(entity.AttendanceSessionId);
        return entity is null || session is null || !await _branchAccess.CanAccessBranchAsync(User, session.BranchId)
            ? NotFound(ApiResponse<AttendanceRecordDto>.Fail($"Attendance record {id} was not found."))
            : Ok(ApiResponse<AttendanceRecordDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceRecordRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _records.CreateAsync(Apply(new AttendanceRecord(), request, CurrentUserId()));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AttendanceRecordDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceRecordRequest request)
    {
        var existing = await _records.GetAsync(id);
        if (existing is null ||
            await _sessions.GetAsync(existing.AttendanceSessionId) is not { } existingSession ||
            !await _branchAccess.CanAccessBranchAsync(User, existingSession.BranchId))
        {
            return NotFound(ApiResponse<object>.Fail($"Attendance record {id} was not found."));
        }

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _records.UpdateAsync(id, e => Apply(e, request, CurrentUserId()));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Attendance record {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _records.GetAsync(id);
        if (existing is null ||
            await _sessions.GetAsync(existing.AttendanceSessionId) is not { } session ||
            !await _branchAccess.CanAccessBranchAsync(User, session.BranchId))
        {
            return NotFound(ApiResponse<object>.Fail($"Attendance record {id} was not found."));
        }

        return await _records.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Attendance record {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateAttendanceRecordRequest request)
    {
        var session = await _sessions.GetAsync(request.AttendanceSessionId);
        if (session is null)
            return $"Attendance session {request.AttendanceSessionId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, session.BranchId))
            return "You can manage attendance records only for your assigned branch.";

        var student = await _students.GetAsync(request.StudentProfileId);
        if (student is null)
            return $"Student {request.StudentProfileId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, student.BranchId))
            return "You can manage attendance records only for your assigned branch.";

        if (student.SectionId != session.SectionId)
            return "Student does not belong to the attendance session section.";

        return null;
    }

    private Guid? CurrentUserId()
        => Guid.TryParse(_userManager.GetUserId(User), out var userId) ? userId : null;

    private static AttendanceRecord Apply(AttendanceRecord entity, CreateAttendanceRecordRequest request, Guid? currentUserId)
    {
        entity.AttendanceSessionId = request.AttendanceSessionId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.Status = request.Status;
        entity.MarkedByUserId = request.MarkedByUserId ?? currentUserId;
        entity.MarkedOn = request.MarkedOn == default ? DateTime.UtcNow : request.MarkedOn;
        entity.Remarks = request.Remarks;
        return entity;
    }

    private static AttendanceRecordDto Map(AttendanceRecord e) => new AttendanceRecordDto
    {
        AttendanceSessionId = e.AttendanceSessionId,
        StudentProfileId = e.StudentProfileId,
        Status = e.Status,
        MarkedByUserId = e.MarkedByUserId,
        MarkedOn = e.MarkedOn,
        Remarks = e.Remarks
    }.WithMetadata(e);
}
