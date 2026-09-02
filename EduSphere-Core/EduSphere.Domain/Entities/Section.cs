using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class Section : ITenantEntity, IAuditableEntity, ISoftDeletable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TenantId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!; // e.g. "Section A"

    public int Capacity { get; set; }

    public int BatchId { get; set; }
    public Batch? Batch { get; set; }

    // Optional class teacher (ApplicationUser Id). Kept as a plain column for now;
    // a navigation to the teacher can be wired when the Teacher module lands.
    public Guid? ClassTeacherUserId { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
