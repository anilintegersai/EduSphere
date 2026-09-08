using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.UserManagement;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool RequiresActivation { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? ActivatedOn { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastPasswordChangedOn { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class RoleOptionDto
{
    public string RoleName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public int Rank { get; set; }
    public bool RequiresBranch { get; set; }
}

public class UserManagementQuery
{
    public Guid? TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string? RoleName { get; set; }
    public string? Search { get; set; }
}

public class CreateManagedUserRequest
{
    public Guid? TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? LinkedStudentProfileId { get; set; }

    public string RoleName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Designation { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? AdmissionNumber { get; set; }
    public string? RollNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;
    public DateOnly? JoiningDate { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public string? Qualifications { get; set; }
    public string? Specializations { get; set; }
    public decimal ExperienceYears { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.Permanent;
    public string? Occupation { get; set; }
    public string? Address { get; set; }
    public GuardianRelationship GuardianRelationship { get; set; } = GuardianRelationship.Guardian;
    public bool HasPortalAccess { get; set; } = true;
    public bool CanPickup { get; set; } = true;
    public bool SendActivationEmail { get; set; } = true;
}

public class UpdateManagedUserStatusRequest
{
    public bool IsActive { get; set; }
}

public class BulkUserImportRequest
{
    public Guid? TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string CsvText { get; set; } = string.Empty;
    public bool SendActivationEmail { get; set; } = true;
}

public class BulkUserImportResult
{
    public int TotalRows { get; set; }
    public int CreatedRows { get; set; }
    public int FailedRows { get; set; }
    public List<BulkUserImportRowResult> Rows { get; set; } = new();
}

public class BulkUserImportRowResult
{
    public int RowNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ActivateAccountRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserManagementOperationResult
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

    public static UserManagementOperationResult Success(string? message = null, IReadOnlyList<string>? warnings = null)
        => new() { Succeeded = true, Message = message, Warnings = warnings ?? Array.Empty<string>() };

    public static UserManagementOperationResult Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}

public class UserManagementOperationResult<T> : UserManagementOperationResult
{
    public T? Data { get; init; }

    public static UserManagementOperationResult<T> Success(T data, string? message = null, IReadOnlyList<string>? warnings = null)
        => new() { Succeeded = true, Data = data, Message = message, Warnings = warnings ?? Array.Empty<string>() };

    public new static UserManagementOperationResult<T> Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}
