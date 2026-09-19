using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IBackgroundJobMonitor
{
    Task<BackgroundJobLog> EnqueueAsync(string jobType, string jobKey, string payloadJson, Guid? branchId = null, int maxAttempts = 3, DateTime? scheduledOn = null, CancellationToken cancellationToken = default);
    Task MarkRunningAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task MarkSucceededAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(Guid jobId, Exception exception, DateTime? nextRetryOn = null, CancellationToken cancellationToken = default);
}
