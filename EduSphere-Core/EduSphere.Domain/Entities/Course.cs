using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Course : TenantEntityBase
{
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
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
}
