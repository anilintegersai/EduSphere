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
[Route("api/v{version:apiVersion}/departments")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class DepartmentsController : ApiControllerBase
{
    private readonly ICrudService<Department> _service;

    public DepartmentsController(ICrudService<Department> service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<DepartmentDto>>.Ok((await _service.ListAsync()).Select(Map)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<DepartmentDto>.Fail($"Department {id} was not found."))
            : Ok(ApiResponse<DepartmentDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var created = await _service.CreateAsync(new Department
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<DepartmentDto>.Ok(Map(created)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequest request)
    {
        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Name = request.Name;
            e.Code = request.Code;
            e.Description = request.Description;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Department {id} was not found."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Department {id} was not found."));

    private static DepartmentDto Map(Department e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Name = e.Name,
        Code = e.Code,
        Description = e.Description,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
