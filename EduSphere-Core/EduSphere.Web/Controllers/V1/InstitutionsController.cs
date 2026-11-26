using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/institutions")]
[RequireTenant]
public sealed class InstitutionsController(TenantDbContext db, ITenantContext tenant) : ApiControllerBase
{
    private const string Managers = Roles.SuperAdmin + "," + Roles.TenantAdmin;
    private Guid TenantId => tenant.TenantId!.Value;

    [HttpGet("profile"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Profile(CancellationToken token) =>
        Ok(ApiResponse<object?>.Ok(await db.InstitutionProfiles.AsNoTracking().SingleOrDefaultAsync(token)));

    [HttpPut("profile"), Authorize(Roles = Managers)]
    public async Task<IActionResult> SaveProfile(ProfileInput input, CancellationToken token)
    {
        if (!Enum.IsDefined(input.Kind)) return BadRequest();
        var profile = await db.InstitutionProfiles.SingleOrDefaultAsync(token);
        if (profile is null)
        {
            profile = new InstitutionProfile { TenantId = TenantId };
            db.Add(profile);
        }
        profile.DisplayName = input.DisplayName.Trim();
        profile.Kind = input.Kind;
        profile.Jurisdiction = input.Jurisdiction;
        if (!await db.OrganizationUnits.AnyAsync(x => x.ParentId == null, token))
            db.OrganizationUnits.Add(new OrganizationUnit { Id = TenantId, TenantId = TenantId,
                Name = profile.DisplayName, Kind = OrganizationUnitKind.Institution });
        return await Save(profile, token);
    }

    [HttpGet("units"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Units(CancellationToken token) =>
        Ok(ApiResponse<object>.Ok(await db.OrganizationUnits.AsNoTracking().OrderBy(x => x.Name).ToListAsync(token)));

    [HttpPost("units"), Authorize(Roles = Managers)]
    public async Task<IActionResult> CreateUnit(UnitInput input, CancellationToken token)
    {
        if (!Enum.IsDefined(input.Kind) || input.Kind == OrganizationUnitKind.Institution) return BadRequest();
        if (!await db.OrganizationUnits.AnyAsync(x => x.Id == input.ParentId, token))
            return BadRequest(ApiResponse<object>.Fail("Parent unit must belong to the selected tenant."));
        if (input.BranchId is Guid branch && !await db.Branches.AnyAsync(x => x.Id == branch, token))
            return BadRequest(ApiResponse<object>.Fail("Branch must belong to the selected tenant."));
        var unit = new OrganizationUnit { TenantId = TenantId, ParentId = input.ParentId, BranchId = input.BranchId, Name = input.Name.Trim(), Kind = input.Kind };
        db.Add(unit);
        return await Save(unit, token);
    }

    [HttpPut("units/{id:guid}/permissions/{userId:guid}"), Authorize(Roles = Managers)]
    public async Task<IActionResult> SetPermission(Guid id, Guid userId, GrantInput input, CancellationToken token)
    {
        if (!await db.OrganizationUnits.AnyAsync(x => x.Id == id, token) ||
            !await db.Users.AnyAsync(x => x.Id == userId && x.TenantId == TenantId && x.IsActive, token))
            return NotFound();
        var grant = await db.OrganizationPermissions.SingleOrDefaultAsync(x =>
            x.OrganizationUnitId == id && x.UserId == userId && x.Permission == "policy.read", token);
        if (grant is null)
        {
            grant = new OrganizationPermission { TenantId = TenantId, OrganizationUnitId = id, UserId = userId, Permission = "policy.read" };
            db.Add(grant);
        }
        grant.IsActive = input.Enabled;
        return await Save(grant, token);
    }

    [HttpPost("units/{id:guid}/policies/{key}"), Authorize(Roles = Managers)]
    public async Task<IActionResult> PublishPolicy(Guid id, string key, PolicyInput input, CancellationToken token)
    {
        if (key.Length > 80 || input.Definition.ValueKind != JsonValueKind.Object)
            return BadRequest(ApiResponse<object>.Fail("Supply a policy key of up to 80 characters and a JSON object."));
        if (!await db.OrganizationUnits.AnyAsync(x => x.Id == id, token)) return NotFound();
        key = key.ToLowerInvariant();
        var previous = await db.InstitutionPolicyVersions.Where(x => x.OrganizationUnitId == id && x.PolicyKey == key)
            .OrderByDescending(x => x.Version).FirstOrDefaultAsync(token);
        if (previous is not null && input.EffectiveFrom <= previous.EffectiveFrom)
            return Conflict(ApiResponse<object>.Fail("Effective date must follow the previous version."));
        var policy = new InstitutionPolicyVersion { TenantId = TenantId, OrganizationUnitId = id, PolicyKey = key,
            EffectiveFrom = input.EffectiveFrom, Version = (previous?.Version ?? 0) + 1, DefinitionJson = input.Definition.GetRawText() };
        db.Add(policy);
        return await Save(policy, token);
    }

    [HttpGet("units/{id:guid}/policies/{key}")]
    public async Task<IActionResult> ResolvePolicy(Guid id, string key, [FromQuery] DateOnly? asOf, CancellationToken token)
    {
        if (!await db.OrganizationUnits.AnyAsync(x => x.Id == id, token)) return NotFound();
        if (!User.IsInRole(Roles.SuperAdmin) && !User.IsInRole(Roles.TenantAdmin))
        {
            if (!Guid.TryParse(User.FindFirstValue("sub"), out var userId) ||
                !await db.OrganizationPermissions.AnyAsync(x => x.OrganizationUnitId == id && x.UserId == userId
                    && x.Permission == "policy.read" && x.IsActive, token)) return Forbid();
        }
        var date = asOf ?? DateOnly.FromDateTime(DateTime.UtcNow);
        key = key.ToLowerInvariant();
        var policy = await db.InstitutionPolicyVersions.AsNoTracking()
            .Where(x => x.OrganizationUnitId == id && x.PolicyKey == key && x.EffectiveFrom <= date)
            .OrderByDescending(x => x.Version).FirstOrDefaultAsync(token);
        return policy is null ? NotFound() : Ok(ApiResponse<object>.Ok(policy));
    }

    [HttpGet("affiliations"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Affiliations(CancellationToken token) =>
        Ok(ApiResponse<object>.Ok(await VisibleAffiliations().AsNoTracking().ToListAsync(token)));

    [HttpGet("affiliations/{id:guid}/capability"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Capability(Guid id, CancellationToken token)
    {
        var link = await VisibleAffiliations().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id &&
            x.AcceptedOn != null && x.RevokedOn == null, token);
        if (link is null || await db.Tenants.CountAsync(x => x.IsActive &&
            (x.Id == link.TenantId || x.Id == link.AffiliateTenantId), token) != 2) return NotFound();
        var source = link.Capability == FederationCapability.CurriculumDistribution ? link.TenantId : link.AffiliateTenantId;
        return Ok(ApiResponse<object>.Ok(new { link.Capability, SourceTenantId = source,
            DestinationTenantId = source == link.TenantId ? link.AffiliateTenantId : link.TenantId }));
    }

    [HttpPost("affiliations"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Invite(AffiliationInput input, CancellationToken token)
    {
        if (!Enum.IsDefined(input.Capability) || input.AffiliateTenantId == TenantId ||
            !await db.Tenants.AnyAsync(x => x.Id == input.AffiliateTenantId && x.IsActive, token))
            return BadRequest(ApiResponse<object>.Fail("Select a different active tenant and a valid capability."));
        var link = new TenantAffiliation { TenantId = TenantId, AffiliateTenantId = input.AffiliateTenantId, Capability = input.Capability };
        db.Add(link);
        return await Save(link, token);
    }

    [HttpPost("affiliations/{id:guid}/accept"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken token)
    {
        var link = await VisibleAffiliations().SingleOrDefaultAsync(x => x.Id == id && x.AffiliateTenantId == TenantId, token);
        if (link is null) return NotFound();
        if (link.RevokedOn.HasValue) return Conflict();
        link.AcceptedOn ??= DateTime.UtcNow;
        return await Save(link, token);
    }

    [HttpPost("affiliations/{id:guid}/revoke"), Authorize(Roles = Managers)]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken token)
    {
        var link = await VisibleAffiliations().SingleOrDefaultAsync(x => x.Id == id, token);
        if (link is null) return NotFound();
        link.RevokedOn ??= DateTime.UtcNow;
        return await Save(link, token);
    }

    private IQueryable<TenantAffiliation> VisibleAffiliations() => db.TenantAffiliations.IgnoreQueryFilters()
        .Where(x => !x.IsDeleted && (x.TenantId == TenantId || x.AffiliateTenantId == TenantId));

    private async Task<IActionResult> Save(object data, CancellationToken token)
    {
        try { await db.SaveChangesAsync(token); }
        catch (DbUpdateConcurrencyException) { return Conflict(ApiResponse<object>.Fail("The record changed. Reload and retry.")); }
        return Ok(ApiResponse<object>.Ok(data));
    }

    public sealed record ProfileInput([Required, StringLength(200)] string DisplayName, InstitutionKind Kind, [StringLength(100)] string? Jurisdiction);
    public sealed record UnitInput(Guid ParentId, Guid? BranchId, [Required, StringLength(200)] string Name, OrganizationUnitKind Kind);
    public sealed record GrantInput(bool Enabled);
    public sealed record PolicyInput(DateOnly EffectiveFrom, JsonElement Definition);
    public sealed record AffiliationInput(Guid AffiliateTenantId, FederationCapability Capability);
}
