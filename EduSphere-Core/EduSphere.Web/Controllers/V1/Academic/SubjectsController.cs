using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Academic;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Academic;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subjects")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class SubjectsController : ApiControllerBase
{
    private readonly ICrudService<Subject> _service;
    private readonly ICrudService<Course> _courses;

    public SubjectsController(ICrudService<Subject> service, ICrudService<Course> courses)
    {
        _service = service;
        _courses = courses;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? courseId)
    {
        var items = await _service.ListAsync(courseId is null ? null : s => s.CourseId == courseId);
        return Ok(ApiResponse<IEnumerable<SubjectDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<SubjectDto>.Fail($"Subject {id} was not found."))
            : Ok(ApiResponse<SubjectDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
    {
        if (request.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Course {courseId} was not found in this tenant."));

        var created = await _service.CreateAsync(new Subject
        {
            Code = request.Code,
            Name = request.Name,
            Type = request.Type,
            Credits = request.Credits,
            CourseId = request.CourseId
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SubjectDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectRequest request)
    {
        if (request.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Course {courseId} was not found in this tenant."));

        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Code = request.Code;
            e.Name = request.Name;
            e.Type = request.Type;
            e.Credits = request.Credits;
            e.CourseId = request.CourseId;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Subject {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Subject {id} was not found."));

    private static SubjectDto Map(Subject e) => new SubjectDto
    {
        Code = e.Code,
        Name = e.Name,
        Type = e.Type,
        Credits = e.Credits,
        CourseId = e.CourseId
    }.WithMetadata(e);
}
