using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
[Authorize(Policy = AdministrationPolicies.View)]
public sealed class ReportingController(TenantDbContext db, IReportingService reporting) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<object>>>> Catalog([FromQuery] ApiQueryOptions query, CancellationToken token)
    {
        var source = db.ReportDefinitions.AsNoTracking().Where(x => x.IsPublished);
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => x.Name.Contains(query.Search) || x.Code.Contains(query.Search));
        source = query.Descending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name);
        var total = await source.CountAsync(token);
        var rows = await source.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new { x.Id, x.Code, x.Name, x.Description, x.DataSource, x.BranchId, x.IsSystem }).ToListAsync(token);
        return Ok(ApiResponse<PagedResult<object>>.Ok(new(rows.Cast<object>().ToList(), query.Page, query.PageSize, total)));
    }

    [HttpPost("{id:guid}/execute")]
    public async Task<IActionResult> Execute(Guid id, [FromQuery] ReportOutputFormat format = ReportOutputFormat.Excel, CancellationToken token = default)
    {
        var execution = await reporting.ExecuteAndStoreAsync(id, format, token);
        if (execution.Status == ReportExecutionStatus.Failed) return Conflict(ApiResponse<object>.Fail(execution.ErrorMessage ?? "Report generation failed."));
        return Ok(ApiResponse<object>.Ok(new { execution.Id, execution.FileName, execution.RowCount, execution.CompletedOn }));
    }

    [HttpGet("executions/{id:guid}/download")]
    [Produces("application/octet-stream")]
    public async Task<IActionResult> Download(Guid id, CancellationToken token)
    {
        var item = await db.ReportExecutions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, token);
        if (item?.OutputContent is null) return NotFound(ApiResponse<object>.Fail("Report output was not found."));
        return File(item.OutputContent, item.ContentType ?? "application/octet-stream", item.FileName ?? $"report-{id}.bin");
    }
}
