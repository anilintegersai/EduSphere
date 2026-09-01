using EduSphere.Domain.Entities;

namespace EduSphere.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
