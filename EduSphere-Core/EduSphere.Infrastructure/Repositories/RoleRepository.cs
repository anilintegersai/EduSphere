using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(TenantDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Role>> GetAllAsync(Func<Role, bool>? predicate = null, Func<IQueryable<Role>, IQueryable<Role>>? include = null)
    {
        var query = Context.Roles.AsQueryable();
        
        if (include != null)
        {
            query = include(query);
        }
        
        if (predicate != null)
        {
            return query.Where(predicate).ToList();
        }
        
        return await query.ToListAsync();
    }

    public override async Task<Role?> GetByIdAsync(int id, Func<IQueryable<Role>, IQueryable<Role>>? include = null)
    {
        var query = Context.Roles.AsQueryable();
        
        if (include != null)
        {
            query = include(query);
        }
        
        return await query.FirstOrDefaultAsync(r => r.Id == id);
    }
}
