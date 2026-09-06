using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Infrastructure.Notifications;

public sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly TenantDbContext _dbContext;
    private readonly IReadOnlyDictionary<CommunicationChannel, INotificationChannelSender> _senders;

    public NotificationDispatcher(TenantDbContext dbContext, IEnumerable<INotificationChannelSender> senders)
    {
        _dbContext = dbContext;
        _senders = senders
            .GroupBy(sender => sender.Channel)
            .ToDictionary(group => group.Key, group => group.First());
    }

    public async Task<NotificationDispatchResult> DispatchAsync(Guid notificationMessageId, CancellationToken cancellationToken = default)
    {
        var message = await _dbContext.NotificationMessages
            .FirstOrDefaultAsync(m => m.Id == notificationMessageId, cancellationToken);
        if (message is null)
            return NotificationDispatchResult.Failure("Notification message was not found.");

        if (!_senders.TryGetValue(message.Channel, out var sender))
            return NotificationDispatchResult.Failure($"No sender is registered for channel '{message.Channel}'.");

        var recipients = await _dbContext.NotificationRecipients
            .Where(r => r.NotificationMessageId == notificationMessageId &&
                        r.Status != NotificationStatus.Sent &&
                        r.Status != NotificationStatus.Cancelled)
            .OrderBy(r => r.DisplayName)
            .ToListAsync(cancellationToken);

        if (recipients.Count == 0)
            return NotificationDispatchResult.Failure("Notification message has no pending recipients.");

        message.Status = NotificationStatus.Queued;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var failures = new List<string>();
        foreach (var recipient in recipients)
        {
            var dispatchMessage = new NotificationDispatchMessage(
                message.Id,
                recipient.Id,
                message.Channel,
                recipient.DestinationAddress,
                recipient.DisplayName,
                message.Subject,
                message.Body,
                message.ProviderKey);

            NotificationDispatchResult result;
            try
            {
                result = await sender.SendAsync(dispatchMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                result = NotificationDispatchResult.Failure(ex.Message);
            }

            var now = DateTime.UtcNow;
            recipient.Status = result.Succeeded ? NotificationStatus.Sent : NotificationStatus.Failed;
            recipient.SentOn = result.Succeeded ? now : null;
            recipient.ErrorMessage = result.ErrorMessage;

            _dbContext.CommunicationLogs.Add(new CommunicationLog
            {
                BranchId = recipient.BranchId ?? message.BranchId,
                Channel = message.Channel,
                Direction = CommunicationDirection.Outbound,
                Recipient = recipient.DestinationAddress,
                Subject = message.Subject,
                Status = recipient.Status,
                ProviderKey = message.ProviderKey,
                ProviderMessageId = result.ProviderMessageId,
                PayloadSummary = message.Body.Length > 500 ? message.Body[..500] : message.Body,
                ErrorMessage = result.ErrorMessage,
                OccurredOn = now
            });

            if (!result.Succeeded && !string.IsNullOrWhiteSpace(result.ErrorMessage))
                failures.Add($"{recipient.DestinationAddress}: {result.ErrorMessage}");
        }

        message.SentOn = failures.Count == 0 ? DateTime.UtcNow : message.SentOn;
        message.Status = failures.Count == recipients.Count
            ? NotificationStatus.Failed
            : failures.Count > 0
                ? NotificationStatus.Queued
                : NotificationStatus.Sent;
        message.ErrorMessage = failures.Count > 0 ? string.Join("; ", failures.Take(3)) : null;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return failures.Count == 0
            ? NotificationDispatchResult.Success()
            : NotificationDispatchResult.Failure(message.ErrorMessage ?? "One or more recipients failed.");
    }
}
