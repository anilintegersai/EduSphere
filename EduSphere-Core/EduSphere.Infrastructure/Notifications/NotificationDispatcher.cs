using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            .ToDictionary(group => group.Key, group => group.Last());
    }

    public async Task<NotificationDispatchResult> DispatchAsync(Guid notificationMessageId, CancellationToken cancellationToken = default)
    {
        var message = await _dbContext.NotificationMessages
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == notificationMessageId, cancellationToken);
        if (message is null)
            return NotificationDispatchResult.Failure("Notification message was not found.");

        if (!_senders.TryGetValue(message.Channel, out var sender))
            return NotificationDispatchResult.Failure($"No sender is registered for channel '{message.Channel}'.");

        var recipients = await _dbContext.NotificationRecipients
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == message.TenantId &&
                        r.NotificationMessageId == notificationMessageId &&
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
            var preferenceBlocked = recipient.UserId.HasValue && await _dbContext.UserCommunicationPreferences
                .IgnoreQueryFilters().AnyAsync(x => x.TenantId == message.TenantId && x.UserId == recipient.UserId &&
                    x.Channel == message.Channel && (!x.IsEnabled || x.Frequency == CommunicationPreferenceFrequency.Never), cancellationToken);
            var consentWithdrawn = recipient.UserId.HasValue && await _dbContext.CommunicationConsents
                .IgnoreQueryFilters().AnyAsync(x => x.TenantId == message.TenantId && x.UserId == recipient.UserId &&
                    x.Channel == message.Channel && x.Status == CommunicationConsentStatus.Withdrawn, cancellationToken);
            var unsubscribed = await _dbContext.CommunicationUnsubscribes.IgnoreQueryFilters()
                .AnyAsync(x => x.TenantId == message.TenantId && x.Channel == message.Channel && x.IsActive &&
                    x.DestinationAddress == recipient.DestinationAddress, cancellationToken);
            if (preferenceBlocked || consentWithdrawn || unsubscribed)
            {
                recipient.Status = NotificationStatus.Cancelled;
                recipient.ErrorMessage = unsubscribed ? "Recipient unsubscribed." : consentWithdrawn ? "Consent was withdrawn." : "Blocked by communication preferences.";
                _dbContext.NotificationDeliveryAttempts.Add(new NotificationDeliveryAttempt
                {
                    TenantId=message.TenantId, BranchId=recipient.BranchId??message.BranchId, NotificationMessageId=message.Id,
                    NotificationRecipientId=recipient.Id, Channel=message.Channel, Status=NotificationAttemptStatus.Skipped,
                    AttemptedOn=DateTime.UtcNow, ErrorMessage=recipient.ErrorMessage
                });
                continue;
            }
            var localizedSubject = message.Subject;
            var localizedBody = message.Body;
            if (recipient.UserId.HasValue && message.NotificationTemplateId.HasValue)
            {
                var locale = await _dbContext.UserCommunicationPreferences.IgnoreQueryFilters()
                    .Where(x => x.TenantId == message.TenantId && x.UserId == recipient.UserId && x.Channel == message.Channel)
                    .Select(x => x.Locale).FirstOrDefaultAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(locale))
                {
                    var localization = await _dbContext.NotificationTemplateLocalizations.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.TenantId == message.TenantId && x.NotificationTemplateId == message.NotificationTemplateId && x.Locale == locale && x.IsActive, cancellationToken);
                    if (localization is not null) { localizedSubject = localization.SubjectTemplate ?? localizedSubject; localizedBody = localization.BodyTemplate; }
                }
            }
            var dispatchMessage = new NotificationDispatchMessage(
                message.Id,
                recipient.Id,
                message.Channel,
                recipient.DestinationAddress,
                recipient.DisplayName,
                localizedSubject,
                localizedBody,
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
            var attemptNumber = await _dbContext.NotificationDeliveryAttempts
                .IgnoreQueryFilters()
                .CountAsync(a => a.TenantId == message.TenantId &&
                                 a.NotificationMessageId == message.Id &&
                                 a.NotificationRecipientId == recipient.Id,
                    cancellationToken) + 1;

            _dbContext.NotificationDeliveryAttempts.Add(new NotificationDeliveryAttempt
            {
                TenantId = message.TenantId,
                BranchId = recipient.BranchId ?? message.BranchId,
                NotificationMessageId = message.Id,
                NotificationRecipientId = recipient.Id,
                AttemptNumber = attemptNumber,
                Channel = message.Channel,
                ProviderKey = message.ProviderKey,
                Status = result.Succeeded ? NotificationAttemptStatus.Succeeded : NotificationAttemptStatus.Failed,
                AttemptedOn = now,
                NextRetryOn = result.Succeeded ? null : now.AddMinutes(Math.Min(240, attemptNumber * 15)),
                ErrorMessage = result.ErrorMessage,
                ProviderResponseJson = string.IsNullOrWhiteSpace(result.ProviderMessageId)
                    ? null
                    : JsonSerializer.Serialize(new { result.ProviderMessageId })
            });

            recipient.Status = result.Succeeded ? NotificationStatus.Sent : NotificationStatus.Failed;
            recipient.SentOn = result.Succeeded ? now : null;
            recipient.ErrorMessage = result.ErrorMessage;

            _dbContext.CommunicationLogs.Add(new CommunicationLog
            {
                TenantId = message.TenantId,
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
