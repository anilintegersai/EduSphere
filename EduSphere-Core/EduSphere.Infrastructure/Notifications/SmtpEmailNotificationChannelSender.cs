using System.Net;
using System.Net.Mail;
using System.Text.Json;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduSphere.Infrastructure.Notifications;

public sealed class SmtpEmailNotificationChannelSender : INotificationChannelSender
{
    private readonly TenantDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public SmtpEmailNotificationChannelSender(TenantDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public CommunicationChannel Channel => CommunicationChannel.Email;

    public async Task<NotificationDispatchResult> SendAsync(NotificationDispatchMessage message, CancellationToken cancellationToken = default)
    {
        var provider = await ResolveProviderAsync(message, cancellationToken);
        if (provider is null)
            return NotificationDispatchResult.Failure("No enabled email provider setting was found for this message scope.");

        var options = ParseOptions(provider.ConfigurationJson);
        if (string.IsNullOrWhiteSpace(options.Host))
            return NotificationDispatchResult.Failure("Email provider configuration must include an SMTP host.");

        var fromAddress = provider.FromAddress ?? options.FromAddress;
        if (string.IsNullOrWhiteSpace(fromAddress))
            return NotificationDispatchResult.Failure("Email provider configuration must include a from address.");

        using var mail = new MailMessage
        {
            From = new MailAddress(fromAddress, provider.FromDisplayName ?? options.FromDisplayName),
            Subject = message.Subject ?? string.Empty,
            Body = message.Body,
            IsBodyHtml = options.IsBodyHtml
        };
        mail.To.Add(new MailAddress(message.DestinationAddress, message.DisplayName));

        using var client = new SmtpClient(options.Host, options.Port <= 0 ? 25 : options.Port)
        {
            EnableSsl = options.EnableSsl
        };

        var password = string.IsNullOrWhiteSpace(provider.SecretReference)
            ? options.Password
            : _configuration[provider.SecretReference] ?? options.Password;
        var userName = options.UserName;
        if (!string.IsNullOrWhiteSpace(userName))
            client.Credentials = new NetworkCredential(userName, password);

        await client.SendMailAsync(mail, cancellationToken);
        return NotificationDispatchResult.Success();
    }

    private async Task<NotificationProviderSetting?> ResolveProviderAsync(NotificationDispatchMessage message, CancellationToken cancellationToken)
    {
        var query = _dbContext.NotificationProviderSettings
            .Where(p => p.Channel == CommunicationChannel.Email && p.IsEnabled);

        if (!string.IsNullOrWhiteSpace(message.ProviderKey))
            query = query.Where(p => p.ProviderKey == message.ProviderKey);

        var messageEntity = await _dbContext.NotificationMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == message.MessageId, cancellationToken);

        return await query
            .OrderByDescending(p => messageEntity != null && p.BranchId == messageEntity.BranchId)
            .ThenByDescending(p => p.BranchId == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static SmtpEmailOptions ParseOptions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new SmtpEmailOptions();

        try
        {
            return JsonSerializer.Deserialize<SmtpEmailOptions>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SmtpEmailOptions();
        }
        catch (JsonException)
        {
            return new SmtpEmailOptions();
        }
    }

    private sealed class SmtpEmailOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; } = 25;
        public bool EnableSsl { get; set; } = true;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FromAddress { get; set; }
        public string? FromDisplayName { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
