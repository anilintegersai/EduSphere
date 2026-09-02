using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Domain.Entities;

public class Branch : ITenantEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

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

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Foreign key to Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
