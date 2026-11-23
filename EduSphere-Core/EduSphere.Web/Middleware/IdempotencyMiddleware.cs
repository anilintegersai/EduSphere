using System.Security.Cryptography;
using System.Text;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Middleware;

[AttributeUsage(AttributeTargets.Method)]
public sealed class IdempotentAttribute : Attribute { }

public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantDbContext db, ITenantContext tenant)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IdempotentAttribute>() is null)
        { await next(context); return; }

        var key = context.Request.Headers["Idempotency-Key"].ToString().Trim();
        if (string.IsNullOrEmpty(key) || key.Length > 100)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(EduSphere.Application.Common.ApiResponse<object>.Fail(
                "A valid Idempotency-Key header (maximum 100 characters) is required.", context.TraceIdentifier));
            return;
        }

        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, leaveOpen: true);
        var body = await reader.ReadToEndAsync(context.RequestAborted);
        context.Request.Body.Position = 0;
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body)));
        if (!tenant.TenantId.HasValue) { context.Response.StatusCode = 400; await context.Response.WriteAsJsonAsync(EduSphere.Application.Common.ApiResponse<object>.Fail("A tenant is required for idempotent commands.")); return; }
        var userId = Guid.TryParse(context.User.FindFirst("sub")?.Value, out var parsedUserId) ? parsedUserId : Guid.Empty;
        var method = context.Request.Method; var path = context.Request.Path.Value ?? "/";
        var cached = await db.ApiIdempotencyRecords.IgnoreQueryFilters().AsNoTracking().SingleOrDefaultAsync(x =>
            x.TenantId == tenant.TenantId.Value && x.UserId == userId && x.HttpMethod == method && x.RequestPath == path && x.IdempotencyKey == key && x.ExpiresOn > DateTime.UtcNow,
            context.RequestAborted);
        if (cached is not null)
        {
            if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(hash), Convert.FromHexString(cached.RequestHash)))
            { context.Response.StatusCode = 409; await context.Response.WriteAsJsonAsync(EduSphere.Application.Common.ApiResponse<object>.Fail("This idempotency key was already used with a different request.")); return; }
            context.Response.StatusCode = cached.StatusCode; context.Response.ContentType = cached.ContentType;
            await context.Response.Body.WriteAsync(cached.ResponseBody, context.RequestAborted); return;
        }

        var original = context.Response.Body;
        await using var buffer = new MemoryStream(); context.Response.Body = buffer;
        try
        {
            await next(context); buffer.Position = 0;
            var responseBytes = buffer.ToArray();
            if (context.Response.StatusCode is >= 200 and < 300)
            {
                db.ApiIdempotencyRecords.Add(new ApiIdempotencyRecord { TenantId = tenant.TenantId.Value, UserId = userId, IdempotencyKey = key,
                    HttpMethod = method, RequestPath = path, RequestHash = hash, StatusCode = context.Response.StatusCode,
                    ContentType = context.Response.ContentType ?? "application/json", ResponseBody = responseBytes, ExpiresOn = DateTime.UtcNow.AddHours(24) });
                await db.SaveChangesAsync(context.RequestAborted);
            }
            await original.WriteAsync(responseBytes, context.RequestAborted);
        }
        finally { context.Response.Body = original; }
    }
}
