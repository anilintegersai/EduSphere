using System.Security.Cryptography;
using System.Text;
using Asp.Versioning;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0"), Route("api/v{version:apiVersion}/email-events")]
public sealed class EmailEventsController(TenantDbContext db, IConfiguration configuration) : ControllerBase
{
    public sealed record BounceRequest(Guid TenantId, Guid? NotificationMessageId, string Recipient, string BounceType, string? ProviderEventId, string? Reason);

    [HttpPost("bounce"), AllowAnonymous]
    public async Task<IActionResult> Bounce([FromBody] BounceRequest request, CancellationToken cancellationToken)
    {
        var expected = configuration["Notifications:BounceWebhookSecret"];
        var supplied = Request.Headers["X-EduSphere-Webhook-Secret"].ToString();
        if (string.IsNullOrWhiteSpace(expected) || !FixedEquals(expected, supplied)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Recipient) || string.IsNullOrWhiteSpace(request.BounceType)) return BadRequest();
        db.EmailBounceEvents.Add(new EmailBounceEvent { TenantId = request.TenantId, NotificationMessageId = request.NotificationMessageId, Recipient = request.Recipient, BounceType = request.BounceType, ProviderEventId = request.ProviderEventId, Reason = request.Reason, OccurredOn = DateTime.UtcNow });
        if (request.NotificationMessageId.HasValue)
        {
            var job = await db.AccountEmailJobs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.TenantId == request.TenantId && x.NotificationMessageId == request.NotificationMessageId, cancellationToken);
            if (job is not null) { job.Status = EmailJobStatus.Bounced; job.LastError = request.Reason ?? request.BounceType; job.CompletedOn = DateTime.UtcNow; }
        }
        await db.SaveChangesAsync(cancellationToken); return Accepted();
    }
    private static bool FixedEquals(string left,string right){var a=Encoding.UTF8.GetBytes(left);var b=Encoding.UTF8.GetBytes(right);return a.Length==b.Length&&CryptographicOperations.FixedTimeEquals(a,b);}
}
