using EduSphere.Application.Interfaces;
using EduSphere.Domain.Enums;

namespace EduSphere.Infrastructure.Notifications;

public abstract class DeferredNotificationChannelSender : INotificationChannelSender
{
    public abstract CommunicationChannel Channel { get; }

    public Task<NotificationDispatchResult> SendAsync(NotificationDispatchMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult(NotificationDispatchResult.Failure(
            $"{Channel} provider adapter is not configured yet. Add a concrete INotificationChannelSender for this channel."));
}

public sealed class WhatsAppNotificationChannelSender : DeferredNotificationChannelSender
{
    public override CommunicationChannel Channel => CommunicationChannel.WhatsApp;
}

public sealed class TelegramNotificationChannelSender : DeferredNotificationChannelSender
{
    public override CommunicationChannel Channel => CommunicationChannel.Telegram;
}

public sealed class SmsNotificationChannelSender : DeferredNotificationChannelSender
{
    public override CommunicationChannel Channel => CommunicationChannel.Sms;
}

public sealed class PushNotificationChannelSender : DeferredNotificationChannelSender
{
    public override CommunicationChannel Channel => CommunicationChannel.Push;
}

public sealed class InAppNotificationChannelSender : DeferredNotificationChannelSender
{
    public override CommunicationChannel Channel => CommunicationChannel.InApp;
}
