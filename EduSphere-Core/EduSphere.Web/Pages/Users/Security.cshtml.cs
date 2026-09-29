using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Users;

[Authorize(Policy = AuthorizationPolicies.UserManager)]
public sealed class SecurityModel(TenantDbContext db, UserManager<ApplicationUser> users, IUserManagementService userManagement) : PageModel
{
    public IReadOnlyList<ApplicationUser> LockedUsers {get;private set;}=[]; public IReadOnlyList<SecurityEvent> Events {get;private set;}=[]; public IReadOnlyList<AccountEmailJob> EmailJobs {get;private set;}=[]; public IReadOnlyList<EmailBounceEvent> Bounces {get;private set;}=[]; public IReadOnlyList<UserImportBatch> Imports {get;private set;}=[];
    [TempData] public string? Feedback {get;set;}
    public async Task OnGetAsync()=>await LoadAsync();
    public async Task<IActionResult> OnPostUnlockAsync(Guid id){var user=await users.FindByIdAsync(id.ToString());if(user is not null){await users.SetLockoutEndDateAsync(user,null);await users.ResetAccessFailedCountAsync(user);db.SecurityEvents.Add(new(){TenantId=user.TenantId,UserId=user.Id,Email=user.Email,EventType=SecurityEventType.AccountUnlocked,Succeeded=true,OccurredOn=DateTime.UtcNow});await db.SaveChangesAsync();Feedback="Account unlocked.";}return RedirectToPage();}
    public async Task<IActionResult> OnPostDisableMfaAsync(Guid id){var user=await users.FindByIdAsync(id.ToString());if(user is not null){await users.SetTwoFactorEnabledAsync(user,false);await users.ResetAuthenticatorKeyAsync(user);Feedback="MFA reset for the user.";}return RedirectToPage();}
    public async Task<IActionResult> OnPostRetryEmailAsync(Guid id){var job=await db.AccountEmailJobs.SingleAsync(x=>x.Id==id);job.Status=EmailJobStatus.Queued;job.AvailableOn=DateTime.UtcNow;job.CompletedOn=null;job.LastError=null;Feedback="Email queued for retry.";await db.SaveChangesAsync();return RedirectToPage();}
    public async Task<IActionResult> OnPostResumeImportAsync(Guid id,CancellationToken cancellationToken){var batch=await db.UserImportBatches.Include(x=>x.Rows).SingleAsync(x=>x.Id==id,cancellationToken);batch.Status=UserImportBatchStatus.Processing;foreach(var row in batch.Rows.Where(x=>x.IsValid&&!x.IsProcessed).OrderBy(x=>x.RowNumber)){var request=JsonSerializer.Deserialize<CreateManagedUserRequest>(row.PayloadJson);if(request is null){row.IsProcessed=true;row.Error="Stored row payload is invalid.";continue;}var result=await userManagement.CreateUserAsync(User,request,cancellationToken);row.IsProcessed=true;row.Succeeded=result.Succeeded;row.CreatedUserId=result.Data?.Id;row.Error=result.Succeeded?null:string.Join(" ",result.Errors);batch.ProcessedRows++;if(result.Succeeded)batch.SucceededRows++;else batch.FailedRows++;await db.SaveChangesAsync(cancellationToken);}batch.Status=batch.Rows.Any(x=>!x.IsProcessed)?UserImportBatchStatus.Queued:batch.Rows.Any(x=>!x.Succeeded)?UserImportBatchStatus.CompletedWithErrors:UserImportBatchStatus.Completed;await db.SaveChangesAsync(cancellationToken);Feedback="Import resume completed.";return RedirectToPage();}
    public async Task<FileContentResult> OnGetImportErrorsAsync(Guid id){var batch=await db.UserImportBatches.Include(x=>x.Rows).SingleAsync(x=>x.Id==id);var csv=new StringBuilder("Row,Email,Error\r\n");foreach(var row in batch.Rows.Where(x=>!x.Succeeded)){var email="";try{email=JsonSerializer.Deserialize<CreateManagedUserRequest>(row.PayloadJson)?.Email??"";}catch{}csv.AppendLine($"{row.RowNumber},\"{email.Replace("\"","\"\"")}\",\"{(row.Error??"").Replace("\"","\"\"")}\"");}return File(Encoding.UTF8.GetBytes(csv.ToString()),"text/csv",$"user-import-{id}-errors.csv");}
    private async Task LoadAsync(){LockedUsers=await users.Users.Where(x=>x.LockoutEnd>DateTimeOffset.UtcNow).OrderBy(x=>x.LockoutEnd).ToListAsync();Events=await db.SecurityEvents.OrderByDescending(x=>x.OccurredOn).Take(200).ToListAsync();EmailJobs=await db.AccountEmailJobs.OrderByDescending(x=>x.CreatedOn).Take(100).ToListAsync();Bounces=await db.EmailBounceEvents.OrderByDescending(x=>x.OccurredOn).Take(100).ToListAsync();Imports=await db.UserImportBatches.OrderByDescending(x=>x.CreatedOn).Take(50).ToListAsync();}
}
