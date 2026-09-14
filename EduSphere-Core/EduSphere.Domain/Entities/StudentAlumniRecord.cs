using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class StudentAlumniRecord : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? BatchId { get; set; }
    public Batch? Batch { get; set; }

    [Required]
    [StringLength(50)]
    public string AlumniNumber { get; set; } = null!;

    public DateOnly GraduationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [StringLength(150)]
    public string? ContactEmail { get; set; }

    [StringLength(30)]
    public string? ContactPhone { get; set; }

    [StringLength(250)]
    public string? HigherEducation { get; set; }

    [StringLength(250)]
    public string? EmployerOrInstitution { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
