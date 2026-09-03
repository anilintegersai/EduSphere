using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Subject : TenantEntityBase
{
    [Required]
    [StringLength(30)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    public SubjectType Type { get; set; } = SubjectType.Core;

    public int Credits { get; set; }

    // Optional owning course
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
}
