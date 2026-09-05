using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class StudentProfile : TenantEntityBase
{
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }

    [Required]
    [StringLength(50)]
    public string AdmissionNumber { get; set; } = null!;

    [StringLength(50)]
    public string? RollNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;
    public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(150)]
    public string? EmergencyContactName { get; set; }

    [StringLength(30)]
    public string? EmergencyContactPhone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public StudentStatus Status { get; set; } = StudentStatus.Active;

    public ICollection<StudentGuardian> Guardians { get; set; } = new List<StudentGuardian>();
}
