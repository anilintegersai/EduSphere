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
[Route("api/v{version:apiVersion}/sections")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class SectionsController : ApiControllerBase
{
    private readonly ICrudService<Section> _service;
    private readonly ICrudService<Batch> _batches;

    public SectionsController(ICrudService<Section> service, ICrudService<Batch> batches)
    {
        _service = service;
        _batches = batches;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? batchId)
    {
        var items = await _service.ListAsync(batchId is null ? null : s => s.BatchId == batchId);
        return Ok(ApiResponse<IEnumerable<SectionDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<SectionDto>.Fail($"Section {id} was not found."))
            : Ok(ApiResponse<SectionDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSectionRequest request)
    {
        if (await _batches.GetAsync(request.BatchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Batch {request.BatchId} was not found in this tenant."));

        var created = await _service.CreateAsync(new Section
        {
            Name = request.Name,
            Capacity = request.Capacity,
            BatchId = request.BatchId,
            ClassTeacherUserId = request.ClassTeacherUserId
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SectionDto>.Ok(Map(created)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSectionRequest request)
    {
        if (await _batches.GetAsync(request.BatchId) is null)
            return BadRequest(ApiResponse<object>.Fail($"Batch {request.BatchId} was not found in this tenant."));

        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Name = request.Name;
            e.Capacity = request.Capacity;
            e.BatchId = request.BatchId;
            e.ClassTeacherUserId = request.ClassTeacherUserId;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Section {id} was not found."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Section {id} was not found."));

    private static SectionDto Map(Section e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Name = e.Name,
        Capacity = e.Capacity,
        BatchId = e.BatchId,
        ClassTeacherUserId = e.ClassTeacherUserId,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
