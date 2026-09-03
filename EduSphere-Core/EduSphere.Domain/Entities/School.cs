using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Represents a School (Primary/Secondary/High School)
/// </summary>
public class School : EntityBase
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string SchoolType { get; set; } = "Secondary"; // Primary, Secondary, Higher Secondary, CBSE, ICSE, State Board

    [Required]
    [StringLength(100)]
    public string Board { get; set; } = "State"; // CBSE, ICSE, State, IB, Cambridge

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
    public string? AffiliationNumber { get; set; }

    [StringLength(255)]
    public string? Grade { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid? TrustId { get; set; }
    public Guid? ManagementId { get; set; }
    public Guid? UniversityId { get; set; }
    public Guid? CollegeId { get; set; }

    public Trust? Trust { get; set; }
    public Management? Management { get; set; }
    public University? University { get; set; }
    public College? College { get; set; }
}
