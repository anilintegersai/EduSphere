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
