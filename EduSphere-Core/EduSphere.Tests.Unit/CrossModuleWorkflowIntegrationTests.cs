using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public class CrossModuleWorkflowIntegrationTests
{
    private static readonly Guid TenantId = Guid.Parse("5c2ddcdd-2185-4d3d-80f3-64ac0ab8d813");

    private static TenantDbContext NewDb(string name)
    {
        var tenant = new TenantContext();
        tenant.SetTenant(TenantId, "workflow-test");
        return new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(name).Options, tenant);
    }

    [Fact]
    public async Task AdmissionToEnrollmentAndFinanceChainPersistsAsOneTenantWorkflow()
    {
        await using var db = NewDb(Guid.NewGuid().ToString());
        var branch = new Branch { Name = "North Campus", Code = "NORTH" };
        var year = new AcademicYear { Name = "2026-27", StartDate = new(2026, 6, 1), EndDate = new(2027, 5, 31) };
        var course = new Course { Code = "BSC-CS", Name = "B.Sc. Computer Science" };
        db.AddRange(branch, year, course);
        await db.SaveChangesAsync();
        var batch = new Batch { Name = "B.Sc. CS 2026", CourseId = course.Id, AcademicYearId = year.Id, Capacity = 60 };
        db.Batches.Add(batch);
        await db.SaveChangesAsync();

        var application = new AdmissionApplication
        {
            BranchId = branch.Id, AcademicYearId = year.Id, CourseId = course.Id,
            ApplicationNumber = "ADM-WF-001", ApplicantFirstName = "Aarav", ApplicantLastName = "Sharma",
            DateOfBirth = new(2008, 4, 12), Status = AdmissionApplicationStatus.FeePending
        };
        db.AdmissionApplications.Add(application);
        await db.SaveChangesAsync();

        var invoice = new FeeInvoice
        {
            BranchId = branch.Id, AdmissionApplicationId = application.Id, InvoiceNumber = "INV-WF-001",
            InvoiceDate = new(2026, 9, 18), DueDate = new(2026, 9, 25), TotalAmount = 25000, PaidAmount = 25000,
            Status = InvoiceStatus.Paid
        };
        var student = new StudentProfile
        {
            BranchId = branch.Id, AdmissionNumber = "STU-WF-001", FirstName = "Aarav", LastName = "Sharma",
            DateOfBirth = application.DateOfBirth, AdmissionDate = new(2026, 9, 18), Status = StudentStatus.Active
        };
        db.AddRange(invoice, student);
        await db.SaveChangesAsync();

        application.Status = AdmissionApplicationStatus.Enrolled;
        application.EnrolledStudentProfileId = student.Id;
        db.Enrollments.Add(new Enrollment
        {
            AdmissionApplicationId = application.Id, StudentProfileId = student.Id, BranchId = branch.Id,
            AcademicYearId = year.Id, CourseId = course.Id, BatchId = batch.Id, EnrollmentNumber = "ENR-WF-001",
            EnrollmentDate = new(2026, 9, 18), Status = EnrollmentStatus.Active
        });
        await db.SaveChangesAsync();

        var persisted = await db.AdmissionApplications.Include(a => a.FeeInvoices).SingleAsync();
        Assert.Equal(AdmissionApplicationStatus.Enrolled, persisted.Status);
        Assert.Equal(student.Id, persisted.EnrolledStudentProfileId);
        Assert.Equal(InvoiceStatus.Paid, Assert.Single(persisted.FeeInvoices).Status);
        Assert.Single(await db.Enrollments.ToListAsync());
    }

    [Fact]
    public async Task PublishedResultRetainsApprovalBatchAndRankingAudit()
    {
        await using var db = NewDb(Guid.NewGuid().ToString());
        var branch = new Branch { Name = "Main Campus", Code = "MAIN" };
        var year = new AcademicYear { Name = "2026-27", StartDate = new(2026, 6, 1), EndDate = new(2027, 5, 31) };
        db.AddRange(branch, year);
        await db.SaveChangesAsync();
        var exam = new Exam { BranchId = branch.Id, AcademicYearId = year.Id, Name = "Mid Term", Status = ExamStatus.ResultsPublished };
        var student = new StudentProfile { BranchId = branch.Id, AdmissionNumber = "STU-101", FirstName = "Meera", LastName = "Nair", DateOfBirth = new(2010, 8, 2) };
        db.AddRange(exam, student);
        await db.SaveChangesAsync();
        db.ResultRankingRules.Add(new ResultRankingRule { BranchId = branch.Id, ExamId = exam.Id, Method = RankingMethod.Competition });
        db.ResultPublicationBatches.Add(new ResultPublicationBatch { BranchId = branch.Id, ExamId = exam.Id, Status = ApprovalStatus.Published, ResultCount = 1, PublishedOn = DateTime.UtcNow });
        db.Results.Add(new Result { BranchId = branch.Id, ExamId = exam.Id, StudentProfileId = student.Id, AcademicYearId = year.Id, TotalMarks = 500, MarksObtained = 445, Percentage = 89, IsPassed = true, Rank = 1, Percentile = 100, Status = ResultStatus.Published, PublishedOn = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var result = await db.Results.SingleAsync();
        Assert.Equal(ResultStatus.Published, result.Status);
        Assert.Equal(1, result.Rank);
        Assert.Equal(100, result.Percentile);
        Assert.Equal(ApprovalStatus.Published, (await db.ResultPublicationBatches.SingleAsync()).Status);
    }

    [Fact]
    public async Task AttendanceRiskCanBeTracedToQueuedCommunication()
    {
        await using var db = NewDb(Guid.NewGuid().ToString());
        var branch = new Branch { Name = "City School", Code = "CITY" };
        db.Branches.Add(branch);
        await db.SaveChangesAsync();
        var student = new StudentProfile { BranchId = branch.Id, AdmissionNumber = "CITY-009", FirstName = "Kabir", LastName = "Singh", DateOfBirth = new(2011, 2, 10), Email = "guardian@example.test" };
        db.StudentProfiles.Add(student);
        await db.SaveChangesAsync();
        var alert = new AttendanceAlert { StudentProfileId = student.Id, PeriodStart = new(2026, 9, 1), PeriodEnd = new(2026, 9, 18), AttendancePercentage = 68, ThresholdPercentage = 75, Message = "Attendance is below the required threshold." };
        var message = new NotificationMessage { BranchId = branch.Id, Channel = CommunicationChannel.Email, Subject = "Attendance alert", Body = alert.Message, Status = NotificationStatus.Queued };
        db.AddRange(alert, message);
        await db.SaveChangesAsync();
        db.NotificationRecipients.Add(new NotificationRecipient { BranchId = branch.Id, NotificationMessageId = message.Id, DisplayName = $"{student.FirstName} {student.LastName}", DestinationAddress = student.Email!, Status = NotificationStatus.Queued });
        await db.SaveChangesAsync();

        Assert.Equal(student.Id, (await db.AttendanceAlerts.SingleAsync()).StudentProfileId);
        Assert.Equal("guardian@example.test", (await db.NotificationRecipients.SingleAsync()).DestinationAddress);
        Assert.Equal(NotificationStatus.Queued, (await db.NotificationMessages.SingleAsync()).Status);
    }
}
