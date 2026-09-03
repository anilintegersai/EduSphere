using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class Tenant : EntityBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string TenantIdentifier { get; set; } = null!; // subdomain / header key, e.g. "school1"

    [StringLength(255)]
    public string? CustomDomain { get; set; } // e.g. "school.edu.in" for custom-domain resolution

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Navigation property for branches
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}
