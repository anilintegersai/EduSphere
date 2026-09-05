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
[Route("api/v{version:apiVersion}/enrollments")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class EnrollmentsController : ApiControllerBase
{
    private readonly ICrudService<Enrollment> _enrollments;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<AdmissionApplication> _applications;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly IBranchAccessService _branchAccess;

    public EnrollmentsController(
        ICrudService<Enrollment> enrollments,
        ICrudService<StudentProfile> students,
        ICrudService<AdmissionApplication> applications,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        IBranchAccessService branchAccess)
    {
        _enrollments = enrollments;
        _students = students;
        _applications = applications;
        _branches = branches;
        _academicYears = academicYears;
        _courses = courses;
        _batches = batches;
        _sections = sections;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? studentProfileId, [FromQuery] EnrollmentStatus? status)
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _enrollments.ListAsync(e =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && e.BranchId == assignedBranchId.Value)) &&
            (!studentProfileId.HasValue || e.StudentProfileId == studentProfileId.Value) &&
            (!status.HasValue || e.Status == status.Value));
        return Ok(ApiResponse<IEnumerable<EnrollmentDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _enrollments.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<EnrollmentDto>.Fail($"Enrollment {id} was not found."))
            : Ok(ApiResponse<EnrollmentDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _enrollments.CreateAsync(Apply(new Enrollment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<EnrollmentDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEnrollmentRequest request)
    {
        var existing = await _enrollments.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _enrollments.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Enrollment {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _enrollments.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Enrollment {id} was not found."));

        return await _enrollments.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Enrollment {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateEnrollmentRequest request)
    {
        var student = await _students.GetAsync(request.StudentProfileId);
        if (student is null)
            return $"Student {request.StudentProfileId} was not found in this tenant.";
        if (student.BranchId != request.BranchId)
            return "Student must belong to the enrollment branch.";
        if (request.AdmissionApplicationId is Guid appId && await _applications.GetAsync(appId) is null)
            return $"Admission application {appId} was not found in this tenant.";
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";
        if (await _academicYears.GetAsync(request.AcademicYearId) is null)
            return $"Academic year {request.AcademicYearId} was not found in this tenant.";
        if (await _courses.GetAsync(request.CourseId) is null)
            return $"Course {request.CourseId} was not found in this tenant.";
        if (await _batches.GetAsync(request.BatchId) is null)
            return $"Batch {request.BatchId} was not found in this tenant.";
        if (request.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            return $"Section {sectionId} was not found in this tenant.";
        return null;
    }

    private static Enrollment Apply(Enrollment entity, CreateEnrollmentRequest request)
    {
        entity.StudentProfileId = request.StudentProfileId;
        entity.AdmissionApplicationId = request.AdmissionApplicationId;
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.CourseId = request.CourseId;
        entity.BatchId = request.BatchId;
        entity.SectionId = request.SectionId;
        entity.EnrollmentNumber = request.EnrollmentNumber;
        entity.EnrollmentDate = request.EnrollmentDate;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }

    private static EnrollmentDto Map(Enrollment e) => new EnrollmentDto
    {
        StudentProfileId = e.StudentProfileId,
        AdmissionApplicationId = e.AdmissionApplicationId,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        SectionId = e.SectionId,
        EnrollmentNumber = e.EnrollmentNumber,
        EnrollmentDate = e.EnrollmentDate,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);
}
