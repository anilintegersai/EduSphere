using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Common;

public abstract class TenantEntityBase : EntityBase, ITenantEntity
{
    public Guid TenantId { get; set; }
}
