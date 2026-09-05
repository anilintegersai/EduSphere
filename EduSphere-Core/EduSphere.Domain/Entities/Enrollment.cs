using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Enrollment : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid? AdmissionApplicationId { get; set; }
    public AdmissionApplication? AdmissionApplication { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid BatchId { get; set; }
    public Batch? Batch { get; set; }

    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }

    [Required]
    [StringLength(50)]
    public string EnrollmentNumber { get; set; } = null!;

    public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    [StringLength(500)]
    public string? Notes { get; set; }
}
