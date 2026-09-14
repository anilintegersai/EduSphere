using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class AdmissionDefaultsBootstrapper
{
    public static async Task ApplyAdmissionDefaultsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdmissionDefaultsBootstrapper");

        if (!configuration.GetValue("Admissions:SeedDefaultForms", true))
            return;

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            var tenants = await dbContext.Tenants
                .IgnoreQueryFilters()
                .Where(t => !t.IsDeleted && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var created = 0;
            foreach (var tenant in tenants)
            {
                var branches = await dbContext.Branches
                    .IgnoreQueryFilters()
                    .Where(b => b.TenantId == tenant.Id && !b.IsDeleted && b.IsActive)
                    .OrderBy(b => b.Name)
                    .ToListAsync();
                var courses = await dbContext.Courses
                    .IgnoreQueryFilters()
                    .Where(c => c.TenantId == tenant.Id && !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
                var currentYear = await dbContext.AcademicYears
                    .IgnoreQueryFilters()
                    .Where(y => y.TenantId == tenant.Id && !y.IsDeleted)
                    .OrderByDescending(y => y.IsCurrent)
                    .ThenByDescending(y => y.StartDate)
                    .FirstOrDefaultAsync();

                foreach (var branch in branches)
                {
                    foreach (var course in courses)
                    {
                        var exists = await dbContext.AdmissionFormTemplates
                            .IgnoreQueryFilters()
                            .AnyAsync(f => f.TenantId == tenant.Id &&
                                           f.BranchId == branch.Id &&
                                           f.CourseId == course.Id &&
                                           f.IsDefault &&
                                           !f.IsDeleted);
                        if (exists)
                            continue;

                        var template = new AdmissionFormTemplate
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenant.Id,
                            BranchId = branch.Id,
                            CourseId = course.Id,
                            AcademicYearId = currentYear?.Id,
                            Name = BuildTemplateName(branch, course),
                            Description = $"Default admission intake for {course.Name} at {branch.Name}.",
                            Instructions = "Complete applicant, guardian, prior education, and service preference details. Upload all required documents before review.",
                            IsDefault = true,
                            IsActive = true,
                            EffectiveFrom = currentYear?.StartDate
                        };

                        dbContext.AdmissionFormTemplates.Add(template);
                        AddFields(dbContext, template);
                        AddDocumentRequirements(dbContext, template, course);
                        created++;
                    }
                }
            }

            if (created > 0)
                await dbContext.SaveChangesAsync();

            logger.LogInformation("Admission default form bootstrap created {Count} template(s).", created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Admission default form bootstrap failed.");
        }
    }

    private static void AddFields(TenantDbContext dbContext, AdmissionFormTemplate template)
    {
        var fields = new[]
        {
            Field(template, "previousInstitution", "Previous institution", AdmissionFormFieldType.Text, true, 10, "School, college, university, or coaching institute", 150),
            Field(template, "lastQualification", "Last qualification / grade", AdmissionFormFieldType.Text, true, 20, "Grade 10, Grade 12, B.Sc. year 1, foundation batch", 120),
            Field(template, "boardOrUniversity", "Board or university", AdmissionFormFieldType.Text, false, 30, "CBSE, ICSE, State Board, University name", 120),
            Field(template, "lastPercentage", "Last percentage / CGPA", AdmissionFormFieldType.Number, false, 40, "Example: 87.5", null),
            Field(template, "preferredMode", "Preferred student service", AdmissionFormFieldType.Select, true, 50, null, null, new[] { "Day scholar", "Transport required", "Hostel required", "Online support" }),
            Field(template, "guardianOccupation", "Guardian occupation", AdmissionFormFieldType.Text, false, 60, "Profession or business", 120),
            Field(template, "siblingStudyingHere", "Sibling already studying here", AdmissionFormFieldType.Checkbox, false, 70, null, null),
            Field(template, "medicalNotes", "Medical or accessibility notes", AdmissionFormFieldType.LongText, false, 80, "Allergies, medication, accessibility needs", 500),
            Field(template, "achievements", "Achievements and interests", AdmissionFormFieldType.LongText, false, 90, "Academic, sports, arts, olympiads, competitions", 500)
        };

        dbContext.AdmissionFormFields.AddRange(fields);
    }

    private static void AddDocumentRequirements(TenantDbContext dbContext, AdmissionFormTemplate template, Course course)
    {
        var needsBirthCertificate = course.Type is CourseType.School or CourseType.Other;
        var documents = new[]
        {
            Requirement(template, AdmissionDocumentType.IdentityProof, "Government ID / Aadhaar", true, 10, "Applicant or guardian identity proof."),
            Requirement(template, AdmissionDocumentType.Photograph, "Recent passport photograph", true, 20, "Clear color photograph."),
            Requirement(template, AdmissionDocumentType.Marksheet, "Latest marksheet or transcript", true, 30, "Most recent academic record."),
            Requirement(template, AdmissionDocumentType.TransferCertificate, "Transfer certificate", false, 40, "Required before final enrollment where applicable."),
            Requirement(template, AdmissionDocumentType.BirthCertificate, "Birth certificate", needsBirthCertificate, 50, "Required for school admissions and age verification."),
            Requirement(template, AdmissionDocumentType.MedicalCertificate, "Medical certificate", false, 60, "Required when medical notes need verification.")
        };

        dbContext.AdmissionDocumentRequirements.AddRange(documents);
    }

    private static AdmissionFormField Field(
        AdmissionFormTemplate template,
        string key,
        string label,
        AdmissionFormFieldType type,
        bool required,
        int order,
        string? placeholder,
        int? maxLength,
        IReadOnlyList<string>? options = null)
        => new()
        {
            TenantId = template.TenantId,
            AdmissionFormTemplateId = template.Id,
            FieldKey = key,
            Label = label,
            FieldType = type,
            IsRequired = required,
            SortOrder = order,
            Placeholder = placeholder,
            MaxLength = maxLength,
            OptionsJson = options is null ? null : JsonSerializer.Serialize(options),
            IsActive = true
        };

    private static AdmissionDocumentRequirement Requirement(
        AdmissionFormTemplate template,
        AdmissionDocumentType type,
        string displayName,
        bool required,
        int order,
        string notes)
        => new()
        {
            TenantId = template.TenantId,
            AdmissionFormTemplateId = template.Id,
            DocumentType = type,
            DisplayName = displayName,
            IsRequired = required,
            SortOrder = order,
            Notes = notes,
            IsActive = true
        };

    private static string BuildTemplateName(Branch branch, Course course)
    {
        var branchPart = Compact(branch.Code ?? branch.Name, 22);
        var coursePart = Compact(course.Code ?? course.Name, 22);
        return $"Default Admission - {branchPart} - {coursePart}";
    }

    private static string Compact(string value, int maxLength)
    {
        var compacted = new string(value.Where(ch => char.IsLetterOrDigit(ch) || ch == '-' || ch == '_').ToArray());
        if (string.IsNullOrWhiteSpace(compacted))
            compacted = "General";
        return compacted.Length <= maxLength ? compacted : compacted[..maxLength];
    }
}
