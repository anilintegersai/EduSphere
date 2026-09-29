using EduSphere.Application.Interfaces;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AccountEmailBackgroundWorker(IServiceScopeFactory scopeFactory, ILogger<AccountEmailBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        do
        {
            try { await ProcessBatchAsync(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { logger.LogError(ex, "Account email background processing failed."); }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var jobs = await db.AccountEmailJobs.IgnoreQueryFilters().Where(x =>
                (x.Status == EmailJobStatus.Queued || x.Status == EmailJobStatus.RetryScheduled) && x.AvailableOn <= DateTime.UtcNow)
            .OrderBy(x => x.AvailableOn).Take(10).ToListAsync(cancellationToken);
        foreach (var job in jobs)
        {
            var tenant = scope.ServiceProvider.GetRequiredService<TenantContext>();
            tenant.SetTenant(job.TenantId, job.TenantId.ToString());
            job.Status = EmailJobStatus.Processing; job.AttemptCount++; await db.SaveChangesAsync(cancellationToken);
            try
            {
                var result = await scope.ServiceProvider.GetRequiredService<INotificationDispatcher>().DispatchAsync(job.NotificationMessageId, cancellationToken);
                if (result.Succeeded) { job.Status = EmailJobStatus.Succeeded; job.CompletedOn = DateTime.UtcNow; job.LastError = null; }
                else ScheduleFailure(job, result.ErrorMessage ?? "Provider rejected the message.");
            }
            catch (Exception ex) { ScheduleFailure(job, ex.Message); }
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static void ScheduleFailure(Domain.Entities.AccountEmailJob job, string error)
    {
        job.LastError = error;
        if (job.AttemptCount >= job.MaximumAttempts) { job.Status = EmailJobStatus.Failed; job.CompletedOn = DateTime.UtcNow; }
        else { job.Status = EmailJobStatus.RetryScheduled; job.AvailableOn = DateTime.UtcNow.AddMinutes(Math.Pow(2, job.AttemptCount)); }
    }
}
