using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ParentProfile : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(120)]
    public string? Occupation { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}

public class StaffProfile : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

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

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(120)]
    public string? Designation { get; set; }

    public EmploymentType EmploymentType { get; set; } = EmploymentType.Permanent;
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [StringLength(500)]
    public string? Qualifications { get; set; }

    public decimal ExperienceYears { get; set; }
    public StaffStatus Status { get; set; } = StaffStatus.Active;
}

public class UserBranchAssignment : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public bool IsPrimary { get; set; } = true;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveUntil { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class UserRoleAssignment : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid RoleId { get; set; }
    public ApplicationRole? Role { get; set; }

    [Required]
    [StringLength(80)]
    public string RoleName { get; set; } = null!;

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? AssignedByUserId { get; set; }
    public ApplicationUser? AssignedByUser { get; set; }

    public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveUntil { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class UserInvitation : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string RoleName { get; set; } = null!;

    [Required]
    [StringLength(128)]
    public string ActivationTokenHash { get; set; } = null!;

    public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.AddDays(7);
    public DateTime? AcceptedOn { get; set; }
    public DateTime? LastSentOn { get; set; }
    public int SendAttempts { get; set; }
    public UserInvitationStatus Status { get; set; } = UserInvitationStatus.Pending;

    public Guid? InvitedByUserId { get; set; }
    public ApplicationUser? InvitedByUser { get; set; }

    [StringLength(1000)]
    public string? LastSendError { get; set; }
}
