using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class SyllabusUnit : ITenantEntity, IAuditableEntity, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int Order { get; set; } // sequence within the subject

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int EstimatedHours { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
