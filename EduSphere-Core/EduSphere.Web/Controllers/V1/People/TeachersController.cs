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
[Route("api/v{version:apiVersion}/teachers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.BranchAdmin)]
[RequireTenant]
public class TeachersController : ApiControllerBase
{
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Branch> _branches;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantContext _tenantContext;
    private readonly IBranchAccessService _branchAccess;

    public TeachersController(
        ICrudService<TeacherProfile> teachers,
        ICrudService<Branch> branches,
        UserManager<ApplicationUser> userManager,
        ITenantContext tenantContext,
        IBranchAccessService branchAccess)
    {
        _teachers = teachers;
        _branches = branches;
        _userManager = userManager;
        _tenantContext = tenantContext;
        _branchAccess = branchAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? branchId, [FromQuery] TeacherStatus? status)
    {
        if (branchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, branchId.Value))
            return Forbid();

        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        var items = await _teachers.ListAsync(t =>
            (!_branchAccess.IsBranchAdminOnly(User) || (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)) &&
            (!branchId.HasValue || t.BranchId == branchId.Value) &&
            (!status.HasValue || t.Status == status.Value));

        return Ok(ApiResponse<IEnumerable<TeacherProfileDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _teachers.GetAsync(id);
        return entity is null || !await _branchAccess.CanAccessBranchAsync(User, entity.BranchId)
            ? NotFound(ApiResponse<TeacherProfileDto>.Fail($"Teacher {id} was not found."))
            : Ok(ApiResponse<TeacherProfileDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherProfileRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var created = await _teachers.CreateAsync(Apply(new TeacherProfile(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TeacherProfileDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherProfileRequest request)
    {
        var existing = await _teachers.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Teacher {id} was not found."));

        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));
        if (!await _branchAccess.CanAccessBranchAsync(User, request.BranchId))
            return Forbid();

        var updated = await _teachers.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Teacher {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _teachers.GetAsync(id);
        if (existing is null || !await _branchAccess.CanAccessBranchAsync(User, existing.BranchId))
            return NotFound(ApiResponse<object>.Fail($"Teacher {id} was not found."));

        return await _teachers.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Teacher {id} was not found."));
    }

    private async Task<string?> ValidateReferencesAsync(CreateTeacherProfileRequest request)
    {
        if (await _branches.GetAsync(request.BranchId) is null)
            return $"Branch {request.BranchId} was not found in this tenant.";

        if (request.UserId is not Guid userId)
            return null;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.TenantId != _tenantContext.TenantId)
            return $"User {userId} was not found in this tenant.";

        if (user.UserType != UserType.Teacher)
            return $"User {userId} is not a teacher user.";

        return null;
    }

    private static TeacherProfile Apply(TeacherProfile entity, CreateTeacherProfileRequest request)
    {
        entity.UserId = request.UserId;
        entity.BranchId = request.BranchId;
        entity.EmployeeNumber = request.EmployeeNumber;
        entity.FirstName = request.FirstName;
        entity.MiddleName = request.MiddleName;
        entity.LastName = request.LastName;
        entity.DateOfBirth = request.DateOfBirth;
        entity.Gender = request.Gender;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Designation = request.Designation;
        entity.Qualifications = request.Qualifications;
        entity.Specializations = request.Specializations;
        entity.ExperienceYears = request.ExperienceYears;
        entity.JoiningDate = request.JoiningDate;
        entity.Status = request.Status;
        return entity;
    }

    private static TeacherProfileDto Map(TeacherProfile e) => new TeacherProfileDto
    {
        UserId = e.UserId,
        BranchId = e.BranchId,
        EmployeeNumber = e.EmployeeNumber,
        FirstName = e.FirstName,
        MiddleName = e.MiddleName,
        LastName = e.LastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Designation = e.Designation,
        Qualifications = e.Qualifications,
        Specializations = e.Specializations,
        ExperienceYears = e.ExperienceYears,
        JoiningDate = e.JoiningDate,
        Status = e.Status
    }.WithMetadata(e);
}
