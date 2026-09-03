using System.Linq.Expressions;
using EduSphere.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TenantDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(TenantDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = Context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = DbSet;
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);
        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(
        Guid id,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        // Use a LINQ query (not DbSet.Find) so global query filters — including the
        // tenant filter — are honored and a by-id lookup can't cross tenant boundaries.
        IQueryable<T> query = DbSet;
        if (include != null) query = include(query);
        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public virtual async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = DbSet;
        if (include != null) query = include(query);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.Where(predicate).ToListAsync();

    public virtual async Task AddAsync(T entity)
        => await DbSet.AddAsync(entity);

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void Remove(T entity)
        => DbSet.Remove(entity);

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.AnyAsync(predicate);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.CountAsync(predicate);
}
