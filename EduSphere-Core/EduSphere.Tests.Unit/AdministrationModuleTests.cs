using EduSphere.Domain.Common;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class AdministrationModuleTests
{
    private static readonly Guid TenantId = Guid.Parse("7e7133fe-6cd8-4ef0-9cea-68300b8c34e2");

    private static (TenantDbContext Db, TenantContext Tenant) CreateDb(string? userId = null)
    {
        var tenant = new TenantContext(); tenant.SetTenant(TenantId, "administration-test");
        var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            tenant, new TestUser(userId ?? "administrator"));
        return (db, tenant);
    }

    [Fact]
    public async Task WritePipelineCreatesTenantAuditAndRedactsSensitiveSetting()
    {
        var (db, _) = CreateDb("audit-user"); await using var scope = db;
        db.TenantSettings.Add(new TenantSetting { Key = "Integration.PrivateKey", Value = "cipher-text", ValueType = SettingValueType.String, IsSensitive = true });
        await db.SaveChangesAsync();
        var audit = await db.AuditLogs.SingleAsync(a => a.EntityType == nameof(TenantSetting));
        Assert.Equal(TenantId, audit.TenantId);
        Assert.Equal("audit-user", audit.ActorId);
        Assert.Contains("[REDACTED]", audit.NewValuesJson);
        Assert.DoesNotContain("cipher-text", audit.NewValuesJson);
    }

    [Fact]
    public async Task UserPermissionDenyOverridesTenantAdminRoleDefault()
    {
        var userId = Guid.NewGuid();
        var (db, tenant) = CreateDb(userId.ToString()); await using var scope = db;
        var roleId = Roles.SeedIds[Roles.TenantAdmin];
        var user = new ApplicationUser { Id = userId, TenantId = TenantId, UserName = "admin@example.test", NormalizedUserName = "ADMIN@EXAMPLE.TEST", Email = "admin@example.test", NormalizedEmail = "ADMIN@EXAMPLE.TEST", FirstName = "Tenant", LastName = "Admin", IsActive = true };
        var role = new ApplicationRole { Id = roleId, Name = Roles.TenantAdmin, NormalizedName = Roles.TenantAdmin.ToUpperInvariant() };
        var permission = new PermissionDefinition { Key = PermissionKeys.AdministrationManageTenant, DisplayName = "Manage tenant", Module = "Administration" };
        db.AddRange(user, role, permission); await db.SaveChangesAsync();
        db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid> { UserId = userId, RoleId = roleId }); await db.SaveChangesAsync();
        var service = new EffectivePermissionService(db, tenant);
        Assert.True(await service.HasPermissionAsync(userId, PermissionKeys.AdministrationManageTenant));
        db.UserPermissionGrants.Add(new UserPermissionGrant { UserId = userId, PermissionDefinitionId = permission.Id, IsAllowed = false }); await db.SaveChangesAsync();
        Assert.False(await service.HasPermissionAsync(userId, PermissionKeys.AdministrationManageTenant));
    }

    [Fact]
    public async Task UserPermissionDenyOverridesModuleRoleDefault()
    {
        var userId = Guid.NewGuid();
        var (db, tenant) = CreateDb(userId.ToString()); await using var scope = db;
        var roleId = Roles.SeedIds[Roles.Accountant];
        var user = new ApplicationUser { Id = userId, TenantId = TenantId, UserName = "accountant@example.test", NormalizedUserName = "ACCOUNTANT@EXAMPLE.TEST", Email = "accountant@example.test", NormalizedEmail = "ACCOUNTANT@EXAMPLE.TEST", FirstName = "Finance", LastName = "Officer", IsActive = true };
        var role = new ApplicationRole { Id = roleId, Name = Roles.Accountant, NormalizedName = Roles.Accountant.ToUpperInvariant() };
        var permission = new PermissionDefinition { Key = PermissionKeys.FinanceManage, DisplayName = "Manage finance", Module = "Finance" };
        db.AddRange(user, role, permission); await db.SaveChangesAsync();
        db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid> { UserId = userId, RoleId = roleId }); await db.SaveChangesAsync();
        var service = new EffectivePermissionService(db, tenant);
        Assert.True(await service.HasPermissionAsync(userId, PermissionKeys.FinanceManage));
        db.UserPermissionGrants.Add(new UserPermissionGrant { UserId = userId, PermissionDefinitionId = permission.Id, IsAllowed = false }); await db.SaveChangesAsync();
        Assert.False(await service.HasPermissionAsync(userId, PermissionKeys.FinanceManage));
    }

    [Fact]
    public async Task FeatureFlagHonorsEnabledWindowAndDefaultsUnknownFeaturesOn()
    {
        var (db, _) = CreateDb(); await using var scope = db;
        db.TenantFeatureFlags.Add(new TenantFeatureFlag { FeatureKey = "module.ai-question-papers", DisplayName = "AI", IsEnabled = true, EnabledFrom = DateTime.UtcNow.AddHours(1) });
        await db.SaveChangesAsync();
        var service = new TenantFeatureService(db);
        Assert.False(await service.IsEnabledAsync("module.ai-question-papers"));
        Assert.True(await service.IsEnabledAsync("module.finance"));
    }

    [Fact]
    public async Task BackgroundJobMonitorTracksRetryableFailureAndCompletion()
    {
        var (db, tenant) = CreateDb(); await using var scope = db;
        var monitor = new BackgroundJobMonitor(db, tenant);
        var job = await monitor.EnqueueAsync("FeeReminder", "fees-2026-09", "{}");
        await monitor.MarkRunningAsync(job.Id);
        await monitor.MarkFailedAsync(job.Id, new InvalidOperationException("SMTP unavailable"), DateTime.UtcNow.AddMinutes(5));
        var failed = await db.BackgroundJobLogs.SingleAsync();
        Assert.Equal(BackgroundJobStatus.RetryScheduled, failed.Status);
        Assert.Equal(1, failed.AttemptCount);
        await monitor.MarkRunningAsync(job.Id); await monitor.MarkSucceededAsync(job.Id);
        Assert.Equal(BackgroundJobStatus.Succeeded, (await db.BackgroundJobLogs.SingleAsync()).Status);
    }

    private sealed class TestUser(string id) : ICurrentUserContext { public string? UserId => id; }
}
