using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class Subject : ITenantEntity, IAuditableEntity, ISoftDeletable
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

    public SubjectType Type { get; set; } = SubjectType.Core;

    public int Credits { get; set; }

    // Optional owning course
    public int? CourseId { get; set; }
    public Course? Course { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
