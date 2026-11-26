using System.Security.Claims;
using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Controllers.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class InstitutionFoundationTests
{
    [Fact]
    public async Task UnrelatedTenantCannotAcceptAffiliation()
    {
        var owner = Guid.NewGuid();
        var affiliate = Guid.NewGuid();
        var tenant = new TenantContext();
        tenant.SetTenant(owner, "owner");
        await using var db = CreateDb(tenant);
        var link = new TenantAffiliation { TenantId = owner, AffiliateTenantId = affiliate };
        db.Add(link);
        await db.SaveChangesAsync();
        var controller = new InstitutionsController(db, tenant);
        Assert.IsType<NotFoundResult>(await controller.Accept(link.Id, default));
        tenant.SetTenant(Guid.NewGuid(), "unrelated");
        Assert.IsType<NotFoundResult>(await controller.Accept(link.Id, default));
        tenant.SetTenant(affiliate, "affiliate");
        Assert.IsType<OkObjectResult>(await controller.Accept(link.Id, default));
        Assert.NotNull(link.AcceptedOn);
    }

    [Fact]
    public async Task PolicyRequiresUnitGrantAndResolvesEffectiveVersion()
    {
        var tenantId = Guid.NewGuid();
        var tenant = new TenantContext();
        tenant.SetTenant(tenantId, "school");
        await using var db = CreateDb(tenant);
        var unit = new OrganizationUnit { TenantId = tenantId, Name = "School" };
        db.Add(unit);
        await db.SaveChangesAsync();
        var userId = Guid.NewGuid();
        var controller = new InstitutionsController(db, tenant) { ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", userId.ToString())], "test")) }
        } };
        using var definition = JsonDocument.Parse("{}");
        var effective = new DateOnly(2026, 1, 1);
        Assert.IsType<OkObjectResult>(await controller.PublishPolicy(unit.Id, "attendance", new(effective, definition.RootElement), default));
        Assert.IsType<ForbidResult>(await controller.ResolvePolicy(unit.Id, "attendance", effective, default));
        db.Add(new OrganizationPermission { TenantId = tenantId, OrganizationUnitId = unit.Id, UserId = userId, Permission = "policy.read" });
        await db.SaveChangesAsync();
        Assert.IsType<NotFoundResult>(await controller.ResolvePolicy(unit.Id, "attendance", effective.AddDays(-1), default));
        Assert.IsType<OkObjectResult>(await controller.ResolvePolicy(unit.Id, "attendance", effective, default));
    }

    private static TenantDbContext CreateDb(TenantContext tenant) => new(
        new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant);
}
