using System.Security.Claims;
using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Infrastructure;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/applicant")]
[Produces("application/json")]
[EnableRateLimiting("public-applicant")]
public sealed class PublicApplicantController(TenantDbContext db) : ControllerBase
{
    [HttpGet("forms")]
    [AllowAnonymous]
    public async Task<IActionResult> Forms([FromQuery] Guid tenantId, [FromQuery] Guid? branchId, [FromQuery] Guid? courseId, CancellationToken token)
    {
        if (tenantId == Guid.Empty) return BadRequest(ApiResponse<object>.Fail("tenantId is required."));
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var rows = await db.AdmissionFormTemplates.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive && (!branchId.HasValue || x.BranchId == null || x.BranchId == branchId) && (!courseId.HasValue || x.CourseId == null || x.CourseId == courseId)
                && (!x.EffectiveFrom.HasValue || x.EffectiveFrom <= today) && (!x.EffectiveTo.HasValue || x.EffectiveTo >= today))
            .Include(x => x.Fields.Where(f => f.IsActive)).Include(x => x.DocumentRequirements.Where(r => r.IsActive))
            .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).Take(50)
            .Select(x => new { x.Id, x.Name, x.Description, x.Instructions, x.BranchId, x.CourseId,
                Fields = x.Fields.OrderBy(f => f.SortOrder).Select(f => new { f.FieldKey, f.Label, f.FieldType, f.IsRequired, f.Placeholder, f.HelpText, f.OptionsJson, f.ValidationRegex, f.MaxLength }),
                Documents = x.DocumentRequirements.OrderBy(r => r.SortOrder).Select(r => new { r.DocumentType, r.DisplayName, r.IsRequired, r.Notes }) }).ToListAsync(token);
        return Ok(ApiResponse<object>.Ok(rows));
    }

    [HttpGet("applications")]
    [Authorize(AuthenticationSchemes = "Applicant")]
    public async Task<IActionResult> Applications(CancellationToken token)
    {
        if (!TryIdentity(out var accountId, out var tenantId)) return Unauthorized(ApiResponse<object>.Fail("Applicant session is invalid."));
        var rows = await db.AdmissionApplications.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.ApplicantAccountId == accountId)
            .OrderByDescending(x => x.LastSavedOn ?? x.SubmittedOn)
            .Select(x => new { x.Id, x.ApplicationNumber, x.ApplicantFirstName, x.ApplicantLastName, x.Status, x.LastSavedOn, x.SubmittedOn, x.OfferExpiresOn, x.FeeDueOn, x.WaitlistPosition, x.ReviewNotes }).ToListAsync(token);
        return Ok(ApiResponse<object>.Ok(rows));
    }

    [HttpGet("applications/{id:guid}")]
    [Authorize(AuthenticationSchemes = "Applicant")]
    public async Task<IActionResult> Application(Guid id, CancellationToken token)
    {
        if (!TryIdentity(out var accountId, out var tenantId)) return Unauthorized(ApiResponse<object>.Fail("Applicant session is invalid."));
        var row = await db.AdmissionApplications.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == id && x.TenantId == tenantId && x.ApplicantAccountId == accountId)
            .Select(x => new { x.Id, x.ApplicationNumber, x.Status, x.FormResponseJson, x.LastSavedOn, x.SubmittedOn, x.OfferExpiresOn, x.FeeDueOn, x.ReviewNotes }).SingleOrDefaultAsync(token);
        return row is null ? NotFound(ApiResponse<object>.Fail("Application was not found.")) : Ok(ApiResponse<object>.Ok(row));
    }

    private bool TryIdentity(out Guid accountId, out Guid tenantId)
    {
        var hasAccount = Guid.TryParse(User.FindFirstValue(ApplicantClaimTypes.AccountId), out accountId);
        var hasTenant = Guid.TryParse(User.FindFirstValue(ApplicantClaimTypes.TenantId), out tenantId);
        return hasAccount && hasTenant;
    }
}
