using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admission-applications")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class AdmissionApplicationsController : ApiControllerBase
{
    private const long MaxUploadBytes = 20 * 1024 * 1024;

    private readonly ICrudService<AdmissionApplication> _applications;
    private readonly ICrudService<AdmissionFormTemplate> _forms;
    private readonly ICrudService<AdmissionDocument> _documents;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly IAdmissionWorkflowService _workflow;
    private readonly IAdmissionDocumentStorageService _storage;
    private readonly IBranchAccessService _branchAccess;

    public AdmissionApplicationsController(
        ICrudService<AdmissionApplication> applications,
        ICrudService<AdmissionFormTemplate> forms,
        ICrudService<AdmissionDocument> documents,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        IAdmissionWorkflowService workflow,
        IAdmissionDocumentStorageService storage,
        IBranchAccessService branchAccess)
    {
        _applications = applications;
        _forms = forms;
        _documents = documents;
        _branches = branches;
        _academicYears = academicYears;
        _courses = courses;
        _batches = batches;
        _sections = sections;
        _workflow = workflow;
        _storage = storage;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? courseId,
        [FromQuery] AdmissionApplicationStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _applications.ListAsync(a =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && a.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || a.BranchId == branchId.Value) &&
            (!courseId.HasValue || a.CourseId == courseId.Value) &&
            (!status.HasValue || a.Status == status.Value));
        return Ok(ApiResponse<IEnumerable<AdmissionApplicationDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _applications.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<AdmissionApplicationDto>.Fail($"Admission application {id} was not found."))
            : Ok(ApiResponse<AdmissionApplicationDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdmissionApplicationRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _applications.CreateAsync(Apply(new AdmissionApplication(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionApplicationDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdmissionApplicationRequest request)
    {
        var existing = await _applications.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Admission application {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _applications.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Admission application {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _applications.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Admission application {id} was not found."));

        return await _applications.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Admission application {id} was not found."));
    }

    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetDocuments(Guid id)
    {
        var documents = await _workflow.ListDocumentsAsync(User, id);
        return Ok(ApiResponse<IEnumerable<AdmissionDocumentDto>>.Ok(documents));
    }

    [HttpPost("{id:guid}/documents")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(Guid id, [FromForm] UploadAdmissionDocumentForm form)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("Select a file to upload."));
        if (form.File.Length > MaxUploadBytes)
            return BadRequest(ApiResponse<object>.Fail("Admission documents cannot exceed 20 MB."));

        await using var stream = form.File.OpenReadStream();
        var result = await _workflow.UploadDocumentAsync(
            User,
            id,
            new UploadAdmissionDocumentRequest
            {
                DocumentType = form.DocumentType,
                DisplayName = form.DisplayName,
                Notes = form.Notes
            },
            stream,
            form.File.FileName,
            form.File.ContentType,
            form.File.Length);

        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionDocumentDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("{id:guid}/documents/{documentId:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id, Guid documentId)
    {
        var application = await _applications.GetAsync(id);
        if (application is null || !await _branchAccess.CanAccessBranchAsync(User, application.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Admission application {id} was not found."));

        var document = await _documents.GetAsync(documentId);
        if (document is null || document.AdmissionApplicationId != id || string.IsNullOrWhiteSpace(document.StoragePath))
            return NotFound(ApiResponse<object>.Fail($"Admission document {documentId} was not found."));

        try
        {
            var stream = await _storage.OpenReadAsync(document.StoragePath);
            return File(
                stream,
                string.IsNullOrWhiteSpace(document.ContentType) ? "application/octet-stream" : document.ContentType,
                string.IsNullOrWhiteSpace(document.FileName) ? document.DisplayName : document.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(ApiResponse<object>.Fail($"Admission document {documentId} file was not found."));
        }
    }

    [HttpPost("{id:guid}/documents/{documentId:guid}/verification")]
    public async Task<IActionResult> SetDocumentVerification(Guid id, Guid documentId, [FromBody] VerifyAdmissionDocumentRequest request)
    {
        var result = await _workflow.SetDocumentVerificationAsync(User, id, documentId, request.IsVerified, request.Notes);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionDocumentDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("{id:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id)
    {
        var reviews = await _workflow.ListReviewsAsync(User, id);
        return Ok(ApiResponse<IEnumerable<AdmissionReviewDto>>.Ok(reviews));
    }

    [HttpGet("{id:guid}/interviews")]
    public async Task<IActionResult> GetInterviews(Guid id)
    {
        var interviews = await _workflow.ListInterviewsAsync(User, id);
        return Ok(ApiResponse<IEnumerable<AdmissionInterviewDto>>.Ok(interviews));
    }

    [HttpPost("{id:guid}/interviews")]
    public async Task<IActionResult> ScheduleInterview(Guid id, [FromBody] ScheduleAdmissionInterviewRequest request)
    {
        var result = await _workflow.ScheduleInterviewAsync(User, id, request);
        return result.Succeeded && result.Data is not null
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionInterviewDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPut("{id:guid}/interviews/{interviewId:guid}")]
    public async Task<IActionResult> UpdateInterview(Guid id, Guid interviewId, [FromBody] UpdateAdmissionInterviewRequest request)
    {
        var result = await _workflow.UpdateInterviewAsync(User, id, interviewId, request);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionInterviewDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("{id:guid}/reviews")]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewAdmissionApplicationRequest request)
    {
        var result = await _workflow.ReviewAsync(User, id, request);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionApplicationDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpGet("{id:guid}/finance-readiness")]
    public async Task<IActionResult> GetFinanceReadiness(Guid id)
    {
        var result = await _workflow.GetFinanceReadinessAsync(User, id);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionFinanceReadinessDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("{id:guid}/admission-fee-invoice")]
    public async Task<IActionResult> GenerateAdmissionFeeInvoice(Guid id)
    {
        var result = await _workflow.GenerateAdmissionFeeInvoiceAsync(User, id);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionFeeInvoiceDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("{id:guid}/enroll")]
    public async Task<IActionResult> ConvertToEnrollment(Guid id, [FromBody] ConvertAdmissionToEnrollmentRequest request)
    {
        var result = await _workflow.ConvertToEnrollmentAsync(User, id, request);
        return result.Succeeded && result.Data is not null
            ? Ok(ApiResponse<AdmissionEnrollmentResultDto>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    private async Task<string?> ValidateReferencesAsync(CreateAdmissionApplicationRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _courses.GetAsync(request.CourseId) is null)
            return $"Course {request.CourseId} was not found in this tenant.";
        if (request.AcademicYearId is Guid yearId && await _academicYears.GetAsync(yearId) is null)
            return $"Academic year {yearId} was not found in this tenant.";
        if (request.BatchId is Guid batchId && await _batches.GetAsync(batchId) is null)
            return $"Batch {batchId} was not found in this tenant.";
        if (request.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            return $"Section {sectionId} was not found in this tenant.";
        if (request.AdmissionFormTemplateId is Guid formId)
        {
            var form = await _forms.GetAsync(formId);
            if (form is null)
                return $"Admission form {formId} was not found in this tenant.";
            if (form.BranchId.HasValue && form.BranchId.Value != request.BranchId)
                return "Selected admission form is scoped to a different branch.";
            if (form.CourseId.HasValue && form.CourseId.Value != request.CourseId)
                return "Selected admission form is scoped to a different course.";
            if (form.AcademicYearId.HasValue && request.AcademicYearId != form.AcademicYearId.Value)
                return "Selected admission form is scoped to a different academic year.";
        }
        return null;
    }

    private static AdmissionApplication Apply(AdmissionApplication entity, CreateAdmissionApplicationRequest request)
    {
        entity.AdmissionFormTemplateId = request.AdmissionFormTemplateId;
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.CourseId = request.CourseId;
        entity.BatchId = request.BatchId;
        entity.SectionId = request.SectionId;
        entity.ApplicationNumber = request.ApplicationNumber;
        entity.ApplicantFirstName = request.ApplicantFirstName;
        entity.ApplicantMiddleName = request.ApplicantMiddleName;
        entity.ApplicantLastName = request.ApplicantLastName;
        entity.DateOfBirth = request.DateOfBirth;
        entity.Gender = request.Gender;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.GuardianName = request.GuardianName;
        entity.GuardianPhone = request.GuardianPhone;
        entity.Address = request.Address;
        entity.AppliedOn = request.AppliedOn;
        entity.Status = request.Status;
        entity.ReviewNotes = request.ReviewNotes;
        entity.FormResponseJson = request.FormResponseJson;
        return entity;
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

    public class UploadAdmissionDocumentForm
    {
        public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;
        public string DisplayName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public IFormFile? File { get; set; }
    }

    public class VerifyAdmissionDocumentRequest
    {
        public bool IsVerified { get; set; }
        public string? Notes { get; set; }
    }
}
