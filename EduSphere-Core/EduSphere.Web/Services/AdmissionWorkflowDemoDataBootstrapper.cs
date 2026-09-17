using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class AdmissionWorkflowDemoDataBootstrapper
{
    private static readonly (string First, string Last, Gender Gender)[] Applicants =
    [
        ("Aanya", "Mehta", Gender.Female), ("Vivaan", "Rao", Gender.Male),
        ("Diya", "Kapoor", Gender.Female), ("Arjun", "Menon", Gender.Male),
        ("Sara", "Khan", Gender.Female), ("Ishaan", "Patel", Gender.Male),
        ("Naina", "Joshi", Gender.Female), ("Aditya", "Bose", Gender.Male)
    ];

    private static readonly AdmissionApplicationStatus[] Statuses =
    [
        AdmissionApplicationStatus.Submitted,
        AdmissionApplicationStatus.UnderReview,
        AdmissionApplicationStatus.InterviewScheduled,
        AdmissionApplicationStatus.Accepted,
        AdmissionApplicationStatus.Waitlisted,
        AdmissionApplicationStatus.Rejected,
        AdmissionApplicationStatus.FeePending,
        AdmissionApplicationStatus.Enrolled
    ];

    public static async Task ApplyAdmissionWorkflowDemoDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue("Admissions:SeedWorkflowDemoData", false)) return;

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdmissionWorkflowDemoDataBootstrapper");
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            var templates = await db.AdmissionFormTemplates.IgnoreQueryFilters()
                .Where(t => !t.IsDeleted && t.IsActive && t.BranchId.HasValue && t.CourseId.HasValue)
                .OrderBy(t => t.TenantId).ThenBy(t => t.BranchId).ThenBy(t => t.Name)
                .ToListAsync();
            var selectedTemplates = templates.GroupBy(t => new { t.TenantId, t.BranchId }).SelectMany(g => g.Take(2)).ToList();
            var created = 0;

            foreach (var template in selectedTemplates)
            {
                var branch = await db.Branches.IgnoreQueryFilters().FirstAsync(b => b.Id == template.BranchId);
                var course = await db.Courses.IgnoreQueryFilters().FirstAsync(c => c.Id == template.CourseId);
                var existingStudent = await db.StudentProfiles.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(s => s.TenantId == template.TenantId && s.BranchId == branch.Id && !s.IsDeleted);

                for (var index = 0; index < Statuses.Length; index++)
                {
                    var status = Statuses[index];
                    if (status == AdmissionApplicationStatus.Enrolled && existingStudent is null) continue;
                    var number = $"DEMO-{Compact(branch.Code ?? branch.Name, 8)}-{Compact(course.Code ?? course.Name, 8)}-{index + 1:00}";
                    if (await db.AdmissionApplications.IgnoreQueryFilters().AnyAsync(a => a.TenantId == template.TenantId && a.ApplicationNumber == number))
                        continue;

                    var person = Applicants[index];
                    var application = new AdmissionApplication
                    {
                        TenantId = template.TenantId,
                        AdmissionFormTemplateId = template.Id,
                        BranchId = branch.Id,
                        AcademicYearId = template.AcademicYearId,
                        CourseId = course.Id,
                        ApplicationNumber = number,
                        ApplicantFirstName = person.First,
                        ApplicantLastName = person.Last,
                        DateOfBirth = new DateOnly(2007 + index % 4, 2 + index, 4 + index),
                        Gender = person.Gender,
                        Email = $"{person.First.ToLowerInvariant()}.{person.Last.ToLowerInvariant()}.{index + 1}@example.test",
                        PhoneNumber = $"98{template.Id.ToString("N")[..6]}{index + 10}",
                        GuardianName = index % 2 == 0 ? $"Raj {person.Last}" : $"Priya {person.Last}",
                        GuardianPhone = $"97{template.Id.ToString("N")[6..12]}{index + 10}",
                        Address = $"{24 + index}, Knowledge Park, {branch.City ?? "Campus City"}",
                        AppliedOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-(index + 2) * 3)),
                        Status = status,
                        ReviewNotes = StatusNote(status),
                        EnrolledStudentProfileId = status == AdmissionApplicationStatus.Enrolled ? existingStudent!.Id : null,
                        FormResponseJson = JsonSerializer.Serialize(new Dictionary<string, object?>
                        {
                            ["previousInstitution"] = index % 2 == 0 ? "Green Valley Public School" : "National Academy",
                            ["lastQualification"] = course.Type == CourseType.School ? "Previous grade completed" : "Grade 12",
                            ["boardOrUniversity"] = index % 2 == 0 ? "CBSE" : "State Board",
                            ["lastPercentage"] = 72 + index * 2.4m,
                            ["preferredMode"] = index % 3 == 0 ? "Transport required" : index % 3 == 1 ? "Day scholar" : "Hostel required",
                            ["guardianOccupation"] = index % 2 == 0 ? "Engineer" : "Teacher",
                            ["siblingStudyingHere"] = index % 4 == 0,
                            ["achievements"] = index % 2 == 0 ? "District-level quiz finalist" : "School sports team member"
                        })
                    };
                    db.AdmissionApplications.Add(application);
                    await db.SaveChangesAsync();

                    if (status != AdmissionApplicationStatus.Submitted)
                    {
                        db.AdmissionReviews.Add(new AdmissionReview
                        {
                            TenantId = template.TenantId,
                            AdmissionApplicationId = application.Id,
                            FromStatus = AdmissionApplicationStatus.Submitted,
                            ToStatus = status,
                            ReviewedOn = DateTime.UtcNow.AddDays(-index),
                            Notes = StatusNote(status)
                        });
                    }
                    if (status is AdmissionApplicationStatus.InterviewScheduled or AdmissionApplicationStatus.Accepted or AdmissionApplicationStatus.Waitlisted)
                    {
                        db.AdmissionInterviews.Add(new AdmissionInterview
                        {
                            TenantId = template.TenantId,
                            AdmissionApplicationId = application.Id,
                            BranchId = branch.Id,
                            StartsOn = DateTime.UtcNow.Date.AddDays(status == AdmissionApplicationStatus.InterviewScheduled ? 3 : -3).AddHours(10 + index),
                            EndsOn = DateTime.UtcNow.Date.AddDays(status == AdmissionApplicationStatus.InterviewScheduled ? 3 : -3).AddHours(11 + index),
                            Status = status == AdmissionApplicationStatus.InterviewScheduled ? AdmissionInterviewStatus.Scheduled : AdmissionInterviewStatus.Completed,
                            Location = status == AdmissionApplicationStatus.InterviewScheduled ? "Admissions meeting room" : "Academic block - Room 102",
                            OutcomeNotes = status == AdmissionApplicationStatus.Accepted ? "Recommended by the interview panel." : null
                        });
                    }
                    if (status == AdmissionApplicationStatus.FeePending)
                    {
                        db.FeeInvoices.Add(new FeeInvoice
                        {
                            TenantId = template.TenantId,
                            BranchId = branch.Id,
                            AdmissionApplicationId = application.Id,
                            InvoiceNumber = $"ADM-FEE-{number}",
                            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                            Status = InvoiceStatus.Issued,
                            SubTotal = 15000,
                            TotalAmount = 15000,
                            Notes = "Admission confirmation fee generated from demo workflow data."
                        });
                    }
                    await db.SaveChangesAsync();
                    created++;
                }
            }
            logger.LogInformation("Admission workflow demo bootstrap created {Count} application(s).", created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Admission workflow demo-data bootstrap failed.");
        }
    }

    private static string StatusNote(AdmissionApplicationStatus status) => status switch
    {
        AdmissionApplicationStatus.UnderReview => "Academic records are under initial review.",
        AdmissionApplicationStatus.InterviewScheduled => "Applicant shortlisted for the next available interview slot.",
        AdmissionApplicationStatus.Accepted => "Academic review and interview criteria satisfied.",
        AdmissionApplicationStatus.Waitlisted => "Eligible applicant retained on the capacity waitlist.",
        AdmissionApplicationStatus.Rejected => "Current intake criteria were not met; applicant may reapply next term.",
        AdmissionApplicationStatus.FeePending => "Offer accepted; confirmation fee is pending.",
        AdmissionApplicationStatus.Enrolled => "Admission completed and linked to an active student profile.",
        _ => "Application received through the public admission portal."
    };

    private static string Compact(string value, int maxLength)
    {
        var compact = new string(value.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        return compact.Length <= maxLength ? compact : compact[..maxLength];
    }
}
