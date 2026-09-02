using EduSphere.Application.Common;
using EduSphere.Domain.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduSphere.Web.Filters;

/// <summary>
/// Short-circuits with 400 when no tenant is resolved for the request. Applied to
/// tenant-scoped API controllers so reads are never silently empty and writes are
/// never stamped with tenant 0.
/// </summary>
public class RequireTenantAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var tenant = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();
        if (!tenant.HasTenant)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Fail("A tenant must be specified via the X-Tenant-ID header."));
        }
    }
}
