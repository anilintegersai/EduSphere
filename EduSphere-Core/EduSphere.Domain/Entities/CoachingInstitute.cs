using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Represents a Coaching Institute (IIT-JEE, NEET, CAT, etc.)
/// </summary>
public class CoachingInstitute
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string InstituteType { get; set; } = "Competitive"; // Competitive, Language, Skill Development, Professional

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
    [StringLength(50)]
    public string EstablishedYear { get; set; } = null!;

    [StringLength(255)]
    public string? CoursesOffered { get; set; }

    [StringLength(255)]
    public string? SuccessRate { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public int? TrustId { get; set; }
    public int? ManagementId { get; set; }

    public Trust? Trust { get; set; }
    public Management? Management { get; set; }
}
