using EduSphere.Domain.Entities;

namespace EduSphere.Domain.Interfaces;

public interface ITenantRepository : IGenericRepository<Tenant>
{
    Task<Tenant?> GetByIdentifierAsync(string tenantIdentifier);
    Task<Tenant?> GetByCustomDomainAsync(string host);
}
