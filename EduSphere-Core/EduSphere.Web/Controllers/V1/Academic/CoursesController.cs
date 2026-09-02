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
[Route("api/v{version:apiVersion}/courses")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class CoursesController : ApiControllerBase
{
    private readonly ICrudService<Course> _service;
    private readonly ICrudService<Department> _departments;

    public CoursesController(ICrudService<Course> service, ICrudService<Department> departments)
    {
        _service = service;
        _departments = departments;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? departmentId)
    {
        var items = await _service.ListAsync(departmentId is null ? null : c => c.DepartmentId == departmentId);
        return Ok(ApiResponse<IEnumerable<CourseDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<CourseDto>.Fail($"Course {id} was not found."))
            : Ok(ApiResponse<CourseDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        if (request.DepartmentId is int deptId && await _departments.GetAsync(deptId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Department {deptId} was not found in this tenant."));

        var created = await _service.CreateAsync(new Course
        {
            Code = request.Code,
            Name = request.Name,
            Type = request.Type,
            DurationMonths = request.DurationMonths,
            DepartmentId = request.DepartmentId,
            Description = request.Description
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<CourseDto>.Ok(Map(created)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        if (request.DepartmentId is int deptId && await _departments.GetAsync(deptId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Department {deptId} was not found in this tenant."));

        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Code = request.Code;
            e.Name = request.Name;
            e.Type = request.Type;
            e.DurationMonths = request.DurationMonths;
            e.DepartmentId = request.DepartmentId;
            e.Description = request.Description;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Course {id} was not found."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Course {id} was not found."));

    private static CourseDto Map(Course e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Code = e.Code,
        Name = e.Name,
        Type = e.Type,
        DurationMonths = e.DurationMonths,
        DepartmentId = e.DepartmentId,
        Description = e.Description,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
