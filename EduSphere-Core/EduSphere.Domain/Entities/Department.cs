using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class Department : TenantEntityBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }
}
