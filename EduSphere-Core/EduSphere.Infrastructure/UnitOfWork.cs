using EduSphere.Domain.Interfaces;

namespace EduSphere.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly TenantDbContext _context;

    public UnitOfWork(TenantDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        _context.Dispose();
}