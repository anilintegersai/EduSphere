using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface ITenantService
{
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
    Task<Tenant?> GetTenantByIdAsync(int id);
    Task<Tenant> CreateTenantAsync(string name, string tenantIdentifier, string? description = null, string? customDomain = null);
    Task<bool> UpdateTenantAsync(int id, string name, string? description = null);
    Task<bool> DeleteTenantAsync(int id);
    Task<bool> ActivateTenantAsync(int id);
    Task<bool> DeactivateTenantAsync(int id);
}
