using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class HostelDepthTests
{
    private sealed class User : ICurrentUserContext { public string? UserId => "hostel-tests"; }
    private sealed class Clients : IHttpClientFactory { public HttpClient CreateClient(string name) => new(); }

    private static (TenantDbContext Db, HostelDepthService Service, Guid TenantId) Create()
    {
        var tenantId = Guid.NewGuid();
        var tenant = new TenantContext();
        tenant.SetTenant(tenantId, "hostel-tests");
        var db = new TenantDbContext(
            new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            tenant,
            new User());
        var finance = new FinanceAccountingService(db, new Clients(), new EphemeralDataProtectionProvider());
        return (db, new HostelDepthService(db, finance), tenantId);
    }

    [Fact]
    public async Task OccupancyForecastSubtractsExpectedDepartures()
    {
        var x = Create();
        await using var db = x.Db;
        var branchId = Guid.NewGuid();
        var block = new HostelBlock { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = branchId, Name = "North Hall", Code = "NH" };
        var room = new HostelRoom { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = branchId, HostelBlockId = block.Id, RoomNumber = "101", Capacity = 3 };
        db.AddRange(block, room,
            new HostelAllocation { TenantId = x.TenantId, BranchId = branchId, HostelRoomId = room.Id, StudentProfileId = Guid.NewGuid(), Status = HostelAllocationStatus.Active, ExpectedCheckoutOn = new(2026, 11, 1) },
            new HostelAllocation { TenantId = x.TenantId, BranchId = branchId, HostelRoomId = room.Id, StudentProfileId = Guid.NewGuid(), Status = HostelAllocationStatus.Active });
        await db.SaveChangesAsync();

        var result = Assert.Single(await x.Service.ForecastAsync(branchId, new(2026, 11, 30)));

        Assert.Equal(2, result.Occupied);
        Assert.Equal(1, result.ExpectedDepartures);
        Assert.Equal(1, result.ForecastOccupied);
    }

    [Fact]
    public async Task FinalClearanceRequiresEveryNoDueAreaAndReleasesBed()
    {
        var x = Create();
        await using var db = x.Db;
        var bed = new HostelBed { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = Guid.NewGuid(), HostelRoomId = Guid.NewGuid(), BedNumber = "A", Status = HostelBedStatus.Occupied };
        var allocation = new HostelAllocation { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = bed.BranchId, HostelRoomId = bed.HostelRoomId, HostelBedId = bed.Id, StudentProfileId = Guid.NewGuid(), Status = HostelAllocationStatus.Active };
        db.AddRange(bed, allocation);
        await db.SaveChangesAsync();
        var request = await x.Service.StartClearanceAsync(allocation.Id, new(2026, 10, 20), null, true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => x.Service.CompleteClearanceAsync(request.Id));
        foreach (var item in request.Items) item.IsCleared = true;
        await db.SaveChangesAsync();
        await x.Service.CompleteClearanceAsync(request.Id);

        Assert.Equal(HostelAllocationStatus.CheckedOut, allocation.Status);
        Assert.Equal(HostelBedStatus.Available, bed.Status);
    }
}
