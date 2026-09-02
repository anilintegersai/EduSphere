using System.Net;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using EduSphere.Domain.MultiTenancy;

namespace EduSphere.Web.Middleware;

/// <summary>
/// Resolves the current tenant once per request and places it in the request-scoped
/// <see cref="ITenantContext"/>. Strategies, in priority order:
///   1. Header      X-Tenant-ID: &lt;identifier&gt;         (API clients)
///   2. Subdomain   tenant1.&lt;BaseDomain&gt;               (requires MultiTenancy:BaseDomain)
///   3. Path        /t/{identifier}/...                  (fallback)
///   4. Custom      full host matched against Tenant.CustomDomain
/// A database lookup only runs when one of the strategies yields a candidate, so
/// requests to localhost / the apex domain never touch the database.
/// </summary>
public class TenantResolutionMiddleware
{
    public const string TenantHeader = "X-Tenant-ID";
    public const string TenantCookie = "edusphere_tenant";

    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;
    private readonly string? _baseDomain;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _baseDomain = configuration["MultiTenancy:BaseDomain"]?.Trim().TrimStart('.');
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, ITenantRepository tenantRepository)
    {
        var host = context.Request.Host.Host;

        var identifier = FromHeader(context)
                         ?? FromSubdomain(host)
                         ?? FromPath(context)
                         ?? FromCookie(context); // browser sessions (tenant switcher)

        Tenant? tenant = null;
        if (!string.IsNullOrWhiteSpace(identifier))
        {
            tenant = await tenantRepository.GetByIdentifierAsync(identifier);
        }
        else if (IsCustomDomainCandidate(host))
        {
            tenant = await tenantRepository.GetByCustomDomainAsync(host);
        }

        if (tenant is { IsActive: true })
        {
            tenantContext.SetTenant(tenant.Id, tenant.TenantIdentifier);
            _logger.LogDebug("Resolved tenant '{Identifier}' (id {TenantId}) for host {Host}",
                tenant.TenantIdentifier, tenant.Id, host);
        }
        else if (tenant is { IsActive: false })
        {
            _logger.LogWarning("Tenant '{Identifier}' is suspended; request left without tenant context.", identifier);
        }

        await _next(context);
    }

    private static string? FromHeader(HttpContext context)
        => context.Request.Headers.TryGetValue(TenantHeader, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString().Trim()
            : null;

    private static string? FromCookie(HttpContext context)
        => context.Request.Cookies.TryGetValue(TenantCookie, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : null;

    private string? FromSubdomain(string host)
    {
        if (string.IsNullOrEmpty(_baseDomain)) return null;
        if (!host.EndsWith("." + _baseDomain, StringComparison.OrdinalIgnoreCase)) return null;

        var prefix = host[..^(_baseDomain.Length + 1)];
        if (string.IsNullOrEmpty(prefix)) return null;

        var label = prefix.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrEmpty(label) || label.Equals("www", StringComparison.OrdinalIgnoreCase) ? null : label;
    }

    private static string? FromPath(HttpContext context)
    {
        var segments = context.Request.Path.Value?.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments is { Length: >= 2 } && segments[0].Equals("t", StringComparison.OrdinalIgnoreCase))
            return segments[1];
        return null;
    }

    private bool IsCustomDomainCandidate(string host)
    {
        if (string.IsNullOrWhiteSpace(host)) return false;
        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return false;
        if (IPAddress.TryParse(host, out _)) return false;
        if (!host.Contains('.')) return false;
        // A host under our own base domain is handled by subdomain resolution, not custom-domain.
        if (!string.IsNullOrEmpty(_baseDomain) &&
            host.EndsWith(_baseDomain, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }
}
