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
[Route("api/v{version:apiVersion}/academic-years")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class AcademicYearsController : ApiControllerBase
{
    private readonly ICrudService<AcademicYear> _service;

    public AcademicYearsController(ICrudService<AcademicYear> service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<AcademicYearDto>>.Ok((await _service.ListAsync()).Select(Map)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _service.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<AcademicYearDto>.Fail($"Academic year {id} was not found."))
            : Ok(ApiResponse<AcademicYearDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicYearRequest request)
    {
        var created = await _service.CreateAsync(new AcademicYear
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AcademicYearDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicYearRequest request)
    {
        var updated = await _service.UpdateAsync(id, e =>
        {
            e.Name = request.Name;
            e.StartDate = request.StartDate;
            e.EndDate = request.EndDate;
            e.IsCurrent = request.IsCurrent;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Academic year {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Academic year {id} was not found."));

    private static AcademicYearDto Map(AcademicYear e) => new AcademicYearDto
    {
        Name = e.Name,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        IsCurrent = e.IsCurrent
    }.WithMetadata(e);
}
