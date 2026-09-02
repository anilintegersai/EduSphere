using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class AcademicYear : ITenantEntity, IAuditableEntity, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TenantId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!; // e.g. "2026-2027"

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public bool IsCurrent { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
