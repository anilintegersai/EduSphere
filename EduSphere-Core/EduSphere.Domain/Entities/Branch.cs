using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduSphere.Domain.Entities;

public class Branch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string BoardType { get; set; } = null!; // CBSE, ICSE, State, etc.

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? State { get; set; }

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
