using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Auth;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserManagementService _userManagement;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IUserManagementService userManagement)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _userManagement = userManagement;
    }

    /// <summary>Exchange email + password for a signed JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null ||
            !user.IsActive ||
            user.RequiresActivation ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid email or password."));

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            Email = user.Email!,
            Roles = roles
        }));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _userManagement.SendPasswordResetAsync(request.Email);
        return Ok(ApiResponse<object>.Ok(new { result.Warnings }, result.Message));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _userManagement.ResetPasswordAsync(request.UserId, request.Code, request.NewPassword);
        return result.Succeeded
            ? Ok(ApiResponse<object>.Ok(new { }, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("activate")]
    [AllowAnonymous]
    public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountRequest request)
    {
        var result = await _userManagement.ActivateAccountAsync(request.UserId, request.Code, request.NewPassword);
        return result.Succeeded
            ? Ok(ApiResponse<object>.Ok(new { }, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(idValue, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Invalid user identity."));

        var result = await _userManagement.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        return result.Succeeded
            ? Ok(ApiResponse<object>.Ok(new { }, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
