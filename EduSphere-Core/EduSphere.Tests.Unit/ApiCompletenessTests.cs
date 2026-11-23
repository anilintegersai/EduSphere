using EduSphere.Application.Common;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class ApiCompletenessTests
{
    [Fact]
    public void PagingOptionsClampUnsafeValues()
    {
        var options = new ApiQueryOptions { Page = -2, PageSize = 50_000, SortDirection = "desc" };
        Assert.Equal(1, options.Page);
        Assert.Equal(100, options.PageSize);
        Assert.True(options.Descending);
    }

    [Fact]
    public void IdempotencyKeyIsUniqueForTenantUserMethodAndPath()
    {
        var tenantId = Guid.NewGuid(); var userId = Guid.NewGuid();
        var tenant = new TenantContext(); tenant.SetTenant(tenantId, "api-test");
        using var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant);
        var entity = db.Model.FindEntityType(typeof(ApiIdempotencyRecord))!;
        Assert.Contains(entity.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(x => x.Name).SequenceEqual(new[] { "TenantId", "UserId", "HttpMethod", "RequestPath", "IdempotencyKey" }));
    }
}
