using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;
public sealed class HostelOperationsWorker(IServiceScopeFactory scopes,ILogger<HostelOperationsWorker> logger):BackgroundService
{
 protected override async Task ExecuteAsync(CancellationToken token){while(!token.IsCancellationRequested){try{await Run(token);}catch(Exception ex){logger.LogError(ex,"Hostel SLA cycle failed.");}await Task.Delay(TimeSpan.FromMinutes(30),token);}}
 private async Task Run(CancellationToken token){using var discovery=scopes.CreateScope();var root=discovery.ServiceProvider.GetRequiredService<TenantDbContext>();var tenants=await root.HostelAllocations.IgnoreQueryFilters().Where(x=>!x.IsDeleted&&x.Status==HostelAllocationStatus.Active).Select(x=>x.TenantId).Distinct().ToListAsync(token);foreach(var id in tenants){using var scope=scopes.CreateScope();scope.ServiceProvider.GetRequiredService<TenantContext>().SetTenant(id,id.ToString());var service=scope.ServiceProvider.GetRequiredService<IHostelDepthService>();await service.EscalateBreachedMaintenanceAsync(token);var db=scope.ServiceProvider.GetRequiredService<TenantDbContext>();var overdue=await db.HostelGatePasses.Where(x=>x.Status==GatePassStatus.CheckedOut&&x.PlannedReturnOn<DateTime.UtcNow).ToListAsync(token);foreach(var pass in overdue)pass.Status=GatePassStatus.Overdue;await db.SaveChangesAsync(token);}}
}
