using System.IO.Compression;
using EduSphere.Domain.Common;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class ExaminationDepthTests
{
    private sealed class TestUser : ICurrentUserContext { public string? UserId => "exam-depth-tests"; }

    [Fact]
    public void MarksTemplateIsAReadableWorkbookWithExpectedColumns()
    {
        var tenant = new TenantContext();
        tenant.SetTenant(Guid.NewGuid(), "exam-depth-test");
        using var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new TestUser());

        var bytes = new ExaminationDepthService(db).BuildMarksTemplate();

        using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.NotNull(archive.GetEntry("xl/workbook.xml"));
        using var stream = archive.GetEntry("xl/worksheets/sheet1.xml")!.Open();
        using var reader = new StreamReader(stream);
        var sheet = reader.ReadToEnd();
        Assert.Contains("StudentIdentifier", sheet);
        Assert.Contains("MarksObtained", sheet);
        Assert.Contains("IsAbsent", sheet);
    }
}
