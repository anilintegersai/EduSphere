using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly TenantDbContext _context;

    public ITenantRepository TenantRepository { get; }
    public IBranchRepository BranchRepository { get; }

    public UnitOfWork(
        TenantDbContext context,
        ITenantRepository tenantRepository,
        IBranchRepository branchRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        TenantRepository = tenantRepository;
        BranchRepository = branchRepository;
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task<int> CommitAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
