using EduSphere.Application.Common;

namespace EduSphere.Web.Middleware;

public sealed class ApiStatusCodeEnvelopeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);
        if (!context.Request.Path.StartsWithSegments("/api") || context.Response.HasStarted ||
            context.Response.ContentLength.HasValue || context.Response.StatusCode < 400) return;
        var message = context.Response.StatusCode switch
        {
            401 => "Authentication is required.", 403 => "You do not have permission to perform this operation.",
            404 => "The requested API resource was not found.", 405 => "The HTTP method is not supported.",
            413 => "The request exceeds the configured size limit.", 429 => "Too many requests. Please retry later.",
            _ => "The request could not be completed."
        };
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message, context.TraceIdentifier), context.RequestAborted);
    }
}
