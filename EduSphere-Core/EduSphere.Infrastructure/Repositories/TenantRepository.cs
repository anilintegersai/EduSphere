using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly TenantDbContext _context;

    public TenantRepository(TenantDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Tenant?> GetTenantByIdentifierAsync(string tenantIdentifier)
        => await _context.Tenants.FirstOrDefaultAsync(t => t.TenantIdentifier == tenantIdentifier);

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        => await _context.Tenants.ToListAsync();

    public async Task AddTenantAsync(Tenant tenant)
        => await _context.Tenants.AddAsync(tenant);

    public void UpdateTenantAsync(Tenant tenant)
        => _context.Tenants.Update(tenant);

    public async Task<bool> TenantExistsAsync(string tenantIdentifier)
        => await _context.Tenants.AnyAsync(t => t.TenantIdentifier == tenantIdentifier);
}