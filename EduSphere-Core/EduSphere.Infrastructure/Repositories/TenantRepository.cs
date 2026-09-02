using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Infrastructure.Repositories;

public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(TenantDbContext context) : base(context)
    {
    }

    // Tenant is the tenancy root and is not itself tenant-filtered, so these
    // lookups run without a resolved tenant in scope (used during resolution).
    public async Task<Tenant?> GetByIdentifierAsync(string tenantIdentifier)
        => await Context.Tenants.FirstOrDefaultAsync(t => t.TenantIdentifier == tenantIdentifier);

    public async Task<Tenant?> GetByCustomDomainAsync(string host)
        => await Context.Tenants.FirstOrDefaultAsync(t => t.CustomDomain == host);
}
