using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Admissions;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class ApplicationsModel : PageModel
{
    private const long MaxUploadBytes = 20 * 1024 * 1024;

    private readonly ICrudService<AdmissionApplication> _applications;
    private readonly ICrudService<AdmissionFormTemplate> _forms;
    private readonly ICrudService<AdmissionFormField> _fields;
    private readonly ICrudService<AdmissionDocumentRequirement> _requirements;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;
    private readonly IAdmissionWorkflowService _workflow;
    private readonly IAdmissionDocumentStorageService _storage;
    private readonly TenantDbContext _dbContext;

    public ApplicationsModel(
        ICrudService<AdmissionApplication> applications,
        ICrudService<AdmissionFormTemplate> forms,
        ICrudService<AdmissionFormField> fields,
        ICrudService<AdmissionDocumentRequirement> requirements,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        ITenantContext tenant,
        IBranchAccessService branchAccess,
        IAdmissionWorkflowService workflow,
        IAdmissionDocumentStorageService storage,
        TenantDbContext dbContext)
    {
        _applications = applications;
        _forms = forms;
        _fields = fields;
        _requirements = requirements;
        _branches = branches;
        _years = years;
        _courses = courses;
        _batches = batches;
        _sections = sections;
        _tenant = tenant;
        _branchAccess = branchAccess;
        _workflow = workflow;
        _storage = storage;
        _dbContext = dbContext;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<AdmissionApplication> Items { get; private set; } = new List<AdmissionApplication>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<AdmissionFormTemplateDto> FormTemplates { get; private set; } = new List<AdmissionFormTemplateDto>();
    public IReadOnlyList<AdmissionFormFieldDto> ApplicationFormFields { get; private set; } = new List<AdmissionFormFieldDto>();
    public IReadOnlyList<AdmissionFormFieldDto> SelectedFormFields { get; private set; } = new List<AdmissionFormFieldDto>();
    public IReadOnlyList<AdmissionDocumentRequirementDto> SelectedDocumentRequirements { get; private set; } = new List<AdmissionDocumentRequirementDto>();
    public IReadOnlyList<AdmissionDocumentDto> SelectedDocuments { get; private set; } = new List<AdmissionDocumentDto>();
    public IReadOnlyList<AdmissionReviewDto> SelectedReviews { get; private set; } = new List<AdmissionReviewDto>();
    public IReadOnlyList<AdmissionInterviewDto> SelectedInterviews { get; private set; } = new List<AdmissionInterviewDto>();
    public AdmissionFinanceReadinessDto? SelectedFinanceReadiness { get; private set; }
    public AdmissionApplication? SelectedApplication { get; private set; }

    [BindProperty] public ApplicationInputModel ApplicationInput { get; set; } = new();
    [BindProperty] public Dictionary<string, string?> FormResponses { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    [BindProperty] public TemplateInputModel TemplateInput { get; set; } = new();
    [BindProperty] public FieldInputModel FieldInput { get; set; } = new();
    [BindProperty] public RequirementInputModel RequirementInput { get; set; } = new();
    [BindProperty] public DocumentInputModel DocumentInput { get; set; } = new();
    [BindProperty] public ReviewInputModel ReviewInput { get; set; } = new();
    [BindProperty] public EnrollmentInputModel EnrollmentInput { get; set; } = new();
    [BindProperty] public InterviewInputModel InterviewInput { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }

    public bool IsEditing => ApplicationInput.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string CourseName(Guid id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "-";
    public string BatchName(Guid? id) => Batches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string SectionName(Guid? id) => Sections.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string FormName(Guid? id) => id.HasValue
        ? FormTemplates.FirstOrDefault(f => f.Id == id.Value)?.Name ?? "-"
        : "-";

    public string ApplicantName(AdmissionApplication application) =>
        string.Join(' ', new[] { application.ApplicantFirstName, application.ApplicantMiddleName, application.ApplicantLastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    public async Task OnGetAsync(Guid? editId, Guid? formId)
    {
        if (!HasTenant) return;

        await LoadAsync(editId, formId);
        if (SelectedApplication is not null)
            ApplicationInput = Map(SelectedApplication);
        else if (formId.HasValue)
            ApplicationInput.AdmissionFormTemplateId = formId.Value;
        if (formId.HasValue && FormTemplates.Any(f => f.Id == formId.Value))
            TemplateInput.Id = formId.Value;
    }

    public async Task<IActionResult> OnPostSaveApplicationAsync()
    {
        if (!HasTenant) return RedirectToPage();
        ModelState.Clear();

        var branchId = ApplicationInput.BranchId ?? Guid.Empty;
        var courseId = ApplicationInput.CourseId ?? Guid.Empty;
        await ValidateApplicationInputAsync(branchId, courseId);
        await ApplyDynamicFormResponsesAsync();

        if (!ModelState.IsValid)
        {
            await LoadAsync(ApplicationInput.Id == Guid.Empty ? null : ApplicationInput.Id, ApplicationInput.AdmissionFormTemplateId);
            return Page();
        }

        if (ApplicationInput.Id == Guid.Empty)
        {
            await _applications.CreateAsync(Apply(new AdmissionApplication(), branchId, courseId));
            StatusMessage = "Admission application created.";
        }
        else
        {
            await _applications.UpdateAsync(ApplicationInput.Id, e => Apply(e, branchId, courseId));
            StatusMessage = "Admission application updated.";
        }

        return RedirectToPage(new { editId = ApplicationInput.Id == Guid.Empty ? (Guid?)null : ApplicationInput.Id });
    }

    public async Task<IActionResult> OnPostDeleteApplicationAsync(Guid id)
    {
        if (await _applications.GetAsync(id) is { } application && await CanUseBranchAsync(application.BranchId))
            await _applications.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveTemplateAsync()
    {
        if (!HasTenant) return RedirectToPage();
        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(TemplateInput.Name))
            ModelState.AddModelError("TemplateInput.Name", "Template name is required.");
        if (_branchAccess.IsBranchAdminOnly(User) && TemplateInput.BranchId is null)
            ModelState.AddModelError("TemplateInput.BranchId", "Branch admins must scope admission forms to their assigned branch.");
        if (TemplateInput.BranchId is Guid branchId && !await CanUseBranchAsync(branchId))
            ModelState.AddModelError("TemplateInput.BranchId", "You can configure forms only for your assigned branch.");
        if (TemplateInput.Id != Guid.Empty)
        {
            var existingForm = await _forms.GetAsync(TemplateInput.Id);
            if (existingForm is null)
                ModelState.AddModelError(string.Empty, "Admission form was not found.");
            else if (!await CanManageFormAsync(existingForm))
                ModelState.AddModelError(string.Empty, "You can update only branch-scoped admission forms assigned to you.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(null, TemplateInput.Id == Guid.Empty ? null : TemplateInput.Id);
            return Page();
        }

        if (TemplateInput.Id == Guid.Empty)
        {
            var created = await _forms.CreateAsync(Apply(new AdmissionFormTemplate(), TemplateInput));
            StatusMessage = "Admission form template created.";
            return RedirectToPage(new { formId = created.Id });
        }

        await _forms.UpdateAsync(TemplateInput.Id, e => Apply(e, TemplateInput));
        StatusMessage = "Admission form template updated.";
        return RedirectToPage(new { formId = TemplateInput.Id });
    }

    public async Task<IActionResult> OnPostSaveFieldAsync()
    {
        if (!HasTenant) return RedirectToPage();
        ModelState.Clear();

        var form = await _forms.GetAsync(FieldInput.AdmissionFormTemplateId);
        if (form is null || !await CanManageFormAsync(form))
            ModelState.AddModelError("FieldInput.AdmissionFormTemplateId", "Select a valid admission form.");
        if (string.IsNullOrWhiteSpace(FieldInput.FieldKey))
            ModelState.AddModelError("FieldInput.FieldKey", "Field key is required.");
        if (string.IsNullOrWhiteSpace(FieldInput.Label))
            ModelState.AddModelError("FieldInput.Label", "Label is required.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(null, FieldInput.AdmissionFormTemplateId);
            return Page();
        }

        await _fields.CreateAsync(Apply(new AdmissionFormField(), FieldInput));
        StatusMessage = "Form field added.";
        return RedirectToPage(new { formId = FieldInput.AdmissionFormTemplateId });
    }

    public async Task<IActionResult> OnPostDeleteFieldAsync(Guid id, Guid formId)
    {
        if (await _fields.GetAsync(id) is { } field &&
            field.AdmissionFormTemplateId == formId &&
            await _forms.GetAsync(formId) is { } form &&
            await CanManageFormAsync(form))
        {
            await _fields.SoftDeleteAsync(id);
            StatusMessage = "Form field removed.";
        }

        return RedirectToPage(new { formId });
    }

    public async Task<IActionResult> OnPostSaveRequirementAsync()
    {
        if (!HasTenant) return RedirectToPage();
        ModelState.Clear();

        var form = await _forms.GetAsync(RequirementInput.AdmissionFormTemplateId);
        if (form is null || !await CanManageFormAsync(form))
            ModelState.AddModelError("RequirementInput.AdmissionFormTemplateId", "Select a valid admission form.");
        if (string.IsNullOrWhiteSpace(RequirementInput.DisplayName))
            ModelState.AddModelError("RequirementInput.DisplayName", "Display name is required.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(null, RequirementInput.AdmissionFormTemplateId);
            return Page();
        }

        await _requirements.CreateAsync(Apply(new AdmissionDocumentRequirement(), RequirementInput));
        StatusMessage = "Document requirement added.";
        return RedirectToPage(new { formId = RequirementInput.AdmissionFormTemplateId });
    }

    public async Task<IActionResult> OnPostDeleteRequirementAsync(Guid id, Guid formId)
    {
        if (await _requirements.GetAsync(id) is { } requirement &&
            requirement.AdmissionFormTemplateId == formId &&
            await _forms.GetAsync(formId) is { } form &&
            await CanManageFormAsync(form))
        {
            await _requirements.SoftDeleteAsync(id);
            StatusMessage = "Document requirement removed.";
        }

        return RedirectToPage(new { formId });
    }

    public async Task<IActionResult> OnPostUploadDocumentAsync(Guid applicationId)
    {
        if (!HasTenant) return RedirectToPage();
        ModelState.Clear();

        if (DocumentInput.File is null || DocumentInput.File.Length == 0)
            ModelState.AddModelError("DocumentInput.File", "Select a file to upload.");
        else if (DocumentInput.File.Length > MaxUploadBytes)
            ModelState.AddModelError("DocumentInput.File", "Admission documents cannot exceed 20 MB.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(applicationId, null);
            return Page();
        }

        await using var stream = DocumentInput.File!.OpenReadStream();
        var result = await _workflow.UploadDocumentAsync(
            User,
            applicationId,
            new UploadAdmissionDocumentRequest
            {
                DocumentType = DocumentInput.DocumentType,
                DisplayName = DocumentInput.DisplayName,
                Notes = DocumentInput.Notes
            },
            stream,
            DocumentInput.File.FileName,
            DocumentInput.File.ContentType,
            DocumentInput.File.Length);

        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnPostScheduleInterviewAsync(Guid applicationId)
    {
        var result = await _workflow.ScheduleInterviewAsync(User, applicationId, new ScheduleAdmissionInterviewRequest
        {
            InterviewerUserId = InterviewInput.InterviewerUserId,
            StartsOn = InterviewInput.StartsOn,
            EndsOn = InterviewInput.EndsOn,
            Location = InterviewInput.Location,
            MeetingLink = InterviewInput.MeetingLink,
            Notes = InterviewInput.Notes
        });
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnPostGenerateAdmissionInvoiceAsync(Guid applicationId)
    {
        var result = await _workflow.GenerateAdmissionFeeInvoiceAsync(User, applicationId);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnPostMarkAdmissionInvoicePaidAsync(Guid applicationId, Guid invoiceId)
    {
        var application = await _applications.GetAsync(applicationId);
        if (application is null || !await CanUseBranchAsync(application.BranchId))
            return RedirectToPage();

        var invoice = await _dbContext.FeeInvoices
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.AdmissionApplicationId == applicationId);
        if (invoice is null || invoice.BranchId != application.BranchId)
        {
            StatusMessage = "Admission fee invoice was not found.";
            return RedirectToPage(new { editId = applicationId });
        }

        var outstanding = Math.Max(invoice.TotalAmount - invoice.PaidAmount, 0);
        if (outstanding > 0)
        {
            _dbContext.FeePayments.Add(new FeePayment
            {
                TenantId = invoice.TenantId,
                BranchId = invoice.BranchId,
                FeeInvoiceId = invoice.Id,
                PaymentNumber = await NextAdmissionPaymentNumberAsync(invoice.TenantId),
                Amount = outstanding,
                PaidOn = DateTime.UtcNow,
                Mode = PaymentMode.OnlineGateway,
                Status = PaymentStatus.Completed,
                Notes = "Admission fee marked paid from admissions workflow."
            });
        }

        invoice.PaidAmount = invoice.TotalAmount;
        invoice.Status = InvoiceStatus.Paid;
        await _dbContext.SaveChangesAsync();
        StatusMessage = "Admission fee marked paid.";
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnGetExportAsync()
    {
        if (!HasTenant) return RedirectToPage();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = (await _applications.ListAsync(a =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && a.BranchId == assignedBranchId.Value)))
            .OrderByDescending(a => a.AppliedOn)
            .ThenBy(a => a.ApplicationNumber)
            .ToList();

        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        var csv = new StringBuilder();
        csv.AppendLine("ApplicationNumber,ApplicantName,Email,Phone,Guardian,Branch,Course,AppliedOn,Status,ReviewNotes");
        foreach (var item in items)
        {
            csv.AppendLine(string.Join(',', new[]
            {
                Csv(item.ApplicationNumber),
                Csv(ApplicantName(item)),
                Csv(item.Email),
                Csv(item.PhoneNumber),
                Csv(item.GuardianName),
                Csv(BranchName(item.BranchId)),
                Csv(CourseName(item.CourseId)),
                Csv(item.AppliedOn.ToString("yyyy-MM-dd")),
                Csv(item.Status.ToString()),
                Csv(item.ReviewNotes)
            }));
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"admission-applications-{DateTime.UtcNow:yyyyMMddHHmm}.csv");
    }

    public async Task<IActionResult> OnPostVerifyDocumentAsync(Guid applicationId, Guid documentId, bool isVerified)
    {
        var result = await _workflow.SetDocumentVerificationAsync(User, applicationId, documentId, isVerified, DocumentInput.Notes);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnGetDownloadDocumentAsync(Guid applicationId, Guid documentId)
    {
        var document = (await _workflow.ListDocumentsAsync(User, applicationId)).FirstOrDefault(d => d.Id == documentId);
        if (document is null || string.IsNullOrWhiteSpace(document.StoragePath))
            return NotFound();

        try
        {
            var stream = await _storage.OpenReadAsync(document.StoragePath);
            return File(stream, document.ContentType ?? "application/octet-stream", document.FileName ?? "admission-document");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostReviewAsync(Guid applicationId)
    {
        var result = await _workflow.ReviewAsync(User, applicationId, new ReviewAdmissionApplicationRequest
        {
            ToStatus = ReviewInput.ToStatus,
            Notes = ReviewInput.Notes
        });
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    public async Task<IActionResult> OnPostEnrollAsync(Guid applicationId)
    {
        var result = await _workflow.ConvertToEnrollmentAsync(User, applicationId, new ConvertAdmissionToEnrollmentRequest
        {
            AcademicYearId = EnrollmentInput.AcademicYearId,
            CourseId = EnrollmentInput.CourseId,
            BatchId = EnrollmentInput.BatchId,
            SectionId = EnrollmentInput.SectionId,
            AdmissionNumber = EnrollmentInput.AdmissionNumber,
            EnrollmentNumber = EnrollmentInput.EnrollmentNumber,
            EnrollmentDate = EnrollmentInput.EnrollmentDate,
            Notes = EnrollmentInput.Notes
        });
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage(new { editId = applicationId });
    }

    private async Task LoadAsync(Guid? selectedApplicationId, Guid? selectedFormId)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Items = (await _applications.ListAsync(a =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && a.BranchId == assignedBranchId.Value)))
            .OrderByDescending(a => a.AppliedOn)
            .ThenBy(a => a.ApplicationNumber)
            .ToList();
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Batches = (await _batches.ListAsync()).OrderBy(b => b.Name).ToList();
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        FormTemplates = await _workflow.ListFormTemplatesAsync(User);

        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue && ApplicationInput.BranchId is null)
            ApplicationInput.BranchId = assignedBranchId.Value;

        if (selectedApplicationId.HasValue)
        {
            SelectedApplication = Items.FirstOrDefault(a => a.Id == selectedApplicationId.Value);
            if (SelectedApplication is not null)
            {
                SelectedDocuments = await _workflow.ListDocumentsAsync(User, SelectedApplication.Id);
                SelectedReviews = await _workflow.ListReviewsAsync(User, SelectedApplication.Id);
                SelectedInterviews = await _workflow.ListInterviewsAsync(User, SelectedApplication.Id);
                var readiness = await _workflow.GetFinanceReadinessAsync(User, SelectedApplication.Id);
                SelectedFinanceReadiness = readiness.Succeeded ? readiness.Data : null;
                selectedFormId ??= SelectedApplication.AdmissionFormTemplateId;
                EnrollmentInput.AcademicYearId ??= SelectedApplication.AcademicYearId;
                EnrollmentInput.CourseId ??= SelectedApplication.CourseId;
                EnrollmentInput.BatchId ??= SelectedApplication.BatchId;
                EnrollmentInput.SectionId ??= SelectedApplication.SectionId;
                if (FormResponses.Count == 0)
                    FormResponses = ParseResponses(SelectedApplication.FormResponseJson);
            }
        }

        var applicationFormId = ApplicationInput.AdmissionFormTemplateId
                                ?? SelectedApplication?.AdmissionFormTemplateId
                                ?? selectedFormId
                                ?? FormTemplates.FirstOrDefault(f => f.IsDefault || f.IsActive)?.Id;
        if (applicationFormId.HasValue)
        {
            ApplicationInput.AdmissionFormTemplateId ??= applicationFormId.Value;
            ApplicationFormFields = await _workflow.ListFormFieldsAsync(applicationFormId.Value);
        }

        if (selectedFormId.HasValue)
        {
            var selectedForm = FormTemplates.FirstOrDefault(f => f.Id == selectedFormId.Value);
            if (selectedForm is not null)
            {
                TemplateInput = new TemplateInputModel
                {
                    Id = selectedForm.Id,
                    BranchId = selectedForm.BranchId,
                    AcademicYearId = selectedForm.AcademicYearId,
                    CourseId = selectedForm.CourseId,
                    Name = selectedForm.Name,
                    Description = selectedForm.Description,
                    Instructions = selectedForm.Instructions,
                    IsDefault = selectedForm.IsDefault,
                    IsActive = selectedForm.IsActive,
                    EffectiveFrom = selectedForm.EffectiveFrom,
                    EffectiveTo = selectedForm.EffectiveTo
                };
                FieldInput.AdmissionFormTemplateId = selectedForm.Id;
                RequirementInput.AdmissionFormTemplateId = selectedForm.Id;
                SelectedFormFields = await _workflow.ListFormFieldsAsync(selectedForm.Id);
                SelectedDocumentRequirements = await _workflow.ListDocumentRequirementsAsync(selectedForm.Id);
            }
        }
        else if (FormTemplates.FirstOrDefault(f => f.IsDefault || f.IsActive) is { } firstForm)
        {
            FieldInput.AdmissionFormTemplateId = firstForm.Id;
            RequirementInput.AdmissionFormTemplateId = firstForm.Id;
        }
    }

    private async Task ValidateApplicationInputAsync(Guid branchId, Guid courseId)
    {
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("ApplicationInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("ApplicationInput.BranchId", "You can manage applications only for your assigned branch.");
        if (courseId == Guid.Empty || await _courses.GetAsync(courseId) is null)
            ModelState.AddModelError("ApplicationInput.CourseId", "Selected course was not found.");
        if (ApplicationInput.AdmissionFormTemplateId is Guid formId)
        {
            var form = await _forms.GetAsync(formId);
            if (form is null)
            {
                ModelState.AddModelError("ApplicationInput.AdmissionFormTemplateId", "Selected admission form was not found.");
            }
            else
            {
                if (form.BranchId.HasValue && form.BranchId.Value != branchId)
                    ModelState.AddModelError("ApplicationInput.AdmissionFormTemplateId", "Selected admission form is scoped to a different branch.");
                if (form.CourseId.HasValue && form.CourseId.Value != courseId)
                    ModelState.AddModelError("ApplicationInput.AdmissionFormTemplateId", "Selected admission form is scoped to a different course.");
                if (form.AcademicYearId.HasValue && ApplicationInput.AcademicYearId != form.AcademicYearId.Value)
                    ModelState.AddModelError("ApplicationInput.AcademicYearId", "Selected admission form is scoped to a different academic year.");
            }
        }
        if (ApplicationInput.AcademicYearId is Guid yearId && await _years.GetAsync(yearId) is null)
            ModelState.AddModelError("ApplicationInput.AcademicYearId", "Selected academic year was not found.");
        if (ApplicationInput.BatchId is Guid batchId && await _batches.GetAsync(batchId) is null)
            ModelState.AddModelError("ApplicationInput.BatchId", "Selected batch was not found.");
        if (ApplicationInput.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            ModelState.AddModelError("ApplicationInput.SectionId", "Selected section was not found.");
        if (ApplicationInput.Id != Guid.Empty && await _applications.GetAsync(ApplicationInput.Id) is { } existing && !await CanUseBranchAsync(existing.BranchId))
            ModelState.AddModelError(string.Empty, "You can update applications only in your assigned branch.");
        if (string.IsNullOrWhiteSpace(ApplicationInput.ApplicationNumber))
            ModelState.AddModelError("ApplicationInput.ApplicationNumber", "Application number is required.");
        if (string.IsNullOrWhiteSpace(ApplicationInput.ApplicantFirstName))
            ModelState.AddModelError("ApplicationInput.ApplicantFirstName", "First name is required.");
        if (string.IsNullOrWhiteSpace(ApplicationInput.ApplicantLastName))
            ModelState.AddModelError("ApplicationInput.ApplicantLastName", "Last name is required.");
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private async Task<bool> CanManageFormAsync(AdmissionFormTemplate form)
    {
        if (!form.BranchId.HasValue)
            return !_branchAccess.IsBranchAdminOnly(User);

        return await CanUseBranchAsync(form.BranchId.Value);
    }

    private async Task ApplyDynamicFormResponsesAsync()
    {
        if (ApplicationInput.AdmissionFormTemplateId is not Guid formId)
        {
            ApplicationInput.FormResponseJson = null;
            return;
        }

        var fields = await _workflow.ListFormFieldsAsync(formId);
        ApplicationFormFields = fields;
        var responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in fields)
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
            ModelState.AddModelError("ApplicationInput.FormResponseJson", "Form responses cannot exceed 4000 characters.");

        ApplicationInput.FormResponseJson = json;
    }

    public string DynamicValue(AdmissionFormFieldDto field)
        => FormResponses.TryGetValue(field.FieldKey, out var value) ? value ?? string.Empty : string.Empty;

    public IReadOnlyList<string> FieldOptions(AdmissionFormFieldDto field)
    {
        if (string.IsNullOrWhiteSpace(field.OptionsJson))
            return Array.Empty<string>();

        try
        {
            var options = JsonSerializer.Deserialize<List<string>>(field.OptionsJson);
            return options is null
                ? Array.Empty<string>()
                : options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }
        catch (JsonException)
        {
            return field.OptionsJson
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .ToList();
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

    private static Dictionary<string, string?> ParseResponses(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string?>>(json) ??
                   new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private async Task<string> NextAdmissionPaymentNumberAsync(Guid tenantId)
    {
        for (var i = 1; i < 100_000; i++)
        {
            var number = $"ADMPAY-{DateTime.UtcNow:yyyy}-{i:0000}";
            if (!await _dbContext.FeePayments
                    .IgnoreQueryFilters()
                    .AnyAsync(p => p.TenantId == tenantId && p.PaymentNumber == number))
                return number;
        }

        throw new InvalidOperationException("Could not generate a unique admission payment number.");
    }

    private static string Csv(string? value)
    {
        value ??= string.Empty;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private AdmissionApplication Apply(AdmissionApplication entity, Guid branchId, Guid courseId)
    {
        entity.AdmissionFormTemplateId = ApplicationInput.AdmissionFormTemplateId;
        entity.BranchId = branchId;
        entity.AcademicYearId = ApplicationInput.AcademicYearId;
        entity.CourseId = courseId;
        entity.BatchId = ApplicationInput.BatchId;
        entity.SectionId = ApplicationInput.SectionId;
        entity.ApplicationNumber = ApplicationInput.ApplicationNumber.Trim();
        entity.ApplicantFirstName = ApplicationInput.ApplicantFirstName.Trim();
        entity.ApplicantMiddleName = Normalize(ApplicationInput.ApplicantMiddleName);
        entity.ApplicantLastName = ApplicationInput.ApplicantLastName.Trim();
        entity.DateOfBirth = ApplicationInput.DateOfBirth;
        entity.Gender = ApplicationInput.Gender;
        entity.Email = Normalize(ApplicationInput.Email);
        entity.PhoneNumber = Normalize(ApplicationInput.PhoneNumber);
        entity.GuardianName = Normalize(ApplicationInput.GuardianName);
        entity.GuardianPhone = Normalize(ApplicationInput.GuardianPhone);
        entity.Address = Normalize(ApplicationInput.Address);
        entity.AppliedOn = ApplicationInput.AppliedOn;
        entity.Status = ApplicationInput.Status;
        entity.ReviewNotes = Normalize(ApplicationInput.ReviewNotes);
        entity.FormResponseJson = Normalize(ApplicationInput.FormResponseJson);
        return entity;
    }

    private static AdmissionFormTemplate Apply(AdmissionFormTemplate entity, TemplateInputModel input)
    {
        entity.BranchId = input.BranchId;
        entity.AcademicYearId = input.AcademicYearId;
        entity.CourseId = input.CourseId;
        entity.Name = input.Name.Trim();
        entity.Description = Normalize(input.Description);
        entity.Instructions = Normalize(input.Instructions);
        entity.IsDefault = input.IsDefault;
        entity.IsActive = input.IsActive;
        entity.EffectiveFrom = input.EffectiveFrom;
        entity.EffectiveTo = input.EffectiveTo;
        return entity;
    }

    private static AdmissionFormField Apply(AdmissionFormField entity, FieldInputModel input)
    {
        entity.AdmissionFormTemplateId = input.AdmissionFormTemplateId;
        entity.FieldKey = input.FieldKey.Trim();
        entity.Label = input.Label.Trim();
        entity.FieldType = input.FieldType;
        entity.IsRequired = input.IsRequired;
        entity.SortOrder = input.SortOrder;
        entity.Placeholder = Normalize(input.Placeholder);
        entity.HelpText = Normalize(input.HelpText);
        entity.OptionsJson = Normalize(input.OptionsJson);
        entity.ValidationRegex = Normalize(input.ValidationRegex);
        entity.MaxLength = input.MaxLength;
        entity.IsActive = input.IsActive;
        return entity;
    }

    private static AdmissionDocumentRequirement Apply(AdmissionDocumentRequirement entity, RequirementInputModel input)
    {
        entity.AdmissionFormTemplateId = input.AdmissionFormTemplateId;
        entity.DocumentType = input.DocumentType;
        entity.DisplayName = input.DisplayName.Trim();
        entity.IsRequired = input.IsRequired;
        entity.SortOrder = input.SortOrder;
        entity.Notes = Normalize(input.Notes);
        entity.IsActive = input.IsActive;
        return entity;
    }

    private static ApplicationInputModel Map(AdmissionApplication e) => new()
    {
        Id = e.Id,
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
        FormResponseJson = e.FormResponseJson
    };

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public class ApplicationInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Admission form")] public Guid? AdmissionFormTemplateId { get; set; }
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Display(Name = "Batch")] public Guid? BatchId { get; set; }
        [Display(Name = "Section")] public Guid? SectionId { get; set; }
        [StringLength(50), Display(Name = "Application #")] public string ApplicationNumber { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "First name")] public string ApplicantFirstName { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "Middle name")] public string? ApplicantMiddleName { get; set; }
        [StringLength(100), Display(Name = "Last name")] public string ApplicantLastName { get; set; } = string.Empty;
        [Display(Name = "Date of birth")] public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [EmailAddress, StringLength(150)] public string? Email { get; set; }
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
        [StringLength(150), Display(Name = "Guardian")] public string? GuardianName { get; set; }
        [StringLength(30), Display(Name = "Guardian phone")] public string? GuardianPhone { get; set; }
        [StringLength(500)] public string? Address { get; set; }
        [Display(Name = "Applied on")] public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public AdmissionApplicationStatus Status { get; set; } = AdmissionApplicationStatus.Submitted;
        [StringLength(1000), Display(Name = "Review notes")] public string? ReviewNotes { get; set; }
        [StringLength(4000), Display(Name = "Form responses JSON")] public string? FormResponseJson { get; set; }
    }

    public class TemplateInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [StringLength(120)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        [StringLength(1000)] public string? Instructions { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        [Display(Name = "Effective from")] public DateOnly? EffectiveFrom { get; set; }
        [Display(Name = "Effective to")] public DateOnly? EffectiveTo { get; set; }
    }

    public class FieldInputModel
    {
        public Guid AdmissionFormTemplateId { get; set; }
        [Display(Name = "Field key"), StringLength(80)] public string FieldKey { get; set; } = string.Empty;
        [StringLength(150)] public string Label { get; set; } = string.Empty;
        [Display(Name = "Type")] public AdmissionFormFieldType FieldType { get; set; } = AdmissionFormFieldType.Text;
        [Display(Name = "Required")] public bool IsRequired { get; set; }
        [Display(Name = "Order")] public int SortOrder { get; set; }
        [StringLength(200)] public string? Placeholder { get; set; }
        [StringLength(500), Display(Name = "Help text")] public string? HelpText { get; set; }
        [StringLength(2000), Display(Name = "Options JSON")] public string? OptionsJson { get; set; }
        [StringLength(200), Display(Name = "Validation regex")] public string? ValidationRegex { get; set; }
        [Display(Name = "Max length")] public int? MaxLength { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RequirementInputModel
    {
        public Guid AdmissionFormTemplateId { get; set; }
        [Display(Name = "Document type")] public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;
        [StringLength(150), Display(Name = "Display name")] public string DisplayName { get; set; } = string.Empty;
        [Display(Name = "Required")] public bool IsRequired { get; set; } = true;
        [Display(Name = "Order")] public int SortOrder { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class DocumentInputModel
    {
        [Display(Name = "Document type")] public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;
        [StringLength(150), Display(Name = "Display name")] public string DisplayName { get; set; } = string.Empty;
        [StringLength(500)] public string? Notes { get; set; }
        public IFormFile? File { get; set; }
    }

    public class ReviewInputModel
    {
        [Display(Name = "Move to")] public AdmissionApplicationStatus ToStatus { get; set; } = AdmissionApplicationStatus.UnderReview;
        [StringLength(1000)] public string? Notes { get; set; }
    }

    public class EnrollmentInputModel
    {
        [Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Display(Name = "Batch")] public Guid? BatchId { get; set; }
        [Display(Name = "Section")] public Guid? SectionId { get; set; }
        [StringLength(50), Display(Name = "Admission #")] public string? AdmissionNumber { get; set; }
        [StringLength(50), Display(Name = "Enrollment #")] public string? EnrollmentNumber { get; set; }
        [Display(Name = "Enrollment date")] public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [StringLength(500)] public string? Notes { get; set; }
    }

    public class InterviewInputModel
    {
        [Display(Name = "Interviewer user id")] public Guid? InterviewerUserId { get; set; }
        [Display(Name = "Starts on")] public DateTime StartsOn { get; set; } = DateTime.Today.AddDays(1).AddHours(10);
        [Display(Name = "Ends on")] public DateTime EndsOn { get; set; } = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
        [StringLength(180)] public string? Location { get; set; }
        [StringLength(500), Display(Name = "Meeting link")] public string? MeetingLink { get; set; }
        [StringLength(1000)] public string? Notes { get; set; }
    }
}
