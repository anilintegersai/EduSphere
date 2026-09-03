using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class SyllabusUnit : TenantEntityBase
{
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int Order { get; set; } // sequence within the subject

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int EstimatedHours { get; set; }
}
