using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class StudentGuardian : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid? ParentUserId { get; set; }
    public ApplicationUser? ParentUser { get; set; }

    public GuardianRelationship Relationship { get; set; } = GuardianRelationship.Guardian;

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(120)]
    public string? Occupation { get; set; }

    public bool IsPrimary { get; set; }
    public bool HasPortalAccess { get; set; }
    public bool CanPickup { get; set; }
}
