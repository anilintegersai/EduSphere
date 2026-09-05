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
[Route("api/v{version:apiVersion}/time-slots")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class TimeSlotsController : ApiControllerBase
{
    private readonly ICrudService<TimeSlot> _slots;
    private readonly ICrudService<Branch> _branches;
    private readonly IBranchAccessService _branchAccess;

    public TimeSlotsController(ICrudService<TimeSlot> slots, ICrudService<Branch> branches, IBranchAccessService branchAccess)
    {
        _slots = slots;
        _branches = branches;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? branchId, [FromQuery] DayOfWeek? dayOfWeek)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _slots.ListAsync(s =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || s.BranchId == branchId.Value) &&
            (!dayOfWeek.HasValue || s.DayOfWeek == dayOfWeek.Value));
        return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _slots.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<TimeSlotDto>.Fail($"Time slot {id} was not found."))
            : Ok(ApiResponse<TimeSlotDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Branch {request.BranchId} was not found in this tenant."));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _slots.CreateAsync(Apply(new TimeSlot(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TimeSlotDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeSlotRequest request)
    {
        var existing = await _slots.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Time slot {id} was not found."));

        if (await _branches.GetAsync(request.BranchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Branch {request.BranchId} was not found in this tenant."));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _slots.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Time slot {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _slots.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Time slot {id} was not found."));

        return await _slots.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Time slot {id} was not found."));
    }

    private static TimeSlot Apply(TimeSlot entity, CreateTimeSlotRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Name = request.Name;
        entity.DayOfWeek = request.DayOfWeek;
        entity.PeriodNumber = request.PeriodNumber;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.IsBreak = request.IsBreak;
        return entity;
    }

    private static TimeSlotDto Map(TimeSlot e) => new TimeSlotDto
    {
        BranchId = e.BranchId,
        Name = e.Name,
        DayOfWeek = e.DayOfWeek,
        PeriodNumber = e.PeriodNumber,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        IsBreak = e.IsBreak
    }.WithMetadata(e);
}
