using System.IO.Compression;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public class AdmissionsRefinementTests
{
    private static (TenantDbContext Db, Guid TenantId) CreateDb()
    {
        var tenantId = Guid.NewGuid(); var tenant = new TenantContext(); tenant.SetTenant(tenantId, "admissions-refinements");
        return (new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant), tenantId);
    }

    [Fact]
    public void ConfigurableNumberFormatsRenderYearAndSequence() => Assert.Matches(@"^APP-\d{4}-0042$", AdmissionNumberService.Format("APP-{YYYY}-{SEQ:0000}", 42));

    [Fact]
    public async Task AutomationExpiresOfferAndPromotesNextWaitlistedApplicant()
    {
        var (db, tenantId) = CreateDb(); await using var cleanup = db;
        var branch = new Branch { Name = "Main", Code = "MAIN" }; var course = new Course { Name = "Science", Code = "SCI" }; var year = new AcademicYear { Name = "2026-27", StartDate = new(2026,6,1), EndDate = new(2027,5,31) }; db.AddRange(branch, course, year); await db.SaveChangesAsync();
        var batch = new Batch { Name = "Science 2026", CourseId = course.Id, AcademicYearId = year.Id, Capacity = 1 }; db.Batches.Add(batch); await db.SaveChangesAsync();
        db.BranchAdmissionSettings.Add(new BranchAdmissionSetting { BranchId = branch.Id, AutoPromoteWaitlist = true, OfferValidityDays = 5 });
        db.AdmissionApplications.AddRange(
            new AdmissionApplication { BranchId = branch.Id, CourseId = course.Id, AcademicYearId = year.Id, BatchId = batch.Id, ApplicationNumber = "APP-1", ApplicantFirstName = "A", ApplicantLastName = "One", DateOfBirth = new(2010,1,1), Status = AdmissionApplicationStatus.Accepted, OfferExpiresOn = DateTime.UtcNow.AddMinutes(-1) },
            new AdmissionApplication { BranchId = branch.Id, CourseId = course.Id, AcademicYearId = year.Id, BatchId = batch.Id, ApplicationNumber = "APP-2", ApplicantFirstName = "B", ApplicantLastName = "Two", DateOfBirth = new(2010,1,1), Status = AdmissionApplicationStatus.Waitlisted, WaitlistPosition = 1 });
        await db.SaveChangesAsync(); var service = new AdmissionAutomationService(db); Assert.Equal(2, await service.ProcessAsync());
        Assert.Contains(await db.AdmissionApplications.ToListAsync(), x => x.ApplicationNumber == "APP-1" && x.Status == AdmissionApplicationStatus.OfferExpired);
        Assert.Contains(await db.AdmissionApplications.ToListAsync(), x => x.ApplicationNumber == "APP-2" && x.Status == AdmissionApplicationStatus.Accepted);
        Assert.Equal(2, await db.AdmissionAutomationLogs.CountAsync());
    }

    [Fact]
    public void ExcelAndPdfExportsProduceRecognizableDocuments()
    {
        var app = new AdmissionApplication { ApplicationNumber = "APP-2026-0001", ApplicantFirstName = "Meera", ApplicantLastName = "Nair", DateOfBirth = new(2010,1,1), ApplicationSource = "Website", Status = AdmissionApplicationStatus.Submitted };
        var excel = AdmissionExportBuilder.Excel(new[] { app }); using var zip = new ZipArchive(new MemoryStream(excel)); Assert.NotNull(zip.GetEntry("xl/worksheets/sheet1.xml"));
        var pdf = AdmissionExportBuilder.Pdf(new[] { app }); Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(pdf, 0, 4));
    }
}
