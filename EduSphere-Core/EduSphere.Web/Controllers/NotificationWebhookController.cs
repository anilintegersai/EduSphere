using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers;

[ApiController,AllowAnonymous,Route("api/notifications/webhooks")]
public sealed class NotificationWebhookController(TenantDbContext db):ControllerBase
{
 [HttpPost("{providerKey}")]
 public async Task<IActionResult> Receive(string providerKey,CancellationToken ct)
 {
  var values=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);string raw;
  if(Request.HasFormContentType){var form=await Request.ReadFormAsync(ct);foreach(var x in form)values[x.Key]=x.Value.ToString();raw=string.Join('&',values.Select(x=>$"{x.Key}={x.Value}"));}
  else{using var reader=new StreamReader(Request.Body);raw=await reader.ReadToEndAsync(ct);if(!string.IsNullOrWhiteSpace(raw)){using var json=JsonDocument.Parse(raw);foreach(var p in json.RootElement.EnumerateObject())values[p.Name]=p.Value.ToString();}}
  var supplied=Request.Headers["X-Webhook-Secret"].FirstOrDefault()??values.GetValueOrDefault("webhookSecret");
  var settings=await db.NotificationProviderSettings.IgnoreQueryFilters().Where(x=>x.ProviderKey==providerKey&&x.IsEnabled).ToListAsync(ct);NotificationProviderSetting? setting=null;
  foreach(var candidate in settings){if(candidate.ConfigurationJson is null)continue;using var cfg=JsonDocument.Parse(candidate.ConfigurationJson);var expected=cfg.RootElement.TryGetProperty("webhookSecret",out var secret)?secret.GetString():null;if(!string.IsNullOrWhiteSpace(expected)&&!string.IsNullOrWhiteSpace(supplied)&&CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected),Encoding.UTF8.GetBytes(supplied))){setting=candidate;break;}}
  if(setting is null)return Unauthorized();
  var messageId=values.GetValueOrDefault("MessageSid")??values.GetValueOrDefault("messageId")??values.GetValueOrDefault("id");var status=values.GetValueOrDefault("MessageStatus")??values.GetValueOrDefault("status");if(string.IsNullOrWhiteSpace(messageId)||string.IsNullOrWhiteSpace(status))return BadRequest(new{message="Provider message id and status are required."});
  var type=Map(status);if(await db.NotificationProviderWebhookEvents.IgnoreQueryFilters().AnyAsync(x=>x.TenantId==setting.TenantId&&x.ProviderKey==providerKey&&x.ProviderMessageId==messageId&&x.EventType==type,ct))return Ok();
  db.NotificationProviderWebhookEvents.Add(new(){TenantId=setting.TenantId,ProviderKey=providerKey,ProviderMessageId=messageId,EventType=type,PayloadJson=raw.Length>4000?raw[..4000]:raw,SignatureVerified=true,ProcessedOn=DateTime.UtcNow});
  var log=await db.CommunicationLogs.IgnoreQueryFilters().OrderByDescending(x=>x.OccurredOn).FirstOrDefaultAsync(x=>x.TenantId==setting.TenantId&&x.ProviderMessageId==messageId,ct);if(log is not null){log.Status=type is DeliveryEventType.Delivered or DeliveryEventType.Read?NotificationStatus.Sent:type is DeliveryEventType.Bounced or DeliveryEventType.Rejected?NotificationStatus.Failed:log.Status;log.ErrorMessage=log.Status==NotificationStatus.Failed?$"Provider reported {status}":null;}
  await db.SaveChangesAsync(ct);return Ok();
 }
 private static DeliveryEventType Map(string value)=>value.ToLowerInvariant() switch{"delivered"=>DeliveryEventType.Delivered,"read"=>DeliveryEventType.Read,"bounced" or "undelivered"=>DeliveryEventType.Bounced,"rejected" or "failed"=>DeliveryEventType.Rejected,"unsubscribe" or "unsubscribed"=>DeliveryEventType.Unsubscribed,_=>DeliveryEventType.Accepted};
}
