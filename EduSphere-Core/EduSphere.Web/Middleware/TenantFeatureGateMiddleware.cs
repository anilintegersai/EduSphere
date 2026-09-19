using EduSphere.Application.Interfaces;

namespace EduSphere.Web.Middleware;

public sealed class TenantFeatureGateMiddleware(RequestDelegate next)
{
    private static readonly IReadOnlyDictionary<string, string> RouteFeatures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["/AI"] = "module.ai-question-papers",
        ["/Enterprise/Finance"] = "module.finance",
        ["/Enterprise/Transport"] = "module.transport",
        ["/Enterprise/Library"] = "module.library",
        ["/Enterprise/Hostel"] = "module.hostel",
        ["/Enterprise/Communications"] = "module.communications"
    };

    public async Task InvokeAsync(HttpContext context, ITenantFeatureService features)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var featureKey = ResolveFeature(path);
        if (featureKey is not null && !await features.IsEnabledAsync(featureKey, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("This module is not enabled for the selected tenant.");
            return;
        }
        await next(context);
    }

    private static string? ResolveFeature(string path)
    {
        var page = RouteFeatures.FirstOrDefault(item => path.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(page.Key)) return page.Value;
        if (path.Contains("/ai/question-papers", StringComparison.OrdinalIgnoreCase)) return "module.ai-question-papers";
        foreach (var module in new[] { "finance", "transport", "library", "hostel", "communications" })
            if (path.Contains("/enterprise/" + module, StringComparison.OrdinalIgnoreCase)) return "module." + module;
        return null;
    }
}
