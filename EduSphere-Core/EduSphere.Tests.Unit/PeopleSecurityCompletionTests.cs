using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class PeopleSecurityCompletionTests
{
    private static (TenantDbContext Db, Guid TenantId) CreateDb()
    {
        var tenantId=Guid.NewGuid();var tenant=new TenantContext();tenant.SetTenant(tenantId,"people-security-test");
        return(new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,tenant,new TestUser()),tenantId);
    }

    [Fact]
    public async Task ParentCanLinkToStudentsAcrossBranches()
    {
        var (db,tenantId)=CreateDb();await using var scope=db;
        var parent=new ApplicationUser{TenantId=tenantId,UserName="parent@test.local",Email="parent@test.local",FirstName="Priya",LastName="Shah",UserType=UserType.Parent,IsActive=true};
        var firstBranch=new Branch{Name="City School",Code="CITY"};var secondBranch=new Branch{Name="North College",Code="NORTH"};db.AddRange(parent,firstBranch,secondBranch);await db.SaveChangesAsync();
        var first=new StudentProfile{BranchId=firstBranch.Id,AdmissionNumber="C-1",FirstName="Aarav",LastName="Shah",AdmissionDate=new(2026,6,1)};var second=new StudentProfile{BranchId=secondBranch.Id,AdmissionNumber="N-1",FirstName="Diya",LastName="Shah",AdmissionDate=new(2026,6,1)};db.AddRange(first,second);await db.SaveChangesAsync();
        db.StudentGuardians.AddRange(new StudentGuardian{StudentProfileId=first.Id,ParentUserId=parent.Id,FullName=parent.FullName,HasPortalAccess=true},new StudentGuardian{StudentProfileId=second.Id,ParentUserId=parent.Id,FullName=parent.FullName,HasPortalAccess=true});await db.SaveChangesAsync();
        var linked=await db.StudentGuardians.Where(x=>x.ParentUserId==parent.Id&&x.HasPortalAccess).Select(x=>x.StudentProfile!.BranchId).Distinct().ToListAsync();
        Assert.Equal(2,linked.Count);
    }

    [Fact]
    public async Task PayrollEntryCalculatesNetAndEmailJobIsDurable()
    {
        var (db,_)=CreateDb();await using var scope=db;var branch=new Branch{Name="Main",Code="MAIN"};db.Add(branch);await db.SaveChangesAsync();
        var user=new ApplicationUser{TenantId=branch.TenantId,UserName="staff@test.local",Email="staff@test.local",FirstName="Ravi",LastName="Kumar",UserType=UserType.Staff,IsActive=true};db.Add(user);await db.SaveChangesAsync();
        var staff=new StaffProfile{UserId=user.Id,BranchId=branch.Id,EmployeeNumber="S-1",FirstName="Ravi",LastName="Kumar",JoiningDate=new(2025,1,1)};db.Add(staff);await db.SaveChangesAsync();
        var run=new PayrollRun{BranchId=branch.Id,Year=2026,Month=9,Status=PayrollRunStatus.Calculated,Entries=[new PayrollEntry{StaffProfileId=staff.Id,BasicSalary=50000,Allowances=5000,Deductions=2500,NetPay=52500}]};
        var message=new NotificationMessage{BranchId=branch.Id,Channel=CommunicationChannel.Email,Subject="Activate",Body="Body",Status=NotificationStatus.Queued};db.AddRange(run,message);await db.SaveChangesAsync();
        db.AccountEmailJobs.Add(new AccountEmailJob{NotificationMessageId=message.Id,UserId=user.Id,JobType="AccountInvitation",Status=EmailJobStatus.Queued,AvailableOn=DateTime.UtcNow});db.SecurityEvents.Add(new SecurityEvent{UserId=user.Id,Email=user.Email,EventType=SecurityEventType.LoginFailed,Succeeded=false});await db.SaveChangesAsync();
        Assert.Equal(52500,(await db.PayrollEntries.SingleAsync()).NetPay);Assert.Equal(EmailJobStatus.Queued,(await db.AccountEmailJobs.SingleAsync()).Status);Assert.False((await db.SecurityEvents.SingleAsync()).Succeeded);
    }
    private sealed class TestUser:ICurrentUserContext{public string? UserId=>"people-security-tests";}
}
