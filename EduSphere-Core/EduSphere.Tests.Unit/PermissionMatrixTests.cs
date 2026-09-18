using EduSphere.Domain.Constants;
using EduSphere.Web.Authorization;
using Xunit;

namespace EduSphere.Tests.Unit;

public class PermissionMatrixTests
{
    public static IEnumerable<object[]> MatrixCases()
    {
        var expected = new Dictionary<ModulePermission, string[]>
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

        foreach (var permission in Enum.GetValues<ModulePermission>())
        foreach (var role in Roles.SeedIds.Keys)
            yield return [permission, role, expected[permission].Contains(role)];
    }

    [Theory]
    [MemberData(nameof(MatrixCases))]
    public void EverySeededRoleHasExplicitPermissionDecision(ModulePermission permission, string role, bool expected)
        => Assert.Equal(expected, RolePermissionMatrix.Allows(permission, role));

    [Fact]
    public void PolicyMappingsStayAlignedWithNavigationPermissions()
    {
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageTenants), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.SuperAdmin));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageTenantWorkspace), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.TenantAdmin));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageBranchWorkspace), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.BranchAdmin));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.MarkAttendance), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.AttendanceMarker));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageFinance), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.FinanceManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageTransport), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.TransportManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageLibrary), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.LibraryManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageHostel), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.HostelManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageCommunications), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.CommunicationManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageUsers), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.UserManager));
        Assert.Equal(RolePermissionMatrix.RolesFor(ModulePermission.ManageAIQuestionPapers), RolePermissionMatrix.RolesForPolicy(AuthorizationPolicies.AIQuestionPaperManager));
    }
}
