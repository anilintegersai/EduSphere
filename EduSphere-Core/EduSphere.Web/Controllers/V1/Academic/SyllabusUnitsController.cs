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
[Route("api/v{version:apiVersion}/syllabus-units")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class SyllabusUnitsController : ApiControllerBase
{
    private readonly ICrudService<SyllabusUnit> _service;
    private readonly ICrudService<Subject> _subjects;

    public SyllabusUnitsController(ICrudService<SyllabusUnit> service, ICrudService<Subject> subjects)
    {
        _service = service;
        _subjects = subjects;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? subjectId)
    {
        var items = await _service.ListAsync(subjectId is null ? null : u => u.SubjectId == subjectId);
        return Ok(ApiResponse<IEnumerable<SyllabusUnitDto>>.Ok(items.OrderBy(u => u.Order).Select(Map)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<SyllabusUnitDto>.Fail($"Syllabus unit {id} was not found."))
            : Ok(ApiResponse<SyllabusUnitDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSyllabusUnitRequest request)
    {
        if (await _subjects.GetAsync(request.SubjectId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Subject {request.SubjectId} was not found in this tenant."));

        var created = await _service.CreateAsync(new SyllabusUnit
        {
            SubjectId = request.SubjectId,
            Order = request.Order,
            Title = request.Title,
            Description = request.Description,
            EstimatedHours = request.EstimatedHours
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SyllabusUnitDto>.Ok(Map(created)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSyllabusUnitRequest request)
    {
        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Order = request.Order;
            e.Title = request.Title;
            e.Description = request.Description;
            e.EstimatedHours = request.EstimatedHours;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Syllabus unit {id} was not found."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Syllabus unit {id} was not found."));

    private static SyllabusUnitDto Map(SyllabusUnit e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        SubjectId = e.SubjectId,
        Order = e.Order,
        Title = e.Title,
        Description = e.Description,
        EstimatedHours = e.EstimatedHours,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
