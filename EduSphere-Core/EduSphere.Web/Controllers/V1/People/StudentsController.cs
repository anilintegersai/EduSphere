using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.People;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/students")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class StudentsController : ApiControllerBase
{
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Section> _sections;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;

    public StudentsController(
        ICrudService<StudentProfile> students,
        ICrudService<Branch> branches,
        ICrudService<Section> sections,
        UserManager<ApplicationUser> userManager,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess)
    {
        _students = students;
        _branches = branches;
        _sections = sections;
        _userManager = userManager;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? sectionId,
        [FromQuery] StudentStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _students.ListAsync(s =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || s.BranchId == branchId.Value) &&
            (!sectionId.HasValue || s.SectionId == sectionId.Value) &&
            (!status.HasValue || s.Status == status.Value));

        return Ok(ApiResponse<IEnumerable<StudentProfileDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _students.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<StudentProfileDto>.Fail($"Student {id} was not found."))
            : Ok(ApiResponse<StudentProfileDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentProfileRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _students.CreateAsync(Apply(new StudentProfile(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<StudentProfileDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentProfileRequest request)
    {
        var existing = await _students.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Student {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _students.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Student {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _students.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Student {id} was not found."));

        return await _students.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Student {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateStudentProfileRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";

        if (request.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            return $"Section {sectionId} was not found in this tenant.";

        if (request.UserId is not Guid userId)
            return null;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.TenantId != _tenantContext.TenantId)
            return $"User {userId} was not found in this tenant.";

        if (user.UserType != UserType.Student)
            return $"User {userId} is not a student user.";

        return null;
    }

    private static StudentProfile Apply(StudentProfile entity, CreateStudentProfileRequest request)
    {
        entity.UserId = request.UserId;
        entity.BranchId = request.BranchId;
        entity.SectionId = request.SectionId;
        entity.AdmissionNumber = request.AdmissionNumber;
        entity.RollNumber = request.RollNumber;
        entity.FirstName = request.FirstName;
        entity.MiddleName = request.MiddleName;
        entity.LastName = request.LastName;
        entity.DateOfBirth = request.DateOfBirth;
        entity.Gender = request.Gender;
        entity.BloodGroup = request.BloodGroup;
        entity.AdmissionDate = request.AdmissionDate;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.EmergencyContactName = request.EmergencyContactName;
        entity.EmergencyContactPhone = request.EmergencyContactPhone;
        entity.Address = request.Address;
        entity.Status = request.Status;
        return entity;
    }

    private static StudentProfileDto Map(StudentProfile e) => new StudentProfileDto
    {
        UserId = e.UserId,
        BranchId = e.BranchId,
        SectionId = e.SectionId,
        AdmissionNumber = e.AdmissionNumber,
        RollNumber = e.RollNumber,
        FirstName = e.FirstName,
        MiddleName = e.MiddleName,
        LastName = e.LastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        BloodGroup = e.BloodGroup,
        AdmissionDate = e.AdmissionDate,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        EmergencyContactName = e.EmergencyContactName,
        EmergencyContactPhone = e.EmergencyContactPhone,
        Address = e.Address,
        Status = e.Status
    }.WithMetadata(e);
}
