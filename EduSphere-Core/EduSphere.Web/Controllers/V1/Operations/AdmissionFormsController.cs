using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Operations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Operations;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admission-forms")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class AdmissionFormsController : ApiControllerBase
{
    private readonly ICrudService<AdmissionFormTemplate> _forms;
    private readonly ICrudService<AdmissionFormField> _fields;
    private readonly ICrudService<AdmissionDocumentRequirement> _requirements;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Course> _courses;
    private readonly IAdmissionWorkflowService _workflow;
    private readonly IBranchAccessService _branchAccess;

    public AdmissionFormsController(
        ICrudService<AdmissionFormTemplate> forms,
        ICrudService<AdmissionFormField> fields,
        ICrudService<AdmissionDocumentRequirement> requirements,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Course> courses,
        IAdmissionWorkflowService workflow,
        IBranchAccessService branchAccess)
    {
        _forms = forms;
        _fields = fields;
        _requirements = requirements;
        _branches = branches;
        _academicYears = academicYears;
        _courses = courses;
        _workflow = workflow;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? branchId, [FromQuery] Guid? courseId)
    {
        var forms = await _workflow.ListFormTemplatesAsync(User, branchId, courseId);
        return Ok(ApiResponse<IEnumerable<AdmissionFormTemplateDto>>.Ok(forms));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var form = await _forms.GetAsync(id);
        if (form is null || !await CanUseFormAsync(form))
            return NotFound(ApiResponse<AdmissionFormTemplateDto>.Fail($"Admission form {id} was not found."));

        return Ok(ApiResponse<AdmissionFormTemplateDto>.Ok(Map(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdmissionFormTemplateRequest request)
    {
        var validationError = await ValidateFormReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _forms.CreateAsync(Apply(new AdmissionFormTemplate(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionFormTemplateDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdmissionFormTemplateRequest request)
    {
        var existing = await _forms.GetAsync(id);
        if (existing is null || !await CanManageFormAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        var validationError = await ValidateFormReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _forms.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _forms.GetAsync(id);
        if (existing is null || !await CanManageFormAsync(existing))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        return await _forms.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));
    }

    [HttpGet("{id:guid}/fields")]
    public async Task<IActionResult> GetFields(Guid id)
    {
        var form = await _forms.GetAsync(id);
        if (form is null || !await CanUseFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        return Ok(ApiResponse<IEnumerable<AdmissionFormFieldDto>>.Ok(await _workflow.ListFormFieldsAsync(id)));
    }

    [HttpPost("{id:guid}/fields")]
    public async Task<IActionResult> CreateField(Guid id, [FromBody] CreateAdmissionFormFieldRequest request)
    {
        request.AdmissionFormTemplateId = id;
        var form = await _forms.GetAsync(id);
        if (form is null || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        var created = await _fields.CreateAsync(Apply(new AdmissionFormField(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionFormFieldDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> UpdateField(Guid id, Guid fieldId, [FromBody] UpdateAdmissionFormFieldRequest request)
    {
        request.AdmissionFormTemplateId = id;
        var form = await _forms.GetAsync(id);
        var field = await _fields.GetAsync(fieldId);
        if (form is null || field is null || field.AdmissionFormTemplateId != id || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form field {fieldId} was not found."));

        var updated = await _fields.UpdateAsync(fieldId, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Admission form field {fieldId} was not found."));
    }

    [HttpDelete("{id:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> DeleteField(Guid id, Guid fieldId)
    {
        var form = await _forms.GetAsync(id);
        var field = await _fields.GetAsync(fieldId);
        if (form is null || field is null || field.AdmissionFormTemplateId != id || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form field {fieldId} was not found."));

        return await _fields.SoftDeleteAsync(fieldId)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Admission form field {fieldId} was not found."));
    }

    [HttpGet("{id:guid}/document-requirements")]
    public async Task<IActionResult> GetDocumentRequirements(Guid id)
    {
        var form = await _forms.GetAsync(id);
        if (form is null || !await CanUseFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        return Ok(ApiResponse<IEnumerable<AdmissionDocumentRequirementDto>>.Ok(await _workflow.ListDocumentRequirementsAsync(id)));
    }

    [HttpPost("{id:guid}/document-requirements")]
    public async Task<IActionResult> CreateDocumentRequirement(Guid id, [FromBody] CreateAdmissionDocumentRequirementRequest request)
    {
        request.AdmissionFormTemplateId = id;
        var form = await _forms.GetAsync(id);
        if (form is null || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission form {id} was not found."));

        var created = await _requirements.CreateAsync(Apply(new AdmissionDocumentRequirement(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AdmissionDocumentRequirementDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}/document-requirements/{requirementId:guid}")]
    public async Task<IActionResult> UpdateDocumentRequirement(
        Guid id,
        Guid requirementId,
        [FromBody] UpdateAdmissionDocumentRequirementRequest request)
    {
        request.AdmissionFormTemplateId = id;
        var form = await _forms.GetAsync(id);
        var requirement = await _requirements.GetAsync(requirementId);
        if (form is null || requirement is null || requirement.AdmissionFormTemplateId != id || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission document requirement {requirementId} was not found."));

        var updated = await _requirements.UpdateAsync(requirementId, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Admission document requirement {requirementId} was not found."));
    }

    [HttpDelete("{id:guid}/document-requirements/{requirementId:guid}")]
    public async Task<IActionResult> DeleteDocumentRequirement(Guid id, Guid requirementId)
    {
        var form = await _forms.GetAsync(id);
        var requirement = await _requirements.GetAsync(requirementId);
        if (form is null || requirement is null || requirement.AdmissionFormTemplateId != id || !await CanManageFormAsync(form))
            return NotFound(ApiResponse<object>.Fail($"Admission document requirement {requirementId} was not found."));

        return await _requirements.SoftDeleteAsync(requirementId)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Admission document requirement {requirementId} was not found."));
    }

    private async Task<string?> ValidateFormReferencesAsync(CreateAdmissionFormTemplateRequest request)
    {
        if (_branchAccess.IsBranchAdminOnly(User) && request.BranchId is null)
            return "Branch admins must scope admission forms to their assigned branch.";

        if (request.BranchId is Guid branchId)
        {
            if (await _branches.GetAsync(branchId) is null)
                return $"Branch {branchId} was not found in this tenant.";
            if (!await _branchAccess.CanAccessBranchAsync(User, branchId))
                return "You can manage admission forms only for your assigned branch.";
        }

        if (request.AcademicYearId is Guid yearId && await _academicYears.GetAsync(yearId) is null)
            return $"Academic year {yearId} was not found in this tenant.";
        if (request.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null)
            return $"Course {courseId} was not found in this tenant.";
        return null;
    }

    private async Task<bool> CanUseFormAsync(AdmissionFormTemplate form)
        => !form.BranchId.HasValue || await _branchAccess.CanAccessBranchAsync(User, form.BranchId.Value);

    private async Task<bool> CanManageFormAsync(AdmissionFormTemplate form)
    {
        if (!form.BranchId.HasValue)
            return !_branchAccess.IsBranchAdminOnly(User);

        return await _branchAccess.CanAccessBranchAsync(User, form.BranchId.Value);
    }

    private static AdmissionFormTemplate Apply(AdmissionFormTemplate entity, CreateAdmissionFormTemplateRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.CourseId = request.CourseId;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Instructions = request.Instructions;
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        return entity;
    }

    private static AdmissionFormField Apply(AdmissionFormField entity, CreateAdmissionFormFieldRequest request)
    {
        entity.AdmissionFormTemplateId = request.AdmissionFormTemplateId;
        entity.FieldKey = request.FieldKey;
        entity.Label = request.Label;
        entity.FieldType = request.FieldType;
        entity.IsRequired = request.IsRequired;
        entity.SortOrder = request.SortOrder;
        entity.Placeholder = request.Placeholder;
        entity.HelpText = request.HelpText;
        entity.OptionsJson = request.OptionsJson;
        entity.ValidationRegex = request.ValidationRegex;
        entity.MaxLength = request.MaxLength;
        entity.IsActive = request.IsActive;
        return entity;
    }

    private static AdmissionDocumentRequirement Apply(AdmissionDocumentRequirement entity, CreateAdmissionDocumentRequirementRequest request)
    {
        entity.AdmissionFormTemplateId = request.AdmissionFormTemplateId;
        entity.DocumentType = request.DocumentType;
        entity.DisplayName = request.DisplayName;
        entity.IsRequired = request.IsRequired;
        entity.SortOrder = request.SortOrder;
        entity.Notes = request.Notes;
        entity.IsActive = request.IsActive;
        return entity;
    }

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
}
