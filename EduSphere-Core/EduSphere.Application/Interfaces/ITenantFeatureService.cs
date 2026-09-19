namespace EduSphere.Application.Interfaces;

public interface ITenantFeatureService
{
    Task<bool> IsEnabledAsync(string featureKey, CancellationToken cancellationToken = default);
}
