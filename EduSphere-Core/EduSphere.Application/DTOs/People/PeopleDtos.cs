using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.People;

public abstract class PeopleTenantScopedDto
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

public class StudentProfileDto : PeopleTenantScopedDto
{
    public Guid? UserId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? SectionId { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string? RollNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public BloodGroup BloodGroup { get; set; }
    public DateOnly AdmissionDate { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Address { get; set; }
    public StudentStatus Status { get; set; }
}

public class CreateStudentProfileRequest
{
    public Guid? UserId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? SectionId { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string? RollNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;
    public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Address { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
}

public class UpdateStudentProfileRequest : CreateStudentProfileRequest { }

public class TeacherProfileDto : PeopleTenantScopedDto
{
    public Guid? UserId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Designation { get; set; }
    public string? Qualifications { get; set; }
    public string? Specializations { get; set; }
    public decimal ExperienceYears { get; set; }
    public DateOnly JoiningDate { get; set; }
    public TeacherStatus Status { get; set; }
}

public class CreateTeacherProfileRequest
{
    public Guid? UserId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Designation { get; set; }
    public string? Qualifications { get; set; }
    public string? Specializations { get; set; }
    public decimal ExperienceYears { get; set; }
    public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;
}

public class UpdateTeacherProfileRequest : CreateTeacherProfileRequest { }

public class StudentLifecycleEventDto : PeopleTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid BranchId { get; set; }
    public StudentLifecycleEventType EventType { get; set; }
    public StudentStatus FromStatus { get; set; }
    public StudentStatus ToStatus { get; set; }
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromAcademicYearId { get; set; }
    public Guid? ToAcademicYearId { get; set; }
    public Guid? FromCourseId { get; set; }
    public Guid? ToCourseId { get; set; }
    public Guid? FromBatchId { get; set; }
    public Guid? ToBatchId { get; set; }
    public Guid? FromSectionId { get; set; }
    public Guid? ToSectionId { get; set; }
    public DateOnly EffectiveOn { get; set; }
    public DateTime RecordedOn { get; set; }
    public Guid? RecordedByUserId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class StudentLifecycleRequestDto : PeopleTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid BranchId { get; set; }
    public StudentLifecycleEventType EventType { get; set; }
    public StudentStatus ToStatus { get; set; }
    public ApprovalStatus Status { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? ToAcademicYearId { get; set; }
    public Guid? ToCourseId { get; set; }
    public Guid? ToBatchId { get; set; }
    public Guid? ToSectionId { get; set; }
    public DateOnly EffectiveOn { get; set; }
    public DateTime RequestedOn { get; set; }
    public Guid? RequestedByUserId { get; set; }
    public DateTime? DecidedOn { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public Guid? AppliedStudentLifecycleEventId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? DecisionNotes { get; set; }
}

public class CreateStudentLifecycleEventRequest
{
    public Guid StudentProfileId { get; set; }
    public StudentLifecycleEventType EventType { get; set; } = StudentLifecycleEventType.Enrolled;
    public StudentStatus ToStatus { get; set; } = StudentStatus.Active;
    public Guid? ToBranchId { get; set; }
    public Guid? ToAcademicYearId { get; set; }
    public Guid? ToCourseId { get; set; }
    public Guid? ToBatchId { get; set; }
    public Guid? ToSectionId { get; set; }
    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class CreateStudentLifecycleRequestRequest : CreateStudentLifecycleEventRequest { }

public class DecideStudentLifecycleRequestRequest
{
    public string? DecisionNotes { get; set; }
}

public class StudentAlumniRecordDto : PeopleTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? AcademicYearId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public string AlumniNumber { get; set; } = string.Empty;
    public DateOnly GraduationDate { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? HigherEducation { get; set; }
    public string? EmployerOrInstitution { get; set; }
    public string? Notes { get; set; }
}

public class TeacherLifecycleEventDto : PeopleTenantScopedDto
{
    public Guid TeacherProfileId { get; set; }
    public Guid BranchId { get; set; }
    public TeacherLifecycleEventType EventType { get; set; }
    public TeacherStatus FromStatus { get; set; }
    public TeacherStatus ToStatus { get; set; }
    public Guid? FromBranchId { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? FromDepartmentId { get; set; }
    public Guid? ToDepartmentId { get; set; }
    public DateOnly EffectiveOn { get; set; }
    public DateTime RecordedOn { get; set; }
    public Guid? RecordedByUserId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class TeacherLifecycleRequestDto : PeopleTenantScopedDto
{
    public Guid TeacherProfileId { get; set; }
    public Guid BranchId { get; set; }
    public TeacherLifecycleEventType EventType { get; set; }
    public TeacherStatus ToStatus { get; set; }
    public ApprovalStatus Status { get; set; }
    public Guid? ToBranchId { get; set; }
    public Guid? ToDepartmentId { get; set; }
    public DateOnly EffectiveOn { get; set; }
    public DateTime RequestedOn { get; set; }
    public Guid? RequestedByUserId { get; set; }
    public DateTime? DecidedOn { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public Guid? AppliedTeacherLifecycleEventId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? DecisionNotes { get; set; }
}

public class CreateTeacherLifecycleEventRequest
{
    public Guid TeacherProfileId { get; set; }
    public TeacherLifecycleEventType EventType { get; set; } = TeacherLifecycleEventType.Onboarded;
    public TeacherStatus ToStatus { get; set; } = TeacherStatus.Active;
    public Guid? ToBranchId { get; set; }
    public Guid? ToDepartmentId { get; set; }
    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class CreateTeacherLifecycleRequestRequest : CreateTeacherLifecycleEventRequest { }

public class DecideTeacherLifecycleRequestRequest
{
    public string? DecisionNotes { get; set; }
}

public class PeopleOperationResult
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static PeopleOperationResult Success(string? message = null)
        => new() { Succeeded = true, Message = message };

    public static PeopleOperationResult Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}

public class PeopleOperationResult<T> : PeopleOperationResult
{
    public T? Data { get; init; }

    public static PeopleOperationResult<T> Success(T data, string? message = null)
        => new() { Succeeded = true, Data = data, Message = message };

    public new static PeopleOperationResult<T> Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}

public class StudentGuardianDto : PeopleTenantScopedDto
{
    public Guid StudentProfileId { get; set; }
    public Guid? ParentUserId { get; set; }
    public GuardianRelationship Relationship { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }
    public bool IsPrimary { get; set; }
    public bool HasPortalAccess { get; set; }
    public bool CanPickup { get; set; }
}

public class CreateStudentGuardianRequest
{
    public Guid StudentProfileId { get; set; }
    public Guid? ParentUserId { get; set; }
    public GuardianRelationship Relationship { get; set; } = GuardianRelationship.Guardian;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Occupation { get; set; }
    public bool IsPrimary { get; set; }
    public bool HasPortalAccess { get; set; }
    public bool CanPickup { get; set; }
}

public class UpdateStudentGuardianRequest : CreateStudentGuardianRequest { }

public class TeacherSubjectAssignmentDto : PeopleTenantScopedDto
{
    public Guid TeacherProfileId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? SectionId { get; set; }
    public bool IsPrimary { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveUntil { get; set; }
}

public class CreateTeacherSubjectAssignmentRequest
{
    public Guid TeacherProfileId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? SectionId { get; set; }
    public bool IsPrimary { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveUntil { get; set; }
}

public class UpdateTeacherSubjectAssignmentRequest : CreateTeacherSubjectAssignmentRequest { }

public class ProfileDocumentDto : PeopleTenantScopedDto
{
    public ProfileDocumentOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public long? SizeBytes { get; set; }
    public DateTime UploadedOn { get; set; }
    public bool IsVerified { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? Notes { get; set; }
}

public class CreateProfileDocumentRequest
{
    public ProfileDocumentOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public long? SizeBytes { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProfileDocumentRequest : CreateProfileDocumentRequest
{
    public bool IsVerified { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
}
