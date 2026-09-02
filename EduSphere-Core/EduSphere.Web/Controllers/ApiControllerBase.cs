using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers;

/// <summary>
/// Base for all /api/v1 controllers: JSON, model-state validation, and JWT bearer
/// authentication by default (actions opt out with [AllowAnonymous]).
/// </summary>
[ApiController]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class ApiControllerBase : ControllerBase
{
}
