using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(TenantDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Branch>> GetAllAsync(Func<Branch, bool>? predicate = null, Func<IQueryable<Branch>, IQueryable<Branch>>? include = null)
    {
        var query = Context.Branches.AsQueryable();
        
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

    public override async Task<Branch?> GetByIdAsync(int id, Func<IQueryable<Branch>, IQueryable<Branch>>? include = null)
    {
        var query = Context.Branches.AsQueryable();
        
        if (include != null)
        {
            query = include(query);
        }
        
        return await query.FirstOrDefaultAsync(b => b.Id == id);
    }
}
