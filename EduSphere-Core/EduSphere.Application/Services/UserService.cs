using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EduSphere.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId)
    {
        return await _unitOfWork.UserRepository.GetAllAsync(
            u => u.BranchId == branchId,
            include: u => u.Include(x => x.Role)
        );
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        return await _unitOfWork.UserRepository.GetAllAsync(
            u => u.RoleId == roleId,
            include: u => u.Include(x => x.Branch)
        );
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(id, 
            include: u => u.Include(x => x.Role).Include(x => x.Branch)
        );
    }

    public async Task<User> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        int roleId,
        int branchId,
        string? phoneNumber = null)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(null!, password);

        // Create user
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            RoleId = roleId,
            BranchId = branchId,
            PhoneNumber = phoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("User created: {UserEmail} in Branch {BranchId}", email, branchId);
        return user;
    }

    public async Task<bool> UpdateUserAsync(int id, string firstName, string lastName, string? phoneNumber, bool isActive)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;
        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("User updated: {UserId}", id);
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        // Soft delete
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("User deleted: {UserId}", id);
        return true;
    }

    public async Task<bool> AssignUserToBranchAsync(int userId, int branchId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.BranchId = branchId;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("User {UserId} assigned to Branch {BranchId}", userId, branchId);
        return true;
    }

    public async Task<bool> AssignUserToRoleAsync(int userId, int roleId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.RoleId = roleId;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("User {UserId} assigned to Role {RoleId}", userId, roleId);
        return true;
    }
}
