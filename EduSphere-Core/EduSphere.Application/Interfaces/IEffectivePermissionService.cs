namespace EduSphere.Application.Interfaces;

public interface IEffectivePermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string permissionKey, CancellationToken cancellationToken = default);
}
