using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionApplication : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? BatchId { get; set; }
    public Batch? Batch { get; set; }

    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }

    [Required]
    [StringLength(50)]
    public string ApplicationNumber { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string ApplicantFirstName { get; set; } = null!;

    [StringLength(100)]
    public string? ApplicantMiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string ApplicantLastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(150)]
    public string? GuardianName { get; set; }

    [StringLength(30)]
    public string? GuardianPhone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public AdmissionApplicationStatus Status { get; set; } = AdmissionApplicationStatus.Submitted;

    [StringLength(1000)]
    public string? ReviewNotes { get; set; }

    public ICollection<AdmissionDocument> Documents { get; set; } = new List<AdmissionDocument>();
    public ICollection<AdmissionReview> Reviews { get; set; } = new List<AdmissionReview>();
}
