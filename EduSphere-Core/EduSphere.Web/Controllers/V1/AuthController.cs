using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Auth;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
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
    private readonly TenantDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IUserManagementService userManagement,
        TenantDbContext db)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _userManagement = userManagement;
        _db = db;
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

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            if (string.IsNullOrWhiteSpace(request.MfaCode))
                return Unauthorized(ApiResponse<LoginResponse>.Fail("MFA code is required."));
            var code = request.MfaCode.Replace(" ", string.Empty);
            var valid = request.UseRecoveryCode
                ? (await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, code)).Succeeded
                : await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code.Replace("-", string.Empty));
            if (!valid)
            {
                await RecordSecurityEventAsync(user, SecurityEventType.MfaFailed, false);
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid MFA code."));
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await RecordSecurityEventAsync(user, SecurityEventType.LoginSucceeded, true);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            Email = user.Email!,
            Roles = roles
        }));
    }

    private async Task RecordSecurityEventAsync(ApplicationUser user, SecurityEventType eventType, bool succeeded)
    {
        _db.SecurityEvents.Add(new SecurityEvent { TenantId = user.TenantId, UserId = user.Id, Email = user.Email, EventType = eventType, Succeeded = succeeded, IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), UserAgent = Request.Headers.UserAgent.ToString(), OccurredOn = DateTime.UtcNow });
        await _db.SaveChangesAsync();
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
