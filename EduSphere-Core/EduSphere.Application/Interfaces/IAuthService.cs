using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(string firstName, string lastName, string email, string password, int roleId, int branchId);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email, string newPassword);
}
