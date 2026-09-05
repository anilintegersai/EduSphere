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
    private readonly ICrudService<AdmissionApplication> _applications;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly IBranchAccessService _branchAccess;

    public AdmissionApplicationsController(
        ICrudService<AdmissionApplication> applications,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        IBranchAccessService branchAccess)
    {
        _applications = applications;
        _branches = branches;
        _academicYears = academicYears;
        _courses = courses;
        _batches = batches;
        _sections = sections;
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
        return null;
    }

    private static AdmissionApplication Apply(AdmissionApplication entity, CreateAdmissionApplicationRequest request)
    {
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
        return entity;
    }

    private static AdmissionApplicationDto Map(AdmissionApplication e) => new AdmissionApplicationDto
    {
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
        ReviewNotes = e.ReviewNotes
    }.WithMetadata(e);
}
