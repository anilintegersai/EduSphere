using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Authorize(Policy = AdministrationPolicies.View)]
public sealed class SettingsController(TenantDbContext db, ITenantContext tenant) : ApiControllerBase
{
    [HttpGet("tenant")]
    public async Task<IActionResult> TenantSettings([FromQuery] ApiQueryOptions query, CancellationToken token)
    {
        var source = db.TenantSettings.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => x.Key.Contains(query.Search));
        var total = await source.CountAsync(token);
        var rows = await source.OrderBy(x => x.Key).Skip((query.Page-1)*query.PageSize).Take(query.PageSize)
            .Select(x => new { x.Id, x.Key, Value = x.IsSensitive ? "********" : x.Value, x.ValueType, x.Description, x.IsSensitive }).ToListAsync(token);
        return Ok(ApiResponse<PagedResult<object>>.Ok(new(rows.Cast<object>().ToList(), query.Page, query.PageSize, total)));
    }

    [HttpPut("tenant/{key}")]
    [Authorize(Policy = AdministrationPolicies.ManageTenant)]
    public async Task<IActionResult> PutTenantSetting(string key, SettingRequest request, CancellationToken token)
    {
        if (!tenant.TenantId.HasValue) return BadRequest(ApiResponse<object>.Fail("Select a tenant."));
        var item = await db.TenantSettings.SingleOrDefaultAsync(x => x.Key == key, token) ?? new TenantSetting { TenantId = tenant.TenantId.Value, Key = key };
        if (item.Id == Guid.Empty) db.Add(item);
        item.Value = request.Value; item.ValueType = request.ValueType; item.Description = request.Description; item.IsSensitive = request.IsSensitive;
        await db.SaveChangesAsync(token); return Ok(ApiResponse<Guid>.Ok(item.Id));
    }

    [HttpGet("integrations")]
    public async Task<IActionResult> Integrations([FromQuery] ApiQueryOptions query, CancellationToken token)
    {
        var source = db.TenantIntegrationSettings.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => x.DisplayName.Contains(query.Search) || x.IntegrationKey.Contains(query.Search));
        var total = await source.CountAsync(token);
        var rows = await source.OrderBy(x => x.DisplayName).Skip((query.Page-1)*query.PageSize).Take(query.PageSize)
            .Select(x => new { x.Id, x.IntegrationKey, x.DisplayName, x.Category, x.IsEnabled, x.ConfigurationJson, x.SecretHint, x.LastTestedOn, x.LastTestSucceeded }).ToListAsync(token);
        return Ok(ApiResponse<PagedResult<object>>.Ok(new(rows.Cast<object>().ToList(), query.Page, query.PageSize, total)));
    }

    public sealed record SettingRequest(string Value, SettingValueType ValueType, string? Description, bool IsSensitive);
}
