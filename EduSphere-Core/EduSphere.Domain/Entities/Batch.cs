using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class Batch : TenantEntityBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!; // e.g. "B.Tech CSE 2026"

    public int Capacity { get; set; }

    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }
}
