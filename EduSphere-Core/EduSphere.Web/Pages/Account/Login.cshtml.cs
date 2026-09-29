using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly TenantDbContext _db;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger,
        TenantDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = SafeReturnUrl(returnUrl);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = SafeReturnUrl(returnUrl);
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email.Trim());
        if (user is null || !user.IsActive)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        if (user.RequiresActivation)
        {
            ErrorMessage = "Please activate your account from the email invitation before signing in.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await RecordSecurityEventAsync(user, SecurityEventType.LoginSucceeded, true);
            _logger.LogInformation("User {Email} signed in.", Input.Email);
            return LocalRedirect(returnUrl);
        }

        if (result.RequiresTwoFactor)
            return RedirectToPage("/Account/MfaChallenge", new { returnUrl });

        if (result.IsLockedOut)
        {
            await RecordSecurityEventAsync(user, SecurityEventType.AccountLocked, false);
            _logger.LogWarning("User {Email} is locked out.", Input.Email);
            ErrorMessage = "This account is locked. Please try again later.";
            return Page();
        }

        await RecordSecurityEventAsync(user, SecurityEventType.LoginFailed, false);
        ErrorMessage = "Invalid email or password.";
        return Page();
    }

    private async Task RecordSecurityEventAsync(ApplicationUser user, SecurityEventType type, bool succeeded)
    {
        _db.SecurityEvents.Add(new SecurityEvent { TenantId = user.TenantId, UserId = user.Id, Email = user.Email, EventType = type, Succeeded = succeeded, IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), UserAgent = Request.Headers.UserAgent.ToString(), OccurredOn = DateTime.UtcNow });
        await _db.SaveChangesAsync();
    }

    // Guards against the empty hidden-field value and open-redirect attempts.
    private string SafeReturnUrl(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Content("~/");
}
