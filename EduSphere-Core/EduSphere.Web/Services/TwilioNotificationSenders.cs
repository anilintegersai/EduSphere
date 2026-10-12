using System.Net.Http.Headers;
using System.Text.Json;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public abstract class TwilioNotificationSender(TenantDbContext db, IHttpClientFactory clients, IDataProtectionProvider protection) : INotificationChannelSender
{
    public abstract CommunicationChannel Channel { get; }
    public async Task<NotificationDispatchResult> SendAsync(NotificationDispatchMessage message, CancellationToken token = default)
    {
        var tenantId = await db.NotificationMessages.IgnoreQueryFilters().Where(x => x.Id == message.MessageId).Select(x => x.TenantId).FirstOrDefaultAsync(token);
        var setting = await db.TenantIntegrationSettings.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IntegrationKey == "twilio" && x.IsEnabled, token);
        if (setting?.ProtectedSecret is null) return NotificationDispatchResult.Failure("Twilio is not configured for this tenant.");
        using var config = JsonDocument.Parse(setting.ConfigurationJson); var root = config.RootElement;
        var sid = root.TryGetProperty("accountSid", out var sidNode) ? sidNode.GetString() : null;
        var from = Channel == CommunicationChannel.WhatsApp && root.TryGetProperty("whatsAppFrom", out var wa) ? wa.GetString() : root.TryGetProperty("smsFrom", out var sms) ? sms.GetString() : null;
        if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(from)) return NotificationDispatchResult.Failure("Twilio accountSid/from configuration is incomplete.");
        string secret; try { secret = protection.CreateProtector("EduSphere.Administration.IntegrationSecrets.v1").Unprotect(setting.ProtectedSecret); } catch { return NotificationDispatchResult.Failure("Twilio secret could not be decrypted."); }
        var to = Channel == CommunicationChannel.WhatsApp && !message.DestinationAddress.StartsWith("whatsapp:") ? "whatsapp:" + message.DestinationAddress : message.DestinationAddress;
        if (Channel == CommunicationChannel.WhatsApp && !from.StartsWith("whatsapp:")) from = "whatsapp:" + from;
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.twilio.com/2010-04-01/Accounts/{sid}/Messages.json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{sid}:{secret}")));
        request.Content = new FormUrlEncodedContent(new Dictionary<string,string> { ["To"] = to, ["From"] = from, ["Body"] = message.Body });
        using var response = await clients.CreateClient().SendAsync(request, token); var body = await response.Content.ReadAsStringAsync(token);
        if (!response.IsSuccessStatusCode) return NotificationDispatchResult.Failure("Twilio rejected the message.");
        using var result = JsonDocument.Parse(body); return NotificationDispatchResult.Success(result.RootElement.TryGetProperty("sid", out var id) ? id.GetString() : null);
    }
}
public sealed class TwilioSmsNotificationSender(TenantDbContext db, IHttpClientFactory clients, IDataProtectionProvider protection) : TwilioNotificationSender(db, clients, protection) { public override CommunicationChannel Channel => CommunicationChannel.Sms; }
public sealed class TwilioWhatsAppNotificationSender(TenantDbContext db, IHttpClientFactory clients, IDataProtectionProvider protection) : TwilioNotificationSender(db, clients, protection) { public override CommunicationChannel Channel => CommunicationChannel.WhatsApp; }
