using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class UserManagementService : IUserManagementService
{
    private const string ActivationProviderKey = "smtp-dev";
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(7);

    private static readonly IReadOnlyDictionary<string, RoleDefinition> RoleDefinitions =
        new[]
        {
            new RoleDefinition(Roles.SuperAdmin, "Super Admin", 100, UserType.SuperAdmin, false, UserProfileKind.None),
            new RoleDefinition(Roles.TenantAdmin, "Tenant Admin", 90, UserType.TenantAdmin, false, UserProfileKind.None),
            new RoleDefinition(Roles.BranchAdmin, "Branch Admin", 80, UserType.BranchAdmin, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.Principal, "Principal / Director", 70, UserType.Principal, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.DepartmentAdmin, "HOD / Department Admin", 60, UserType.HeadOfDepartment, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.StaffAdmin, "HR / Staff Admin", 55, UserType.Staff, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.ExamController, "Exam Controller", 50, UserType.Staff, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.Accountant, "Accountant", 45, UserType.Accountant, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.Librarian, "Librarian", 45, UserType.Librarian, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.TransportManager, "Transport Manager", 45, UserType.Staff, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.HostelManager, "Hostel Manager", 45, UserType.Staff, true, UserProfileKind.Staff),
            new RoleDefinition(Roles.Teacher, "Teacher", 40, UserType.Teacher, true, UserProfileKind.Teacher),
            new RoleDefinition(Roles.Parent, "Parent", 30, UserType.Parent, true, UserProfileKind.Parent),
            new RoleDefinition(Roles.Student, "Student", 20, UserType.Student, true, UserProfileKind.Student)
        }.ToDictionary(r => r.RoleName, StringComparer.OrdinalIgnoreCase);

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess,
        INotificationDispatcher notificationDispatcher,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
        _notificationDispatcher = notificationDispatcher;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RoleOptionDto>> GetAssignableRolesAsync(
        ClaimsPrincipal actor,
        CancellationToken cancellationToken = default)
    {
        var actorInfo = await ResolveActorAsync(actor);
        if (actorInfo is null)
            return Array.Empty<RoleOptionDto>();

        return RoleDefinitions.Values
            .Where(role => role.RoleName != Roles.SuperAdmin && role.Rank < actorInfo.HighestRoleRank)
            .OrderByDescending(role => role.Rank)
            .ThenBy(role => role.Label)
            .Select(role => new RoleOptionDto
            {
                RoleName = role.RoleName,
                Label = role.Label,
                UserType = role.UserType,
                Rank = role.Rank,
                RequiresBranch = role.RequiresBranch
            })
            .ToList();
    }

    public async Task<IReadOnlyList<UserSummaryDto>> ListUsersAsync(
        ClaimsPrincipal actor,
        UserManagementQuery query,
        CancellationToken cancellationToken = default)
    {
        var actorInfo = await ResolveActorAsync(actor);
        if (actorInfo is null)
            return Array.Empty<UserSummaryDto>();

        var usersQuery = _userManager.Users.AsNoTracking();

        if (actorInfo.IsSuperAdmin)
        {
            var tenantId = query.TenantId ?? _tenantContext.TenantId;
            if (tenantId.HasValue)
                usersQuery = usersQuery.Where(u => u.TenantId == tenantId.Value);
        }
        else
        {
            usersQuery = usersQuery.Where(u => u.TenantId == actorInfo.User.TenantId);
        }

        if (query.BranchId.HasValue)
            usersQuery = usersQuery.Where(u => u.BranchId == query.BranchId.Value);

        if (actorInfo.IsBranchScoped)
        {
            var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
            usersQuery = assignedBranchId.HasValue
                ? usersQuery.Where(u => u.BranchId == assignedBranchId.Value)
                : usersQuery.Where(u => false);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            usersQuery = usersQuery.Where(u =>
                (u.Email != null && u.Email.Contains(search)) ||
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search));
        }

        var users = await usersQuery
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ThenBy(u => u.Email)
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(query.RoleName))
        {
            var roleUsers = await _userManager.GetUsersInRoleAsync(query.RoleName.Trim());
            var roleUserIds = roleUsers.Select(u => u.Id).ToHashSet();
            users = users.Where(u => roleUserIds.Contains(u.Id)).ToList();
        }

        var tenantIds = users.Select(u => u.TenantId).Distinct().ToList();
        var tenantMap = tenantIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _dbContext.Tenants
                .IgnoreQueryFilters()
                .Where(t => tenantIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var branchIds = users.Where(u => u.BranchId.HasValue).Select(u => u.BranchId!.Value).Distinct().ToList();
        var branchMap = branchIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _dbContext.Branches
                .IgnoreQueryFilters()
                .Where(b => branchIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        var result = new List<UserSummaryDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(MapUser(
                user,
                roles,
                tenantMap.TryGetValue(user.TenantId, out var tenantName) ? tenantName : null,
                branchMap.TryGetValue(user.BranchId ?? Guid.Empty, out var branchName) ? branchName : null));
        }

        return result;
    }

    public async Task<UserManagementOperationResult<UserSummaryDto>> CreateUserAsync(
        ClaimsPrincipal actor,
        CreateManagedUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var actorInfo = await ResolveActorAsync(actor);
        if (actorInfo is null)
            return UserManagementOperationResult<UserSummaryDto>.Failure("Only authenticated administrators can create users.");

        var role = ResolveRole(request.RoleName);
        if (role is null)
            return UserManagementOperationResult<UserSummaryDto>.Failure("The selected role is not supported.");

        if (role.RoleName == Roles.SuperAdmin || role.Rank >= actorInfo.HighestRoleRank)
            return UserManagementOperationResult<UserSummaryDto>.Failure("You can create users only below your own administrative level.");

        var tenantValidation = await ResolveTargetTenantIdAsync(actorInfo, request.TenantId, cancellationToken);
        if (!tenantValidation.Succeeded || tenantValidation.Data is null)
            return UserManagementOperationResult<UserSummaryDto>.Failure(tenantValidation.Errors.ToArray());

        var tenantId = tenantValidation.Data.Value;
        var branchValidation = await ResolveTargetBranchIdAsync(actor, actorInfo, tenantId, role, request.BranchId, cancellationToken);
        if (!branchValidation.Succeeded)
            return UserManagementOperationResult<UserSummaryDto>.Failure(branchValidation.Errors.ToArray());

        var branchId = branchValidation.Data;
        var email = request.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
            return UserManagementOperationResult<UserSummaryDto>.Failure("A user with this email already exists.");

        if (!await _roleManager.RoleExistsAsync(role.RoleName))
            return UserManagementOperationResult<UserSummaryDto>.Failure($"Role '{role.RoleName}' has not been provisioned.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = false,
            RequiresActivation = true,
            TenantId = tenantId,
            BranchId = branchId,
            FirstName = request.FirstName.Trim(),
            MiddleName = NormalizeOptional(request.MiddleName),
            LastName = request.LastName.Trim(),
            PhoneNumber = NormalizeOptional(request.PhoneNumber),
            UserType = role.UserType,
            IsActive = true
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return UserManagementOperationResult<UserSummaryDto>.Failure(createResult.Errors.Select(e => e.Description).ToArray());
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role.RoleName);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return UserManagementOperationResult<UserSummaryDto>.Failure(roleResult.Errors.Select(e => e.Description).ToArray());
            }

            var roleEntity = await _roleManager.FindByNameAsync(role.RoleName)
                ?? throw new InvalidOperationException($"Role '{role.RoleName}' disappeared during user creation.");

            _dbContext.UserRoleAssignments.Add(new UserRoleAssignment
            {
                TenantId = tenantId,
                UserId = user.Id,
                RoleId = roleEntity.Id,
                RoleName = role.RoleName,
                BranchId = branchId,
                AssignedByUserId = actorInfo.User.Id == Guid.Empty ? null : actorInfo.User.Id,
                AssignedOn = DateTime.UtcNow,
                IsActive = true,
                Notes = "Created through user management."
            });

            if (branchId.HasValue)
            {
                _dbContext.UserBranchAssignments.Add(new UserBranchAssignment
                {
                    TenantId = tenantId,
                    UserId = user.Id,
                    BranchId = branchId.Value,
                    IsPrimary = true,
                    IsActive = true,
                    EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                    Notes = "Primary branch assignment created with user."
                });
            }

            var profileError = await CreateProfileAsync(user, role, request, branchId, cancellationToken);
            if (profileError is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return UserManagementOperationResult<UserSummaryDto>.Failure(profileError);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Profile or assignment creation failed for {Email}.", user.Email);
            return UserManagementOperationResult<UserSummaryDto>.Failure("User profile or assignment could not be created. Check for duplicate employee/admission numbers.");
        }

        var warnings = new List<string>();
        if (request.SendActivationEmail)
            warnings.AddRange(await SendActivationEmailAsync(user, role.RoleName, actorInfo.User.Id, cancellationToken));

        var mapped = MapUser(user, new[] { role.RoleName }, null, null);
        return UserManagementOperationResult<UserSummaryDto>.Success(
            mapped,
            $"User {user.Email} was created and awaits activation.",
            warnings);
    }

    public async Task<UserManagementOperationResult<BulkUserImportResult>> BulkImportUsersAsync(
        ClaimsPrincipal actor,
        BulkUserImportRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = new BulkUserImportResult();
        var lines = request.CsvText
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (lines.Count == 0)
            return UserManagementOperationResult<BulkUserImportResult>.Failure("CSV text did not contain any rows.");

        var startIndex = LooksLikeHeader(lines[0]) ? 1 : 0;
        for (var i = startIndex; i < lines.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rowNumber = i + 1;
            var fields = SplitCsvLine(lines[i]);
            if (fields.Count < 3)
            {
                result.FailedRows++;
                result.Rows.Add(new BulkUserImportRowResult { RowNumber = rowNumber, Message = "Expected at least email, first name and last name." });
                continue;
            }

            var roleName = fields.ElementAtOrDefault(3);
            var branchToken = fields.ElementAtOrDefault(4);
            var branchId = request.BranchId;
            if (!branchId.HasValue && !string.IsNullOrWhiteSpace(branchToken))
                branchId = await ResolveBranchTokenAsync(request.TenantId, branchToken, cancellationToken);

            var createRequest = new CreateManagedUserRequest
            {
                TenantId = request.TenantId,
                BranchId = branchId,
                RoleName = string.IsNullOrWhiteSpace(roleName) ? request.RoleName : roleName.Trim(),
                Email = fields[0].Trim(),
                FirstName = fields[1].Trim(),
                LastName = fields[2].Trim(),
                PhoneNumber = fields.ElementAtOrDefault(5),
                Designation = fields.ElementAtOrDefault(6),
                SendActivationEmail = request.SendActivationEmail
            };

            result.TotalRows++;
            var created = await CreateUserAsync(actor, createRequest, cancellationToken);
            if (created.Succeeded)
            {
                result.CreatedRows++;
                result.Rows.Add(new BulkUserImportRowResult
                {
                    RowNumber = rowNumber,
                    Email = createRequest.Email,
                    Succeeded = true,
                    Message = created.Warnings.Count == 0
                        ? "Created."
                        : $"Created. {string.Join(" ", created.Warnings)}"
                });
            }
            else
            {
                result.FailedRows++;
                result.Rows.Add(new BulkUserImportRowResult
                {
                    RowNumber = rowNumber,
                    Email = createRequest.Email,
                    Message = string.Join(" ", created.Errors)
                });
            }
        }

        return UserManagementOperationResult<BulkUserImportResult>.Success(
            result,
            $"Bulk import finished: {result.CreatedRows} created, {result.FailedRows} failed.");
    }

    public async Task<UserManagementOperationResult> SetUserActiveAsync(
        ClaimsPrincipal actor,
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var actorInfo = await ResolveActorAsync(actor);
        var target = await _userManager.FindByIdAsync(userId.ToString());
        if (actorInfo is null || target is null)
            return UserManagementOperationResult.Failure("User was not found.");

        if (target.Id == actorInfo.User.Id)
            return UserManagementOperationResult.Failure("You cannot deactivate your own account.");

        var canManage = await CanManageTargetUserAsync(actor, actorInfo, target, cancellationToken);
        if (!canManage)
            return UserManagementOperationResult.Failure("You do not have permission to manage this user.");

        target.IsActive = isActive;
        var result = await _userManager.UpdateAsync(target);
        return result.Succeeded
            ? UserManagementOperationResult.Success(isActive ? "User activated." : "User deactivated.")
            : UserManagementOperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<UserManagementOperationResult> ResendActivationAsync(
        ClaimsPrincipal actor,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var actorInfo = await ResolveActorAsync(actor);
        var target = await _userManager.FindByIdAsync(userId.ToString());
        if (actorInfo is null || target is null)
            return UserManagementOperationResult.Failure("User was not found.");

        if (!target.RequiresActivation)
            return UserManagementOperationResult.Failure("This user has already activated the account.");

        var canManage = await CanManageTargetUserAsync(actor, actorInfo, target, cancellationToken);
        if (!canManage)
            return UserManagementOperationResult.Failure("You do not have permission to resend activation for this user.");

        var roles = await _userManager.GetRolesAsync(target);
        var warnings = await SendActivationEmailAsync(target, roles.FirstOrDefault() ?? "User", actorInfo.User.Id, cancellationToken);
        return UserManagementOperationResult.Success(
            warnings.Count == 0 ? "Activation email sent." : "Activation email was queued, but delivery reported a problem.",
            warnings);
    }

    public async Task<UserManagementOperationResult> ActivateAccountAsync(
        Guid userId,
        string encodedCode,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return UserManagementOperationResult.Failure("Activation link is invalid.");

        if (!user.RequiresActivation && user.EmailConfirmed)
            return UserManagementOperationResult.Success("This account is already active.");

        var code = DecodeToken(encodedCode);
        if (code is null)
            return UserManagementOperationResult.Failure("Activation link is invalid.");

        var tokenHash = HashToken(code);
        var pendingInvitation = await _dbContext.UserInvitations
            .IgnoreQueryFilters()
            .Where(i => i.TenantId == user.TenantId &&
                        i.UserId == user.Id &&
                        i.Status == UserInvitationStatus.Pending &&
                        i.ActivationTokenHash == tokenHash)
            .OrderByDescending(i => i.LastSentOn)
            .FirstOrDefaultAsync(cancellationToken);

        if (pendingInvitation is null)
            return UserManagementOperationResult.Failure("Activation link is invalid.");

        if (pendingInvitation.ExpiresOn < DateTime.UtcNow)
        {
            pendingInvitation.Status = UserInvitationStatus.Expired;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return UserManagementOperationResult.Failure("Activation link has expired. Ask your administrator to resend the invitation.");
        }

        if (!user.EmailConfirmed)
        {
            var confirmResult = await _userManager.ConfirmEmailAsync(user, code);
            if (!confirmResult.Succeeded)
                return UserManagementOperationResult.Failure(confirmResult.Errors.Select(e => e.Description).ToArray());
        }

        if (!await _userManager.HasPasswordAsync(user))
        {
            var passwordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!passwordResult.Succeeded)
                return UserManagementOperationResult.Failure(passwordResult.Errors.Select(e => e.Description).ToArray());
        }

        user.RequiresActivation = false;
        user.ActivatedOn = DateTime.UtcNow;
        user.LastPasswordChangedOn = DateTime.UtcNow;
        user.IsActive = true;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return UserManagementOperationResult.Failure(updateResult.Errors.Select(e => e.Description).ToArray());

        var invitations = await _dbContext.UserInvitations
            .IgnoreQueryFilters()
            .Where(i => i.TenantId == user.TenantId &&
                        i.UserId == user.Id &&
                        i.Status == UserInvitationStatus.Pending &&
                        i.ActivationTokenHash == tokenHash)
            .ToListAsync(cancellationToken);

        foreach (var invitation in invitations)
        {
            invitation.Status = UserInvitationStatus.Accepted;
            invitation.AcceptedOn = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return UserManagementOperationResult.Success("Account activated. You can now sign in.");
    }

    public async Task<UserManagementOperationResult> SendPasswordResetAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null || !user.IsActive || user.RequiresActivation)
            return UserManagementOperationResult.Success("If the account exists, a password reset email has been sent.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedCode = EncodeToken(token);
        var resetUrl = BuildPageUrl("/Account/ResetPassword", new Dictionary<string, string?>
        {
            ["userId"] = user.Id.ToString(),
            ["code"] = encodedCode
        });

        user.LastPasswordResetRequestedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var warnings = await SendAccountEmailAsync(
            user,
            "Reset your EduSphere password",
            $"Hello {user.FullName},<br /><br />Use this secure link to reset your EduSphere password:<br /><a href=\"{resetUrl}\">{resetUrl}</a><br /><br />If you did not request this, you can ignore this message.",
            cancellationToken);

        return UserManagementOperationResult.Success(
            "If the account exists, a password reset email has been sent.",
            warnings);
    }

    public async Task<UserManagementOperationResult> ResetPasswordAsync(
        Guid userId,
        string encodedCode,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.RequiresActivation)
            return UserManagementOperationResult.Failure("Password reset link is invalid.");

        var code = DecodeToken(encodedCode);
        if (code is null)
            return UserManagementOperationResult.Failure("Password reset link is invalid.");

        var result = await _userManager.ResetPasswordAsync(user, code, newPassword);
        if (!result.Succeeded)
            return UserManagementOperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());

        user.LastPasswordChangedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return UserManagementOperationResult.Success("Password reset complete. You can now sign in.");
    }

    public async Task<UserManagementOperationResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || !user.IsActive || user.RequiresActivation)
            return UserManagementOperationResult.Failure("The account is not active.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return UserManagementOperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());

        user.LastPasswordChangedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return UserManagementOperationResult.Success("Password changed.");
    }

    private async Task<string?> CreateProfileAsync(
        ApplicationUser user,
        RoleDefinition role,
        CreateManagedUserRequest request,
        Guid? branchId,
        CancellationToken cancellationToken)
    {
        switch (role.ProfileKind)
        {
            case UserProfileKind.Teacher:
                if (!branchId.HasValue)
                    return "Teacher users require a branch.";

                _dbContext.TeacherProfiles.Add(new TeacherProfile
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    BranchId = branchId.Value,
                    EmployeeNumber = string.IsNullOrWhiteSpace(request.EmployeeNumber)
                        ? await NextTeacherEmployeeNumberAsync(user.TenantId, cancellationToken)
                        : request.EmployeeNumber.Trim(),
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Designation = NormalizeOptional(request.Designation) ?? "Teacher",
                    Qualifications = NormalizeOptional(request.Qualifications),
                    Specializations = NormalizeOptional(request.Specializations),
                    ExperienceYears = request.ExperienceYears,
                    JoiningDate = request.JoiningDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = TeacherStatus.Active
                });
                break;

            case UserProfileKind.Student:
                if (!branchId.HasValue)
                    return "Student users require a branch.";

                _dbContext.StudentProfiles.Add(new StudentProfile
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    BranchId = branchId.Value,
                    AdmissionNumber = string.IsNullOrWhiteSpace(request.AdmissionNumber)
                        ? await NextStudentAdmissionNumberAsync(user.TenantId, cancellationToken)
                        : request.AdmissionNumber.Trim(),
                    RollNumber = NormalizeOptional(request.RollNumber),
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    DateOfBirth = request.DateOfBirth ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12)),
                    Gender = request.Gender,
                    BloodGroup = request.BloodGroup,
                    AdmissionDate = request.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = NormalizeOptional(request.Address),
                    Status = StudentStatus.Active
                });
                break;

            case UserProfileKind.Parent:
                _dbContext.ParentProfiles.Add(new ParentProfile
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    BranchId = branchId,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    Occupation = NormalizeOptional(request.Occupation),
                    Address = NormalizeOptional(request.Address),
                    IsActive = true
                });

                if (request.LinkedStudentProfileId.HasValue)
                {
                    var student = await _dbContext.StudentProfiles
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(s => s.TenantId == user.TenantId &&
                                                  s.Id == request.LinkedStudentProfileId.Value &&
                                                  !s.IsDeleted,
                            cancellationToken);

                    if (student is null)
                        return "Linked student was not found in the selected tenant.";

                    if (branchId.HasValue && student.BranchId != branchId.Value)
                        return "Linked student must belong to the selected branch.";

                    _dbContext.StudentGuardians.Add(new StudentGuardian
                    {
                        TenantId = user.TenantId,
                        StudentProfileId = student.Id,
                        ParentUserId = user.Id,
                        Relationship = request.GuardianRelationship,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Occupation = NormalizeOptional(request.Occupation),
                        IsPrimary = false,
                        HasPortalAccess = request.HasPortalAccess,
                        CanPickup = request.CanPickup
                    });
                }
                break;

            case UserProfileKind.Staff:
                _dbContext.StaffProfiles.Add(new StaffProfile
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    BranchId = branchId,
                    DepartmentId = request.DepartmentId,
                    EmployeeNumber = string.IsNullOrWhiteSpace(request.EmployeeNumber)
                        ? await NextStaffEmployeeNumberAsync(user.TenantId, cancellationToken)
                        : request.EmployeeNumber.Trim(),
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Designation = NormalizeOptional(request.Designation) ?? role.Label,
                    EmploymentType = request.EmploymentType,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    JoiningDate = request.JoiningDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    Qualifications = NormalizeOptional(request.Qualifications),
                    ExperienceYears = request.ExperienceYears,
                    Status = StaffStatus.Active
                });
                break;
        }

        return null;
    }

    private async Task<IReadOnlyList<string>> SendActivationEmailAsync(
        ApplicationUser user,
        string roleName,
        Guid? invitedByUserId,
        CancellationToken cancellationToken)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedCode = EncodeToken(token);
        var activationUrl = BuildPageUrl("/Account/Activate", new Dictionary<string, string?>
        {
            ["userId"] = user.Id.ToString(),
            ["code"] = encodedCode
        });

        var existingPending = await _dbContext.UserInvitations
            .IgnoreQueryFilters()
            .Where(i => i.TenantId == user.TenantId &&
                        i.UserId == user.Id &&
                        i.Status == UserInvitationStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var pending in existingPending)
            pending.Status = UserInvitationStatus.Expired;

        var invitation = new UserInvitation
        {
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            UserId = user.Id,
            Email = user.Email!,
            RoleName = roleName,
            ActivationTokenHash = HashToken(token),
            ExpiresOn = DateTime.UtcNow.Add(InvitationLifetime),
            LastSentOn = DateTime.UtcNow,
            SendAttempts = 1,
            Status = UserInvitationStatus.Pending,
            InvitedByUserId = invitedByUserId
        };
        _dbContext.UserInvitations.Add(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var warnings = await SendAccountEmailAsync(
            user,
            "Activate your EduSphere account",
            $"Hello {user.FullName},<br /><br />Your EduSphere account has been created with the role <strong>{roleName}</strong>.<br />Activate your account and set your password using this secure link:<br /><a href=\"{activationUrl}\">{activationUrl}</a><br /><br />This invitation expires on {invitation.ExpiresOn:yyyy-MM-dd HH:mm} UTC.",
            cancellationToken);

        if (warnings.Count > 0)
        {
            invitation.LastSendError = string.Join(" ", warnings);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return warnings;
    }

    private async Task<IReadOnlyList<string>> SendAccountEmailAsync(
        ApplicationUser user,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        var messageId = Guid.NewGuid();
        _dbContext.NotificationMessages.Add(new NotificationMessage
        {
            Id = messageId,
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            Channel = CommunicationChannel.Email,
            Subject = subject,
            Body = body,
            Status = NotificationStatus.Queued,
            ScheduledOn = DateTime.UtcNow,
            ProviderKey = ActivationProviderKey,
            CreatedForUserId = user.Id
        });

        _dbContext.NotificationRecipients.Add(new NotificationRecipient
        {
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            NotificationMessageId = messageId,
            UserId = user.Id,
            DisplayName = user.FullName,
            DestinationAddress = user.Email!,
            Status = NotificationStatus.Queued
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dispatchResult = await _notificationDispatcher.DispatchAsync(messageId, cancellationToken);
        if (dispatchResult.Succeeded)
            return Array.Empty<string>();

        _logger.LogWarning(
            "Account email {Subject} for {Email} could not be delivered: {Error}",
            subject,
            user.Email,
            dispatchResult.ErrorMessage);

        return new[] { $"Email delivery failed: {dispatchResult.ErrorMessage}" };
    }

    private async Task<UserManagementOperationResult<Guid?>> ResolveTargetTenantIdAsync(
        ActorInfo actorInfo,
        Guid? requestedTenantId,
        CancellationToken cancellationToken)
    {
        Guid? tenantId;
        if (actorInfo.IsSuperAdmin)
        {
            tenantId = requestedTenantId ?? _tenantContext.TenantId;
            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return UserManagementOperationResult<Guid?>.Failure("Select a tenant before creating tenant-owned users.");
        }
        else
        {
            tenantId = actorInfo.User.TenantId;
            if (requestedTenantId.HasValue && requestedTenantId.Value != tenantId.Value)
                return UserManagementOperationResult<Guid?>.Failure("You cannot create users for another tenant.");
        }

        var exists = await _dbContext.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Id == tenantId.Value && !t.IsDeleted, cancellationToken);

        return exists
            ? UserManagementOperationResult<Guid?>.Success(tenantId)
            : UserManagementOperationResult<Guid?>.Failure("Selected tenant was not found.");
    }

    private async Task<UserManagementOperationResult<Guid?>> ResolveTargetBranchIdAsync(
        ClaimsPrincipal actor,
        ActorInfo actorInfo,
        Guid tenantId,
        RoleDefinition role,
        Guid? requestedBranchId,
        CancellationToken cancellationToken)
    {
        if (!role.RequiresBranch)
            return UserManagementOperationResult<Guid?>.Success(null);

        if (!requestedBranchId.HasValue || requestedBranchId.Value == Guid.Empty)
            return UserManagementOperationResult<Guid?>.Failure("The selected role requires a branch.");

        var branch = await _dbContext.Branches
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.TenantId == tenantId && b.Id == requestedBranchId.Value && !b.IsDeleted, cancellationToken);

        if (branch is null)
            return UserManagementOperationResult<Guid?>.Failure("Selected branch was not found in the tenant.");

        if (actorInfo.IsBranchScoped && !await _branchAccess.CanAccessBranchAsync(actor, requestedBranchId.Value))
            return UserManagementOperationResult<Guid?>.Failure("You cannot create users outside your assigned branch.");

        return UserManagementOperationResult<Guid?>.Success(branch.Id);
    }

    private async Task<bool> CanManageTargetUserAsync(
        ClaimsPrincipal actor,
        ActorInfo actorInfo,
        ApplicationUser target,
        CancellationToken cancellationToken)
    {
        if (!actorInfo.IsSuperAdmin && target.TenantId != actorInfo.User.TenantId)
            return false;

        if (actorInfo.IsBranchScoped)
        {
            if (!target.BranchId.HasValue)
                return false;
            if (!await _branchAccess.CanAccessBranchAsync(actor, target.BranchId.Value))
                return false;
        }

        var targetRoles = await _userManager.GetRolesAsync(target);
        var targetRank = targetRoles
            .Select(r => ResolveRole(r)?.Rank ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        return targetRank < actorInfo.HighestRoleRank;
    }

    private async Task<ActorInfo?> ResolveActorAsync(ClaimsPrincipal actor)
    {
        if (actor.Identity?.IsAuthenticated != true)
            return null;

        var idValue = actor.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? actor.FindFirstValue("sub");

        if (!Guid.TryParse(idValue, out var actorId))
            return null;

        var user = await _userManager.FindByIdAsync(actorId.ToString());
        if (user is null || !user.IsActive)
            return null;

        var roleNames = await _userManager.GetRolesAsync(user);
        var highestRank = roleNames
            .Select(r => ResolveRole(r)?.Rank ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        if (highestRank == 0)
            return null;

        var isBranchScoped = _branchAccess.IsBranchAdminOnly(actor);
        return new ActorInfo(user, roleNames, highestRank, roleNames.Contains(Roles.SuperAdmin), isBranchScoped);
    }

    private async Task<Guid?> ResolveBranchTokenAsync(Guid? tenantId, string token, CancellationToken cancellationToken)
    {
        var value = token.Trim();
        if (Guid.TryParse(value, out var branchId))
            return branchId;

        var query = _dbContext.Branches
            .IgnoreQueryFilters()
            .Where(b => !b.IsDeleted && (b.Code == value || b.Name == value));

        if (tenantId.HasValue)
            query = query.Where(b => b.TenantId == tenantId.Value);

        return await query
            .OrderBy(b => b.Name)
            .Select(b => (Guid?)b.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<string> NextTeacherEmployeeNumberAsync(Guid tenantId, CancellationToken cancellationToken)
        => await NextNumberAsync(
            "TCH",
            async number => await _dbContext.TeacherProfiles
                .IgnoreQueryFilters()
                .AnyAsync(t => t.TenantId == tenantId && t.EmployeeNumber == number, cancellationToken),
            cancellationToken);

    private async Task<string> NextStudentAdmissionNumberAsync(Guid tenantId, CancellationToken cancellationToken)
        => await NextNumberAsync(
            "STD",
            async number => await _dbContext.StudentProfiles
                .IgnoreQueryFilters()
                .AnyAsync(s => s.TenantId == tenantId && s.AdmissionNumber == number, cancellationToken),
            cancellationToken);

    private async Task<string> NextStaffEmployeeNumberAsync(Guid tenantId, CancellationToken cancellationToken)
        => await NextNumberAsync(
            "STF",
            async number => await _dbContext.StaffProfiles
                .IgnoreQueryFilters()
                .AnyAsync(s => s.TenantId == tenantId && s.EmployeeNumber == number, cancellationToken),
            cancellationToken);

    private static async Task<string> NextNumberAsync(
        string prefix,
        Func<string, Task<bool>> exists,
        CancellationToken cancellationToken)
    {
        for (var i = 1; i < 100_000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var number = $"{prefix}-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await exists(number))
                return number;
        }

        throw new InvalidOperationException($"Could not generate a unique {prefix} number.");
    }

    private string BuildPageUrl(string page, Dictionary<string, string?> values)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var url = httpContext is null
            ? null
            : _linkGenerator.GetUriByPage(httpContext, page, values: values);

        if (!string.IsNullOrWhiteSpace(url))
            return url;

        var baseUrl = (_configuration["App:PublicBaseUrl"] ?? "http://localhost:5217").TrimEnd('/');
        return QueryHelpers.AddQueryString($"{baseUrl}{page}", values);
    }

    private static UserSummaryDto MapUser(ApplicationUser user, IEnumerable<string> roles, string? tenantName, string? branchName)
        => new()
        {
            Id = user.Id,
            TenantId = user.TenantId,
            TenantName = tenantName,
            BranchId = user.BranchId,
            BranchName = branchName,
            Email = user.Email ?? user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            FullName = user.FullName,
            UserType = user.UserType,
            Roles = roles.OrderBy(r => r).ToList(),
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            RequiresActivation = user.RequiresActivation,
            TwoFactorEnabled = user.TwoFactorEnabled,
            ActivatedOn = user.ActivatedOn,
            LastLoginAt = user.LastLoginAt,
            LastPasswordChangedOn = user.LastPasswordChangedOn,
            CreatedOn = user.CreatedOn
        };

    private static RoleDefinition? ResolveRole(string? roleName)
        => !string.IsNullOrWhiteSpace(roleName) && RoleDefinitions.TryGetValue(roleName.Trim(), out var role)
            ? role
            : null;

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string EncodeToken(string token)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    private static string? DecodeToken(string encodedToken)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static bool LooksLikeHeader(string line)
        => line.Contains("email", StringComparison.OrdinalIgnoreCase) &&
           line.Contains("first", StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyList<string> SplitCsvLine(string line)
        => line.Split(',').Select(part => part.Trim()).ToList();

    private sealed record ActorInfo(
        ApplicationUser User,
        IList<string> Roles,
        int HighestRoleRank,
        bool IsSuperAdmin,
        bool IsBranchScoped);

    private sealed record RoleDefinition(
        string RoleName,
        string Label,
        int Rank,
        UserType UserType,
        bool RequiresBranch,
        UserProfileKind ProfileKind);

    private enum UserProfileKind
    {
        None,
        Teacher,
        Student,
        Parent,
        Staff
    }
}
