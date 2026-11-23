using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Notifications;
[Authorize]
public sealed class IndexModel(TenantDbContext db):PageModel
{
 public IReadOnlyList<InAppNotification> Items{get;private set;}=[]; public IReadOnlyList<UserCommunicationPreference> Preferences{get;private set;}=[]; public IReadOnlyList<CommunicationConsent> Consents{get;private set;}=[];
 [BindProperty]public PreferenceInput Preference{get;set;}=new(); [BindProperty]public ConsentInput Consent{get;set;}=new(); [BindProperty]public PushInput Push{get;set;}=new();
 public sealed class PreferenceInput{public CommunicationChannel Channel{get;set;}public bool IsEnabled{get;set;}=true;public CommunicationPreferenceFrequency Frequency{get;set;}=CommunicationPreferenceFrequency.Immediate;[StringLength(10)]public string Locale{get;set;}="en";}
 public sealed class ConsentInput{public CommunicationChannel Channel{get;set;}[Required,StringLength(80)]public string Purpose{get;set;}="General";public bool Granted{get;set;}=true;}
 public sealed class PushInput{[Required,StringLength(500)]public string Endpoint{get;set;}="";[Required,StringLength(500)]public string DeviceToken{get;set;}="";[StringLength(80)]public string Platform{get;set;}="Web";[StringLength(200)]public string? DeviceName{get;set;}}
 public async Task OnGetAsync()=>await Load();
 public async Task<IActionResult> OnPostReadAsync(Guid id){var uid=UserId();var item=await db.InAppNotifications.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==uid);if(item is not null)item.ReadOn=DateTime.UtcNow;await db.SaveChangesAsync();return RedirectToPage();}
 public async Task<IActionResult> OnPostReadAllAsync(){var uid=UserId();await db.InAppNotifications.Where(x=>x.UserId==uid&&x.ReadOn==null).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.ReadOn,DateTime.UtcNow));return RedirectToPage();}
 public async Task<IActionResult> OnPostPreferenceAsync(){var uid=UserId();var item=await db.UserCommunicationPreferences.FirstOrDefaultAsync(x=>x.UserId==uid&&x.Channel==Preference.Channel);if(item is null)db.UserCommunicationPreferences.Add(new(){UserId=uid,Channel=Preference.Channel,IsEnabled=Preference.IsEnabled,Frequency=Preference.Frequency,Locale=Preference.Locale});else{item.IsEnabled=Preference.IsEnabled;item.Frequency=Preference.Frequency;item.Locale=Preference.Locale;item.UpdatedOn=DateTime.UtcNow;}await db.SaveChangesAsync();return RedirectToPage();}
 public async Task<IActionResult> OnPostConsentAsync(){var uid=UserId();db.CommunicationConsents.Add(new(){UserId=uid,Channel=Consent.Channel,Purpose=Consent.Purpose,Status=Consent.Granted?CommunicationConsentStatus.Granted:CommunicationConsentStatus.Withdrawn,WithdrawnOn=Consent.Granted?null:DateTime.UtcNow,EvidenceHash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{uid}|{Consent.Channel}|{Consent.Purpose}|{DateTime.UtcNow:O}")))});await db.SaveChangesAsync();return RedirectToPage();}
 public async Task<IActionResult> OnPostPushAsync(){if(!ModelState.IsValid){await Load();return Page();}var uid=UserId();var existing=await db.PushSubscriptions.FirstOrDefaultAsync(x=>x.UserId==uid&&x.Endpoint==Push.Endpoint);if(existing is null)db.PushSubscriptions.Add(new(){UserId=uid,Endpoint=Push.Endpoint,DeviceToken=Push.DeviceToken,Platform=Push.Platform,DeviceName=Push.DeviceName});else{existing.DeviceToken=Push.DeviceToken;existing.IsActive=true;existing.LastSeenOn=DateTime.UtcNow;}await db.SaveChangesAsync();return RedirectToPage();}
 public async Task<IActionResult> OnPostUnsubscribeAsync(CommunicationChannel channel){var uid=UserId();var user=await db.Users.FindAsync(uid);var destination=channel==CommunicationChannel.Email?user?.Email:channel is CommunicationChannel.Sms or CommunicationChannel.WhatsApp?user?.PhoneNumber:uid.ToString();if(string.IsNullOrWhiteSpace(destination))return RedirectToPage();var token=Convert.ToHexString(RandomNumberGenerator.GetBytes(24));db.CommunicationUnsubscribes.Add(new(){UserId=uid,DestinationAddress=destination,Channel=channel,Purpose="Marketing",TokenHash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)))});await db.SaveChangesAsync();return RedirectToPage();}
 private async Task Load(){var uid=UserId();Items=await db.InAppNotifications.Where(x=>x.UserId==uid&&!x.IsDismissed).OrderByDescending(x=>x.CreatedAt).Take(100).ToListAsync();Preferences=await db.UserCommunicationPreferences.Where(x=>x.UserId==uid).OrderBy(x=>x.Channel).ToListAsync();Consents=await db.CommunicationConsents.Where(x=>x.UserId==uid).OrderByDescending(x=>x.CapturedOn).Take(30).ToListAsync();}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
