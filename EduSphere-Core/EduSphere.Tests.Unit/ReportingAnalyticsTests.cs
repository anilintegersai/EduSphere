using System.IO.Compression;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class ReportingAnalyticsTests
{
    private sealed class User : ICurrentUserContext { public string? UserId => "reporting-tests"; }
    private static (TenantDbContext Db, ReportingService Service, Guid TenantId) Create()
    {
        var tenantId = Guid.NewGuid(); var tenant = new TenantContext(); tenant.SetTenant(tenantId, "reporting-tests");
        var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new User());
        return (db, new ReportingService(db), tenantId);
    }

    [Fact]
    public async Task CatalogSeedsCoreAnalyticalReportsOnce()
    {
        var x = Create(); await using var db = x.Db;
        await x.Service.EnsureCatalogAsync(); await x.Service.EnsureCatalogAsync();
        Assert.Equal(6, await db.ReportDefinitions.CountAsync());
        Assert.Contains(await db.ReportDefinitions.ToListAsync(), x => x.DataSource == ReportDataSource.AttendancePerformance);
    }

    [Fact]
    public async Task CustomReportProjectsOnlyAllowlistedFieldsAndExportsConsistently()
    {
        var x = Create(); await using var db = x.Db; var branchId = Guid.NewGuid();
        db.Branches.Add(new Branch { Id = branchId, TenantId = x.TenantId, Name = "Central", Code = "CTR" });
        db.StudentProfiles.Add(new StudentProfile { TenantId = x.TenantId, BranchId = branchId, AdmissionNumber = "ST-001", FirstName = "Asha", LastName = "Rao", DateOfBirth = new(2010, 1, 1) });
        await db.SaveChangesAsync();
        var definition = new ReportDefinition { TenantId = x.TenantId, BranchId = branchId, Code = "student-list", Name = "Student list", DataSource = ReportDataSource.Students, SelectedFieldsJson = "[\"AdmissionNumber\",\"Student\",\"UnsafeSql\"]" };
        var data = await x.Service.ExecuteAsync(definition);
        Assert.Equal(["AdmissionNumber", "Student"], data.Columns);
        Assert.Single(data.Rows);
        var excel = x.Service.Export(data, ReportOutputFormat.Excel); using var zip = new ZipArchive(new MemoryStream(excel.Content)); Assert.NotNull(zip.GetEntry("xl/worksheets/sheet1.xml"));
        var pdf = x.Service.Export(data, ReportOutputFormat.Pdf); Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(pdf.Content, 0, 4));
    }
}
