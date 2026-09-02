using System.Linq.Expressions;

namespace EduSphere.Application.Interfaces;

/// <summary>
/// Generic CRUD over a tenant-owned, auditable, soft-deletable entity. Tenant
/// scoping, TenantId stamping and audit timestamps are handled underneath, so
/// callers only supply the entity shape and an optional filter.
/// </summary>
public interface ICrudService<T> where T : class
{
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null);
    Task<T?> GetAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<bool> UpdateAsync(int id, Action<T> apply);
    Task<bool> SoftDeleteAsync(int id);
}
