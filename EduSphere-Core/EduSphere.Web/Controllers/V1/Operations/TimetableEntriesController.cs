using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/timetable-entries")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class TimetableEntriesController : ApiControllerBase
{
    private readonly ICrudService<TimetableEntry> _entries;
    private readonly ICrudService<Timetable> _timetables;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<TimeSlot> _slots;
    private readonly ICrudService<Room> _rooms;
    private readonly IBranchAccessService _branchAccess;

    public TimetableEntriesController(
        ICrudService<TimetableEntry> entries,
        ICrudService<Timetable> timetables,
        ICrudService<Section> sections,
        ICrudService<Subject> subjects,
        ICrudService<TeacherProfile> teachers,
        ICrudService<TimeSlot> slots,
        ICrudService<Room> rooms,
        IBranchAccessService branchAccess)
    {
        _entries = entries;
        _timetables = timetables;
        _sections = sections;
        _subjects = subjects;
        _teachers = teachers;
        _slots = slots;
        _rooms = rooms;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? timetableId, [FromQuery] Guid? teacherProfileId)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var allowedTimetables = await _timetables.ListAsync(t =>
            !_branchAccess.IsBranchAdminOnly(User) ||
            (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value));
        var allowedTimetableIds = allowedTimetables.Select(t => t.Id).ToHashSet();

        if (timetableId.HasValue && !allowedTimetableIds.Contains(timetableId.Value))
            return Forbid();

        var items = await _entries.ListAsync(e =>
            (!_branchAccess.IsBranchAdminOnly(User) || allowedTimetableIds.Contains(e.TimetableId)) &&
            (!timetableId.HasValue || e.TimetableId == timetableId.Value) &&
            (!teacherProfileId.HasValue || e.TeacherProfileId == teacherProfileId.Value));
        return Ok(ApiResponse<IEnumerable<TimetableEntryDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _entries.GetAsync(id);
        var timetable = entity is null ? null : await _timetables.GetAsync(entity.TimetableId);
        return entity is null || timetable is null || !await _branchAccess.CanAccessBranchAsync(User, timetable.BranchId)
            ? NotFound(ApiResponse<TimetableEntryDto>.Fail($"Timetable entry {id} was not found."))
            : Ok(ApiResponse<TimetableEntryDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimetableEntryRequest request)
    {
        var validationError = await ValidateReferencesAndConflictsAsync(request, null);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _entries.CreateAsync(Apply(new TimetableEntry(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TimetableEntryDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimetableEntryRequest request)
    {
        var existing = await _entries.GetAsync(id);
        if (existing is null ||
            await _timetables.GetAsync(existing.TimetableId) is not { } existingTimetable ||
            !await _branchAccess.CanAccessBranchAsync(User, existingTimetable.BranchId))
        {
            return NotFound(ApiResponse<object>.Fail($"Timetable entry {id} was not found."));
        }

        var validationError = await ValidateReferencesAndConflictsAsync(request, id);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _entries.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Timetable entry {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _entries.GetAsync(id);
        if (existing is null ||
            await _timetables.GetAsync(existing.TimetableId) is not { } timetable ||
            !await _branchAccess.CanAccessBranchAsync(User, timetable.BranchId))
        {
            return NotFound(ApiResponse<object>.Fail($"Timetable entry {id} was not found."));
        }

        return await _entries.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Timetable entry {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAndConflictsAsync(CreateTimetableEntryRequest request, Guid? currentId)
    {
        var timetable = await _timetables.GetAsync(request.TimetableId);
        if (timetable is null)
            return $"Timetable {request.TimetableId} was not found in this tenant.";
        if (!await _branchAccess.CanAccessBranchAsync(User, timetable.BranchId))
            return "You can manage timetable entries only for your assigned branch.";
        if (await _sections.GetAsync(request.SectionId) is null)
            return $"Section {request.SectionId} was not found in this tenant.";
        if (await _subjects.GetAsync(request.SubjectId) is null)
            return $"Subject {request.SubjectId} was not found in this tenant.";
        var teacher = await _teachers.GetAsync(request.TeacherProfileId);
        if (teacher is null)
            return $"Teacher {request.TeacherProfileId} was not found in this tenant.";
        if (teacher.BranchId != timetable.BranchId)
            return "Teacher must belong to the timetable branch.";
        var slot = await _slots.GetAsync(request.TimeSlotId);
        if (slot is null)
            return $"Time slot {request.TimeSlotId} was not found in this tenant.";
        if (slot.BranchId != timetable.BranchId)
            return "Time slot must belong to the timetable branch.";
        if (request.RoomId is Guid roomId)
        {
            var room = await _rooms.GetAsync(roomId);
            if (room is null)
                return $"Room {roomId} was not found in this tenant.";
            if (room.BranchId != timetable.BranchId)
                return "Room must belong to the timetable branch.";
        }
        if (timetable.SectionId != request.SectionId)
            return "Timetable entries must use the timetable's section.";

        var entries = await _entries.ListAsync(e =>
            e.TimetableId == request.TimetableId &&
            e.TimeSlotId == request.TimeSlotId &&
            (!currentId.HasValue || e.Id != currentId.Value));

        if (entries.Any(e => e.SectionId == request.SectionId))
            return "The selected section already has a class in this time slot.";
        if (entries.Any(e => e.TeacherProfileId == request.TeacherProfileId))
            return "The selected teacher already has a class in this time slot.";
        if (request.RoomId.HasValue && entries.Any(e => e.RoomId == request.RoomId))
            return "The selected room already has a class in this time slot.";

        return null;
    }

    private static TimetableEntry Apply(TimetableEntry entity, CreateTimetableEntryRequest request)
    {
        entity.TimetableId = request.TimetableId;
        entity.SectionId = request.SectionId;
        entity.SubjectId = request.SubjectId;
        entity.TeacherProfileId = request.TeacherProfileId;
        entity.TimeSlotId = request.TimeSlotId;
        entity.RoomId = request.RoomId;
        entity.Notes = request.Notes;
        return entity;
    }

    private static TimetableEntryDto Map(TimetableEntry e) => new TimetableEntryDto
    {
        TimetableId = e.TimetableId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        TeacherProfileId = e.TeacherProfileId,
        TimeSlotId = e.TimeSlotId,
        RoomId = e.RoomId,
        Notes = e.Notes
    }.WithMetadata(e);
}
