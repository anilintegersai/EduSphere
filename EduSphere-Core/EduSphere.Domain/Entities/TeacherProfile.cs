using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class TeacherProfile : TenantEntityBase
{
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(50)]
    public string EmployeeNumber { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(120)]
    public string? Designation { get; set; }

    [StringLength(500)]
    public string? Qualifications { get; set; }

    [StringLength(500)]
    public string? Specializations { get; set; }

    public decimal ExperienceYears { get; set; }
    public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;

    public ICollection<TeacherSubjectAssignment> SubjectAssignments { get; set; } = new List<TeacherSubjectAssignment>();
}
