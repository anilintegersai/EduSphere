using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class FinanceAutomationWorker(IServiceScopeFactory scopes,ILogger<FinanceAutomationWorker> logger):BackgroundService
{
 protected override async Task ExecuteAsync(CancellationToken token){while(!token.IsCancellationRequested){try{await Run(token);}catch(Exception ex){logger.LogError(ex,"Finance automation cycle failed.");}await Task.Delay(TimeSpan.FromHours(1),token);}}
 private async Task Run(CancellationToken token){using var discovery=scopes.CreateScope();var root=discovery.ServiceProvider.GetRequiredService<TenantDbContext>();var tenants=await root.FeeInvoices.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&(x.Status==InvoiceStatus.Issued||x.Status==InvoiceStatus.PartiallyPaid||x.Status==InvoiceStatus.Overdue)).Select(x=>x.TenantId).Distinct().ToListAsync(token);foreach(var tenantId in tenants){using var scope=scopes.CreateScope();var tenant=scope.ServiceProvider.GetRequiredService<TenantContext>();tenant.SetTenant(tenantId,tenantId.ToString());var db=scope.ServiceProvider.GetRequiredService<TenantDbContext>();var finance=scope.ServiceProvider.GetRequiredService<IFinanceAccountingService>();await finance.ApplyLateFeesAsync(DateOnly.FromDateTime(DateTime.UtcNow),token);var due=await db.FeeReminders.Include(x=>x.FeeInvoice).Where(x=>x.Status==FeeReminderStatus.Scheduled&&x.ReminderOn<=DateTime.UtcNow).Take(100).ToListAsync(token);foreach(var reminder in due){db.NotificationMessages.Add(new NotificationMessage{BranchId=reminder.BranchId,Channel=reminder.Channel,Subject=$"Fee reminder - {reminder.FeeInvoice?.InvoiceNumber}",Body=$"A fee balance of {(reminder.FeeInvoice!.TotalAmount-reminder.FeeInvoice.PaidAmount):0.00} is due on {reminder.FeeInvoice.DueDate:dd MMM yyyy}.",Status=NotificationStatus.Queued,ScheduledOn=DateTime.UtcNow,Recipients=[new NotificationRecipient{BranchId=reminder.BranchId,DisplayName=reminder.Recipient,DestinationAddress=reminder.Recipient,Status=NotificationStatus.Queued}]});reminder.Status=FeeReminderStatus.Queued;reminder.AttemptCount++;}await db.SaveChangesAsync(token);}}
}
