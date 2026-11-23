using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EduSphere.Web.Swagger;

public sealed class ApiConventionsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Validation failed; response uses the standard API error envelope." });
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "A valid bearer token is required." });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "The caller lacks the required tenant permission." });
        operation.Responses.TryAdd("429", new OpenApiResponse { Description = "Tenant/user/IP rate limit exceeded." });
        if (context.ApiDescription.HttpMethod is "POST" or "PUT" or "PATCH" &&
            context.MethodInfo.GetCustomAttributes(true).Any(x => x.GetType().Name == "IdempotentAttribute"))
        {
            operation.Parameters.Add(new OpenApiParameter { Name = "Idempotency-Key", In = ParameterLocation.Header, Required = true, Description = "Unique key for safely retrying this command (retained for 24 hours).", Schema = new OpenApiSchema { Type = "string", Example = new OpenApiString(Guid.NewGuid().ToString()) } });
        }
    }
}
