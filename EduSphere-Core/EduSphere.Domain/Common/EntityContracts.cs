namespace EduSphere.Domain.Common;

/// <summary>Common Guid primary key contract for persisted domain entities.</summary>
public interface IGuidEntity
{
    Guid Id { get; set; }
}

/// <summary>Standard created/modified audit metadata.</summary>
public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? ModifiedOn { get; set; }
}

/// <summary>Supports standardized soft delete without overloading active/inactive status.</summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    string? DeletedBy { get; set; }
    DateTime? DeletedOn { get; set; }
}

/// <summary>Application-managed optimistic concurrency token.</summary>
public interface IConcurrencyTrackedEntity
{
    Guid ConcurrencyToken { get; set; }
}

public abstract class EntityBase : IGuidEntity, IAuditableEntity, ISoftDeletable, IConcurrencyTrackedEntity
{
    public Guid Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();
}
