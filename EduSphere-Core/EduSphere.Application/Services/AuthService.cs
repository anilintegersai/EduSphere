using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EduSphere.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
            u => u.Email == email,
            include: u => u.Include(x => x.Role).Include(x => x.Branch)
        );

        if (user == null || !user.IsActive)
        {
            return null;
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
            return user;
        }

        return null;
    }

    public async Task<User> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        int roleId,
        int branchId)
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("User registered successfully: {Email}", email);
        return user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Password changed for user: {UserId}", userId);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Password reset for user: {Email}", email);
        return true;
    }
}
