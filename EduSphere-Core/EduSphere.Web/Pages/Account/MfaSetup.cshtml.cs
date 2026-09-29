using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Account;

[Authorize]
public sealed class MfaSetupModel(UserManager<ApplicationUser> users, TenantDbContext db, UrlEncoder encoder) : PageModel
{
    [BindProperty, Required, StringLength(7,MinimumLength=6)] public string Code {get;set;}="";
    public string SharedKey {get;private set;}=""; public string AuthenticatorUri {get;private set;}=""; public IReadOnlyList<string> RecoveryCodes {get;private set;}=[]; public bool IsEnabled {get;private set;}
    public async Task OnGetAsync()=>await LoadAsync();
    public async Task<IActionResult> OnPostEnableAsync(){var user=await users.GetUserAsync(User);if(user is null)return Challenge();var valid=await users.VerifyTwoFactorTokenAsync(user,users.Options.Tokens.AuthenticatorTokenProvider,Code.Replace(" ","").Replace("-",""));if(!valid){ModelState.AddModelError(nameof(Code),"The verification code is invalid.");await LoadAsync();return Page();}await users.SetTwoFactorEnabledAsync(user,true);RecoveryCodes=(await users.GenerateNewTwoFactorRecoveryCodesAsync(user,10))?.ToList()??[];db.SecurityEvents.Add(new(){TenantId=user.TenantId,UserId=user.Id,Email=user.Email,EventType=SecurityEventType.MfaEnabled,Succeeded=true,OccurredOn=DateTime.UtcNow});await db.SaveChangesAsync();await LoadAsync(false);return Page();}
    public async Task<IActionResult> OnPostDisableAsync(){var user=await users.GetUserAsync(User);if(user is null)return Challenge();await users.SetTwoFactorEnabledAsync(user,false);await users.ResetAuthenticatorKeyAsync(user);db.SecurityEvents.Add(new(){TenantId=user.TenantId,UserId=user.Id,Email=user.Email,EventType=SecurityEventType.MfaDisabled,Succeeded=true});await db.SaveChangesAsync();return RedirectToPage();}
    private async Task LoadAsync(bool ensureKey=true){var user=await users.GetUserAsync(User);if(user is null)return;IsEnabled=await users.GetTwoFactorEnabledAsync(user);var key=await users.GetAuthenticatorKeyAsync(user);if(string.IsNullOrEmpty(key)&&ensureKey){await users.ResetAuthenticatorKeyAsync(user);key=await users.GetAuthenticatorKeyAsync(user);}SharedKey=FormatKey(key??"");AuthenticatorUri=$"otpauth://totp/{encoder.Encode("EduSphere:"+(user.Email??user.UserName))}?secret={key}&issuer={encoder.Encode("EduSphere")}&digits=6";}
    private static string FormatKey(string key)=>string.Join(" ",Enumerable.Range(0,(key.Length+3)/4).Select(i=>key.Substring(i*4,Math.Min(4,key.Length-i*4))).Select(x=>x.ToLowerInvariant()));
}
