using System.Text.Json;
using EduSphere.Application.DTOs.AI;
using EduSphere.Application.Interfaces;
using EduSphere.Application.Services;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class AIQuestionPaperTests
{
    private static readonly Guid TenantId = Guid.Parse("a16ea5f8-1a77-44bf-bccd-12ae39fd6cef");

    [Fact]
    public void ParserValidatesAndSeparatesStudentPaperFromMarkingScheme()
    {
        var json = ValidProviderJson();
        var document = AIQuestionPaperDocumentParser.ParseAndValidate(json, 10);
        var student = AIQuestionPaperDocumentParser.SerializeStudentPaper(document);
        var scheme = AIQuestionPaperDocumentParser.SerializeMarkingScheme(document);
        Assert.DoesNotContain("Model answer", student);
        Assert.Contains("Model answer", scheme);
    }

    [Fact]
    public void ParserRejectsIncorrectTotalMarks()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => AIQuestionPaperDocumentParser.ParseAndValidate(ValidProviderJson(), 100));
        Assert.Contains("marks total", exception.Message);
    }

    [Fact]
    public void PiiFilterRemovesEmailPhoneAndIdentityNumbers()
    {
        var cleaned = AIQuestionPaperDocumentParser.RemovePotentialPii("Contact learner@example.com, +91 98765 43210, ID 123456789012");
        Assert.DoesNotContain("learner@example.com", cleaned);
        Assert.DoesNotContain("98765", cleaned);
        Assert.DoesNotContain("123456789012", cleaned);
    }

    [Fact]
    public async Task GenerationPersistsPaperVersionUsageAndSeparatedResponse()
    {
        var tenant = new TenantContext();
        tenant.SetTenant(TenantId, "ai-test");
        await using var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant, new CurrentUser());

        var branch = new Branch { Name = "Innovation Campus", Code = "AI" };
        var course = new Course { Name = "Computer Science", Code = "CS" };
        db.AddRange(branch, course);
        await db.SaveChangesAsync();
        var subject = new Subject { Name = "Artificial Intelligence", Code = "AI-101", CourseId = course.Id };
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();
        var unit = new SyllabusUnit { SubjectId = subject.Id, Order = 1, Title = "Search", Description = "State-space search", EstimatedHours = 8 };
        var setting = new AIProviderSetting
        {
            Name = "Fake", ProviderType = AIProviderType.OpenAI, Endpoint = "https://example.test", ModelOrDeployment = "fake-model",
            ProtectedApiKey = "protected", IsEnabled = true, IsDefault = true, InputCostPerMillionTokens = 1, OutputCostPerMillionTokens = 2
        };
        var template = new AIPromptTemplate
        {
            Code = "TEST", Name = "Test", Version = 1, SystemPrompt = "Return JSON", UserPromptTemplate = "{{GenerationInputJson}}", OutputSchemaJson = "{}", IsActive = true
        };
        db.AddRange(unit, setting, template);
        await db.SaveChangesAsync();

        var service = new AIQuestionPaperService(db, tenant, new CurrentUser(), new PassthroughSecretProtector(), [new FakeProvider()]);
        var outcome = await service.GenerateAsync(new AIQuestionPaperGenerationInput
        {
            BranchId = branch.Id, SubjectId = subject.Id, ProviderSettingId = setting.Id, PromptTemplateId = template.Id,
            Title = "AI Midterm", TotalMarks = 10, DurationMinutes = 60, SyllabusUnitIds = [unit.Id],
            BloomDistribution = new() { [BloomLevel.Apply] = 100 }, DifficultyDistribution = new() { [QuestionDifficulty.Medium] = 100 },
            QuestionTypeCounts = new() { [QuestionType.ShortAnswer] = 1 }
        });

        Assert.Equal(AIGenerationStatus.Succeeded, outcome.Status);
        Assert.Single(await db.QuestionPapers.ToListAsync());
        Assert.Single(await db.QuestionPaperVersions.ToListAsync());
        Assert.True((await db.AIGenerationResponses.SingleAsync()).SchemaValidated);
        Assert.Equal(30 / 1_000_000m, (await db.AIUsageRecords.SingleAsync()).EstimatedCost);
    }

    [Fact]
    public async Task ProviderFailureCreatesManualFallbackWithoutLeakingSecret()
    {
        var tenant = new TenantContext(); tenant.SetTenant(TenantId, "fallback-test");
        await using var db = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, tenant);
        var branch = new Branch { Name = "Main", Code = "MAIN" }; var course = new Course { Name = "Science", Code = "SCI" };
        db.AddRange(branch, course); await db.SaveChangesAsync();
        var subject = new Subject { Name = "Physics", Code = "PHY", CourseId = course.Id }; db.Add(subject); await db.SaveChangesAsync();
        var unit = new SyllabusUnit { SubjectId = subject.Id, Order = 1, Title = "Motion" };
        var setting = new AIProviderSetting { Name = "Broken", ProviderType = AIProviderType.OpenAI, Endpoint = "https://example.test", ModelOrDeployment = "fake", ProtectedApiKey = "super-secret", IsEnabled = true };
        var template = new AIPromptTemplate { Code = "T", Name = "T", SystemPrompt = "JSON", UserPromptTemplate = "{{GenerationInputJson}}", OutputSchemaJson = "{}" };
        db.AddRange(unit, setting, template); await db.SaveChangesAsync();
        var service = new AIQuestionPaperService(db, tenant, new CurrentUser(), new PassthroughSecretProtector(), [new FailingProvider()]);
        var outcome = await service.GenerateAsync(new AIQuestionPaperGenerationInput
        {
            BranchId = branch.Id, SubjectId = subject.Id, ProviderSettingId = setting.Id, PromptTemplateId = template.Id,
            Title = "Fallback", TotalMarks = 10, SyllabusUnitIds = [unit.Id],
            BloomDistribution = new() { [BloomLevel.Understand] = 100 },
            DifficultyDistribution = new() { [QuestionDifficulty.Medium] = 100 },
            QuestionTypeCounts = new() { [QuestionType.ShortAnswer] = 1 }
        });
        Assert.Equal(AIGenerationStatus.ManualFallback, outcome.Status);
        var request = await db.AIGenerationRequests.SingleAsync();
        Assert.True(request.UsedManualFallback);
        Assert.DoesNotContain("super-secret", request.FailureMessage ?? string.Empty);
        Assert.Equal(GenerationSource.Manual, (await db.QuestionPaperVersions.SingleAsync()).Source);
    }

    private static string ValidProviderJson() => JsonSerializer.Serialize(new
    {
        title = "Search Assessment",
        sections = new[] { new { code = "A", title = "Short answers", instructions = "Answer all", questions = new[] { new { text = "Explain A star search.", answer = "Model answer", marks = 10, questionType = "ShortAnswer", difficulty = "Medium", bloomLevel = "Apply", syllabusUnit = "Search" } } } }
    });

    private sealed class FakeProvider : IAIQuestionPaperProvider
    {
        public AIProviderType ProviderType => AIProviderType.OpenAI;
        public Task<AIProviderGenerationResult> GenerateAsync(AIProviderGenerationRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new AIProviderGenerationResult(ValidProviderJson(), 10, 10, 25, "test-request", "fake-model"));
    }
    private sealed class FailingProvider : IAIQuestionPaperProvider
    {
        public AIProviderType ProviderType => AIProviderType.OpenAI;
        public Task<AIProviderGenerationResult> GenerateAsync(AIProviderGenerationRequest request, CancellationToken cancellationToken = default) => throw new HttpRequestException("Provider unavailable");
    }
    private sealed class PassthroughSecretProtector : IAISecretProtector
    {
        public string Protect(string secret) => secret;
        public string Unprotect(string protectedSecret) => protectedSecret;
    }
    private sealed class CurrentUser : ICurrentUserContext { public string? UserId => "ai-test-user"; }
}
