using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Tenants;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.SuperAdmin)]
public class TenantsController : ApiControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TenantDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tenants = await _tenantService.GetAllTenantsAsync();
        return Ok(ApiResponse<IEnumerable<TenantDto>>.Ok(tenants.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(id);
        return tenant is null
            ? NotFound(ApiResponse<TenantDto>.Fail($"Tenant {id} was not found."))
            : Ok(ApiResponse<TenantDto>.Ok(Map(tenant)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            var tenant = await _tenantService.CreateTenantAsync(
                request.Name, request.TenantIdentifier, request.Description, request.CustomDomain);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<TenantDto>.Ok(Map(tenant)));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<TenantDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantRequest request)
    {
        var updated = await _tenantService.UpdateTenantAsync(id, request.Name, request.Description);
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Tenant {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _tenantService.DeleteTenantAsync(id);
        return deleted ? NoContent() : NotFound(ApiResponse<object>.Fail($"Tenant {id} was not found."));
    }

    private static TenantDto Map(Tenant t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        TenantIdentifier = t.TenantIdentifier,
        CustomDomain = t.CustomDomain,
        Description = t.Description,
        IsActive = t.IsActive,
        IsDeleted = t.IsDeleted,
        CreatedBy = t.CreatedBy,
        CreatedOn = t.CreatedOn,
        ModifiedBy = t.ModifiedBy,
        ModifiedOn = t.ModifiedOn,
        DeletedBy = t.DeletedBy,
        DeletedOn = t.DeletedOn,
        ConcurrencyToken = t.ConcurrencyToken
    };
}
