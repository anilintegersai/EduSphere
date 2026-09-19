namespace EduSphere.Domain.Constants;

public static class PermissionKeys
{
    public const string AdministrationView = "administration.view";
    public const string AdministrationManageTenant = "administration.manage-tenant";
    public const string AdministrationManageGlobal = "administration.manage-global";
    public const string AdministrationManagePermissions = "administration.manage-permissions";
    public const string TenantsManage = "tenants.manage";
    public const string TenantWorkspaceManage = "tenant-workspace.manage";
    public const string BranchWorkspaceManage = "branch-workspace.manage";
    public const string AttendanceMark = "attendance.mark";
    public const string FinanceManage = "finance.manage";
    public const string TransportManage = "transport.manage";
    public const string LibraryManage = "library.manage";
    public const string HostelManage = "hostel.manage";
    public const string CommunicationsManage = "communications.manage";
    public const string UsersManage = "users.manage";
    public const string AIQuestionPapersManage = "ai-question-papers.manage";

    public static readonly IReadOnlyDictionary<string, (string Name, string Module, string Description)> Definitions =
        new Dictionary<string, (string, string, string)>
        {
            [AdministrationView] = ("View administration", "Administration", "View settings, audit history and background jobs."),
            [AdministrationManageTenant] = ("Manage tenant settings", "Administration", "Manage branding, domains, integrations, flags, branch configuration and lookups."),
            [AdministrationManageGlobal] = ("Manage global settings", "Administration", "Manage subscription plans and platform-wide settings."),
            [AdministrationManagePermissions] = ("Manage permission grants", "Security", "Assign tenant-scoped role and user permission overrides."),
            [TenantsManage] = ("Manage tenants", "Tenants", "Create, update, activate and suspend tenants."),
            [TenantWorkspaceManage] = ("Manage tenant workspace", "Tenants", "Manage branches and tenant-wide academic configuration."),
            [BranchWorkspaceManage] = ("Manage branch workspace", "Branches", "Manage branch academic and operational records."),
            [AttendanceMark] = ("Mark attendance", "Attendance", "Create and correct attendance records."),
            [FinanceManage] = ("Manage finance", "Finance", "Manage fees, payments and finance workflows."),
            [TransportManage] = ("Manage transport", "Transport", "Manage transport operations."),
            [LibraryManage] = ("Manage library", "Library", "Manage library operations."),
            [HostelManage] = ("Manage hostel", "Hostel", "Manage hostel operations."),
            [CommunicationsManage] = ("Manage communications", "Communications", "Manage announcements and outbound communications."),
            [UsersManage] = ("Manage users", "Users", "Create users and manage role assignments."),
            [AIQuestionPapersManage] = ("Manage AI question papers", "AI", "Generate and govern AI-assisted question papers.")
        };
}
