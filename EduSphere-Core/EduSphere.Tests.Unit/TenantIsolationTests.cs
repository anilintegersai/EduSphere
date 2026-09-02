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
    private static TenantDbContext NewDb(string dbName, TenantContext tenantContext)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TenantDbContext(options, tenantContext);
    }

    [Fact]
    public void GlobalQueryFilter_IsolatesReadsByTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        // Seed two tenants' branches (writes are not tenant-filtered).
        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - North", TenantId = 1 });
            db.Branches.Add(new Branch { Name = "Tenant1 - South", TenantId = 1 });
            db.Branches.Add(new Branch { Name = "Tenant2 - Main", TenantId = 2 });
            db.SaveChanges();
        }

        // Read as tenant 1.
        var t1 = new TenantContext();
        t1.SetTenant(1, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            var branches = db.Branches.ToList();
            Assert.Equal(2, branches.Count);
            Assert.All(branches, b => Assert.Equal(1, b.TenantId));
        }

        // Read as tenant 2.
        var t2 = new TenantContext();
        t2.SetTenant(2, "tenant2");
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
        ctx.SetTenant(7, "tenant7");

        using var db = NewDb(dbName, ctx);
        var branch = new Branch { Name = "Unstamped" }; // TenantId left at 0
        db.Branches.Add(branch);
        db.SaveChanges();

        Assert.Equal(7, branch.TenantId);
    }

    [Fact]
    public void NoResolvedTenant_ReturnsNoTenantOwnedRows()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - Only", TenantId = 1 });
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
            db.Courses.Add(new Course { Code = "A", Name = "Course A", TenantId = 1 });
            db.Courses.Add(new Course { Code = "B", Name = "Course B", TenantId = 2 });
            db.SaveChanges();
        }

        var t1 = new TenantContext();
        t1.SetTenant(1, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            // Read: filtered to tenant 1 only.
            var course = Assert.Single(db.Courses.ToList());
            Assert.Equal(1, course.TenantId);

            // Write: TenantId is stamped from the current tenant when left unset.
            var created = new Course { Code = "C", Name = "Course C" };
            db.Courses.Add(created);
            db.SaveChanges();
            Assert.Equal(1, created.TenantId);
        }
    }

    [Fact]
    public void CrossTenantData_IsNotVisibleToAnotherTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Secret", TenantId = 100 });
            db.SaveChanges();
        }

        var other = new TenantContext();
        other.SetTenant(200, "tenant200");
        using (var db = NewDb(dbName, other))
        {
            Assert.Empty(db.Branches.ToList());
        }
    }
}
