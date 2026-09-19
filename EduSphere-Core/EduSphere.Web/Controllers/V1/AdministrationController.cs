using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/administration")]
[Authorize(Policy = AdministrationPolicies.View)]
public sealed class AdministrationController(TenantDbContext db, ITenantContext tenant) : ApiControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<AdministrationOverview>>> Overview(CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant) return BadRequest(ApiResponse<AdministrationOverview>.Fail("Select a tenant."));
        var result = new AdministrationOverview(
            await db.TenantDomains.CountAsync(cancellationToken),
            await db.TenantIntegrationSettings.CountAsync(i => i.IsEnabled, cancellationToken),
            await db.TenantFeatureFlags.CountAsync(f => f.IsEnabled, cancellationToken),
            await db.LookupItems.CountAsync(l => l.IsActive, cancellationToken),
            await db.BackgroundJobLogs.CountAsync(j => j.Status == BackgroundJobStatus.Failed, cancellationToken));
        return Ok(ApiResponse<AdministrationOverview>.Ok(result));
    }

    [HttpGet("audit")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AuditLog>>>> Audit([FromQuery] string? entityType, [FromQuery] int take = 100, CancellationToken cancellationToken = default)
    {
        if (!tenant.TenantId.HasValue) return BadRequest(ApiResponse<IReadOnlyList<AuditLog>>.Fail("Select a tenant."));
        var query = db.AuditLogs.AsNoTracking().Where(a => a.TenantId == tenant.TenantId);
        if (!string.IsNullOrWhiteSpace(entityType)) query = query.Where(a => a.EntityType == entityType);
        var rows = await query.OrderByDescending(a => a.OccurredOn).Take(Math.Clamp(take, 1, 500)).ToListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AuditLog>>.Ok(rows));
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BackgroundJobLog>>>> Jobs([FromQuery] BackgroundJobStatus? status, CancellationToken cancellationToken)
    {
        var query = db.BackgroundJobLogs.AsNoTracking(); if (status.HasValue) query = query.Where(j => j.Status == status);
        return Ok(ApiResponse<IReadOnlyList<BackgroundJobLog>>.Ok(await query.OrderByDescending(j => j.ScheduledOn).Take(200).ToListAsync(cancellationToken)));
    }

    [HttpPut("features/{featureKey}")]
    [Authorize(Policy = AdministrationPolicies.ManageTenant)]
    public async Task<ActionResult<ApiResponse<Guid>>> SetFeature(string featureKey, SetFeatureRequest request, CancellationToken cancellationToken)
    {
        if (!tenant.TenantId.HasValue) return BadRequest(ApiResponse<Guid>.Fail("Select a tenant."));
        var entity = await db.TenantFeatureFlags.SingleOrDefaultAsync(f => f.FeatureKey == featureKey, cancellationToken)
            ?? new TenantFeatureFlag { TenantId = tenant.TenantId.Value, FeatureKey = featureKey };
        if (entity.Id == Guid.Empty) db.TenantFeatureFlags.Add(entity);
        entity.DisplayName = request.DisplayName; entity.Description = request.Description; entity.IsEnabled = request.IsEnabled;
        entity.EnabledFrom = request.EnabledFrom; entity.EnabledUntil = request.EnabledUntil; entity.ConditionsJson = request.ConditionsJson ?? "{}";
        await db.SaveChangesAsync(cancellationToken); return Ok(ApiResponse<Guid>.Ok(entity.Id));
    }

    [HttpPut("permissions/role")]
    [Authorize(Policy = AdministrationPolicies.ManagePermissions)]
    public async Task<ActionResult<ApiResponse<Guid>>> SetRolePermission(PermissionOverrideRequest request, CancellationToken cancellationToken)
    {
        if (!tenant.TenantId.HasValue || !request.RoleId.HasValue) return BadRequest(ApiResponse<Guid>.Fail("Tenant and role are required."));
        var entity = await db.RolePermissionGrants.SingleOrDefaultAsync(g => g.RoleId == request.RoleId && g.PermissionDefinitionId == request.PermissionDefinitionId, cancellationToken)
            ?? new RolePermissionGrant { TenantId = tenant.TenantId.Value, RoleId = request.RoleId.Value, PermissionDefinitionId = request.PermissionDefinitionId };
        if (entity.Id == Guid.Empty) db.RolePermissionGrants.Add(entity); entity.IsAllowed = request.IsAllowed;
        await db.SaveChangesAsync(cancellationToken); return Ok(ApiResponse<Guid>.Ok(entity.Id));
    }

    public sealed record AdministrationOverview(int Domains, int EnabledIntegrations, int EnabledFeatures, int ActiveLookups, int FailedJobs);
    public sealed record SetFeatureRequest(string DisplayName, string? Description, bool IsEnabled, DateTime? EnabledFrom, DateTime? EnabledUntil, string? ConditionsJson);
    public sealed record PermissionOverrideRequest(Guid PermissionDefinitionId, Guid? RoleId, bool IsAllowed);
}
