using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.Operations;

public abstract class OperationsTenantScopedDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

public class AdmissionApplicationDto : OperationsTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid? AcademicYearId { get; set; }
    public Guid CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? SectionId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string ApplicantFirstName { get; set; } = string.Empty;
    public string? ApplicantMiddleName { get; set; }
    public string ApplicantLastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? Address { get; set; }
    public DateOnly AppliedOn { get; set; }
    public AdmissionApplicationStatus Status { get; set; }
    public string? ReviewNotes { get; set; }
}

public class CreateAdmissionApplicationRequest
{
    public Guid BranchId { get; set; }
    public Guid? AcademicYearId { get; set; }
    public Guid CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? SectionId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string ApplicantFirstName { get; set; } = string.Empty;
    public string? ApplicantMiddleName { get; set; }
    public string ApplicantLastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? Address { get; set; }
    public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public AdmissionApplicationStatus Status { get; set; } = AdmissionApplicationStatus.Submitted;
    public string? ReviewNotes { get; set; }
}

public class UpdateAdmissionApplicationRequest : CreateAdmissionApplicationRequest { }

public class AdmissionDocumentDto : OperationsTenantScopedDto
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionDocumentType DocumentType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? StoragePath { get; set; }
    public bool IsVerified { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? Notes { get; set; }
}

public class CreateAdmissionDocumentRequest
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;
    public string DisplayName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? StoragePath { get; set; }
    public bool IsVerified { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAdmissionDocumentRequest : CreateAdmissionDocumentRequest { }

public class AdmissionReviewDto : OperationsTenantScopedDto
{
    public Guid AdmissionApplicationId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public AdmissionApplicationStatus FromStatus { get; set; }
    public AdmissionApplicationStatus ToStatus { get; set; }
    public DateTime ReviewedOn { get; set; }
    public string? Notes { get; set; }
}

public class CreateAdmissionReviewRequest
{
    public Guid AdmissionApplicationId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public AdmissionApplicationStatus FromStatus { get; set; }
    public AdmissionApplicationStatus ToStatus { get; set; }
    public DateTime ReviewedOn { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public class EnrollmentDto : OperationsTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid? AdmissionApplicationId { get; set; }
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid CourseId { get; set; }
    public Guid BatchId { get; set; }
    public Guid? SectionId { get; set; }
    public string EnrollmentNumber { get; set; } = string.Empty;
    public DateOnly EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateEnrollmentRequest
{
    public Guid StudentProfileId { get; set; }
    public Guid? AdmissionApplicationId { get; set; }
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid CourseId { get; set; }
    public Guid BatchId { get; set; }
    public Guid? SectionId { get; set; }
    public string EnrollmentNumber { get; set; } = string.Empty;
    public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public string? Notes { get; set; }
}

public class UpdateEnrollmentRequest : CreateEnrollmentRequest { }

public class AttendanceSessionDto : OperationsTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid SectionId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? TimetableEntryId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public AttendanceSessionType SessionType { get; set; }
    public int? PeriodNumber { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
    public Guid? MarkedByUserId { get; set; }
    public AttendanceSessionStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateAttendanceSessionRequest
{
    public Guid BranchId { get; set; }
    public Guid SectionId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? TimetableEntryId { get; set; }
    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.Daily;
    public int? PeriodNumber { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
    public Guid? MarkedByUserId { get; set; }
    public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Draft;
    public string? Notes { get; set; }
}

public class UpdateAttendanceSessionRequest : CreateAttendanceSessionRequest { }

public class AttendanceRecordDto : OperationsTenantScopedDto
{
    public Guid AttendanceSessionId { get; set; }
    public Guid StudentProfileId { get; set; }
    public AttendanceStatus Status { get; set; }
    public Guid? MarkedByUserId { get; set; }
    public DateTime MarkedOn { get; set; }
    public string? Remarks { get; set; }
}

public class CreateAttendanceRecordRequest
{
    public Guid AttendanceSessionId { get; set; }
    public Guid StudentProfileId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public Guid? MarkedByUserId { get; set; }
    public DateTime MarkedOn { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
}

public class UpdateAttendanceRecordRequest : CreateAttendanceRecordRequest { }

public class AttendancePolicyDto : OperationsTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MinimumPercentage { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateAttendancePolicyRequest
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MinimumPercentage { get; set; } = 75;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsDefault { get; set; }
}

public class UpdateAttendancePolicyRequest : CreateAttendancePolicyRequest { }

public class LeaveApplicationDto : OperationsTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string? Reason { get; set; }
    public LeaveApplicationStatus Status { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public string? ReviewNotes { get; set; }
}

public class CreateLeaveApplicationRequest
{
    public Guid StudentProfileId { get; set; }
    public LeaveType LeaveType { get; set; } = LeaveType.Other;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string? Reason { get; set; }
    public LeaveApplicationStatus Status { get; set; } = LeaveApplicationStatus.Submitted;
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public string? ReviewNotes { get; set; }
}

public class UpdateLeaveApplicationRequest : CreateLeaveApplicationRequest { }

public class AttendanceAlertDto : OperationsTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid? AttendancePolicyId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal ThresholdPercentage { get; set; }
    public DateTime GeneratedOn { get; set; }
    public AttendanceAlertStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CreateAttendanceAlertRequest
{
    public Guid StudentProfileId { get; set; }
    public Guid? AttendancePolicyId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal ThresholdPercentage { get; set; } = 75;
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public AttendanceAlertStatus Status { get; set; } = AttendanceAlertStatus.Pending;
    public string Message { get; set; } = string.Empty;
}

public class UpdateAttendanceAlertRequest : CreateAttendanceAlertRequest { }

public class RoomDto : OperationsTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRoomRequest
{
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public RoomType Type { get; set; } = RoomType.Classroom;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateRoomRequest : CreateRoomRequest { }

public class TimeSlotDto : OperationsTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public bool IsBreak { get; set; }
}

public class CreateTimeSlotRequest
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public bool IsBreak { get; set; }
}

public class UpdateTimeSlotRequest : CreateTimeSlotRequest { }

public class TimetableDto : OperationsTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public TimetableStatus Status { get; set; }
}

public class CreateTimetableRequest
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public TimetableStatus Status { get; set; } = TimetableStatus.Draft;
}

public class UpdateTimetableRequest : CreateTimetableRequest { }

public class TimetableEntryDto : OperationsTenantScopedDto
{
    public Guid TimetableId { get; set; }
    public Guid SectionId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid TeacherProfileId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid? RoomId { get; set; }
    public string? Notes { get; set; }
}

public class CreateTimetableEntryRequest
{
    public Guid TimetableId { get; set; }
    public Guid SectionId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid TeacherProfileId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid? RoomId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTimetableEntryRequest : CreateTimetableEntryRequest { }
