using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class TeacherQualificationRecord : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; } public TeacherProfile? TeacherProfile { get; set; }
    [Required, StringLength(150)] public string Qualification { get; set; } = null!;
    [StringLength(200)] public string? Institution { get; set; }
    [StringLength(120)] public string? Specialization { get; set; }
    public int? CompletionYear { get; set; }
    [StringLength(100)] public string? CertificateNumber { get; set; }
    public bool IsVerified { get; set; }
}

public class TeacherExperienceRecord : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; } public TeacherProfile? TeacherProfile { get; set; }
    [Required, StringLength(180)] public string Organization { get; set; } = null!;
    [Required, StringLength(120)] public string Designation { get; set; } = null!;
    public DateOnly FromDate { get; set; } public DateOnly? ToDate { get; set; }
    [StringLength(500)] public string? Responsibilities { get; set; }
    public bool IsCurrent { get; set; }
}

public class StaffAttendanceRecord : TenantEntityBase
{
    public Guid StaffProfileId { get; set; } public StaffProfile? StaffProfile { get; set; }
    public Guid BranchId { get; set; } public Branch? Branch { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public StaffAttendanceStatus Status { get; set; }
    public TimeOnly? CheckIn { get; set; } public TimeOnly? CheckOut { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public class StaffLeaveRequest : TenantEntityBase
{
    public Guid StaffProfileId { get; set; } public StaffProfile? StaffProfile { get; set; }
    public Guid BranchId { get; set; } public Branch? Branch { get; set; }
    [Required, StringLength(80)] public string LeaveType { get; set; } = null!;
    public DateOnly FromDate { get; set; } public DateOnly ToDate { get; set; }
    [Required, StringLength(1000)] public string Reason { get; set; } = null!;
    public StaffLeaveStatus Status { get; set; } = StaffLeaveStatus.Submitted;
    public Guid? ReviewedByUserId { get; set; } public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime? ReviewedOn { get; set; } [StringLength(500)] public string? ReviewNotes { get; set; }
}

public class StaffPayrollProfile : TenantEntityBase
{
    public Guid StaffProfileId { get; set; } public StaffProfile? StaffProfile { get; set; }
    public decimal BasicSalary { get; set; } public decimal Allowances { get; set; } public decimal Deductions { get; set; }
    [StringLength(100)] public string? BankAccountReference { get; set; }
    [StringLength(80)] public string? TaxIdentifier { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PayrollRun : TenantEntityBase
{
    public Guid BranchId { get; set; } public Branch? Branch { get; set; }
    public int Year { get; set; } public int Month { get; set; }
    public PayrollRunStatus Status { get; set; }
    public DateTime? ApprovedOn { get; set; } public Guid? ApprovedByUserId { get; set; }
    [StringLength(500)] public string? ExportFileName { get; set; }
    public ICollection<PayrollEntry> Entries { get; set; } = new List<PayrollEntry>();
}

public class PayrollEntry : TenantEntityBase
{
    public Guid PayrollRunId { get; set; } public PayrollRun? PayrollRun { get; set; }
    public Guid StaffProfileId { get; set; } public StaffProfile? StaffProfile { get; set; }
    public decimal BasicSalary { get; set; } public decimal Allowances { get; set; } public decimal Deductions { get; set; } public decimal NetPay { get; set; }
    [StringLength(100)] public string? ExternalPayrollReference { get; set; }
}

public class SecurityEvent : TenantEntityBase
{
    public Guid? UserId { get; set; } public ApplicationUser? User { get; set; }
    public SecurityEventType EventType { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    [StringLength(254)] public string? Email { get; set; }
    [StringLength(64)] public string? IpAddress { get; set; }
    [StringLength(500)] public string? UserAgent { get; set; }
    public bool Succeeded { get; set; }
    [StringLength(1000)] public string? Details { get; set; }
}

public class AccountEmailJob : TenantEntityBase
{
    public Guid NotificationMessageId { get; set; } public NotificationMessage? NotificationMessage { get; set; }
    public Guid? UserId { get; set; }
    [Required, StringLength(80)] public string JobType { get; set; } = null!;
    public EmailJobStatus Status { get; set; } = EmailJobStatus.Queued;
    public int AttemptCount { get; set; } public int MaximumAttempts { get; set; } = 5;
    public DateTime AvailableOn { get; set; } = DateTime.UtcNow; public DateTime? CompletedOn { get; set; }
    [StringLength(2000)] public string? LastError { get; set; }
}

public class EmailBounceEvent : TenantEntityBase
{
    public Guid? NotificationMessageId { get; set; }
    [Required, StringLength(254)] public string Recipient { get; set; } = null!;
    [Required, StringLength(80)] public string BounceType { get; set; } = null!;
    [StringLength(200)] public string? ProviderEventId { get; set; }
    [StringLength(2000)] public string? Reason { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}

public class UserImportBatch : TenantEntityBase
{
    public Guid RequestedByUserId { get; set; }
    public Guid? BranchId { get; set; }
    [Required, StringLength(80)] public string RoleName { get; set; } = null!;
    public UserImportBatchStatus Status { get; set; }
    public int TotalRows { get; set; } public int ProcessedRows { get; set; } public int SucceededRows { get; set; } public int FailedRows { get; set; }
    public bool SendActivationEmail { get; set; }
    [StringLength(1000)] public string? LastError { get; set; }
    public ICollection<UserImportRow> Rows { get; set; } = new List<UserImportRow>();
}

public class UserImportRow : TenantEntityBase
{
    public Guid UserImportBatchId { get; set; } public UserImportBatch? UserImportBatch { get; set; }
    public int RowNumber { get; set; }
    [Required, StringLength(4000)] public string PayloadJson { get; set; } = null!;
    public bool IsValid { get; set; } public bool IsProcessed { get; set; } public bool Succeeded { get; set; }
    public Guid? CreatedUserId { get; set; }
    [StringLength(2000)] public string? Error { get; set; }
}
