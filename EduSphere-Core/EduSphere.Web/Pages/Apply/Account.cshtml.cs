using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Apply;

[AllowAnonymous]
public sealed class AccountModel(TenantDbContext db, IPasswordHasher<AdmissionApplicantAccount> hasher) : PageModel
{
    public List<Tenant> Tenants { get; private set; } = [];
    [BindProperty] public InputModel Input { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }
    public async Task OnGetAsync() => Tenants = await db.Tenants.IgnoreQueryFilters().Where(x => !x.IsDeleted && x.IsActive).OrderBy(x => x.Name).ToListAsync();

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        await OnGetAsync();
        if (!ModelState.IsValid) return Page();
        var email = Input.Email.Trim().ToLowerInvariant();
        if (await db.AdmissionApplicantAccounts.IgnoreQueryFilters().AnyAsync(x => x.TenantId == Input.TenantId && x.Email == email)) { ModelState.AddModelError("Input.Email", "An applicant account already exists for this email."); return Page(); }
        var account = new AdmissionApplicantAccount { TenantId = Input.TenantId, Email = email, DisplayName = Input.DisplayName.Trim(), PhoneNumber = Input.PhoneNumber };
        account.PasswordHash = hasher.HashPassword(account, Input.Password);
        db.AdmissionApplicantAccounts.Add(account); await db.SaveChangesAsync();
        await SignInAsync(account); return RedirectToPage("Portal");
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        await OnGetAsync(); var email = Input.Email.Trim().ToLowerInvariant();
        var account = await db.AdmissionApplicantAccounts.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.TenantId == Input.TenantId && x.Email == email);
        if (account is null || account.Status == ApplicantAccountStatus.Disabled || (account.LockoutEnd.HasValue && account.LockoutEnd > DateTime.UtcNow) || hasher.VerifyHashedPassword(account, account.PasswordHash, Input.Password) == PasswordVerificationResult.Failed)
        {
            if (account is not null) { account.FailedLoginCount++; if (account.FailedLoginCount >= 5) { account.Status = ApplicantAccountStatus.Locked; account.LockoutEnd = DateTime.UtcNow.AddMinutes(15); account.FailedLoginCount = 0; } await db.SaveChangesAsync(); }
            ModelState.AddModelError(string.Empty, "Invalid credentials or the account is temporarily locked."); return Page();
        }
        account.FailedLoginCount = 0; account.Status = ApplicantAccountStatus.Active; account.LockoutEnd = null; account.LastLoginOn = DateTime.UtcNow; await db.SaveChangesAsync();
        await SignInAsync(account); return RedirectToPage("Portal");
    }

    public async Task<IActionResult> OnPostLogoutAsync() { await HttpContext.SignOutAsync("Applicant"); return RedirectToPage(); }
    private Task SignInAsync(AdmissionApplicantAccount account) => HttpContext.SignInAsync("Applicant", new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ApplicantClaimTypes.AccountId, account.Id.ToString()), new Claim(ApplicantClaimTypes.TenantId, account.TenantId.ToString()), new Claim(ClaimTypes.Name, account.DisplayName ?? account.Email), new Claim(ClaimTypes.Email, account.Email) }, "Applicant")), new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14) });
    public sealed class InputModel { [Required] public Guid TenantId { get; set; } [Required, EmailAddress] public string Email { get; set; } = ""; [Required, MinLength(8)] public string Password { get; set; } = ""; [Required, StringLength(150)] public string DisplayName { get; set; } = ""; [StringLength(30)] public string? PhoneNumber { get; set; } }
}
