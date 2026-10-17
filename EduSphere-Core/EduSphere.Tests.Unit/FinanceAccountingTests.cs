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
public sealed class FinanceAccountingTests
{
 private sealed class User:ICurrentUserContext{public string? UserId=>"finance-tests";}private sealed class Clients:IHttpClientFactory{public HttpClient CreateClient(string name)=>new();}
 private static (TenantDbContext Db,FinanceAccountingService Service) Create(){var tenant=new TenantContext();tenant.SetTenant(Guid.NewGuid(),"finance");var db=new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,tenant,new User());return(db,new FinanceAccountingService(db,new Clients(),new EphemeralDataProtectionProvider()));}
 [Fact]public async Task PostedJournalMustBalance(){var x=Create();await using var db=x.Db;var a=new ChartOfAccount{Code="1000",Name="Cash",Type=AccountType.Asset};var b=new ChartOfAccount{Code="4000",Name="Income",Type=AccountType.Income};db.AddRange(a,b);await db.SaveChangesAsync();await Assert.ThrowsAsync<InvalidOperationException>(()=>x.Service.PostJournalAsync(new JournalEntry{JournalNumber="JV-1",PostingDate=DateOnly.FromDateTime(DateTime.UtcNow),Narration="Invalid",Lines=[new(){ChartOfAccountId=a.Id,Debit=100},new(){ChartOfAccountId=b.Id,Credit=90}]}));}
 [Fact]public async Task ClosedPeriodRejectsPosting(){var x=Create();await using var db=x.Db;var a=new ChartOfAccount{Code="1000",Name="Cash",Type=AccountType.Asset};var b=new ChartOfAccount{Code="4000",Name="Income",Type=AccountType.Income};db.AddRange(a,b,new AccountingPeriod{Name="Closed",StartsOn=new(2026,4,1),EndsOn=new(2027,3,31),Status=AccountingPeriodStatus.Closed});await db.SaveChangesAsync();await Assert.ThrowsAsync<InvalidOperationException>(()=>x.Service.PostJournalAsync(new JournalEntry{JournalNumber="JV-2",PostingDate=new(2026,10,18),Narration="Locked",Lines=[new(){ChartOfAccountId=a.Id,Debit=100},new(){ChartOfAccountId=b.Id,Credit=100}]}));}
}
