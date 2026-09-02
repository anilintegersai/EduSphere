using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class Batch : ITenantEntity, IAuditableEntity, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!; // e.g. "B.Tech CSE 2026"

    public int Capacity { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
