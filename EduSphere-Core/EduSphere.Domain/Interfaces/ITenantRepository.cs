using EduSphere.Domain.Entities;

namespace EduSphere.Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetTenantByIdentifierAsync(string tenantIdentifier);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
    Task AddTenantAsync(Tenant tenant);
    Task UpdateTenantAsync(Tenant tenant);
    Task<bool> TenantExistsAsync(string tenantIdentifier);
}
