using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class ReportingAutomationWorker(IServiceScopeFactory scopes, ILogger<ReportingAutomationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await Run(stoppingToken); } catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { } catch (Exception ex) { logger.LogError(ex, "Reporting schedule and snapshot cycle failed."); }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    private async Task Run(CancellationToken token)
    {
        using var discovery = scopes.CreateScope(); var root = discovery.ServiceProvider.GetRequiredService<TenantDbContext>();
        var tenants = await root.ReportDefinitions.IgnoreQueryFilters().Where(x => !x.IsDeleted).Select(x => x.TenantId).Distinct().ToListAsync(token);
        foreach (var tenantId in tenants) { using var scope = scopes.CreateScope(); scope.ServiceProvider.GetRequiredService<TenantContext>().SetTenant(tenantId, tenantId.ToString()); var service = scope.ServiceProvider.GetRequiredService<IReportingService>(); await service.ProcessSchedulesAsync(token); await service.CaptureSnapshotsAsync(DateOnly.FromDateTime(DateTime.UtcNow), token); }
    }
}
