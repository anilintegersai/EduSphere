using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Represents a College affiliated to a University
/// </summary>
public class College : EntityBase
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string CollegeType { get; set; } = "Degree"; // Degree, Diploma, Professional, etc.

    [Required]
    [StringLength(100)]
    public string AffiliationType { get; set; } = "University"; // University, Autonomous, Deemed

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
    public string? NAACGrade { get; set; }

    [StringLength(255)]
    public string? NIRFRanking { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid? TrustId { get; set; }
    public Guid? ManagementId { get; set; }
    public Guid? UniversityId { get; set; }

    public Trust? Trust { get; set; }
    public Management? Management { get; set; }
    public University? University { get; set; }

    // Navigation property
    public ICollection<School> Schools { get; set; } = new List<School>();
}
