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
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/rooms")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class RoomsController : ApiControllerBase
{
    private readonly ICrudService<Room> _rooms;
    private readonly ICrudService<Branch> _branches;
    private readonly IBranchAccessService _branchAccess;

    public RoomsController(ICrudService<Room> rooms, ICrudService<Branch> branches, IBranchAccessService branchAccess)
    {
        _rooms = rooms;
        _branches = branches;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? branchId, [FromQuery] RoomType? type)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _rooms.ListAsync(r =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && r.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || r.BranchId == branchId.Value) &&
            (!type.HasValue || r.Type == type.Value));
        return Ok(ApiResponse<IEnumerable<RoomDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _rooms.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<RoomDto>.Fail($"Room {id} was not found."))
            : Ok(ApiResponse<RoomDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Branch {request.BranchId} was not found in this tenant."));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _rooms.CreateAsync(Apply(new Room(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<RoomDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request)
    {
        var existing = await _rooms.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Room {id} was not found."));

        if (await _branches.GetAsync(request.BranchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Branch {request.BranchId} was not found in this tenant."));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _rooms.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Room {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _rooms.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Room {id} was not found."));

        return await _rooms.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Room {id} was not found."));
    }

    private static Room Apply(Room entity, CreateRoomRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.Capacity = request.Capacity;
        entity.IsActive = request.IsActive;
        return entity;
    }

    private static RoomDto Map(Room e) => new RoomDto
    {
        BranchId = e.BranchId,
        Code = e.Code,
        Name = e.Name,
        Type = e.Type,
        Capacity = e.Capacity,
        IsActive = e.IsActive
    }.WithMetadata(e);
}
