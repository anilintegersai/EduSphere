namespace EduSphere.Domain.Common;

/// <summary>Standard created/modified audit timestamps.</summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

/// <summary>Supports soft delete via an active flag.</summary>
public interface ISoftDeletable
{
    bool IsActive { get; set; }
}
