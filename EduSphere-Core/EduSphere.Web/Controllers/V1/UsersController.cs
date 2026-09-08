using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.UserManagement;
using EduSphere.Application.Interfaces;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.UserManager)]
public class UsersController : ApiControllerBase
{
    private readonly IUserManagementService _users;

    public UsersController(IUserManagementService users)
    {
        _users = users;
    }

    [HttpGet("roles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleOptionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignableRoles(CancellationToken cancellationToken)
    {
        var roles = await _users.GetAssignableRolesAsync(User, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RoleOptionDto>>.Ok(roles));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] UserManagementQuery query, CancellationToken cancellationToken)
    {
        var users = await _users.ListUsersAsync(User, query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<UserSummaryDto>>.Ok(users));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateManagedUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _users.CreateUserAsync(User, request, cancellationToken);
        return result.Succeeded
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<UserSummaryDto>.Ok(result.Data!, result.Message))
            : BadRequest(ApiResponse<UserSummaryDto>.Fail(result.Errors));
    }

    [HttpPost("bulk-import")]
    [ProducesResponseType(typeof(ApiResponse<BulkUserImportResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BulkUserImportResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkImport([FromBody] BulkUserImportRequest request, CancellationToken cancellationToken)
    {
        var result = await _users.BulkImportUsersAsync(User, request, cancellationToken);
        return result.Succeeded
            ? Ok(ApiResponse<BulkUserImportResult>.Ok(result.Data!, result.Message))
            : BadRequest(ApiResponse<BulkUserImportResult>.Fail(result.Errors));
    }

    [HttpPost("{id:guid}/activation/resend")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendActivation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.ResendActivationAsync(User, id, cancellationToken);
        return result.Succeeded
            ? Ok(ApiResponse<object>.Ok(new { result.Warnings }, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }

    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] UpdateManagedUserStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _users.SetUserActiveAsync(User, id, request.IsActive, cancellationToken);
        return result.Succeeded
            ? Ok(ApiResponse<object>.Ok(new { }, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Errors));
    }
}
