using System.Security.Claims;
using EduSphere.Domain.Constants;

namespace EduSphere.Web.Authorization;

public enum ModulePermission
{
    ManageTenants,
    ManageTenantWorkspace,
    ManageBranchWorkspace,
    MarkAttendance,
    ManageFinance,
    ManageTransport,
    ManageLibrary,
    ManageHostel,
    ManageCommunications,
    ManageUsers,
    ManageAIQuestionPapers
}

public static class RolePermissionMatrix
{
    private static readonly IReadOnlyDictionary<ModulePermission, string[]> Permissions =
        new Dictionary<ModulePermission, string[]>
        {
            [ModulePermission.ManageTenants] = [Roles.SuperAdmin],
            [ModulePermission.ManageTenantWorkspace] = [Roles.SuperAdmin, Roles.TenantAdmin],
            [ModulePermission.ManageBranchWorkspace] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin],
            [ModulePermission.MarkAttendance] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.Teacher],
            [ModulePermission.ManageFinance] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Accountant],
            [ModulePermission.ManageTransport] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.TransportManager],
            [ModulePermission.ManageLibrary] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Librarian],
            [ModulePermission.ManageHostel] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.HostelManager],
            [ModulePermission.ManageCommunications] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.StaffAdmin],
            [ModulePermission.ManageUsers] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.DepartmentAdmin, Roles.StaffAdmin],
            [ModulePermission.ManageAIQuestionPapers] = [Roles.SuperAdmin, Roles.TenantAdmin, Roles.BranchAdmin, Roles.Principal, Roles.DepartmentAdmin, Roles.Teacher, Roles.ExamController]
        };

    private static readonly IReadOnlyDictionary<string, ModulePermission> PolicyPermissions =
        new Dictionary<string, ModulePermission>
        {
            [AuthorizationPolicies.SuperAdmin] = ModulePermission.ManageTenants,
            [AuthorizationPolicies.TenantAdmin] = ModulePermission.ManageTenantWorkspace,
            [AuthorizationPolicies.BranchAdmin] = ModulePermission.ManageBranchWorkspace,
            [AuthorizationPolicies.AttendanceMarker] = ModulePermission.MarkAttendance,
            [AuthorizationPolicies.FinanceManager] = ModulePermission.ManageFinance,
            [AuthorizationPolicies.TransportManager] = ModulePermission.ManageTransport,
            [AuthorizationPolicies.LibraryManager] = ModulePermission.ManageLibrary,
            [AuthorizationPolicies.HostelManager] = ModulePermission.ManageHostel,
            [AuthorizationPolicies.CommunicationManager] = ModulePermission.ManageCommunications,
            [AuthorizationPolicies.UserManager] = ModulePermission.ManageUsers,
            [AuthorizationPolicies.AIQuestionPaperManager] = ModulePermission.ManageAIQuestionPapers
        };

    public static IReadOnlyCollection<string> RolesFor(ModulePermission permission) => Permissions[permission];

    public static IReadOnlyCollection<string> RolesForPolicy(string policy) => RolesFor(PolicyPermissions[policy]);

    public static bool Allows(ModulePermission permission, string role)
        => Permissions[permission].Contains(role, StringComparer.Ordinal);

    public static bool Allows(ModulePermission permission, ClaimsPrincipal user)
        => Permissions[permission].Any(user.IsInRole);
}
