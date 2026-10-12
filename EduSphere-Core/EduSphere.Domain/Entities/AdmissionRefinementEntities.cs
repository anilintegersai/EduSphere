using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionApplicantAccount : TenantEntityBase
{
    [Required, StringLength(254)] public string Email { get; set; } = null!;
    [Required] public string PasswordHash { get; set; } = null!;
    [StringLength(150)] public string? DisplayName { get; set; }
    [StringLength(30)] public string? PhoneNumber { get; set; }
    public ApplicantAccountStatus Status { get; set; } = ApplicantAccountStatus.Active;
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginOn { get; set; }
}

public class BranchAdmissionSetting : TenantEntityBase
{
    public Guid BranchId { get; set; }
    [Required, StringLength(100)] public string ApplicationNumberFormat { get; set; } = "APP-{YYYY}-{SEQ:0000}";
    [Required, StringLength(100)] public string AdmissionNumberFormat { get; set; } = "ADM-{YYYY}-{SEQ:0000}";
    public int NextApplicationSequence { get; set; } = 1;
    public int NextAdmissionSequence { get; set; } = 1;
    public int OfferValidityDays { get; set; } = 7;
    public int FeeDeadlineDays { get; set; } = 7;
    public bool AutoPromoteWaitlist { get; set; } = true;
}

public class AdmissionInterviewerAvailability : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid InterviewerUserId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveUntil { get; set; }
    [StringLength(180)] public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdmissionAutomationLog : TenantEntityBase
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionAutomationType Type { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    [Required, StringLength(1000)] public string Message { get; set; } = null!;
}
