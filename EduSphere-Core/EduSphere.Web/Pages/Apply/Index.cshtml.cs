using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Apply;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private const long MaxUploadBytes = 20 * 1024 * 1024;

    private readonly TenantDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IAdmissionDocumentStorageService _storage;
    private readonly INotificationDispatcher _notificationDispatcher;

    public IndexModel(
        TenantDbContext dbContext,
        ITenantContext tenantContext,
        IAdmissionDocumentStorageService storage,
        INotificationDispatcher notificationDispatcher)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _storage = storage;
        _notificationDispatcher = notificationDispatcher;
    }

    public IReadOnlyList<Tenant> Tenants { get; private set; } = new List<Tenant>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<AdmissionFormTemplate> Forms { get; private set; } = new List<AdmissionFormTemplate>();
    public IReadOnlyList<AdmissionFormField> Fields { get; private set; } = new List<AdmissionFormField>();
    public IReadOnlyList<AdmissionDocumentRequirement> DocumentRequirements { get; private set; } = new List<AdmissionDocumentRequirement>();
    public AdmissionFormTemplate? SelectedForm { get; private set; }

    [BindProperty(SupportsGet = true)] public Guid? TenantId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? BranchId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? CourseId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? AcademicYearId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? BatchId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? SectionId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? AdmissionFormTemplateId { get; set; }
    [BindProperty] public ApplicantInputModel ApplicantInput { get; set; } = new();
    [BindProperty] public Dictionary<string, string?> FormResponses { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    [TempData] public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        await LoadAsync();
        ModelState.Clear();

        if (TenantId is not Guid tenantId || Tenants.All(t => t.Id != tenantId))
            ModelState.AddModelError(nameof(TenantId), "Select an institution.");
        if (BranchId is not Guid branchId || Branches.All(b => b.Id != branchId))
            ModelState.AddModelError(nameof(BranchId), "Select a branch or campus.");
        if (CourseId is not Guid courseId || Courses.All(c => c.Id != courseId))
            ModelState.AddModelError(nameof(CourseId), "Select a course.");
        if (AdmissionFormTemplateId is not Guid formId || Forms.All(f => f.Id != formId))
            ModelState.AddModelError(nameof(AdmissionFormTemplateId), "Select an admission form.");
        if (string.IsNullOrWhiteSpace(ApplicantInput.ApplicantFirstName))
            ModelState.AddModelError("ApplicantInput.ApplicantFirstName", "First name is required.");
        if (string.IsNullOrWhiteSpace(ApplicantInput.ApplicantLastName))
            ModelState.AddModelError("ApplicantInput.ApplicantLastName", "Last name is required.");

        var responseJson = BuildResponseJson();
        ValidateDocuments();

        if (!ModelState.IsValid)
            return Page();

        var application = new AdmissionApplication
        {
            TenantId = TenantId!.Value,
            AdmissionFormTemplateId = AdmissionFormTemplateId,
            BranchId = BranchId!.Value,
            AcademicYearId = AcademicYearId,
            CourseId = CourseId!.Value,
            BatchId = BatchId,
            SectionId = SectionId,
            ApplicationNumber = await NextApplicationNumberAsync(TenantId.Value),
            ApplicantFirstName = ApplicantInput.ApplicantFirstName.Trim(),
            ApplicantMiddleName = Normalize(ApplicantInput.ApplicantMiddleName),
            ApplicantLastName = ApplicantInput.ApplicantLastName.Trim(),
            DateOfBirth = ApplicantInput.DateOfBirth,
            Gender = ApplicantInput.Gender,
            Email = Normalize(ApplicantInput.Email),
            PhoneNumber = Normalize(ApplicantInput.PhoneNumber),
            GuardianName = Normalize(ApplicantInput.GuardianName),
            GuardianPhone = Normalize(ApplicantInput.GuardianPhone),
            Address = Normalize(ApplicantInput.Address),
            AppliedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = AdmissionApplicationStatus.Submitted,
            FormResponseJson = responseJson
        };

        _dbContext.AdmissionApplications.Add(application);
        await _dbContext.SaveChangesAsync();
        await SaveDocumentsAsync(application);
        await QueueSubmittedNotificationAsync(application);

        return RedirectToPage("Submitted", new { tenantId = application.TenantId, applicationNumber = application.ApplicationNumber });
    }

    public string DynamicValue(AdmissionFormField field)
        => FormResponses.TryGetValue(field.FieldKey, out var value) ? value ?? string.Empty : string.Empty;

    public IReadOnlyList<string> FieldOptions(AdmissionFormField field)
    {
        if (string.IsNullOrWhiteSpace(field.OptionsJson))
            return Array.Empty<string>();

        try
        {
            var options = JsonSerializer.Deserialize<List<string>>(field.OptionsJson);
            return options is null ? Array.Empty<string>() : options;
        }
        catch (JsonException)
        {
            return field.OptionsJson.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }

    public string InputType(AdmissionFormFieldType type) => type switch
    {
        AdmissionFormFieldType.Email => "email",
        AdmissionFormFieldType.Phone => "tel",
        AdmissionFormFieldType.Date => "date",
        AdmissionFormFieldType.Number => "number",
        _ => "text"
    };

    private async Task LoadAsync()
    {
        Tenants = await _dbContext.Tenants
            .IgnoreQueryFilters()
            .Where(t => !t.IsDeleted && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();

        TenantId ??= _tenantContext.TenantId;
        if (TenantId is not Guid tenantId)
            return;

        Branches = await _dbContext.Branches
            .IgnoreQueryFilters()
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
        Courses = await _dbContext.Courses
            .IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
        Years = await _dbContext.AcademicYears
            .IgnoreQueryFilters()
            .Where(y => y.TenantId == tenantId && !y.IsDeleted)
            .OrderByDescending(y => y.IsCurrent)
            .ThenByDescending(y => y.StartDate)
            .ToListAsync();
        Batches = await _dbContext.Batches
            .IgnoreQueryFilters()
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && (!CourseId.HasValue || b.CourseId == CourseId.Value))
            .OrderBy(b => b.Name)
            .ToListAsync();
        Sections = await _dbContext.Sections
            .IgnoreQueryFilters()
            .Where(s => s.TenantId == tenantId && !s.IsDeleted && (!BatchId.HasValue || s.BatchId == BatchId.Value))
            .OrderBy(s => s.Name)
            .ToListAsync();

        AcademicYearId ??= Years.FirstOrDefault(y => y.IsCurrent)?.Id ?? Years.FirstOrDefault()?.Id;
        Forms = await _dbContext.AdmissionFormTemplates
            .IgnoreQueryFilters()
            .Where(f => f.TenantId == tenantId &&
                        !f.IsDeleted &&
                        f.IsActive &&
                        (!BranchId.HasValue || f.BranchId == null || f.BranchId == BranchId.Value) &&
                        (!CourseId.HasValue || f.CourseId == null || f.CourseId == CourseId.Value) &&
                        (!AcademicYearId.HasValue || f.AcademicYearId == null || f.AcademicYearId == AcademicYearId.Value))
            .OrderByDescending(f => f.IsDefault)
            .ThenBy(f => f.Name)
            .ToListAsync();

        AdmissionFormTemplateId ??= Forms.FirstOrDefault()?.Id;
        if (AdmissionFormTemplateId is not Guid formId)
            return;

        SelectedForm = Forms.FirstOrDefault(f => f.Id == formId) ??
                       await _dbContext.AdmissionFormTemplates.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == formId);
        Fields = await _dbContext.AdmissionFormFields
            .IgnoreQueryFilters()
            .Where(f => f.TenantId == tenantId && !f.IsDeleted && f.AdmissionFormTemplateId == formId && f.IsActive)
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Label)
            .ToListAsync();
        DocumentRequirements = await _dbContext.AdmissionDocumentRequirements
            .IgnoreQueryFilters()
            .Where(r => r.TenantId == tenantId && !r.IsDeleted && r.AdmissionFormTemplateId == formId && r.IsActive)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.DisplayName)
            .ToListAsync();
    }

    private string? BuildResponseJson()
    {
        var responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in Fields)
        {
            FormResponses.TryGetValue(field.FieldKey, out var rawValue);
            var value = Normalize(rawValue);
            if (field.FieldType == AdmissionFormFieldType.Checkbox)
                value = string.Equals(rawValue, "true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";

            if (field.IsRequired &&
                (string.IsNullOrWhiteSpace(value) ||
                 (field.FieldType == AdmissionFormFieldType.Checkbox && value != "true")))
                ModelState.AddModelError($"FormResponses[{field.FieldKey}]", $"{field.Label} is required.");
            if (field.MaxLength.HasValue && value?.Length > field.MaxLength.Value)
                ModelState.AddModelError($"FormResponses[{field.FieldKey}]", $"{field.Label} cannot exceed {field.MaxLength.Value} characters.");
            if (!string.IsNullOrWhiteSpace(value))
                responses[field.FieldKey] = value;
        }

        var json = responses.Count == 0 ? null : JsonSerializer.Serialize(responses);
        if (json?.Length > 4000)
            ModelState.AddModelError(string.Empty, "Form responses cannot exceed 4000 characters.");
        return json;
    }

    private void ValidateDocuments()
    {
        foreach (var requirement in DocumentRequirements.Where(r => r.IsRequired))
        {
            var file = Request.Form.Files.GetFile(DocumentInputName(requirement));
            if (file is null || file.Length == 0)
                ModelState.AddModelError(DocumentInputName(requirement), $"{requirement.DisplayName} is required.");
        }

        foreach (var file in Request.Form.Files)
        {
            if (file.Length > MaxUploadBytes)
                ModelState.AddModelError(file.Name, "Admission documents cannot exceed 20 MB.");
        }
    }

    private async Task SaveDocumentsAsync(AdmissionApplication application)
    {
        foreach (var requirement in DocumentRequirements)
        {
            var file = Request.Form.Files.GetFile(DocumentInputName(requirement));
            if (file is null || file.Length == 0)
                continue;

            await using var stream = file.OpenReadStream();
            var stored = await _storage.SaveAsync(application.TenantId, application.Id, stream, file.FileName, file.ContentType);
            _dbContext.AdmissionDocuments.Add(new AdmissionDocument
            {
                TenantId = application.TenantId,
                AdmissionApplicationId = application.Id,
                DocumentType = requirement.DocumentType,
                DisplayName = requirement.DisplayName,
                FileName = stored.FileName,
                ContentType = stored.ContentType,
                StoragePath = stored.StoragePath,
                SizeBytes = stored.SizeBytes,
                UploadedOn = DateTime.UtcNow,
                IsVerified = false
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    public static string DocumentInputName(AdmissionDocumentRequirement requirement)
        => $"Document_{requirement.Id:N}";

    private async Task QueueSubmittedNotificationAsync(AdmissionApplication application)
    {
        var applicantName = string.Join(' ', new[] { application.ApplicantFirstName, application.ApplicantMiddleName, application.ApplicantLastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
        var subject = $"Admission application {application.ApplicationNumber} submitted";
        var body = $"Dear {applicantName},\n\nWe have received your admission application {application.ApplicationNumber}.";

        if (!string.IsNullOrWhiteSpace(application.Email))
        {
            var message = new NotificationMessage
            {
                TenantId = application.TenantId,
                BranchId = application.BranchId,
                Channel = CommunicationChannel.Email,
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Queued
            };
            message.Recipients.Add(new NotificationRecipient
            {
                TenantId = application.TenantId,
                BranchId = application.BranchId,
                DisplayName = applicantName,
                DestinationAddress = application.Email!,
                Status = NotificationStatus.Queued
            });
            _dbContext.NotificationMessages.Add(message);
            await _dbContext.SaveChangesAsync();
            await _notificationDispatcher.DispatchAsync(message.Id);
        }

        var smsDestination = Normalize(application.PhoneNumber) ?? Normalize(application.GuardianPhone);
        if (!string.IsNullOrWhiteSpace(smsDestination))
        {
            _dbContext.CommunicationLogs.Add(new CommunicationLog
            {
                TenantId = application.TenantId,
                BranchId = application.BranchId,
                Channel = CommunicationChannel.Sms,
                Direction = CommunicationDirection.Outbound,
                Recipient = smsDestination,
                Subject = subject,
                Status = NotificationStatus.Queued,
                PayloadSummary = body,
                OccurredOn = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<string> NextApplicationNumberAsync(Guid tenantId)
    {
        for (var i = 1; i < 100_000; i++)
        {
            var number = $"APP-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.AdmissionApplications
                    .IgnoreQueryFilters()
                    .AnyAsync(a => a.TenantId == tenantId && a.ApplicationNumber == number))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique application number.");
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public class ApplicantInputModel
    {
        [Required, StringLength(100), Display(Name = "First name")] public string ApplicantFirstName { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "Middle name")] public string? ApplicantMiddleName { get; set; }
        [Required, StringLength(100), Display(Name = "Last name")] public string ApplicantLastName { get; set; } = string.Empty;
        [Display(Name = "Date of birth")] public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12));
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [EmailAddress, StringLength(150)] public string? Email { get; set; }
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
        [StringLength(150), Display(Name = "Guardian name")] public string? GuardianName { get; set; }
        [StringLength(30), Display(Name = "Guardian phone")] public string? GuardianPhone { get; set; }
        [StringLength(500)] public string? Address { get; set; }
    }
}
