using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class Section : TenantEntityBase
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!; // e.g. "Section A"

    public int Capacity { get; set; }

    public Guid BatchId { get; set; }
    public Batch? Batch { get; set; }

    // Optional class teacher (ApplicationUser Id). Kept as a plain column for now;
    // a navigation to the teacher can be wired when the Teacher module lands.
    public Guid? ClassTeacherUserId { get; set; }
}
