namespace EduSphere.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITenantRepository TenantRepository { get; }
    IBranchRepository BranchRepository { get; }

    Task<int> SaveChangesAsync();
    Task<int> CommitAsync();
}
