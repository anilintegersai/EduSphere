using EduSphere.Application.Common;
using System.Text.Json;

namespace EduSphere.Web.Middleware;

public sealed class ApiExceptionEnvelopeMiddleware(RequestDelegate next, ILogger<ApiExceptionEnvelopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api")) { await next(context); return; }
        try { await next(context); }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var (status, message) = exception switch
            {
                BadHttpRequestException bad when bad.StatusCode == StatusCodes.Status413PayloadTooLarge => (413, "The request exceeds the configured size limit."),
                KeyNotFoundException => (404, exception.Message),
                UnauthorizedAccessException => (403, exception.Message),
                ArgumentException => (400, exception.Message),
                InvalidOperationException => (409, exception.Message),
                _ => (500, "An unexpected error occurred while processing the request.")
            };
            logger.LogError(exception, "API request {TraceId} failed with status {StatusCode}", context.TraceIdentifier, status);
            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body,
                ApiResponse<object>.Fail(message, context.TraceIdentifier),
                new JsonSerializerOptions(JsonSerializerDefaults.Web), context.RequestAborted);
        }
    }
}
