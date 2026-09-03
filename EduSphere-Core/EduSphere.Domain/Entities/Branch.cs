using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class Branch : TenantEntityBase
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Code { get; set; } // Branch/campus code

    [StringLength(50)]
    public string? BoardType { get; set; } // CBSE, ICSE, State, etc.

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? State { get; set; }

    [StringLength(50)]
    public string? Country { get; set; } = "India";

    [StringLength(20)]
    public string? Pincode { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Foreign key to Tenant
    public Tenant Tenant { get; set; } = null!;
}
