namespace EduSphere.Domain.MultiTenancy;

/// <summary>
/// Request-scoped ambient tenant. Populated once per request by the tenant
/// resolution middleware and consumed by the DbContext (query filters + write
/// stamping), authorization, and auditing. When no tenant is resolved
/// (<see cref="HasTenant"/> is false) tenant-owned queries return no rows.
/// </summary>
public interface ITenantContext
{
    int? TenantId { get; }
    string? TenantIdentifier { get; }
    bool HasTenant { get; }

    void SetTenant(int tenantId, string tenantIdentifier);
    void Clear();
}
