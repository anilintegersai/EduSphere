using System.ComponentModel.DataAnnotations;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Users;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public class BranchAdminsModel : PageModel
{
    private readonly IUserManagementService _users;
    private readonly IBranchService _branches;
    private readonly ITenantContext _tenant;

    public BranchAdminsModel(
        IUserManagementService users,
        IBranchService branches,
        ITenantContext tenant)
    {
        _users = users;
        _branches = branches;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<BranchAdminRow> Items { get; private set; } = new List<BranchAdminRow>();
    public string? Notice { get; private set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Required, StringLength(100), Display(Name = "First name")] public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100), Display(Name = "Last name")] public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = string.Empty;
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
    }

    public sealed record BranchAdminRow(
        Guid Id,
        string Name,
        string Email,
        string BranchName,
        string? BranchCode,
        bool IsActive,
        bool RequiresActivation);

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (_tenant.TenantId is not Guid tenantId)
        {
            Notice = "Select a tenant before creating branch administrators.";
            await LoadAsync();
            return Page();
        }

        var branchId = Input.BranchId ?? Guid.Empty;
        var branches = (await _branches.GetBranchesByTenantAsync(tenantId)).ToList();
        if (branchId == Guid.Empty || branches.All(b => b.Id != branchId))
            ModelState.AddModelError("Input.BranchId", "Selected branch was not found in this tenant.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _users.CreateUserAsync(User, new CreateManagedUserRequest
        {
            RoleName = Roles.BranchAdmin,
            TenantId = tenantId,
            BranchId = branchId,
            FirstName = Input.FirstName.Trim(),
            LastName = Input.LastName.Trim(),
            PhoneNumber = Input.PhoneNumber,
            Designation = "Branch Administrator",
            SendActivationEmail = true
        }, HttpContext.RequestAborted);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            await LoadAsync();
            return Page();
        }

        TempData["StatusMessage"] = result.Warnings.Count == 0
            ? result.Message
            : $"{result.Message} {string.Join(" ", result.Warnings)}";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id, bool isActive)
    {
        await _users.SetUserActiveAsync(User, id, isActive, HttpContext.RequestAborted);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostResendActivationAsync(Guid id)
    {
        await _users.ResendActivationAsync(User, id, HttpContext.RequestAborted);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        if (_tenant.TenantId is not Guid tenantId)
        {
            Notice = "Select a tenant from the switcher before managing branch administrators.";
            Branches = new List<Branch>();
            Items = new List<BranchAdminRow>();
            return;
        }

        Branches = (await _branches.GetBranchesByTenantAsync(tenantId)).OrderBy(b => b.Name).ToList();
        if (Input.BranchId is null && Branches.Count == 1)
            Input.BranchId = Branches[0].Id;

        var users = await _users.ListUsersAsync(
            User,
            new UserManagementQuery { TenantId = tenantId, RoleName = Roles.BranchAdmin },
            HttpContext.RequestAborted);

        Items = users
            .OrderBy(u => u.BranchName)
            .ThenBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u =>
                new BranchAdminRow(
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.BranchName ?? "Unassigned",
                    Branches.FirstOrDefault(b => b.Id == u.BranchId)?.Code,
                    u.IsActive,
                    u.RequiresActivation))
            .ToList();
    }
}
