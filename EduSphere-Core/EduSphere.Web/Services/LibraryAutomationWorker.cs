using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class LibraryAutomationWorker(IServiceScopeFactory scopes, ILogger<LibraryAutomationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await Run(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex) { logger.LogError(ex, "Library overdue and reservation processing failed."); }
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
    private async Task Run(CancellationToken token)
    {
        using var discovery = scopes.CreateScope(); var root = discovery.ServiceProvider.GetRequiredService<TenantDbContext>();
        var tenants = await root.LibraryBookIssues.IgnoreQueryFilters().Where(x => !x.IsDeleted).Select(x => x.TenantId).Distinct().ToListAsync(token);
        foreach (var tenantId in tenants)
        {
            using var scope = scopes.CreateScope(); var tenant = scope.ServiceProvider.GetRequiredService<TenantContext>(); tenant.SetTenant(tenantId, tenantId.ToString());
            await scope.ServiceProvider.GetRequiredService<ILibraryDepthService>().ProcessQueuesAndOverduesAsync(token);
        }
    }
}
