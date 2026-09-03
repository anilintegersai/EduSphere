using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface ITenantService
{
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
    Task<Tenant?> GetTenantByIdAsync(Guid id);
    Task<Tenant> CreateTenantAsync(string name, string tenantIdentifier, string? description = null, string? customDomain = null);
    Task<bool> UpdateTenantAsync(Guid id, string name, string? description = null);
    Task<bool> DeleteTenantAsync(Guid id);
    Task<bool> ActivateTenantAsync(Guid id);
    Task<bool> DeactivateTenantAsync(Guid id);
}
