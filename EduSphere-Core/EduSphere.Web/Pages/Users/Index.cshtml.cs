using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Users;

[Authorize(Policy = AuthorizationPolicies.UserManager)]
public class IndexModel : PageModel
{
    private readonly IUserManagementService _users;
    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;

    public IndexModel(
        IUserManagementService users,
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess)
    {
        _users = users;
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
    }

    [BindProperty(SupportsGet = true)]
    public UserManagementQuery Query { get; set; } = new();

    [BindProperty]
    public CreateManagedUserRequest CreateInput { get; set; } = new();

    [BindProperty]
    public BulkUserImportRequest BulkInput { get; set; } = new();

    public IReadOnlyList<UserSummaryDto> Items { get; private set; } = new List<UserSummaryDto>();
    public IReadOnlyList<RoleOptionDto> AssignableRoles { get; private set; } = new List<RoleOptionDto>();
    public IReadOnlyList<TenantOption> Tenants { get; private set; } = new List<TenantOption>();
    public IReadOnlyList<BranchOption> Branches { get; private set; } = new List<BranchOption>();
    public IReadOnlyList<DepartmentOption> Departments { get; private set; } = new List<DepartmentOption>();
    public string? StatusMessage { get; private set; }
    public IReadOnlyList<string> Warnings { get; private set; } = Array.Empty<string>();
    public bool IsSuperAdmin => User.IsInRole(Roles.SuperAdmin);
    public bool HasTenant => IsSuperAdmin || _tenantContext.HasTenant;

    public sealed record TenantOption(Guid Id, string Name);
    public sealed record BranchOption(Guid Id, string Name, string? Code);
    public sealed record DepartmentOption(Guid Id, string Name, string Code);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        KeepOnlyModelStateFor(nameof(CreateInput));
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var result = await _users.CreateUserAsync(User, CreateInput, cancellationToken);
        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            await LoadAsync(cancellationToken);
            return Page();
        }

        StatusMessage = result.Message;
        Warnings = result.Warnings;
        CreateInput = new CreateManagedUserRequest
        {
            TenantId = CreateInput.TenantId,
            BranchId = CreateInput.BranchId,
            RoleName = CreateInput.RoleName
        };
        ModelState.Clear();
        await LoadAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostBulkImportAsync(CancellationToken cancellationToken)
    {
        KeepOnlyModelStateFor(nameof(BulkInput));
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var result = await _users.BulkImportUsersAsync(User, BulkInput, cancellationToken);
        if (!result.Succeeded)
            AddErrors(result.Errors);
        else
            StatusMessage = result.Message;

        Warnings = result.Warnings;
        await LoadAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        ModelState.Clear();
        var result = await _users.SetUserActiveAsync(User, id, isActive, cancellationToken);
        if (!result.Succeeded)
            AddErrors(result.Errors);
        else
            StatusMessage = result.Message;

        Warnings = result.Warnings;
        await LoadAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostResendActivationAsync(Guid id, CancellationToken cancellationToken)
    {
        ModelState.Clear();
        var result = await _users.ResendActivationAsync(User, id, cancellationToken);
        if (!result.Succeeded)
            AddErrors(result.Errors);
        else
            StatusMessage = result.Message;

        Warnings = result.Warnings;
        await LoadAsync(cancellationToken);
        return Page();
    }

    public string RoleText(UserSummaryDto user)
        => user.Roles.Count == 0 ? "-" : string.Join(", ", user.Roles);

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        AlignTenantScope();

        AssignableRoles = await _users.GetAssignableRolesAsync(User, cancellationToken);
        Items = await _users.ListUsersAsync(User, Query, cancellationToken);
        Tenants = IsSuperAdmin
            ? await _dbContext.Tenants
                .IgnoreQueryFilters()
                .Where(t => !t.IsDeleted && t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new TenantOption(t.Id, t.Name))
                .ToListAsync(cancellationToken)
            : new List<TenantOption>();

        var tenantId = ResolveSelectedTenantId();
        Branches = tenantId.HasValue
            ? (await _branchAccess.FilterBranchesAsync(
                    User,
                    await _dbContext.Branches
                        .IgnoreQueryFilters()
                        .Where(b => b.TenantId == tenantId.Value && !b.IsDeleted && b.IsActive)
                        .OrderBy(b => b.Name)
                        .ToListAsync(cancellationToken)))
                .Select(b => new BranchOption(b.Id, b.Name, b.Code))
                .ToList()
            : new List<BranchOption>();

        Departments = tenantId.HasValue
            ? await _dbContext.Departments
                .IgnoreQueryFilters()
                .Where(d => d.TenantId == tenantId.Value && !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentOption(d.Id, d.Name, d.Code))
                .ToListAsync(cancellationToken)
            : new List<DepartmentOption>();

        if (CreateInput.BranchId is null && Branches.Count == 1)
            CreateInput.BranchId = Branches[0].Id;

        if (BulkInput.BranchId is null && Branches.Count == 1)
            BulkInput.BranchId = Branches[0].Id;

        if (string.IsNullOrWhiteSpace(CreateInput.RoleName) && AssignableRoles.Count > 0 && !HttpMethods.IsPost(Request.Method))
            CreateInput.RoleName = AssignableRoles[0].RoleName;

        if (string.IsNullOrWhiteSpace(BulkInput.RoleName) && AssignableRoles.Count > 0 && !HttpMethods.IsPost(Request.Method))
            BulkInput.RoleName = AssignableRoles.Last().RoleName;
    }

    private void AlignTenantScope()
    {
        if (!IsSuperAdmin)
        {
            Query.TenantId = _tenantContext.TenantId;
            CreateInput.TenantId = _tenantContext.TenantId;
            BulkInput.TenantId = _tenantContext.TenantId;
            return;
        }

        CreateInput.TenantId ??= Query.TenantId ?? _tenantContext.TenantId;
        BulkInput.TenantId ??= Query.TenantId ?? _tenantContext.TenantId;
    }

    private Guid? ResolveSelectedTenantId()
        => IsSuperAdmin
            ? CreateInput.TenantId ?? BulkInput.TenantId ?? Query.TenantId ?? _tenantContext.TenantId
            : _tenantContext.TenantId;

    private void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);
    }

    private void KeepOnlyModelStateFor(string prefix)
    {
        var keepPrefix = prefix + ".";
        foreach (var key in ModelState.Keys.Where(k => !k.StartsWith(keepPrefix, StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);

        foreach (var entry in ModelState.Where(kvp => kvp.Key.StartsWith(keepPrefix, StringComparison.Ordinal)).ToList())
        {
            if (entry.Value is null)
                continue;

            foreach (var error in entry.Value.Errors.Where(e => e.Exception is null && string.IsNullOrWhiteSpace(e.ErrorMessage)).ToList())
                entry.Value.Errors.Remove(error);

            if (entry.Value.ValidationState == ModelValidationState.Skipped)
                entry.Value.ValidationState = ModelValidationState.Unvalidated;
        }
    }
}
