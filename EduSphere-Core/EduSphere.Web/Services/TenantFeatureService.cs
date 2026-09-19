using EduSphere.Application.Interfaces;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class TenantFeatureService(TenantDbContext db) : ITenantFeatureService
{
    private Task<Dictionary<string, EduSphere.Domain.Entities.TenantFeatureFlag>>? _flags;

    public async Task<bool> IsEnabledAsync(string featureKey, CancellationToken cancellationToken = default)
    {
        _flags ??= db.TenantFeatureFlags.AsNoTracking().ToDictionaryAsync(f => f.FeatureKey, StringComparer.OrdinalIgnoreCase, cancellationToken);
        var flags = await _flags;
        if (!flags.TryGetValue(featureKey, out var flag)) return true;
        var now = DateTime.UtcNow;
        return flag.IsEnabled && (!flag.EnabledFrom.HasValue || flag.EnabledFrom <= now) && (!flag.EnabledUntil.HasValue || flag.EnabledUntil >= now);
    }
}
