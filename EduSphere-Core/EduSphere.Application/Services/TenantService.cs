using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;

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

    public async Task<Tenant?> GetTenantByIdAsync(int id)
    {
        return await _unitOfWork.TenantRepository.GetByIdAsync(id);
    }

    public async Task<Tenant> CreateTenantAsync(string name, string tenantIdentifier, string? description = null)
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TenantRepository.AddAsync(tenant);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Tenant created: {TenantName} ({TenantIdentifier})", name, tenantIdentifier);
        return tenant;
    }

    public async Task<bool> UpdateTenantAsync(int id, string name, string? description = null)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.Name = name;
        tenant.Description = description;
        tenant.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant updated: {TenantId}", id);
        return true;
    }

    public async Task<bool> DeleteTenantAsync(int id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        // Soft delete
        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant deleted: {TenantId}", id);
        return true;
    }

    public async Task<bool> ActivateTenantAsync(int id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.IsActive = true;
        tenant.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant activated: {TenantId}", id);
        return true;
    }

    public async Task<bool> DeactivateTenantAsync(int id)
    {
        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(id);
        if (tenant == null)
        {
            return false;
        }

        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Tenant deactivated: {TenantId}", id);
        return true;
    }
}
