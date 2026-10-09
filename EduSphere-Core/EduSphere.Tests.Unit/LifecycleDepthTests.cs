using System.Text;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public class LifecycleDepthTests
{
    [Fact]
    public async Task BuiltInScannerRejectsEicarAndHashesCleanDocuments()
    {
        var scanner = new BuiltInDocumentVirusScanner();
        await using var clean = new MemoryStream(Encoding.UTF8.GetBytes("student transfer certificate"));
        var cleanResult = await scanner.ScanAsync(clean);
        Assert.Equal(DocumentScanStatus.Clean, cleanResult.Status);
        Assert.Equal(64, cleanResult.Sha256.Length);

        const string eicar = "X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
        await using var infected = new MemoryStream(Encoding.ASCII.GetBytes(eicar));
        var infectedResult = await scanner.ScanAsync(infected);
        Assert.Equal(DocumentScanStatus.Infected, infectedResult.Status);
    }

    [Fact]
    public async Task MedicalTimelineTranscriptAndSignatureRecordsRemainTenantScoped()
    {
        var tenantId = Guid.NewGuid();
        var tenant = new TenantContext(); tenant.SetTenant(tenantId, "lifecycle-depth");
        await using var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant);
        var student = new StudentProfile { BranchId = Guid.NewGuid(), AdmissionNumber = "ADM-1001", FirstName = "Ishita", LastName = "Rao", DateOfBirth = new(2010, 1, 1) };
        db.StudentProfiles.Add(student); await db.SaveChangesAsync();
        var medical = new StudentMedicalProfile { StudentProfileId = student.Id, BloodGroup = "O+", EmergencyCareConsent = true };
        medical.Conditions.Add(new StudentMedicalCondition { Name = "Asthma", Severity = "Mild", CareInstructions = "Keep inhaler available" });
        var transcript = new StudentTranscript { StudentProfileId = student.Id, TranscriptNumber = "TRN-2026-0001", IssuedOn = new(2026, 9, 30), AggregatePercentage = 88.5m, IsFinal = true };
        transcript.Lines.Add(new StudentTranscriptLine { AssessmentName = "Final Examination", MaximumMarks = 500, MarksObtained = 442.5m, Percentage = 88.5m, Grade = "A", IsPassed = true });
        db.AddRange(medical, transcript, new ProfileStatusHistory { OwnerType = ProfileStatusOwnerType.Student, OwnerId = student.Id, PreviousStatus = "Active", Status = "Graduated", EffectiveOn = new(2026, 9, 30), Source = "Graduated" });
        await db.SaveChangesAsync();

        Assert.Equal("Asthma", (await db.StudentMedicalProfiles.Include(x => x.Conditions).SingleAsync()).Conditions.Single().Name);
        Assert.True((await db.StudentTranscripts.Include(x => x.Lines).SingleAsync()).IsFinal);
        Assert.Equal("Graduated", (await db.ProfileStatusHistories.SingleAsync()).Status);
        Assert.All(db.ChangeTracker.Entries().Where(x => x.Entity is EduSphere.Domain.Common.TenantEntityBase), x => Assert.Equal(tenantId, ((EduSphere.Domain.Common.TenantEntityBase)x.Entity).TenantId));
    }
}
