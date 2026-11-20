using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers;

[ApiController, Route("api/transport/gps")]
public sealed class TransportGpsController(TenantDbContext db, TenantContext tenant, ITransportDepthService service) : ControllerBase
{
    public sealed record GpsWebhook(string DeviceId, decimal Latitude, decimal Longitude, decimal? SpeedKmph, decimal? HeadingDegrees, DateTime? RecordedOn, Guid? RouteId);
    [HttpPost("webhook/{token}")]
    public async Task<IActionResult> Webhook(string token,[FromBody]GpsWebhook payload,CancellationToken ct)
    {
        var hash=TransportSecretProtector.HashToken(token);var setting=await db.TransportGpsProviderSettings.IgnoreQueryFilters().FirstOrDefaultAsync(x=>x.WebhookTokenHash==hash&&x.IsActive,ct);if(setting is null)return Unauthorized();
        tenant.SetTenant(setting.TenantId,setting.TenantId.ToString());var vehicle=await db.Vehicles.FirstOrDefaultAsync(x=>x.GpsDeviceId==payload.DeviceId,ct);if(vehicle is null)return NotFound(new{message="Unknown GPS device."});
        await service.RecordPositionAsync(new GpsPosition(vehicle.Id,vehicle.BranchId,payload.RouteId,payload.Latitude,payload.Longitude,payload.SpeedKmph,payload.HeadingDegrees,payload.RecordedOn??DateTime.UtcNow,setting.ProviderType.ToString(),System.Text.Json.JsonSerializer.Serialize(payload)),ct);setting.LastSuccessfulSyncOn=DateTime.UtcNow;await db.SaveChangesAsync(ct);return Accepted();
    }
}
