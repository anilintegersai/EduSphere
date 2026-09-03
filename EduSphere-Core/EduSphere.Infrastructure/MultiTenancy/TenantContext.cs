using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Infrastructure.MultiTenancy;

/// <summary>
/// Scoped, mutable holder for the current request's tenant. Registered per
/// request; the tenant resolution middleware calls <see cref="SetTenant"/>,
/// everything downstream reads it.
/// </summary>
public class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantIdentifier { get; private set; }
    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string tenantIdentifier)
    {
        TenantId = tenantId;
        TenantIdentifier = tenantIdentifier;
    }

    public void Clear()
    {
        TenantId = null;
        TenantIdentifier = null;
    }
}
