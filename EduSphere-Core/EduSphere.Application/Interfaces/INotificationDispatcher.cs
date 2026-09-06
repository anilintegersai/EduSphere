using EduSphere.Domain.Enums;

namespace EduSphere.Application.Interfaces;

public sealed record NotificationDispatchMessage(
    Guid MessageId,
    Guid RecipientId,
    CommunicationChannel Channel,
    string DestinationAddress,
    string DisplayName,
    string? Subject,
    string Body,
    string? ProviderKey);

public sealed record NotificationDispatchResult(
    bool Succeeded,
    string? ProviderMessageId = null,
    string? ErrorMessage = null)
{
    public static NotificationDispatchResult Success(string? providerMessageId = null) => new(true, providerMessageId);
    public static NotificationDispatchResult Failure(string errorMessage) => new(false, null, errorMessage);
}

public interface INotificationChannelSender
{
    CommunicationChannel Channel { get; }
    Task<NotificationDispatchResult> SendAsync(NotificationDispatchMessage message, CancellationToken cancellationToken = default);
}

public interface INotificationDispatcher
{
    Task<NotificationDispatchResult> DispatchAsync(Guid notificationMessageId, CancellationToken cancellationToken = default);
}
