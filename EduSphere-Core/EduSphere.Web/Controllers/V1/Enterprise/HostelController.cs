using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hostel")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.HostelManager)]
public class HostelController : EnterpriseControllerBase
{
    private readonly ICrudService<HostelBlock> _blocks;
    private readonly ICrudService<HostelRoom> _rooms;
    private readonly ICrudService<HostelBed> _beds;
    private readonly ICrudService<HostelAllocation> _allocations;
    private readonly ICrudService<HostelFee> _fees;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;

    public HostelController(
        ICrudService<HostelBlock> blocks,
        ICrudService<HostelRoom> rooms,
        ICrudService<HostelBed> beds,
        ICrudService<HostelAllocation> allocations,
        ICrudService<HostelFee> fees,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        IBranchAccessService branchAccess)
        : base(branchAccess)
    {
        _blocks = blocks;
        _rooms = rooms;
        _beds = beds;
        _allocations = allocations;
        _fees = fees;
        _branches = branches;
        _students = students;
    }

    [HttpGet("blocks")]
    public async Task<IActionResult> GetBlocks([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<HostelBlockDto>>.Ok((await FilterByBranchAsync(_blocks, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("blocks")]
    public async Task<IActionResult> CreateBlock([FromBody] CreateHostelBlockRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _blocks.CreateAsync(Apply(new HostelBlock(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HostelBlockDto>.Ok(created.Map()));
    }

    [HttpPut("blocks/{id:guid}")]
    public async Task<IActionResult> UpdateBlock(Guid id, [FromBody] UpdateHostelBlockRequest request)
    {
        if (await GetBranchScopedAsync(_blocks, id, x => x.BranchId) is null) return NotFoundResult(id, "Hostel block");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _blocks.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("blocks/{id:guid}")]
    public Task<IActionResult> DeleteBlock(Guid id)
        => DeleteBranchScopedAsync(_blocks, id, x => x.BranchId, "Hostel block");

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<HostelRoomDto>>.Ok((await FilterByBranchAsync(_rooms, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateHostelRoomRequest request)
    {
        if (await ValidateBlockChildAsync(request.HostelBlockId, request.BranchId) is { } invalid) return invalid;
        var created = await _rooms.CreateAsync(Apply(new HostelRoom(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HostelRoomDto>.Ok(created.Map()));
    }

    [HttpPut("rooms/{id:guid}")]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateHostelRoomRequest request)
    {
        if (await GetBranchScopedAsync(_rooms, id, x => x.BranchId) is null) return NotFoundResult(id, "Hostel room");
        if (await ValidateBlockChildAsync(request.HostelBlockId, request.BranchId) is { } invalid) return invalid;
        await _rooms.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("rooms/{id:guid}")]
    public Task<IActionResult> DeleteRoom(Guid id)
        => DeleteBranchScopedAsync(_rooms, id, x => x.BranchId, "Hostel room");

    [HttpGet("beds")]
    public async Task<IActionResult> GetBeds([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<HostelBedDto>>.Ok((await FilterByBranchAsync(_beds, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("beds")]
    public async Task<IActionResult> CreateBed([FromBody] CreateHostelBedRequest request)
    {
        if (await ValidateRoomChildAsync(request.HostelRoomId, request.BranchId) is { } invalid) return invalid;
        var created = await _beds.CreateAsync(Apply(new HostelBed(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HostelBedDto>.Ok(created.Map()));
    }

    [HttpPut("beds/{id:guid}")]
    public async Task<IActionResult> UpdateBed(Guid id, [FromBody] UpdateHostelBedRequest request)
    {
        if (await GetBranchScopedAsync(_beds, id, x => x.BranchId) is null) return NotFoundResult(id, "Hostel bed");
        if (await ValidateRoomChildAsync(request.HostelRoomId, request.BranchId) is { } invalid) return invalid;
        await _beds.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("beds/{id:guid}")]
    public Task<IActionResult> DeleteBed(Guid id)
        => DeleteBranchScopedAsync(_beds, id, x => x.BranchId, "Hostel bed");

    [HttpGet("allocations")]
    public async Task<IActionResult> GetAllocations([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<HostelAllocationDto>>.Ok((await FilterByBranchAsync(_allocations, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("allocations")]
    public async Task<IActionResult> CreateAllocation([FromBody] CreateHostelAllocationRequest request)
    {
        if (await ValidateAllocationAsync(request) is { } invalid) return invalid;
        var created = await _allocations.CreateAsync(Apply(new HostelAllocation(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HostelAllocationDto>.Ok(created.Map()));
    }

    [HttpPut("allocations/{id:guid}")]
    public async Task<IActionResult> UpdateAllocation(Guid id, [FromBody] UpdateHostelAllocationRequest request)
    {
        if (await GetBranchScopedAsync(_allocations, id, x => x.BranchId) is null) return NotFoundResult(id, "Hostel allocation");
        if (await ValidateAllocationAsync(request) is { } invalid) return invalid;
        await _allocations.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("allocations/{id:guid}")]
    public Task<IActionResult> DeleteAllocation(Guid id)
        => DeleteBranchScopedAsync(_allocations, id, x => x.BranchId, "Hostel allocation");

    [HttpGet("fees")]
    public async Task<IActionResult> GetFees([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<HostelFeeDto>>.Ok((await FilterByBranchAsync(_fees, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("fees")]
    public async Task<IActionResult> CreateFee([FromBody] CreateHostelFeeRequest request)
    {
        if (await ValidateAllocationChildAsync(request.HostelAllocationId, request.BranchId) is { } invalid) return invalid;
        var created = await _fees.CreateAsync(Apply(new HostelFee(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HostelFeeDto>.Ok(created.Map()));
    }

    [HttpPut("fees/{id:guid}")]
    public async Task<IActionResult> UpdateFee(Guid id, [FromBody] UpdateHostelFeeRequest request)
    {
        if (await GetBranchScopedAsync(_fees, id, x => x.BranchId) is null) return NotFoundResult(id, "Hostel fee");
        if (await ValidateAllocationChildAsync(request.HostelAllocationId, request.BranchId) is { } invalid) return invalid;
        await _fees.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("fees/{id:guid}")]
    public Task<IActionResult> DeleteFee(Guid id)
        => DeleteBranchScopedAsync(_fees, id, x => x.BranchId, "Hostel fee");

    private async Task<IActionResult?> ValidateBranchAsync(Guid branchId)
    {
        if (await _branches.GetAsync(branchId) is null)
            return BadRequest(ApiResponse<object>.Fail("Selected branch was not found in this tenant."));
        return await CanUseBranchAsync(branchId) ? null : Forbid();
    }

    private async Task<IActionResult?> ValidateBlockChildAsync(Guid blockId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _blocks.GetAsync(blockId) is not { } block || block.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected hostel block was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateRoomChildAsync(Guid roomId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _rooms.GetAsync(roomId) is not { } room || room.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected hostel room was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateAllocationAsync(CreateHostelAllocationRequest request)
    {
        if (await ValidateRoomChildAsync(request.HostelRoomId, request.BranchId) is { } invalid) return invalid;
        if (await _students.GetAsync(request.StudentProfileId) is not { } student || student.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected student was not found for the branch."));
        if (request.HostelBedId is Guid bedId && (await _beds.GetAsync(bedId) is not { } bed || bed.BranchId != request.BranchId || bed.HostelRoomId != request.HostelRoomId))
            return BadRequest(ApiResponse<object>.Fail("Selected bed was not found for the room and branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateAllocationChildAsync(Guid allocationId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _allocations.GetAsync(allocationId) is not { } allocation || allocation.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected hostel allocation was not found for the branch."));
        return null;
    }

    private NotFoundObjectResult NotFoundResult(Guid id, string displayName)
        => NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

    private static HostelBlock Apply(HostelBlock entity, CreateHostelBlockRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Gender = request.Gender;
        entity.WardenName = request.WardenName;
        entity.WardenPhone = request.WardenPhone;
        entity.IsActive = request.IsActive;
        entity.Notes = request.Notes;
        return entity;
    }

    private static HostelRoom Apply(HostelRoom entity, CreateHostelRoomRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.HostelBlockId = request.HostelBlockId;
        entity.RoomNumber = request.RoomNumber;
        entity.Floor = request.Floor;
        entity.Capacity = request.Capacity;
        entity.MonthlyFee = request.MonthlyFee;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }

    private static HostelBed Apply(HostelBed entity, CreateHostelBedRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.HostelRoomId = request.HostelRoomId;
        entity.BedNumber = request.BedNumber;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }

    private static HostelAllocation Apply(HostelAllocation entity, CreateHostelAllocationRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.HostelRoomId = request.HostelRoomId;
        entity.HostelBedId = request.HostelBedId;
        entity.AllocatedOn = request.AllocatedOn;
        entity.ExpectedCheckoutOn = request.ExpectedCheckoutOn;
        entity.CheckedOutOn = request.CheckedOutOn;
        entity.Status = request.Status;
        entity.MonthlyFee = request.MonthlyFee;
        entity.SecurityDeposit = request.SecurityDeposit;
        entity.Notes = request.Notes;
        return entity;
    }

    private static HostelFee Apply(HostelFee entity, CreateHostelFeeRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.HostelAllocationId = request.HostelAllocationId;
        entity.InvoiceNumber = request.InvoiceNumber;
        entity.BillingMonth = request.BillingMonth;
        entity.DueDate = request.DueDate;
        entity.Amount = request.Amount;
        entity.PaidAmount = request.PaidAmount;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }
}
