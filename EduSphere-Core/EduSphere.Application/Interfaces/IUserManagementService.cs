using System.Security.Claims;
using EduSphere.Application.DTOs.UserManagement;

namespace EduSphere.Application.Interfaces;

public interface IUserManagementService
{
    Task<IReadOnlyList<RoleOptionDto>> GetAssignableRolesAsync(
        ClaimsPrincipal actor,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserSummaryDto>> ListUsersAsync(
        ClaimsPrincipal actor,
        UserManagementQuery query,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult<UserSummaryDto>> CreateUserAsync(
        ClaimsPrincipal actor,
        CreateManagedUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult<BulkUserImportResult>> BulkImportUsersAsync(
        ClaimsPrincipal actor,
        BulkUserImportRequest request,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> SetUserActiveAsync(
        ClaimsPrincipal actor,
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> ResendActivationAsync(
        ClaimsPrincipal actor,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> ActivateAccountAsync(
        Guid userId,
        string encodedCode,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> SendPasswordResetAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> ResetPasswordAsync(
        Guid userId,
        string encodedCode,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<UserManagementOperationResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
