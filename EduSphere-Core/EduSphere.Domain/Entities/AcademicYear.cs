using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class AcademicYear : TenantEntityBase
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!; // e.g. "2026-2027"

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public bool IsCurrent { get; set; }
}
