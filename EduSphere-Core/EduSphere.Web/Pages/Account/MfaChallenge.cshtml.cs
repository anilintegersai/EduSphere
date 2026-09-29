using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Account;

public sealed class MfaChallengeModel(SignInManager<ApplicationUser> signIn, TenantDbContext db) : PageModel
{
    [BindProperty,Required] public string Code {get;set;}=""; [BindProperty] public bool RememberMachine {get;set;} [BindProperty] public bool UseRecoveryCode {get;set;} public string ReturnUrl {get;set;}="/";
    public void OnGet(string? returnUrl)=>ReturnUrl=Url.IsLocalUrl(returnUrl)?returnUrl!:"/";
    public async Task<IActionResult> OnPostAsync(string? returnUrl){ReturnUrl=Url.IsLocalUrl(returnUrl)?returnUrl!:"/";var user=await signIn.GetTwoFactorAuthenticationUserAsync();if(user is null)return RedirectToPage("/Account/Login");var code=Code.Replace(" ","");var result=UseRecoveryCode?await signIn.TwoFactorRecoveryCodeSignInAsync(code):await signIn.TwoFactorAuthenticatorSignInAsync(code.Replace("-",""),false,RememberMachine);db.SecurityEvents.Add(new(){TenantId=user.TenantId,UserId=user.Id,Email=user.Email,EventType=result.Succeeded?SecurityEventType.LoginSucceeded:SecurityEventType.MfaFailed,Succeeded=result.Succeeded,IpAddress=HttpContext.Connection.RemoteIpAddress?.ToString()});await db.SaveChangesAsync();if(result.Succeeded)return LocalRedirect(ReturnUrl);if(result.IsLockedOut)return RedirectToPage("/Account/Login");ModelState.AddModelError("",UseRecoveryCode?"Invalid or already-used recovery code.":"Invalid authenticator code.");return Page();}
}
