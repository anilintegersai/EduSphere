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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.People;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/student-guardians")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class StudentGuardiansController : ApiControllerBase
{
    private readonly ICrudService<StudentGuardian> _guardians;
    private readonly ICrudService<StudentProfile> _students;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantContext _tenantContext;

    public StudentGuardiansController(
        ICrudService<StudentGuardian> guardians,
        ICrudService<StudentProfile> students,
        UserManager<ApplicationUser> userManager,
        ITenantContext tenantContext)
    {
        _guardians = guardians;
        _students = students;
        _userManager = userManager;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? studentProfileId)
    {
        var items = await _guardians.ListAsync(g =>
            !studentProfileId.HasValue || g.StudentProfileId == studentProfileId.Value);
        return Ok(ApiResponse<IEnumerable<StudentGuardianDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _guardians.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<StudentGuardianDto>.Fail($"Guardian {id} was not found."))
            : Ok(ApiResponse<StudentGuardianDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentGuardianRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _guardians.CreateAsync(Apply(new StudentGuardian(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<StudentGuardianDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentGuardianRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _guardians.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Guardian {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _guardians.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Guardian {id} was not found."));

    private async Task<string?> ValidateReferencesAsync(CreateStudentGuardianRequest request)
    {
        if (await _students.GetAsync(request.StudentProfileId) is null)
            return $"Student {request.StudentProfileId} was not found in this tenant.";

        if (request.ParentUserId is not Guid parentUserId)
            return null;

        var user = await _userManager.FindByIdAsync(parentUserId.ToString());
        if (user is null || user.TenantId != _tenantContext.TenantId)
            return $"Parent user {parentUserId} was not found in this tenant.";

        if (user.UserType != UserType.Parent)
            return $"User {parentUserId} is not a parent user.";

        return null;
    }

    private static StudentGuardian Apply(StudentGuardian entity, CreateStudentGuardianRequest request)
    {
        entity.StudentProfileId = request.StudentProfileId;
        entity.ParentUserId = request.ParentUserId;
        entity.Relationship = request.Relationship;
        entity.FullName = request.FullName;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Occupation = request.Occupation;
        entity.IsPrimary = request.IsPrimary;
        entity.HasPortalAccess = request.HasPortalAccess;
        entity.CanPickup = request.CanPickup;
        return entity;
    }

    private static StudentGuardianDto Map(StudentGuardian e) => new StudentGuardianDto
    {
        StudentProfileId = e.StudentProfileId,
        ParentUserId = e.ParentUserId,
        Relationship = e.Relationship,
        FullName = e.FullName,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Occupation = e.Occupation,
        IsPrimary = e.IsPrimary,
        HasPortalAccess = e.HasPortalAccess,
        CanPickup = e.CanPickup
    }.WithMetadata(e);
}
