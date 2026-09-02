using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class Course : ITenantEntity, IAuditableEntity, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TenantId { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    public CourseType Type { get; set; } = CourseType.Other;

    public int DurationMonths { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    // Optional owning department
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
