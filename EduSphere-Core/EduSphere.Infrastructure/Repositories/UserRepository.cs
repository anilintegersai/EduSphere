using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(TenantDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<User>> GetAllAsync(Func<User, bool>? predicate = null, Func<IQueryable<User>, IQueryable<User>>? include = null)
    {
        var query = Context.Users.AsQueryable();
        
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

    public override async Task<User?> GetByIdAsync(int id, Func<IQueryable<User>, IQueryable<User>>? include = null)
    {
        var query = Context.Users.AsQueryable();
        
        if (include != null)
        {
            query = include(query);
        }
        
        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Users
            .Include(u => u.Role)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
