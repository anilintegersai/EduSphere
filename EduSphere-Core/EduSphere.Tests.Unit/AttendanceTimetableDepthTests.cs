using System.Text;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class AttendanceTimetableDepthTests
{
    private static TenantDbContext CreateDb()
    {
        var tenant = new TenantContext(); tenant.SetTenant(Guid.NewGuid(), "attendance-depth-test");
        return new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new TestUser());
    }

    [Fact]
    public async Task BulkMarkCreatesOneRecordForEveryActiveStudent()
    {
        await using var db = CreateDb();
        var branch = new Branch { Name = "Central", Code = "CTR" }; var section = new Section { Name = "A", Capacity = 30 };
        db.AddRange(branch, section); await db.SaveChangesAsync();
        var students = new[] { new StudentProfile { BranchId = branch.Id, SectionId = section.Id, AdmissionNumber = "ADM-1", FirstName = "Asha", LastName = "Rao", DateOfBirth = new(2012, 1, 1) }, new StudentProfile { BranchId = branch.Id, SectionId = section.Id, AdmissionNumber = "ADM-2", FirstName = "Rohan", LastName = "Sen", DateOfBirth = new(2012, 2, 1) } };
        var session = new AttendanceSession { BranchId = branch.Id, SectionId = section.Id, AttendanceDate = new(2026, 10, 16) };
        db.AddRange(students[0], students[1], session); await db.SaveChangesAsync();

        var count = await new AttendanceDepthService(db).BulkMarkAsync(session.Id, AttendanceStatus.Present, null, false, default);

        Assert.Equal(2, count); Assert.Equal(2, await db.AttendanceRecords.CountAsync()); Assert.All(await db.AttendanceRecords.ToListAsync(), x => Assert.Equal(AttendanceStatus.Present, x.Status));
    }

    [Fact]
    public async Task DeviceCsvMapsEmployeePunchIntoStaffAttendance()
    {
        await using var db = CreateDb();
        var branch = new Branch { Name = "Central", Code = "CTR" }; db.Add(branch); await db.SaveChangesAsync();
        var device = new AttendanceDevice { BranchId = branch.Id, Name = "Gate", DeviceIdentifier = "BIO-1" };
        var staff = new StaffProfile { UserId = Guid.NewGuid(), BranchId = branch.Id, EmployeeNumber = "EMP-001", FirstName = "Neha", LastName = "Iyer" };
        db.AddRange(device, staff); await db.SaveChangesAsync();
        var csv = "ExternalRecordId,PersonCode,PunchedOn,Direction\r\nP-1,EMP-001,2026-10-16 08:30:00,CheckIn\r\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var batch = await new AttendanceDepthService(db).ImportDeviceCsvAsync(device, stream, "punches.csv", null, default);

        Assert.Equal(AttendanceImportStatus.Completed, batch.Status); Assert.Equal(1, batch.ImportedRows);
        var attendance = await db.StaffAttendanceRecords.SingleAsync(); Assert.Equal(new DateOnly(2026, 10, 16), attendance.AttendanceDate); Assert.Equal(new TimeOnly(8, 30), attendance.CheckIn);
    }

    private sealed class TestUser : ICurrentUserContext { public string? UserId => "attendance-depth-tests"; }
}
