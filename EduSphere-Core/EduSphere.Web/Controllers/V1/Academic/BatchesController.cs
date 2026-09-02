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
[Route("api/v{version:apiVersion}/batches")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class BatchesController : ApiControllerBase
{
    private readonly ICrudService<Batch> _service;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<AcademicYear> _academicYears;

    public BatchesController(
        ICrudService<Batch> service,
        ICrudService<Course> courses,
        ICrudService<AcademicYear> academicYears)
    {
        _service = service;
        _courses = courses;
        _academicYears = academicYears;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? courseId, [FromQuery] int? academicYearId)
    {
        var items = await _service.ListAsync(b =>
            (courseId == null || b.CourseId == courseId) &&
            (academicYearId == null || b.AcademicYearId == academicYearId));
        return Ok(ApiResponse<IEnumerable<BatchDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<BatchDto>.Fail($"Batch {id} was not found."))
            : Ok(ApiResponse<BatchDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBatchRequest request)
    {
        var error = await ValidateReferences(request.CourseId, request.AcademicYearId);
        if (error is not null) return error;

        var created = await _service.CreateAsync(new Batch
        {
            Name = request.Name,
            Capacity = request.Capacity,
            CourseId = request.CourseId,
            AcademicYearId = request.AcademicYearId
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<BatchDto>.Ok(Map(created)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBatchRequest request)
    {
        var error = await ValidateReferences(request.CourseId, request.AcademicYearId);
        if (error is not null) return error;

        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Name = request.Name;
            e.Capacity = request.Capacity;
            e.CourseId = request.CourseId;
            e.AcademicYearId = request.AcademicYearId;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Batch {id} was not found."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Batch {id} was not found."));

    private async Task<IActionResult?> ValidateReferences(int courseId, int academicYearId)
    {
        if (await _courses.GetAsync(courseId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Course {courseId} was not found in this tenant."));
        if (await _academicYears.GetAsync(academicYearId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Academic year {academicYearId} was not found in this tenant."));
        return null;
    }

    private static BatchDto Map(Batch e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Name = e.Name,
        Capacity = e.Capacity,
        CourseId = e.CourseId,
        AcademicYearId = e.AcademicYearId,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
