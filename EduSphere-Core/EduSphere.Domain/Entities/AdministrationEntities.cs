using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class SubscriptionPlan : EntityBase
{
    [Required, StringLength(50)] public string Code { get; set; } = null!;
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int MaxBranches { get; set; }
    public int MaxStudents { get; set; }
    public int MaxStaff { get; set; }
    public bool IsActive { get; set; } = true;
    public string IncludedFeaturesJson { get; set; } = "[]";
}

public class TenantSubscription : TenantEntityBase
{
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;
    public DateOnly StartsOn { get; set; }
    public DateOnly? EndsOn { get; set; }
    public bool AutoRenew { get; set; }
    [StringLength(100)] public string? ExternalSubscriptionId { get; set; }
    public decimal AgreedMonthlyPrice { get; set; }
}

public class TenantBranding : TenantEntityBase
{
    [StringLength(250)] public string? LogoUrl { get; set; }
    [StringLength(250)] public string? FaviconUrl { get; set; }
    [Required, StringLength(7)] public string PrimaryColor { get; set; } = "#176B87";
    [Required, StringLength(7)] public string SecondaryColor { get; set; } = "#F59E0B";
    [Required, StringLength(7)] public string AccentColor { get; set; } = "#16A34A";
    [StringLength(100)] public string? FontFamily { get; set; }
    [StringLength(150)] public string? EmailFromName { get; set; }
    [StringLength(250)] public string? EmailHeaderHtml { get; set; }
    [StringLength(1000)] public string? EmailFooterHtml { get; set; }
    [StringLength(500)] public string? LoginWelcomeText { get; set; }
}

public class TenantDomain : TenantEntityBase
{
    [Required, StringLength(253)] public string DomainName { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }
    [Required, StringLength(100)] public string VerificationToken { get; set; } = null!;
    public DateTime? VerifiedOn { get; set; }
}

public class TenantIntegrationSetting : TenantEntityBase
{
    [Required, StringLength(80)] public string IntegrationKey { get; set; } = null!;
    [Required, StringLength(120)] public string DisplayName { get; set; } = null!;
    public IntegrationCategory Category { get; set; }
    public bool IsEnabled { get; set; }
    public string ConfigurationJson { get; set; } = "{}";
    public string? ProtectedSecret { get; set; }
    [StringLength(120)] public string? SecretHint { get; set; }
    public DateTime? LastTestedOn { get; set; }
    public bool? LastTestSucceeded { get; set; }
    [StringLength(500)] public string? LastTestMessage { get; set; }
}

public class TenantFeatureFlag : TenantEntityBase
{
    [Required, StringLength(100)] public string FeatureKey { get; set; } = null!;
    [Required, StringLength(150)] public string DisplayName { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? EnabledFrom { get; set; }
    public DateTime? EnabledUntil { get; set; }
    public string ConditionsJson { get; set; } = "{}";
}

public class BranchContact : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(50)] public string ContactType { get; set; } = "Primary";
    [Required, StringLength(150)] public string ContactName { get; set; } = null!;
    [StringLength(150)] public string? Designation { get; set; }
    [StringLength(254)] public string? Email { get; set; }
    [StringLength(30)] public string? Phone { get; set; }
    public bool IsPrimary { get; set; }
}

public class BranchAcademicConfig : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(100)] public string TimeZoneId { get; set; } = "Asia/Kolkata";
    [Required, StringLength(10)] public string WeekStartsOn { get; set; } = "Monday";
    [StringLength(10)] public string? WeekendDays { get; set; } = "Sunday";
    public int DefaultPeriodMinutes { get; set; } = 45;
    public int WorkingDaysPerWeek { get; set; } = 6;
    [StringLength(30)] public string DateFormat { get; set; } = "dd-MM-yyyy";
    [StringLength(10)] public string DefaultLanguage { get; set; } = "en-IN";
    [StringLength(20)] public string AdmissionNumberPrefix { get; set; } = "ADM";
    [StringLength(20)] public string StudentNumberPrefix { get; set; } = "STU";
    public string PreferencesJson { get; set; } = "{}";
}

public class AppSetting : EntityBase
{
    [Required, StringLength(150)] public string Key { get; set; } = null!;
    [Required] public string Value { get; set; } = null!;
    public SettingValueType ValueType { get; set; }
    [StringLength(500)] public string? Description { get; set; }
    public bool IsSensitive { get; set; }
}

public class TenantSetting : TenantEntityBase
{
    [Required, StringLength(150)] public string Key { get; set; } = null!;
    [Required] public string Value { get; set; } = null!;
    public SettingValueType ValueType { get; set; }
    [StringLength(500)] public string? Description { get; set; }
    public bool IsSensitive { get; set; }
}

public class LookupItem : TenantEntityBase
{
    [Required, StringLength(80)] public string LookupType { get; set; } = null!;
    [Required, StringLength(80)] public string Code { get; set; } = null!;
    [Required, StringLength(150)] public string DisplayName { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string MetadataJson { get; set; } = "{}";
}

public class AuditLog : EntityBase
{
    public Guid? TenantId { get; set; }
    public Guid? BranchId { get; set; }
    [Required, StringLength(200)] public string EntityType { get; set; } = null!;
    [Required, StringLength(100)] public string EntityId { get; set; } = null!;
    public AuditAction Action { get; set; }
    [StringLength(450)] public string? ActorId { get; set; }
    [StringLength(254)] public string? ActorEmail { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    [StringLength(100)] public string? CorrelationId { get; set; }
    [StringLength(64)] public string? IpAddress { get; set; }
}

public class BackgroundJobLog : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    [Required, StringLength(120)] public string JobType { get; set; } = null!;
    [Required, StringLength(120)] public string JobKey { get; set; } = null!;
    public BackgroundJobStatus Status { get; set; } = BackgroundJobStatus.Queued;
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public DateTime? ScheduledOn { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? NextRetryOn { get; set; }
    public string PayloadJson { get; set; } = "{}";
    [StringLength(2000)] public string? LastError { get; set; }
    [StringLength(100)] public string? CorrelationId { get; set; }
}

public class PermissionDefinition : EntityBase
{
    [Required, StringLength(120)] public string Key { get; set; } = null!;
    [Required, StringLength(150)] public string DisplayName { get; set; } = null!;
    [Required, StringLength(80)] public string Module { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class RolePermissionGrant : TenantEntityBase
{
    public Guid RoleId { get; set; }
    public ApplicationRole? Role { get; set; }
    public Guid PermissionDefinitionId { get; set; }
    public PermissionDefinition? PermissionDefinition { get; set; }
    public bool IsAllowed { get; set; } = true;
}

public class UserPermissionGrant : TenantEntityBase
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public Guid PermissionDefinitionId { get; set; }
    public PermissionDefinition? PermissionDefinition { get; set; }
    public bool IsAllowed { get; set; } = true;
}
