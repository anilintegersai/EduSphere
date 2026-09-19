using System.Security.Claims;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Authorization;

public sealed record PermissionRequirement(string PermissionKey) : IAuthorizationRequirement;

public sealed class PermissionAuthorizationHandler(IEffectivePermissionService permissions) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var value = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub");
        if (Guid.TryParse(value, out var userId) && await permissions.HasPermissionAsync(userId, requirement.PermissionKey))
            context.Succeed(requirement);
    }
}

public sealed class EffectivePermissionService(TenantDbContext db, ITenantContext tenant) : IEffectivePermissionService
{
    private static readonly IReadOnlyDictionary<string, string[]> RoleDefaults = new Dictionary<string, string[]>
    {
        [PermissionKeys.AdministrationView] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin],
        [PermissionKeys.AdministrationManageTenant] = [Roles.SuperAdmin, Roles.TenantAdmin],
        [PermissionKeys.AdministrationManageGlobal] = [Roles.SuperAdmin],
        [PermissionKeys.AdministrationManagePermissions] = [Roles.SuperAdmin, Roles.TenantAdmin],
        [PermissionKeys.TenantsManage] = [Roles.SuperAdmin],
        [PermissionKeys.TenantWorkspaceManage] = [Roles.SuperAdmin, Roles.TenantAdmin],
        [PermissionKeys.BranchWorkspaceManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin],
        [PermissionKeys.AttendanceMark] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.Teacher],
        [PermissionKeys.FinanceManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Accountant],
        [PermissionKeys.TransportManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.TransportManager],
        [PermissionKeys.LibraryManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Librarian],
        [PermissionKeys.HostelManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.HostelManager],
        [PermissionKeys.CommunicationsManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.StaffAdmin],
        [PermissionKeys.UsersManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.DepartmentAdmin, Roles.StaffAdmin],
        [PermissionKeys.AIQuestionPapersManage] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.DepartmentAdmin, Roles.Teacher, Roles.ExamController]
    };

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionKey, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.AsNoTracking().IgnoreQueryFilters().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null || !user.IsActive) return false;
        var permission = await db.PermissionDefinitions.AsNoTracking().IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Key == permissionKey && p.IsActive, cancellationToken);
        var roleNames = await (from ur in db.UserRoles join role in db.Roles on ur.RoleId equals role.Id where ur.UserId == userId select role.Name!)
            .ToListAsync(cancellationToken);
        if (roleNames.Contains(Roles.SuperAdmin) && permissionKey != PermissionKeys.AdministrationManagePermissions) return true;
        if (!tenant.TenantId.HasValue)
            return RoleDefaults.TryGetValue(permissionKey, out var globalDefaults) && globalDefaults.Any(roleNames.Contains);
        if (permission is not null)
        {
            var userOverride = await db.UserPermissionGrants.AsNoTracking().IgnoreQueryFilters()
                .Where(g => g.TenantId == tenant.TenantId && g.UserId == userId && g.PermissionDefinitionId == permission.Id)
                .Select(g => (bool?)g.IsAllowed).SingleOrDefaultAsync(cancellationToken);
            if (userOverride.HasValue) return userOverride.Value;
            var roleIds = await db.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync(cancellationToken);
            var roleOverrides = await db.RolePermissionGrants.AsNoTracking().IgnoreQueryFilters()
                .Where(g => g.TenantId == tenant.TenantId && roleIds.Contains(g.RoleId) && g.PermissionDefinitionId == permission.Id)
                .Select(g => g.IsAllowed).ToListAsync(cancellationToken);
            if (roleOverrides.Contains(false)) return false;
            if (roleOverrides.Contains(true)) return true;
        }
        return RoleDefaults.TryGetValue(permissionKey, out var defaults) && defaults.Any(roleNames.Contains);
    }
}

public static class AdministrationPolicies
{
    public const string View = "Administration.View";
    public const string ManageTenant = "Administration.ManageTenant";
    public const string ManageGlobal = "Administration.ManageGlobal";
    public const string ManagePermissions = "Administration.ManagePermissions";
}
