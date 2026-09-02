using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(TenantDbContext context) : base(context)
    {
    }
}
