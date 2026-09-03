using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Branches;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/branches")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
public class BranchesController : ApiControllerBase
{
    private readonly IBranchService _branchService;
    private readonly ITenantContext _tenantContext;

    public BranchesController(IBranchService branchService, ITenantContext tenantContext)
    {
        _branchService = branchService;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BranchDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetTenant(out var tenantId, out var error))
            return BadRequest(error);

        var branches = await _branchService.GetBranchesByTenantAsync(tenantId);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.Ok(branches.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var branch = await _branchService.GetBranchByIdAsync(id);
        return branch is null
            ? NotFound(ApiResponse<BranchDto>.Fail($"Branch {id} was not found."))
            : Ok(ApiResponse<BranchDto>.Ok(Map(branch)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BranchDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBranchRequest request)
    {
        if (!TryGetTenant(out var tenantId, out var error))
            return BadRequest(error);

        var branch = await _branchService.CreateBranchAsync(
            tenantId, request.Name, request.Code, request.Address,
            request.City, request.State, request.Country, request.Pincode);

        return StatusCode(StatusCodes.Status201Created, ApiResponse<BranchDto>.Ok(Map(branch)));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBranchRequest request)
    {
        var updated = await _branchService.UpdateBranchAsync(
            id, request.Name, request.Code, request.Address,
            request.City, request.State, request.Country, request.Pincode);

        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Branch {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _branchService.DeleteBranchAsync(id);
        return deleted ? NoContent() : NotFound(ApiResponse<object>.Fail($"Branch {id} was not found."));
    }

    private bool TryGetTenant(out Guid tenantId, out ApiResponse<object> error)
    {
        if (_tenantContext.TenantId is { } id)
        {
            tenantId = id;
            error = default!;
            return true;
        }

        tenantId = Guid.Empty;
        error = ApiResponse<object>.Fail("A tenant must be specified via the X-Tenant-ID header.");
        return false;
    }

    private static BranchDto Map(Branch b) => new()
    {
        Id = b.Id,
        TenantId = b.TenantId,
        Name = b.Name,
        Code = b.Code,
        Address = b.Address,
        City = b.City,
        State = b.State,
        Country = b.Country,
        Pincode = b.Pincode,
        IsActive = b.IsActive,
        IsDeleted = b.IsDeleted,
        CreatedBy = b.CreatedBy,
        CreatedOn = b.CreatedOn,
        ModifiedBy = b.ModifiedBy,
        ModifiedOn = b.ModifiedOn,
        DeletedBy = b.DeletedBy,
        DeletedOn = b.DeletedOn,
        ConcurrencyToken = b.ConcurrencyToken
    };
}
