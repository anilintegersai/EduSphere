using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Represents an Educational Trust/Management body in India
/// </summary>
public class Trust : EntityBase
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string RegistrationNumber { get; set; } = null!; // PAN or Trust Registration Number

    [Required]
    public DateTime RegistrationDate { get; set; }

    [Required]
    [StringLength(100)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string State { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Country { get; set; } = "India";

    [Required]
    [StringLength(20)]
    public string Pincode { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? Website { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime? DeactivatedAt { get; set; }

    // Navigation properties
    public ICollection<Management> Managements { get; set; } = new List<Management>();
    public ICollection<University> Universities { get; set; } = new List<University>();
}
