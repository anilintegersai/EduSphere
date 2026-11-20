using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed record GpsPosition(Guid VehicleId, Guid BranchId, Guid? RouteId, decimal Latitude, decimal Longitude, decimal? SpeedKmph, decimal? HeadingDegrees, DateTime RecordedOn, string Provider, string? RawPayload);

public interface ITransportDepthService
{
    Task<VehicleGpsPing> RecordPositionAsync(GpsPosition position, CancellationToken cancellationToken = default);
    Task<RouteOptimizationRun> OptimizeRouteAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task<int> GenerateMonthlyChargesAsync(DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken = default);
}

public sealed class TransportLiveHub : Hub
{
    public Task WatchBranch(string branchId) => Groups.AddToGroupAsync(Context.ConnectionId, $"transport:{branchId}");
}

public sealed class TransportDepthService(TenantDbContext db, IHubContext<TransportLiveHub> hub) : ITransportDepthService
{
    public async Task<VehicleGpsPing> RecordPositionAsync(GpsPosition p, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.FirstOrDefaultAsync(x => x.Id == p.VehicleId && x.BranchId == p.BranchId, ct)
            ?? throw new InvalidOperationException("Vehicle was not found for the selected branch.");
        var ping = new VehicleGpsPing { BranchId=p.BranchId, VehicleId=p.VehicleId, TransportRouteId=p.RouteId, Latitude=p.Latitude, Longitude=p.Longitude, SpeedKmph=p.SpeedKmph, HeadingDegrees=p.HeadingDegrees, RecordedOn=p.RecordedOn, ProviderKey=p.Provider, RawPayloadJson=p.RawPayload };
        db.VehicleGpsPings.Add(ping);
        await db.SaveChangesAsync(ct);

        var fences = await db.TransportGeofences.Where(x => x.BranchId == p.BranchId && x.IsActive).ToListAsync(ct);
        foreach (var fence in fences)
        {
            var distance = DistanceMeters((double)p.Latitude, (double)p.Longitude, (double)fence.Latitude, (double)fence.Longitude);
            if (distance > fence.RadiusMeters) continue;
            var recent = await db.TransportGeofenceEvents.AnyAsync(x => x.TransportGeofenceId == fence.Id && x.VehicleId == vehicle.Id && x.OccurredOn >= p.RecordedOn.AddMinutes(-15), ct);
            if (recent) continue;
            db.TransportGeofenceEvents.Add(new TransportGeofenceEvent { BranchId=p.BranchId, TransportGeofenceId=fence.Id, VehicleId=vehicle.Id, VehicleGpsPingId=ping.Id, EventType=GeofenceEventType.Arriving, OccurredOn=p.RecordedOn, NotificationQueued=fence.NotifyParents });
            if (fence.NotifyParents)
                db.NotificationMessages.Add(new NotificationMessage { BranchId=p.BranchId, Channel=CommunicationChannel.InApp, Subject=$"Bus arriving at {fence.Name}", Body=$"{vehicle.RegistrationNumber} is approaching {fence.Name}.", Status=NotificationStatus.Queued, ScheduledOn=DateTime.UtcNow });
        }
        await db.SaveChangesAsync(ct);
        await hub.Clients.Group($"transport:{p.BranchId}").SendAsync("position", new { vehicleId=p.VehicleId, registration=vehicle.RegistrationNumber, latitude=p.Latitude, longitude=p.Longitude, speed=p.SpeedKmph, recordedOn=p.RecordedOn }, ct);
        return ping;
    }

    public async Task<RouteOptimizationRun> OptimizeRouteAsync(Guid routeId, CancellationToken ct = default)
    {
        var route = await db.TransportRoutes.Include(x => x.Stops).FirstOrDefaultAsync(x => x.Id == routeId, ct) ?? throw new InvalidOperationException("Route not found.");
        var run = new RouteOptimizationRun { BranchId=route.BranchId, TransportRouteId=route.Id, Status=RouteOptimizationStatus.Running, OriginalDistanceKm=route.DistanceKm };
        db.RouteOptimizationRuns.Add(run);
        var remaining = route.Stops.Where(x => x.Latitude.HasValue && x.Longitude.HasValue).ToList();
        var ordered = new List<TransportRouteStop>();
        var current = remaining.OrderBy(x => x.StopOrder).FirstOrDefault();
        while (current is not null)
        {
            ordered.Add(current); remaining.Remove(current);
            current = remaining.OrderBy(x => DistanceMeters((double)ordered[^1].Latitude!.Value, (double)ordered[^1].Longitude!.Value, (double)x.Latitude!.Value, (double)x.Longitude!.Value)).FirstOrDefault();
        }
        var distance = ordered.Zip(ordered.Skip(1), (a,b) => DistanceMeters((double)a.Latitude!.Value,(double)a.Longitude!.Value,(double)b.Latitude!.Value,(double)b.Longitude!.Value)).Sum() / 1000m;
        run.ProposedStopOrderJson=JsonSerializer.Serialize(ordered.Select((x,i)=>new { x.Id, x.StopName, ProposedOrder=i+1 })); run.OptimizedDistanceKm=Math.Round(distance,2); run.Status=RouteOptimizationStatus.Completed; run.CompletedOn=DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return run;
    }

    public async Task<int> GenerateMonthlyChargesAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var assignments=await db.StudentTransportAssignments.Where(x=>x.Status==TransportAssignmentStatus.Active&&x.StartDate<=to&&(x.EndDate==null||x.EndDate>=from)).ToListAsync(ct); var count=0;
        foreach(var a in assignments)
        {
            if(await db.TransportFeeCharges.AnyAsync(x=>x.StudentTransportAssignmentId==a.Id&&x.PeriodStart==from&&x.PeriodEnd==to,ct)) continue;
            var invoice=new FeeInvoice{BranchId=a.BranchId,StudentProfileId=a.StudentProfileId,InvoiceNumber=$"TRN-{from:yyyyMM}-{a.Id.ToString()[..8].ToUpperInvariant()}",InvoiceDate=DateOnly.FromDateTime(DateTime.UtcNow),DueDate=from.AddDays(10),Status=InvoiceStatus.Issued,SubTotal=a.MonthlyFee,TotalAmount=a.MonthlyFee,Notes="Transport monthly fee"};
            invoice.Lines.Add(new FeeInvoiceLine{BranchId=a.BranchId,Description=$"Transport fee {from:MMM yyyy}",ComponentType=FeeComponentType.Transport,Amount=a.MonthlyFee,SortOrder=1});
            db.FeeInvoices.Add(invoice); db.TransportFeeCharges.Add(new TransportFeeCharge{BranchId=a.BranchId,StudentTransportAssignmentId=a.Id,FeeInvoice=invoice,PeriodStart=from,PeriodEnd=to,Amount=a.MonthlyFee,InvoiceNumber=invoice.InvoiceNumber}); count++;
        }
        await db.SaveChangesAsync(ct); return count;
    }

    private static decimal DistanceMeters(double lat1,double lon1,double lat2,double lon2){const double r=6371000;var p1=lat1*Math.PI/180;var p2=lat2*Math.PI/180;var dp=(lat2-lat1)*Math.PI/180;var dl=(lon2-lon1)*Math.PI/180;var a=Math.Sin(dp/2)*Math.Sin(dp/2)+Math.Cos(p1)*Math.Cos(p2)*Math.Sin(dl/2)*Math.Sin(dl/2);return (decimal)(r*2*Math.Atan2(Math.Sqrt(a),Math.Sqrt(1-a)));}
}

public sealed class TransportSecretProtector(IDataProtectionProvider provider)
{
    private readonly IDataProtector protector=provider.CreateProtector("EduSphere.Transport.ProviderSecrets.v1");
    public string Protect(string value)=>protector.Protect(value);
    public string Unprotect(string value)=>protector.Unprotect(value);
    public static string HashToken(string token)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

public sealed class TransportAutomationWorker(IServiceScopeFactory scopes, ILogger<TransportAutomationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try { await RunAsync(stoppingToken); } catch(Exception ex){ logger.LogError(ex,"Transport automation failed."); }
            await Task.Delay(TimeSpan.FromHours(6),stoppingToken);
        }
    }
    private async Task RunAsync(CancellationToken ct)
    {
        using var discovery=scopes.CreateScope(); var root=discovery.ServiceProvider.GetRequiredService<TenantDbContext>();
        var tenants=await root.Vehicles.IgnoreQueryFilters().Where(x=>!x.IsDeleted).Select(x=>x.TenantId).Distinct().ToListAsync(ct);
        foreach(var tenantId in tenants){using var scope=scopes.CreateScope();var tenant=scope.ServiceProvider.GetRequiredService<TenantContext>();tenant.SetTenant(tenantId,tenantId.ToString());var db=scope.ServiceProvider.GetRequiredService<TenantDbContext>();var today=DateOnly.FromDateTime(DateTime.UtcNow);var docs=await db.VehicleDocuments.Where(x=>x.ExpiresOn!=null&&x.Status!=VehicleDocumentStatus.Replaced).ToListAsync(ct);foreach(var d in docs){d.Status=d.ExpiresOn<today?VehicleDocumentStatus.Expired:d.ExpiresOn<=today.AddDays(30)?VehicleDocumentStatus.ExpiringSoon:VehicleDocumentStatus.Valid;if(d.Status!=VehicleDocumentStatus.Valid&&!await db.TransportDocumentReminders.AnyAsync(x=>x.VehicleId==d.VehicleId&&x.DueOn==d.ExpiresOn,ct))db.TransportDocumentReminders.Add(new TransportDocumentReminder{BranchId=d.BranchId,VehicleId=d.VehicleId,ReminderType=d.DocumentType switch{VehicleDocumentType.Insurance=>TransportReminderType.Insurance,VehicleDocumentType.Permit=>TransportReminderType.Permit,VehicleDocumentType.PollutionCertificate=>TransportReminderType.PollutionCertificate,_=>TransportReminderType.Fitness},DueOn=d.ExpiresOn!.Value,Status=d.Status==VehicleDocumentStatus.Expired?OperationalReminderStatus.Overdue:OperationalReminderStatus.DueSoon,Notes=$"Auto-created for {d.FileName}"});}await db.SaveChangesAsync(ct);}
    }
}
