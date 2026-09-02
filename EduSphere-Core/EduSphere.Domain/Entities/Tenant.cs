using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduSphere.Domain.Entities;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

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

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation property for branches
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}
