using EduSphere.Web.Authorization;
using EduSphere.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages;

/// <summary>
/// Sets (or clears) the current tenant for a browser session via a cookie the
/// tenant-resolution middleware reads. SuperAdmin only — a per-user tenant scope
/// for other roles is a follow-up.
/// </summary>
[Authorize(Policy = AuthorizationPolicies.SuperAdmin)]
public class SwitchTenantModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Index");

    public IActionResult OnPost(string? identifier, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            Response.Cookies.Delete(TenantResolutionMiddleware.TenantCookie);
        }
        else
        {
            Response.Cookies.Append(TenantResolutionMiddleware.TenantCookie, identifier.Trim(), new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
        }

        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Content("~/"));
    }
}
