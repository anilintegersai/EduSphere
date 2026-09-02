using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EduSphere.Web.Swagger;

/// <summary>
/// Adds an optional X-Tenant-ID header to every operation so the Swagger UI can
/// drive tenant-scoped endpoints (e.g. branches).
/// </summary>
public class TenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Tenant-ID",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Tenant identifier for tenant-scoped endpoints (e.g. branches).",
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}
