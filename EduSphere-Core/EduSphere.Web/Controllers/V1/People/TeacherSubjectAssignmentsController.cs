using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.People;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/teacher-subject-assignments")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class TeacherSubjectAssignmentsController : ApiControllerBase
{
    private readonly ICrudService<TeacherSubjectAssignment> _assignments;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<Section> _sections;

    public TeacherSubjectAssignmentsController(
        ICrudService<TeacherSubjectAssignment> assignments,
        ICrudService<TeacherProfile> teachers,
        ICrudService<Subject> subjects,
        ICrudService<Section> sections)
    {
        _assignments = assignments;
        _teachers = teachers;
        _subjects = subjects;
        _sections = sections;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? teacherProfileId, [FromQuery] Guid? subjectId)
    {
        var items = await _assignments.ListAsync(a =>
            (!teacherProfileId.HasValue || a.TeacherProfileId == teacherProfileId.Value) &&
            (!subjectId.HasValue || a.SubjectId == subjectId.Value));
        return Ok(ApiResponse<IEnumerable<TeacherSubjectAssignmentDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _assignments.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<TeacherSubjectAssignmentDto>.Fail($"Teacher assignment {id} was not found."))
            : Ok(ApiResponse<TeacherSubjectAssignmentDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherSubjectAssignmentRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _assignments.CreateAsync(Apply(new TeacherSubjectAssignment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TeacherSubjectAssignmentDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherSubjectAssignmentRequest request)
    {
        var validationError = await ValidateReferencesAsync(request);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _assignments.UpdateAsync(id, e => Apply(e, request));
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Teacher assignment {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _assignments.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Teacher assignment {id} was not found."));

    private async Task<string?> ValidateReferencesAsync(CreateTeacherSubjectAssignmentRequest request)
    {
        if (await _teachers.GetAsync(request.TeacherProfileId) is null)
            return $"Teacher {request.TeacherProfileId} was not found in this tenant.";

        if (await _subjects.GetAsync(request.SubjectId) is null)
            return $"Subject {request.SubjectId} was not found in this tenant.";

        if (request.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            return $"Section {sectionId} was not found in this tenant.";

        return null;
    }

    private static TeacherSubjectAssignment Apply(
        TeacherSubjectAssignment entity,
        CreateTeacherSubjectAssignmentRequest request)
    {
        entity.TeacherProfileId = request.TeacherProfileId;
        entity.SubjectId = request.SubjectId;
        entity.SectionId = request.SectionId;
        entity.IsPrimary = request.IsPrimary;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveUntil = request.EffectiveUntil;
        return entity;
    }

    private static TeacherSubjectAssignmentDto Map(TeacherSubjectAssignment e) => new TeacherSubjectAssignmentDto
    {
        TeacherProfileId = e.TeacherProfileId,
        SubjectId = e.SubjectId,
        SectionId = e.SectionId,
        IsPrimary = e.IsPrimary,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveUntil = e.EffectiveUntil
    }.WithMetadata(e);
}
