using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Users;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public class BranchAdminsModel : PageModel
{
    private const string DefaultPassword = "BranchAdmin123!";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBranchService _branches;
    private readonly ITenantContext _tenant;

    public BranchAdminsModel(
        UserManager<ApplicationUser> userManager,
        IBranchService branches,
        ITenantContext tenant)
    {
        _userManager = userManager;
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
        [StringLength(100), DataType(DataType.Password)] public string Password { get; set; } = DefaultPassword;
    }

    public sealed record BranchAdminRow(
        Guid Id,
        string Name,
        string Email,
        string BranchName,
        string? BranchCode,
        bool IsActive);

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

        var normalizedEmail = Input.Email.Trim().ToUpperInvariant();
        if (await _userManager.FindByEmailAsync(Input.Email.Trim()) is not null)
            ModelState.AddModelError("Input.Email", "A user with this email already exists.");

        if (string.IsNullOrWhiteSpace(Input.Password))
            Input.Password = DefaultPassword;

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email.Trim(),
            NormalizedUserName = normalizedEmail,
            Email = Input.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            EmailConfirmed = true,
            TenantId = tenantId,
            BranchId = branchId,
            FirstName = Input.FirstName.Trim(),
            LastName = Input.LastName.Trim(),
            PhoneNumber = Input.PhoneNumber,
            UserType = UserType.BranchAdmin,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, Input.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            await LoadAsync();
            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, Roles.BranchAdmin);
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            await LoadAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Branch admin {user.Email} created.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(Guid id)
    {
        if (_tenant.TenantId is not Guid tenantId)
            return RedirectToPage();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is not null &&
            user.TenantId == tenantId &&
            await _userManager.IsInRoleAsync(user, Roles.BranchAdmin))
        {
            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
        }

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

        var branchMap = Branches.ToDictionary(b => b.Id);
        var branchAdmins = await _userManager.GetUsersInRoleAsync(Roles.BranchAdmin);
        Items = branchAdmins
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => branchMap.TryGetValue(u.BranchId ?? Guid.Empty, out var branch) ? branch.Name : string.Empty)
            .ThenBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u =>
            {
                branchMap.TryGetValue(u.BranchId ?? Guid.Empty, out var branch);
                return new BranchAdminRow(
                    u.Id,
                    u.FullName,
                    u.Email ?? u.UserName ?? "-",
                    branch?.Name ?? "Unassigned",
                    branch?.Code,
                    u.IsActive);
            })
            .ToList();
    }
}
