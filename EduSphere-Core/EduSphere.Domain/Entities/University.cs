using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Represents a University in India
/// </summary>
public class University : EntityBase
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string UniversityType { get; set; } = "Central"; // Central, State, Deemed, Private

    [Required]
    [StringLength(50)]
    public string UGCStatus { get; set; } = "Recognized"; // Recognized, Not Recognized, Deemed

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
    public string? UGCCode { get; set; }

    [StringLength(255)]
    public string? NAACGrade { get; set; }

    [StringLength(255)]
    public string? NIRFRanking { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid? TrustId { get; set; }
    public Guid? ManagementId { get; set; }

    public Trust? Trust { get; set; }
    public Management? Management { get; set; }

    // Navigation properties
    public ICollection<College> Colleges { get; set; } = new List<College>();
    public ICollection<School> Schools { get; set; } = new List<School>();
}
