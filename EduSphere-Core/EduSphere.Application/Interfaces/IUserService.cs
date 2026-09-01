using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(string firstName, string lastName, string email, string password, int roleId, int branchId, string? phoneNumber = null);
    Task<bool> UpdateUserAsync(int id, string firstName, string lastName, string? phoneNumber, bool isActive);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> AssignUserToBranchAsync(int userId, int branchId);
    Task<bool> AssignUserToRoleAsync(int userId, int roleId);
}
