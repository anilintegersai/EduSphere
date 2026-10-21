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

public sealed class LibraryDepthTests
{
    private sealed class User : ICurrentUserContext { public string? UserId => "library-tests"; }
    private sealed class Clients : IHttpClientFactory { public HttpClient CreateClient(string name) => new(); }
    private static (TenantDbContext Db, LibraryDepthService Service, Guid TenantId, Guid BranchId) Create()
    {
        var tenantId = Guid.NewGuid(); var branchId = Guid.NewGuid(); var tenant = new TenantContext(); tenant.SetTenant(tenantId, "library-tests");
        var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new User());
        var finance = new FinanceAccountingService(db, new Clients(), new EphemeralDataProtectionProvider());
        return (db, new LibraryDepthService(db, finance), tenantId, branchId);
    }

    [Fact]
    public async Task IssueRejectsMemberAtConfiguredLimit()
    {
        var x = Create(); await using var db = x.Db;
        var member = new LibraryMember { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, MemberNumber = "S-1", MemberType = LibraryMemberType.Student, MaxBooksAllowed = 5 };
        var book = new LibraryBook { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, Title = "Algorithms", Authors = "A. Author" };
        var oldCopy = new LibraryBookCopy { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, LibraryBookId = book.Id, AccessionNumber = "A-1", Status = BookCopyStatus.Issued };
        var newCopy = new LibraryBookCopy { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, LibraryBookId = book.Id, AccessionNumber = "A-2", Status = BookCopyStatus.Available };
        db.AddRange(member, book, oldCopy, newCopy, new LibraryCirculationPolicy { TenantId = x.TenantId, BranchId = x.BranchId, MemberType = LibraryMemberType.Student, MaxIssues = 1 }, new LibraryBookIssue { TenantId = x.TenantId, BranchId = x.BranchId, LibraryBookCopyId = oldCopy.Id, LibraryMemberId = member.Id, Status = LibraryIssueStatus.Issued }); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => x.Service.IssueAsync(newCopy.Id, member.Id, new(2026, 10, 22)));
    }

    [Fact]
    public async Task ReservationQueueAssignsAvailableCopyToFirstMember()
    {
        var x = Create(); await using var db = x.Db;
        var first = new LibraryMember { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, MemberNumber = "S-1", MemberType = LibraryMemberType.Student };
        var second = new LibraryMember { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, MemberNumber = "S-2", MemberType = LibraryMemberType.Student };
        var book = new LibraryBook { Id = Guid.NewGuid(), TenantId = x.TenantId, BranchId = x.BranchId, Title = "Databases", Authors = "D. Author" };
        db.AddRange(first, second, book); await db.SaveChangesAsync();
        var r1 = await x.Service.ReserveAsync(book.Id, first.Id); var r2 = await x.Service.ReserveAsync(book.Id, second.Id);
        Assert.Equal(1, r1.QueuePosition); Assert.Equal(2, r2.QueuePosition); Assert.Equal(LibraryReservationStatus.Requested, r1.Status);
    }
}
