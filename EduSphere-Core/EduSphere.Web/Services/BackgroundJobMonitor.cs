using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class BackgroundJobMonitor(TenantDbContext db, ITenantContext tenant) : IBackgroundJobMonitor
{
    public async Task<BackgroundJobLog> EnqueueAsync(string jobType, string jobKey, string payloadJson, Guid? branchId = null, int maxAttempts = 3, DateTime? scheduledOn = null, CancellationToken cancellationToken = default)
    {
        var entity = new BackgroundJobLog { TenantId = tenant.TenantId ?? throw new InvalidOperationException("A tenant is required."), BranchId = branchId, JobType = jobType, JobKey = jobKey, PayloadJson = payloadJson, MaxAttempts = maxAttempts, ScheduledOn = scheduledOn ?? DateTime.UtcNow, Status = BackgroundJobStatus.Queued, CorrelationId = Guid.NewGuid().ToString("N") };
        db.BackgroundJobLogs.Add(entity); await db.SaveChangesAsync(cancellationToken); return entity;
    }
    public async Task MarkRunningAsync(Guid jobId, CancellationToken cancellationToken = default) { var job = await Find(jobId, cancellationToken); job.Status = BackgroundJobStatus.Running; job.StartedOn = DateTime.UtcNow; job.AttemptCount++; await db.SaveChangesAsync(cancellationToken); }
    public async Task MarkSucceededAsync(Guid jobId, CancellationToken cancellationToken = default) { var job = await Find(jobId, cancellationToken); job.Status = BackgroundJobStatus.Succeeded; job.CompletedOn = DateTime.UtcNow; job.LastError = null; await db.SaveChangesAsync(cancellationToken); }
    public async Task MarkFailedAsync(Guid jobId, Exception exception, DateTime? nextRetryOn = null, CancellationToken cancellationToken = default) { var job = await Find(jobId, cancellationToken); job.Status = nextRetryOn.HasValue && job.AttemptCount < job.MaxAttempts ? BackgroundJobStatus.RetryScheduled : BackgroundJobStatus.Failed; job.NextRetryOn = nextRetryOn; job.CompletedOn = DateTime.UtcNow; job.LastError = exception.Message.Length <= 2000 ? exception.Message : exception.Message[..2000]; await db.SaveChangesAsync(cancellationToken); }
    private Task<BackgroundJobLog> Find(Guid id, CancellationToken cancellationToken) => db.BackgroundJobLogs.SingleAsync(j => j.Id == id, cancellationToken);
}
