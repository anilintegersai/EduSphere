namespace EduSphere.Domain.MultiTenancy;

/// <summary>
/// Marks an entity as owned by a tenant. Every tenant-owned table carries a
/// <see cref="TenantId"/> discriminator. EF Core applies a global query filter
/// on reads and stamps the value on writes for every type that implements this.
/// </summary>
public interface ITenantEntity
{
    int TenantId { get; set; }
}
