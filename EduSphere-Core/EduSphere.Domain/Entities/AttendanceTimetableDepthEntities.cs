using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AttendanceDevice : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    [Required, StringLength(120)] public string DeviceIdentifier { get; set; } = null!;
    public AttendanceDeviceType DeviceType { get; set; }
    [StringLength(500)] public string? EndpointUrl { get; set; }
    [StringLength(500)] public string? ApiKeyProtected { get; set; }
    public DateTime? LastSynchronizedOn { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AttendanceDeviceImportBatch : TenantEntityBase
{
    public Guid AttendanceDeviceId { get; set; }
    public AttendanceDevice? AttendanceDevice { get; set; }
    [Required, StringLength(255)] public string FileName { get; set; } = null!;
    public AttendanceImportStatus Status { get; set; } = AttendanceImportStatus.Uploaded;
    public int TotalRows { get; set; }
    public int ImportedRows { get; set; }
    public int RejectedRows { get; set; }
    [StringLength(4000)] public string? ErrorSummary { get; set; }
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedOn { get; set; }
}

public class AttendanceDevicePunch : TenantEntityBase
{
    public Guid AttendanceDeviceId { get; set; }
    public AttendanceDevice? AttendanceDevice { get; set; }
    public Guid AttendanceDeviceImportBatchId { get; set; }
    public AttendanceDeviceImportBatch? AttendanceDeviceImportBatch { get; set; }
    [Required, StringLength(160)] public string ExternalRecordId { get; set; } = null!;
    [Required, StringLength(80)] public string PersonCode { get; set; } = null!;
    public DateTime PunchedOn { get; set; }
    public AttendancePunchDirection Direction { get; set; }
    public Guid? StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public Guid? StaffProfileId { get; set; }
    public StaffProfile? StaffProfile { get; set; }
    public Guid? TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
    public bool IsProcessed { get; set; }
    [StringLength(1000)] public string? ProcessingNotes { get; set; }
    [StringLength(4000)] public string? RawPayload { get; set; }
}

public class AttendanceCondonationRequest : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    [Required, StringLength(1000)] public string Reason { get; set; } = null!;
    [StringLength(255)] public string? EvidenceFileName { get; set; }
    [StringLength(500)] public string? EvidenceStoragePath { get; set; }
    [StringLength(120)] public string? EvidenceContentType { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;
    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime? ReviewedOn { get; set; }
    [StringLength(1000)] public string? ReviewNotes { get; set; }
}

public class AttendanceLeaveReconciliation : TenantEntityBase
{
    public Guid LeaveApplicationId { get; set; }
    public LeaveApplication? LeaveApplication { get; set; }
    public Guid AttendanceRecordId { get; set; }
    public AttendanceRecord? AttendanceRecord { get; set; }
    public AttendanceStatus PreviousStatus { get; set; }
    public AttendanceStatus AppliedStatus { get; set; } = AttendanceStatus.Leave;
    public DateTime ReconciledOn { get; set; } = DateTime.UtcNow;
    public Guid? ReconciledByUserId { get; set; }
    public ApplicationUser? ReconciledByUser { get; set; }
}

public class BusAttendanceRecord : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public Guid TransportRouteId { get; set; }
    public TransportRoute? TransportRoute { get; set; }
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Guid? TransportRouteStopId { get; set; }
    public TransportRouteStop? TransportRouteStop { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public BusAttendanceStatus Status { get; set; }
    public DateTime RecordedOn { get; set; } = DateTime.UtcNow;
    public Guid? RecordedByUserId { get; set; }
    public ApplicationUser? RecordedByUser { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public class TeacherAttendanceRecord : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public StaffAttendanceStatus Status { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public class ParentAbsenceExplanation : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public Guid? AttendanceRecordId { get; set; }
    public AttendanceRecord? AttendanceRecord { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    [Required, StringLength(1000)] public string Explanation { get; set; } = null!;
    public AttendanceExplanationStatus Status { get; set; } = AttendanceExplanationStatus.Submitted;
    public Guid SubmittedByUserId { get; set; }
    public ApplicationUser? SubmittedByUser { get; set; }
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime? ReviewedOn { get; set; }
    [StringLength(1000)] public string? ReviewNotes { get; set; }
}

public class AttendanceAlertSchedule : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    public decimal ThresholdPercentage { get; set; } = 75;
    public int LookbackDays { get; set; } = 30;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public bool IsActive { get; set; } = true;
    public DateTime NextRunOn { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunOn { get; set; }
    public int RunEveryHours { get; set; } = 24;
}
