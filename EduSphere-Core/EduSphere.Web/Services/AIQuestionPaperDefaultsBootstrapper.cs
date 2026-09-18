using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class AIQuestionPaperDefaultsBootstrapper
{
    public const string TemplateCode = "GENERAL-QUESTION-PAPER";
    public const string SystemPrompt = "You are an expert assessment designer. Produce only valid JSON matching the requested schema. Do not include personal data, markdown fences, or commentary. Ensure exact total marks and unique questions. Every question must include a complete marking answer.";
    public const string UserPrompt = "Create a question paper from this sanitized configuration: {{GenerationInputJson}}";
    public const string OutputSchema = "{\"title\":\"string\",\"sections\":[{\"code\":\"string\",\"title\":\"string\",\"instructions\":\"string|null\",\"questions\":[{\"text\":\"string\",\"answer\":\"string\",\"marks\":1,\"questionType\":\"ShortAnswer\",\"difficulty\":\"Medium\",\"bloomLevel\":\"Understand\",\"syllabusUnit\":\"string|null\"}]}]}";

    public static async Task ApplyAIQuestionPaperDefaultsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        try
        {
            var tenantIds = await db.Tenants.AsNoTracking().Select(t => t.Id).ToListAsync();
            var configured = await db.AIPromptTemplates.IgnoreQueryFilters().Where(t => t.Code == TemplateCode)
                .Select(t => t.TenantId).Distinct().ToListAsync();
            foreach (var tenantId in tenantIds.Except(configured))
                db.AIPromptTemplates.Add(new AIPromptTemplate
                {
                    TenantId = tenantId, Code = TemplateCode, Name = "General question paper", Version = 1,
                    IsActive = true, SystemPrompt = SystemPrompt, UserPromptTemplate = UserPrompt, OutputSchemaJson = OutputSchema
                });
            if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AIQuestionPaperDefaultsBootstrapper")
                .LogWarning(ex, "AI question-paper defaults could not be initialized. Apply the AI migration before enabling the module.");
        }
    }
}
