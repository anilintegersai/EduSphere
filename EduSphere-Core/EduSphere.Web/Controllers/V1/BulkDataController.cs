using System.Text;
using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bulk")]
[Authorize(Policy = AdministrationPolicies.ManageTenant)]
[RequestSizeLimit(10 * 1024 * 1024)]
public sealed class BulkDataController(TenantDbContext db, ITenantContext tenant) : ApiControllerBase
{
    [HttpGet("lookups/export")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportLookups([FromQuery] string? lookupType, CancellationToken token)
    {
        var query = db.LookupItems.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(lookupType)) query = query.Where(x => x.LookupType == lookupType);
        var rows = await query.OrderBy(x => x.LookupType).ThenBy(x => x.SortOrder).ToListAsync(token);
        var csv = new StringBuilder("LookupType,Code,DisplayName,SortOrder,IsActive,MetadataJson\r\n");
        foreach (var x in rows) csv.AppendLine(string.Join(',', Csv(x.LookupType), Csv(x.Code), Csv(x.DisplayName), x.SortOrder, x.IsActive, Csv(x.MetadataJson)));
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "lookup-master-data.csv");
    }

    [HttpPost("lookups/preview")]
    public async Task<IActionResult> PreviewLookups(IFormFile file, CancellationToken token)
    {
        var parsed = await Parse(file, token);
        return Ok(ApiResponse<object>.Ok(new { parsed.Rows, ValidRows = parsed.Rows.Count(x => x.Errors.Count == 0), InvalidRows = parsed.Rows.Count(x => x.Errors.Count > 0) }));
    }

    [HttpPost("lookups/import")]
    [Idempotent]
    public async Task<IActionResult> ImportLookups(IFormFile file, CancellationToken token)
    {
        if (!tenant.TenantId.HasValue) return BadRequest(ApiResponse<object>.Fail("Select a tenant."));
        var parsed = await Parse(file, token);
        if (parsed.Rows.Any(x => x.Errors.Count > 0)) return BadRequest(ApiResponse<object>.Fail("Import contains invalid rows. Run preview and correct the file first."));
        var changed = 0;
        foreach (var row in parsed.Rows)
        {
            var item = await db.LookupItems.SingleOrDefaultAsync(x => x.LookupType == row.LookupType && x.Code == row.Code, token)
                ?? new LookupItem { TenantId = tenant.TenantId.Value, LookupType = row.LookupType, Code = row.Code };
            if (item.Id == Guid.Empty) db.Add(item);
            item.DisplayName = row.DisplayName; item.SortOrder = row.SortOrder; item.IsActive = row.IsActive; item.MetadataJson = row.MetadataJson; changed++;
        }
        await db.SaveChangesAsync(token);
        return Ok(ApiResponse<object>.Ok(new { Imported = changed }, "Master data import completed."));
    }

    private static async Task<ParseResult> Parse(IFormFile file, CancellationToken token)
    {
        if (file.Length == 0) throw new ArgumentException("A non-empty CSV file is required.");
        var result = new ParseResult(); using var reader = new StreamReader(file.OpenReadStream());
        var header = await reader.ReadLineAsync(token);
        if (header is null || !header.Contains("LookupType", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("The CSV header is invalid. Export the template first.");
        var lineNo = 1; string? line;
        while ((line = await reader.ReadLineAsync(token)) is not null)
        {
            lineNo++; if (string.IsNullOrWhiteSpace(line)) continue; var values = SplitCsv(line);
            var row = new LookupImportRow { RowNumber = lineNo };
            if (values.Count < 3) row.Errors.Add("Expected at least LookupType, Code and DisplayName.");
            else
            {
                row.LookupType = values[0].Trim(); row.Code = values[1].Trim(); row.DisplayName = values[2].Trim();
                if (string.IsNullOrWhiteSpace(row.LookupType)) row.Errors.Add("LookupType is required.");
                if (string.IsNullOrWhiteSpace(row.Code)) row.Errors.Add("Code is required.");
                if (string.IsNullOrWhiteSpace(row.DisplayName)) row.Errors.Add("DisplayName is required.");
                if (values.Count > 3) { if (int.TryParse(values[3], out var order)) row.SortOrder = order; else row.Errors.Add("SortOrder must be an integer."); }
                if (values.Count > 4) { if (bool.TryParse(values[4], out var active)) row.IsActive = active; else row.Errors.Add("IsActive must be true or false."); }
                row.MetadataJson = values.Count > 5 && !string.IsNullOrWhiteSpace(values[5]) ? values[5] : "{}";
            }
            result.Rows.Add(row);
        }
        return result;
    }

    private static List<string> SplitCsv(string line) { var values = new List<string>(); var value = new StringBuilder(); var quoted = false; for (var i=0;i<line.Length;i++){var c=line[i]; if(c=='\"'){if(quoted&&i+1<line.Length&&line[i+1]=='\"'){value.Append('\"');i++;}else quoted=!quoted;}else if(c==','&&!quoted){values.Add(value.ToString());value.Clear();}else value.Append(c);} values.Add(value.ToString()); return values; }
    private static string Csv(string? value) => $"\"{(value ?? "").Replace("\"", "\"\"")}\"";
    private sealed class ParseResult { public List<LookupImportRow> Rows { get; } = []; }
    public sealed class LookupImportRow { public int RowNumber { get; set; } public string LookupType { get; set; } = ""; public string Code { get; set; } = ""; public string DisplayName { get; set; } = ""; public int SortOrder { get; set; } public bool IsActive { get; set; } = true; public string MetadataJson { get; set; } = "{}"; public List<string> Errors { get; } = []; }
}
