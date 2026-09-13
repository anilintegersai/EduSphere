using System.Security.Claims;
using System.Text.Json;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Controllers.V1.Operations;
using EduSphere.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AdmissionWorkflowService : IAdmissionWorkflowService
{
    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;
    private readonly IAdmissionDocumentStorageService _storage;

    public AdmissionWorkflowService(
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess,
        IAdmissionDocumentStorageService storage)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
        _storage = storage;
    }

    public async Task<IReadOnlyList<AdmissionFormTemplateDto>> ListFormTemplatesAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        Guid? courseId = null,
        CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(actor, branchId.Value))
            return Array.Empty<AdmissionFormTemplateDto>();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(actor);
        var query = _dbContext.AdmissionFormTemplates.AsNoTracking();

        if (_branchAccess.IsBranchAdminOnly(actor))
        {
            query = assignedBranchId.HasValue
                ? query.Where(f => f.BranchId == null || f.BranchId == assignedBranchId.Value)
                : query.Where(_ => false);
        }

        if (branchId.HasValue)
            query = query.Where(f => f.BranchId == null || f.BranchId == branchId.Value);
        if (courseId.HasValue)
            query = query.Where(f => f.CourseId == null || f.CourseId == courseId.Value);

        var templates = await query
            .OrderByDescending(f => f.IsDefault)
            .ThenBy(f => f.Name)
            .ToListAsync(cancellationToken);

        return templates.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AdmissionFormFieldDto>> ListFormFieldsAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default)
    {
        var fields = await _dbContext.AdmissionFormFields
            .AsNoTracking()
            .Where(f => f.AdmissionFormTemplateId == formTemplateId && f.IsActive)
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Label)
            .ToListAsync(cancellationToken);

        return fields.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AdmissionDocumentRequirementDto>> ListDocumentRequirementsAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default)
    {
        var requirements = await _dbContext.AdmissionDocumentRequirements
            .AsNoTracking()
            .Where(r => r.AdmissionFormTemplateId == formTemplateId && r.IsActive)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.DisplayName)
            .ToListAsync(cancellationToken);

        return requirements.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AdmissionDocumentDto>> ListDocumentsAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(admissionApplicationId, cancellationToken);
        if (application is null || !await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return Array.Empty<AdmissionDocumentDto>();

        var documents = await _dbContext.AdmissionDocuments
            .AsNoTracking()
            .Where(d => d.AdmissionApplicationId == admissionApplicationId)
            .OrderByDescending(d => d.UploadedOn)
            .ThenBy(d => d.DisplayName)
            .ToListAsync(cancellationToken);

        return documents.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AdmissionReviewDto>> ListReviewsAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(admissionApplicationId, cancellationToken);
        if (application is null || !await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return Array.Empty<AdmissionReviewDto>();

        var reviews = await _dbContext.AdmissionReviews
            .AsNoTracking()
            .Where(r => r.AdmissionApplicationId == admissionApplicationId)
            .OrderByDescending(r => r.ReviewedOn)
            .ToListAsync(cancellationToken);

        return reviews.Select(Map).ToList();
    }

    public async Task<OperationsWorkflowResult<AdmissionDocumentDto>> UploadDocumentAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        UploadAdmissionDocumentRequest request,
        Stream content,
        string fileName,
        string? contentType,
        long sizeBytes,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(admissionApplicationId, cancellationToken);
        if (application is null)
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("Admission application was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("You cannot upload documents for this branch.");
        if (sizeBytes <= 0)
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("Uploaded file is empty.");

        var stored = await _storage.SaveAsync(
            application.TenantId,
            application.Id,
            content,
            fileName,
            contentType,
            cancellationToken);

        var document = new AdmissionDocument
        {
            TenantId = application.TenantId,
            AdmissionApplicationId = application.Id,
            DocumentType = request.DocumentType,
            DisplayName = Normalize(request.DisplayName) ?? stored.FileName,
            FileName = stored.FileName,
            ContentType = stored.ContentType,
            StoragePath = stored.StoragePath,
            SizeBytes = stored.SizeBytes,
            UploadedOn = DateTime.UtcNow,
            UploadedByUserId = _branchAccess.GetUserId(actor),
            IsVerified = false,
            Notes = Normalize(request.Notes)
        };

        _dbContext.AdmissionDocuments.Add(document);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<AdmissionDocumentDto>.Success(Map(document), "Document uploaded.");
    }

    public async Task<OperationsWorkflowResult<AdmissionDocumentDto>> SetDocumentVerificationAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        Guid documentId,
        bool isVerified,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(admissionApplicationId, cancellationToken);
        if (application is null)
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("Admission application was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("You cannot review documents for this branch.");

        var document = await _dbContext.AdmissionDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.AdmissionApplicationId == admissionApplicationId, cancellationToken);
        if (document is null)
            return OperationsWorkflowResult<AdmissionDocumentDto>.Failure("Admission document was not found.");

        document.IsVerified = isVerified;
        document.VerifiedByUserId = isVerified ? _branchAccess.GetUserId(actor) : null;
        document.VerifiedOn = isVerified ? DateTime.UtcNow : null;
        document.Notes = Normalize(notes) ?? document.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<AdmissionDocumentDto>.Success(Map(document), isVerified ? "Document verified." : "Document marked pending.");
    }

    public async Task<OperationsWorkflowResult<AdmissionApplicationDto>> ReviewAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        ReviewAdmissionApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(admissionApplicationId, cancellationToken);
        if (application is null)
            return OperationsWorkflowResult<AdmissionApplicationDto>.Failure("Admission application was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return OperationsWorkflowResult<AdmissionApplicationDto>.Failure("You cannot review applications for this branch.");
        if (request.ToStatus == AdmissionApplicationStatus.Enrolled)
            return OperationsWorkflowResult<AdmissionApplicationDto>.Failure("Use the enroll action to move an accepted application to enrolled.");

        var fromStatus = application.Status;
        application.Status = request.ToStatus;
        application.ReviewNotes = Normalize(request.Notes);

        _dbContext.AdmissionReviews.Add(new AdmissionReview
        {
            TenantId = application.TenantId,
            AdmissionApplicationId = application.Id,
            ReviewedByUserId = _branchAccess.GetUserId(actor),
            FromStatus = fromStatus,
            ToStatus = request.ToStatus,
            ReviewedOn = DateTime.UtcNow,
            Notes = Normalize(request.Notes)
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return OperationsWorkflowResult<AdmissionApplicationDto>.Success(Map(application), "Application reviewed.");
    }

    public async Task<OperationsWorkflowResult<AdmissionEnrollmentResultDto>> ConvertToEnrollmentAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        ConvertAdmissionToEnrollmentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Select a tenant before enrolling an applicant.");

        var application = await _dbContext.AdmissionApplications
            .FirstOrDefaultAsync(a => a.Id == admissionApplicationId, cancellationToken);
        if (application is null)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Admission application was not found.");
        if (!await _branchAccess.CanAccessBranchAsync(actor, application.BranchId))
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("You cannot enroll applications for this branch.");
        if (application.Status == AdmissionApplicationStatus.Enrolled || application.EnrolledStudentProfileId.HasValue)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("This application has already been enrolled.");
        if (application.Status is not AdmissionApplicationStatus.Accepted and not AdmissionApplicationStatus.FeePending)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Only accepted or fee-pending applications can be enrolled.");

        var readinessErrors = await ValidateEnrollmentReadinessAsync(application, cancellationToken);
        if (readinessErrors.Count > 0)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure(readinessErrors.ToArray());

        var academicYearId = request.AcademicYearId ?? application.AcademicYearId;
        var courseId = request.CourseId ?? application.CourseId;
        var batchId = request.BatchId ?? application.BatchId;
        var sectionId = request.SectionId ?? application.SectionId;

        if (!academicYearId.HasValue)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Select an academic year before enrolling.");
        if (!batchId.HasValue)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Select a batch before enrolling.");

        var referenceError = await ValidateEnrollmentReferencesAsync(
            application.BranchId,
            academicYearId.Value,
            courseId,
            batchId.Value,
            sectionId,
            cancellationToken);
        if (referenceError is not null)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure(referenceError);

        var admissionNumber = Normalize(request.AdmissionNumber)
            ?? await NextStudentAdmissionNumberAsync(application.TenantId, application.ApplicationNumber, cancellationToken);
        var enrollmentNumber = Normalize(request.EnrollmentNumber)
            ?? await NextEnrollmentNumberAsync(application.TenantId, cancellationToken);
        var actorUserId = _branchAccess.GetUserId(actor);

        StudentProfile? student = null;
        Enrollment? enrollment = null;
        var fromStatus = application.Status;
        var saveError = (string?)null;
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var admissionExists = await _dbContext.StudentProfiles.AnyAsync(s =>
                s.TenantId == application.TenantId &&
                s.AdmissionNumber == admissionNumber,
                cancellationToken);
            if (admissionExists)
            {
                saveError = "A student with this admission number already exists.";
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            student = new StudentProfile
            {
                Id = Guid.NewGuid(),
                TenantId = application.TenantId,
                BranchId = application.BranchId,
                SectionId = sectionId,
                AdmissionNumber = admissionNumber,
                FirstName = application.ApplicantFirstName,
                MiddleName = application.ApplicantMiddleName,
                LastName = application.ApplicantLastName,
                DateOfBirth = application.DateOfBirth,
                Gender = application.Gender,
                AdmissionDate = request.EnrollmentDate,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                EmergencyContactName = application.GuardianName,
                EmergencyContactPhone = application.GuardianPhone,
                Address = application.Address,
                Status = StudentStatus.Active
            };
            _dbContext.StudentProfiles.Add(student);

            enrollment = new Enrollment
            {
                TenantId = application.TenantId,
                StudentProfileId = student.Id,
                AdmissionApplicationId = application.Id,
                BranchId = application.BranchId,
                AcademicYearId = academicYearId.Value,
                CourseId = courseId,
                BatchId = batchId.Value,
                SectionId = sectionId,
                EnrollmentNumber = enrollmentNumber,
                EnrollmentDate = request.EnrollmentDate,
                Status = EnrollmentStatus.Active,
                Notes = Normalize(request.Notes) ?? "Created from admission approval."
            };
            _dbContext.Enrollments.Add(enrollment);

            _dbContext.StudentLifecycleEvents.Add(new StudentLifecycleEvent
            {
                TenantId = application.TenantId,
                StudentProfileId = student.Id,
                BranchId = application.BranchId,
                EventType = StudentLifecycleEventType.Enrolled,
                FromStatus = StudentStatus.Active,
                ToStatus = StudentStatus.Active,
                FromBranchId = application.BranchId,
                ToBranchId = application.BranchId,
                ToAcademicYearId = academicYearId.Value,
                ToCourseId = courseId,
                ToBatchId = batchId.Value,
                ToSectionId = sectionId,
                EffectiveOn = request.EnrollmentDate,
                RecordedOn = DateTime.UtcNow,
                RecordedByUserId = actorUserId,
                Reason = "Application approved and enrolled.",
                Notes = Normalize(request.Notes)
            });

            application.Status = AdmissionApplicationStatus.Enrolled;
            application.EnrolledStudentProfileId = student.Id;
            application.ReviewNotes = Normalize(request.Notes) ?? application.ReviewNotes;

            _dbContext.AdmissionReviews.Add(new AdmissionReview
            {
                TenantId = application.TenantId,
                AdmissionApplicationId = application.Id,
                ReviewedByUserId = actorUserId,
                FromStatus = fromStatus,
                ToStatus = AdmissionApplicationStatus.Enrolled,
                ReviewedOn = DateTime.UtcNow,
                Notes = Normalize(request.Notes) ?? "Converted to student enrollment."
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });

        if (saveError is not null)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure(saveError);
        if (student is null || enrollment is null)
            return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Failure("Enrollment could not be completed.");

        return OperationsWorkflowResult<AdmissionEnrollmentResultDto>.Success(new AdmissionEnrollmentResultDto
        {
            Application = Map(application),
            StudentProfileId = student.Id,
            EnrollmentId = enrollment.Id,
            AdmissionNumber = student.AdmissionNumber,
            EnrollmentNumber = enrollment.EnrollmentNumber
        }, "Application converted to enrollment.");
    }

    private Task<AdmissionApplication?> GetApplicationAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.AdmissionApplications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    private async Task<IReadOnlyList<string>> ValidateEnrollmentReadinessAsync(
        AdmissionApplication application,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        if (!application.AdmissionFormTemplateId.HasValue)
            return errors;

        var requiredFields = await _dbContext.AdmissionFormFields
            .AsNoTracking()
            .Where(f => f.AdmissionFormTemplateId == application.AdmissionFormTemplateId.Value &&
                        f.IsActive &&
                        f.IsRequired)
            .ToListAsync(cancellationToken);

        if (requiredFields.Count > 0)
        {
            var responses = ParseResponses(application.FormResponseJson);
            foreach (var field in requiredFields)
            {
                if (!responses.TryGetValue(field.FieldKey, out var value) || string.IsNullOrWhiteSpace(value))
                    errors.Add($"Required admission form field '{field.Label}' is missing.");
            }
        }

        var requiredDocuments = await _dbContext.AdmissionDocumentRequirements
            .AsNoTracking()
            .Where(r => r.AdmissionFormTemplateId == application.AdmissionFormTemplateId.Value &&
                        r.IsActive &&
                        r.IsRequired)
            .ToListAsync(cancellationToken);

        foreach (var requirement in requiredDocuments)
        {
            var isSatisfied = await _dbContext.AdmissionDocuments.AnyAsync(d =>
                d.AdmissionApplicationId == application.Id &&
                d.DocumentType == requirement.DocumentType &&
                d.IsVerified,
                cancellationToken);
            if (!isSatisfied)
                errors.Add($"Required document '{requirement.DisplayName}' must be uploaded and verified.");
        }

        return errors;
    }

    private async Task<string?> ValidateEnrollmentReferencesAsync(
        Guid branchId,
        Guid academicYearId,
        Guid courseId,
        Guid batchId,
        Guid? sectionId,
        CancellationToken cancellationToken)
    {
        if (!await _dbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken))
            return "Selected branch was not found.";
        if (!await _dbContext.AcademicYears.AnyAsync(a => a.Id == academicYearId, cancellationToken))
            return "Selected academic year was not found.";
        if (!await _dbContext.Courses.AnyAsync(c => c.Id == courseId, cancellationToken))
            return "Selected course was not found.";
        if (!await _dbContext.Batches.AnyAsync(b => b.Id == batchId && b.CourseId == courseId && b.AcademicYearId == academicYearId, cancellationToken))
            return "Selected batch does not match the selected course and academic year.";
        if (sectionId.HasValue && !await _dbContext.Sections.AnyAsync(s => s.Id == sectionId.Value && s.BatchId == batchId, cancellationToken))
            return "Selected section does not belong to the selected batch.";
        return null;
    }

    private async Task<string> NextStudentAdmissionNumberAsync(
        Guid tenantId,
        string applicationNumber,
        CancellationToken cancellationToken)
    {
        var candidate = applicationNumber.Trim();
        if (!await _dbContext.StudentProfiles
                .IgnoreQueryFilters()
                .AnyAsync(s => s.TenantId == tenantId && s.AdmissionNumber == candidate, cancellationToken))
            return candidate;

        for (var i = 1; i < 100_000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var number = $"ADM-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.StudentProfiles
                    .IgnoreQueryFilters()
                    .AnyAsync(s => s.TenantId == tenantId && s.AdmissionNumber == number, cancellationToken))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique admission number.");
    }

    private async Task<string> NextEnrollmentNumberAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        for (var i = 1; i < 100_000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var number = $"ENR-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.Enrollments
                    .IgnoreQueryFilters()
                    .AnyAsync(e => e.TenantId == tenantId && e.EnrollmentNumber == number, cancellationToken))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique enrollment number.");
    }

    private static Dictionary<string, string> ParseResponses(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            return document.RootElement
                .EnumerateObject()
                .ToDictionary(
                    p => p.Name,
                    p => p.Value.ValueKind == JsonValueKind.String ? p.Value.GetString() ?? string.Empty : p.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static AdmissionApplicationDto Map(AdmissionApplication e) => new AdmissionApplicationDto
    {
        AdmissionFormTemplateId = e.AdmissionFormTemplateId,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        SectionId = e.SectionId,
        ApplicationNumber = e.ApplicationNumber,
        ApplicantFirstName = e.ApplicantFirstName,
        ApplicantMiddleName = e.ApplicantMiddleName,
        ApplicantLastName = e.ApplicantLastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        GuardianName = e.GuardianName,
        GuardianPhone = e.GuardianPhone,
        Address = e.Address,
        AppliedOn = e.AppliedOn,
        Status = e.Status,
        ReviewNotes = e.ReviewNotes,
        EnrolledStudentProfileId = e.EnrolledStudentProfileId,
        FormResponseJson = e.FormResponseJson
    }.WithMetadata(e);

    private static AdmissionFormTemplateDto Map(AdmissionFormTemplate e) => new AdmissionFormTemplateDto
    {
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        Name = e.Name,
        Description = e.Description,
        Instructions = e.Instructions,
        IsDefault = e.IsDefault,
        IsActive = e.IsActive,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo
    }.WithMetadata(e);

    private static AdmissionFormFieldDto Map(AdmissionFormField e) => new AdmissionFormFieldDto
    {
        AdmissionFormTemplateId = e.AdmissionFormTemplateId,
        FieldKey = e.FieldKey,
        Label = e.Label,
        FieldType = e.FieldType,
        IsRequired = e.IsRequired,
        SortOrder = e.SortOrder,
        Placeholder = e.Placeholder,
        HelpText = e.HelpText,
        OptionsJson = e.OptionsJson,
        ValidationRegex = e.ValidationRegex,
        MaxLength = e.MaxLength,
        IsActive = e.IsActive
    }.WithMetadata(e);

    private static AdmissionDocumentRequirementDto Map(AdmissionDocumentRequirement e) => new AdmissionDocumentRequirementDto
    {
        AdmissionFormTemplateId = e.AdmissionFormTemplateId,
        DocumentType = e.DocumentType,
        DisplayName = e.DisplayName,
        IsRequired = e.IsRequired,
        SortOrder = e.SortOrder,
        Notes = e.Notes,
        IsActive = e.IsActive
    }.WithMetadata(e);

    private static AdmissionDocumentDto Map(AdmissionDocument e) => new AdmissionDocumentDto
    {
        AdmissionApplicationId = e.AdmissionApplicationId,
        DocumentType = e.DocumentType,
        DisplayName = e.DisplayName,
        FileName = e.FileName,
        ContentType = e.ContentType,
        StoragePath = e.StoragePath,
        SizeBytes = e.SizeBytes,
        UploadedOn = e.UploadedOn,
        UploadedByUserId = e.UploadedByUserId,
        IsVerified = e.IsVerified,
        VerifiedByUserId = e.VerifiedByUserId,
        VerifiedOn = e.VerifiedOn,
        Notes = e.Notes
    }.WithMetadata(e);

    private static AdmissionReviewDto Map(AdmissionReview e) => new AdmissionReviewDto
    {
        AdmissionApplicationId = e.AdmissionApplicationId,
        ReviewedByUserId = e.ReviewedByUserId,
        FromStatus = e.FromStatus,
        ToStatus = e.ToStatus,
        ReviewedOn = e.ReviewedOn,
        Notes = e.Notes
    }.WithMetadata(e);

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
