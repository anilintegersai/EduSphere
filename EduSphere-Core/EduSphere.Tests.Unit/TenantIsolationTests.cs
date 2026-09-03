using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

/// <summary>
/// Verifies the shared-DB / shared-schema tenancy model: the global query filter
/// isolates reads, the ChangeTracker stamps TenantId on writes, and an unresolved
/// tenant sees no tenant-owned rows.
/// </summary>
public class TenantIsolationTests
{
    private static readonly Guid Tenant1Id = Guid.Parse("11111111-aaaa-4111-8111-111111111111");
    private static readonly Guid Tenant2Id = Guid.Parse("22222222-bbbb-4222-8222-222222222222");
    private static readonly Guid Tenant7Id = Guid.Parse("77777777-cccc-4777-8777-777777777777");
    private static readonly Guid Tenant100Id = Guid.Parse("aaaaaaaa-dddd-4aaa-8aaa-aaaaaaaaaaaa");
    private static readonly Guid Tenant200Id = Guid.Parse("bbbbbbbb-eeee-4bbb-8bbb-bbbbbbbbbbbb");

    private static TenantDbContext NewDb(string dbName, TenantContext tenantContext, TestCurrentUserContext? currentUser = null)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TenantDbContext(options, tenantContext, currentUser);
    }

    [Fact]
    public void GlobalQueryFilter_IsolatesReadsByTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        // Seed two tenants' branches (writes are not tenant-filtered).
        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - North", TenantId = Tenant1Id });
            db.Branches.Add(new Branch { Name = "Tenant1 - South", TenantId = Tenant1Id });
            db.Branches.Add(new Branch { Name = "Tenant2 - Main", TenantId = Tenant2Id });
            db.SaveChanges();
        }

        // Read as tenant 1.
        var t1 = new TenantContext();
        t1.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            var branches = db.Branches.ToList();
            Assert.Equal(2, branches.Count);
            Assert.All(branches, b => Assert.Equal(Tenant1Id, b.TenantId));
        }

        // Read as tenant 2.
        var t2 = new TenantContext();
        t2.SetTenant(Tenant2Id, "tenant2");
        using (var db = NewDb(dbName, t2))
        {
            var branch = Assert.Single(db.Branches.ToList());
            Assert.Equal("Tenant2 - Main", branch.Name);
        }
    }

    [Fact]
    public void SaveChanges_StampsCurrentTenantOnInsert()
    {
        var dbName = Guid.NewGuid().ToString();
        var ctx = new TenantContext();
        ctx.SetTenant(Tenant7Id, "tenant7");

        using var db = NewDb(dbName, ctx);
        var branch = new Branch { Name = "Unstamped" }; // TenantId left empty
        db.Branches.Add(branch);
        db.SaveChanges();

        Assert.Equal(Tenant7Id, branch.TenantId);
    }

    [Fact]
    public void NoResolvedTenant_ReturnsNoTenantOwnedRows()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - Only", TenantId = Tenant1Id });
            db.SaveChanges();
        }

        // A request with no tenant resolved must not see tenant-owned data.
        using (var db = NewDb(dbName, new TenantContext()))
        {
            Assert.Empty(db.Branches.ToList());
        }
    }

    [Fact]
    public void AcademicEntities_AreTenantFilteredAndStamped()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Courses.Add(new Course { Code = "A", Name = "Course A", TenantId = Tenant1Id });
            db.Courses.Add(new Course { Code = "B", Name = "Course B", TenantId = Tenant2Id });
            db.SaveChanges();
        }

        var t1 = new TenantContext();
        t1.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            // Read: filtered to tenant 1 only.
            var course = Assert.Single(db.Courses.ToList());
            Assert.Equal(Tenant1Id, course.TenantId);

            // Write: TenantId is stamped from the current tenant when left unset.
            var created = new Course { Code = "C", Name = "Course C" };
            db.Courses.Add(created);
            db.SaveChanges();
            Assert.Equal(Tenant1Id, created.TenantId);
        }
    }

    [Fact]
    public void CrossTenantData_IsNotVisibleToAnotherTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Secret", TenantId = Tenant100Id });
            db.SaveChanges();
        }

        var other = new TenantContext();
        other.SetTenant(Tenant200Id, "tenant200");
        using (var db = NewDb(dbName, other))
        {
            Assert.Empty(db.Branches.ToList());
        }
    }

    [Fact]
    public void SaveChanges_StampsGuidAuditAndConcurrencyToken()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        var currentUser = new TestCurrentUserContext { UserId = "creator-user" };

        using var db = NewDb(dbName, tenant, currentUser);
        var branch = new Branch { Name = "Audited" };
        db.Branches.Add(branch);
        db.SaveChanges();

        Assert.NotEqual(Guid.Empty, branch.Id);
        Assert.Equal(Tenant1Id, branch.TenantId);
        Assert.Equal("creator-user", branch.CreatedBy);
        Assert.NotEqual(default, branch.CreatedOn);
        Assert.Null(branch.ModifiedBy);
        Assert.Null(branch.ModifiedOn);
        Assert.NotEqual(Guid.Empty, branch.ConcurrencyToken);

        var originalToken = branch.ConcurrencyToken;
        currentUser.UserId = "editor-user";
        branch.Name = "Audited Updated";
        db.SaveChanges();

        Assert.Equal("creator-user", branch.CreatedBy);
        Assert.Equal("editor-user", branch.ModifiedBy);
        Assert.NotNull(branch.ModifiedOn);
        Assert.NotEqual(originalToken, branch.ConcurrencyToken);
    }

    [Fact]
    public void SoftDelete_HidesRowsAndStampsDeleteMetadata()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        var currentUser = new TestCurrentUserContext { UserId = "deleter-user" };

        using var db = NewDb(dbName, tenant, currentUser);
        var branch = new Branch { Name = "Archived" };
        db.Branches.Add(branch);
        db.SaveChanges();

        db.Branches.Remove(branch);
        db.SaveChanges();

        Assert.Empty(db.Branches.ToList());

        var deleted = Assert.Single(db.Branches.IgnoreQueryFilters().ToList());
        Assert.True(deleted.IsDeleted);
        Assert.Equal("deleter-user", deleted.DeletedBy);
        Assert.NotNull(deleted.DeletedOn);
    }

    private sealed class TestCurrentUserContext : ICurrentUserContext
    {
        public string? UserId { get; set; }
    }
}
