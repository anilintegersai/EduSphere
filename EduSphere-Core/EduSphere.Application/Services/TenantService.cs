using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduSphere.Application.Services;

public class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TenantService> _logger;

    public TenantService(IUnitOfWork unitOfWork, ILogger<TenantService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        return await _unitOfWork.TenantRepository.GetAllAsync();
    }

    public async Task<Tenant?> GetTenantByIdAsync(Guid id)
    {
        return await _unitOfWork.TenantRepository.GetByIdAsync(id);
    }

    public async Task<Tenant> CreateTenantAsync(string name, string tenantIdentifier, string? description = null, string? customDomain = null)
    {
        // Check if tenant identifier already exists
        var existingTenant = await _unitOfWork.TenantRepository.GetFirstOrDefaultAsync(
            t => t.TenantIdentifier == tenantIdentifier
        );

        if (existingTenant != null)
        {
            throw new InvalidOperationException($"Tenant with identifier '{tenantIdentifier}' already exists.");
        }

        var tenant = new Tenant
        {
            Name = name,
            TenantIdentifier = tenantIdentifier,
            Description = description,
            CustomDomain = customDomain,
            IsActive = true
        };

        await _unitOfWork.TenantRepository.AddAsync(tenant);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Tenant created: {TenantName} ({TenantIdentifier})", name, tenantIdentifier);
        return tenant;
    }

    public async Task<bool> UpdateTenantAsync(Guid id, string name, string? description = null)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.Name = name;
        tenant.Description = description;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant updated: {TenantId}", id);
        return true;
    }

    public async Task<bool> DeleteTenantAsync(Guid id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.IsDeleted = true;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant deleted: {TenantId}", id);
        return true;
    }

    public async Task<bool> ActivateTenantAsync(Guid id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.IsActive = true;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant activated: {TenantId}", id);
        return true;
    }

    public async Task<bool> DeactivateTenantAsync(Guid id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.IsActive = false;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant deactivated: {TenantId}", id);
        return true;
    }
}
