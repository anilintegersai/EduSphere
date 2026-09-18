using System.Text.Json;
using System.Text.Json.Serialization;
using EduSphere.Application.DTOs.AI;
using EduSphere.Application.Interfaces;
using EduSphere.Application.Services;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public sealed class AIQuestionPaperService : IAIQuestionPaperService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly TenantDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAISecretProtector _secretProtector;
    private readonly IReadOnlyDictionary<AIProviderType, IAIQuestionPaperProvider> _providers;

    public AIQuestionPaperService(
        TenantDbContext db,
        ITenantContext tenantContext,
        ICurrentUserContext currentUser,
        IAISecretProtector secretProtector,
        IEnumerable<IAIQuestionPaperProvider> providers)
    {
        _db = db;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _secretProtector = secretProtector;
        _providers = providers.ToDictionary(p => p.ProviderType);
    }

    public async Task<AIGenerationOutcome> GenerateAsync(AIQuestionPaperGenerationInput input, CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        ValidateInputDimensions(input);
        var subject = await _db.Subjects.Include(s => s.Course).SingleAsync(s => s.Id == input.SubjectId, cancellationToken);
        var units = await _db.SyllabusUnits.Where(u => input.SyllabusUnitIds.Contains(u.Id) && u.SubjectId == input.SubjectId)
            .OrderBy(u => u.Order).ToListAsync(cancellationToken);
        if (units.Count == 0) throw new InvalidOperationException("Select at least one syllabus unit for this subject.");

        var template = await _db.AIPromptTemplates.SingleAsync(t => t.Id == input.PromptTemplateId && t.IsActive, cancellationToken);
        var setting = input.ProviderSettingId.HasValue
            ? await _db.AIProviderSettings.SingleOrDefaultAsync(p => p.Id == input.ProviderSettingId && p.IsEnabled, cancellationToken)
            : await _db.AIProviderSettings.Where(p => p.IsEnabled).OrderByDescending(p => p.IsDefault).ThenBy(p => p.Name).FirstOrDefaultAsync(cancellationToken);

        var requestParameters = JsonSerializer.Serialize(input, JsonOptions);
        var prompt = BuildPrompt(template.UserPromptTemplate, input, subject.Name, subject.Course?.Name, units)
            + "\nRequired JSON schema:\n" + template.OutputSchemaJson;
        prompt = AIQuestionPaperDocumentParser.RemovePotentialPii(prompt);
        var requestEntity = new AIGenerationRequest
        {
            TenantId = tenantId,
            BranchId = input.BranchId,
            SubjectId = input.SubjectId,
            QuestionPaperId = input.QuestionPaperId,
            ProviderSettingId = setting?.Id,
            PromptTemplateId = template.Id,
            RegeneratedFromRequestId = input.RegeneratedFromRequestId,
            InputParametersJson = requestParameters,
            PromptHash = AIQuestionPaperDocumentParser.Hash(template.SystemPrompt + "\n" + prompt),
            RequestedBy = _currentUser.UserId,
            Status = AIGenerationStatus.Processing
        };
        _db.AIGenerationRequests.Add(requestEntity);
        await _db.SaveChangesAsync(cancellationToken);

        try
        {
            ValidateProvider(setting);
            var provider = _providers[setting!.ProviderType];
            var providerResult = await provider.GenerateAsync(new AIProviderGenerationRequest
            {
                Endpoint = setting.Endpoint,
                ModelOrDeployment = setting.ModelOrDeployment,
                ApiKey = _secretProtector.Unprotect(setting.ProtectedApiKey),
                ApiVersion = setting.ApiVersion,
                SystemPrompt = AIQuestionPaperDocumentParser.RemovePotentialPii(template.SystemPrompt),
                UserPrompt = prompt,
                TimeoutSeconds = setting.TimeoutSeconds
            }, cancellationToken);

            var document = AIQuestionPaperDocumentParser.ParseAndValidate(providerResult.Content, input.TotalMarks);
            var outcome = await PersistGeneratedPaperAsync(input, requestEntity, document, providerResult, setting, cancellationToken);
            return outcome;
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested && IsGenerationFailure(ex))
        {
            requestEntity.Status = input.AllowManualFallback ? AIGenerationStatus.ManualFallback : AIGenerationStatus.Failed;
            requestEntity.UsedManualFallback = input.AllowManualFallback;
            requestEntity.CompletedOn = DateTime.UtcNow;
            requestEntity.FailureMessage = ex.Message.Length <= 2000 ? ex.Message : ex.Message[..2000];
            requestEntity.Logs.Add(new AIGenerationLog
            {
                TenantId = tenantId,
                EventType = "GenerationFailed",
                Message = requestEntity.FailureMessage
            });

            if (!input.AllowManualFallback)
            {
                await _db.SaveChangesAsync(cancellationToken);
                throw;
            }

            var fallback = await CreateManualFallbackAsync(input, requestEntity, cancellationToken);
            return fallback with { Warning = "AI generation failed. A manual draft was created so work can continue. " + requestEntity.FailureMessage };
        }
    }

    public async Task<Guid> RollbackAsync(Guid questionPaperId, Guid versionId, CancellationToken cancellationToken = default)
    {
        var paper = await _db.QuestionPapers.SingleAsync(p => p.Id == questionPaperId, cancellationToken);
        var source = await _db.QuestionPaperVersions.SingleAsync(v => v.Id == versionId && v.QuestionPaperId == questionPaperId, cancellationToken);
        var next = await _db.QuestionPaperVersions.Where(v => v.QuestionPaperId == questionPaperId).MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0;
        var restored = new QuestionPaperVersion
        {
            TenantId = paper.TenantId,
            QuestionPaperId = paper.Id,
            VersionNumber = next + 1,
            Source = source.Source,
            Status = ApprovalStatus.Draft,
            StudentContentJson = source.StudentContentJson,
            SolutionContentJson = source.SolutionContentJson,
            Notes = $"Restored from version {source.VersionNumber}."
        };
        _db.QuestionPaperVersions.Add(restored);
        paper.Status = ApprovalStatus.Draft;
        await _db.SaveChangesAsync(cancellationToken);
        return restored.Id;
    }

    public async Task<int> EnrichQuestionBankAsync(Guid questionPaperId, Guid versionId, CancellationToken cancellationToken = default)
    {
        var paper = await _db.QuestionPapers.SingleAsync(p => p.Id == questionPaperId, cancellationToken);
        var version = await _db.QuestionPaperVersions.SingleAsync(v => v.Id == versionId && v.QuestionPaperId == questionPaperId, cancellationToken);
        if (paper.Status != ApprovalStatus.Approved && paper.Status != ApprovalStatus.Published)
            throw new InvalidOperationException("Only an approved or published paper can enrich the question bank.");
        if (version.Status != ApprovalStatus.Approved && version.Status != ApprovalStatus.Published)
            throw new InvalidOperationException("Only an approved or published version can enrich the question bank.");

        var document = AIQuestionPaperDocumentParser.ParseAndValidate(version.SolutionContentJson, paper.TotalMarks);
        var existingTexts = await _db.QuestionBankItems.Where(q => q.SubjectId == paper.SubjectId)
            .Select(q => q.QuestionText).ToListAsync(cancellationToken);
        var existing = existingTexts.Select(NormalizeQuestion).ToHashSet(StringComparer.Ordinal);
        var added = 0;
        foreach (var question in document.Sections.SelectMany(s => s.Questions).Where(q => !existing.Contains(NormalizeQuestion(q.Text))))
        {
            var item = new QuestionBankItem
            {
                TenantId = paper.TenantId,
                BranchId = paper.BranchId,
                SubjectId = paper.SubjectId,
                QuestionType = question.QuestionType,
                Difficulty = question.Difficulty,
                BloomLevel = question.BloomLevel,
                Source = GenerationSource.AiGenerated,
                Marks = question.Marks,
                QuestionText = question.Text,
                ExpectedAnswer = question.Answer,
                Tags = $"AI;Paper:{paper.Title};Version:{version.VersionNumber}",
                ApprovalStatus = ApprovalStatus.Approved,
                ApprovedOn = DateTime.UtcNow
            };
            _db.QuestionBankItems.Add(item);
            existing.Add(NormalizeQuestion(question.Text));
            added++;
        }
        await _db.SaveChangesAsync(cancellationToken);
        return added;
    }

    private async Task<AIGenerationOutcome> PersistGeneratedPaperAsync(
        AIQuestionPaperGenerationInput input,
        AIGenerationRequest request,
        GeneratedQuestionPaperDocument document,
        AIProviderGenerationResult providerResult,
        AIProviderSetting setting,
        CancellationToken cancellationToken)
    {
        var paper = await ResolvePaperAsync(input, cancellationToken);
        if (paper.Id == Guid.Empty) _db.QuestionPapers.Add(paper);
        var nextVersion = paper.Id == Guid.Empty ? 1 : (await _db.QuestionPaperVersions.Where(v => v.QuestionPaperId == paper.Id)
            .MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0) + 1;

        if (paper.Id != Guid.Empty)
        {
            var oldSections = await _db.QuestionPaperSections.Include(s => s.Questions).Where(s => s.QuestionPaperId == paper.Id).ToListAsync(cancellationToken);
            _db.QuestionPaperQuestions.RemoveRange(oldSections.SelectMany(s => s.Questions));
            _db.QuestionPaperSections.RemoveRange(oldSections);
        }

        var order = 0;
        foreach (var sectionDocument in document.Sections)
        {
            var section = new QuestionPaperSection
            {
                TenantId = request.TenantId,
                Code = sectionDocument.Code,
                Title = sectionDocument.Title,
                Instructions = sectionDocument.Instructions,
                SortOrder = ++order,
                Marks = sectionDocument.Questions.Sum(q => q.Marks)
            };
            var questionOrder = 0;
            foreach (var q in sectionDocument.Questions)
                section.Questions.Add(new QuestionPaperQuestion
                {
                    TenantId = request.TenantId,
                    SortOrder = ++questionOrder,
                    QuestionText = q.Text,
                    SolutionText = q.Answer,
                    Marks = q.Marks,
                    QuestionType = q.QuestionType,
                    Difficulty = q.Difficulty,
                    BloomLevel = q.BloomLevel
                });
            paper.Sections.Add(section);
        }

        var studentJson = AIQuestionPaperDocumentParser.SerializeStudentPaper(document);
        var solutionJson = AIQuestionPaperDocumentParser.SerializeMarkingScheme(document);
        var version = new QuestionPaperVersion
        {
            TenantId = request.TenantId,
            VersionNumber = nextVersion,
            Source = GenerationSource.AiGenerated,
            Status = ApprovalStatus.Draft,
            StudentContentJson = studentJson,
            SolutionContentJson = solutionJson,
            Notes = request.RegeneratedFromRequestId.HasValue ? "AI regeneration draft." : "Initial AI generation draft."
        };
        paper.Versions.Add(version);
        await _db.SaveChangesAsync(cancellationToken);

        request.QuestionPaperId = paper.Id;
        request.GeneratedVersionId = version.Id;
        request.Status = AIGenerationStatus.Succeeded;
        request.CompletedOn = DateTime.UtcNow;
        request.Response = new AIGenerationResponse
        {
            TenantId = request.TenantId,
            StudentPaperJson = studentJson,
            MarkingSchemeJson = solutionJson,
            ContentHash = AIQuestionPaperDocumentParser.Hash(studentJson + solutionJson),
            SchemaValidated = true
        };
        request.Logs.Add(new AIGenerationLog
        {
            TenantId = request.TenantId,
            EventType = "GenerationSucceeded",
            Message = "Provider response validated and stored as a draft version.",
            ProviderRequestId = providerResult.ProviderRequestId,
            LatencyMilliseconds = providerResult.LatencyMilliseconds
        });
        request.UsageRecords.Add(new AIUsageRecord
        {
            TenantId = request.TenantId,
            InputTokens = providerResult.InputTokens,
            OutputTokens = providerResult.OutputTokens,
            EstimatedCost = providerResult.InputTokens / 1_000_000m * setting.InputCostPerMillionTokens + providerResult.OutputTokens / 1_000_000m * setting.OutputCostPerMillionTokens,
            LatencyMilliseconds = providerResult.LatencyMilliseconds,
            Model = providerResult.Model
        });
        await _db.SaveChangesAsync(cancellationToken);
        return new AIGenerationOutcome(request.Id, paper.Id, version.Id, request.Status, null);
    }

    private async Task<AIGenerationOutcome> CreateManualFallbackAsync(AIQuestionPaperGenerationInput input, AIGenerationRequest request, CancellationToken cancellationToken)
    {
        var paper = await ResolvePaperAsync(input, cancellationToken);
        if (paper.Id == Guid.Empty) _db.QuestionPapers.Add(paper);
        var next = paper.Id == Guid.Empty ? 1 : (await _db.QuestionPaperVersions.Where(v => v.QuestionPaperId == paper.Id)
            .MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0) + 1;
        var version = new QuestionPaperVersion
        {
            TenantId = request.TenantId,
            VersionNumber = next,
            Source = GenerationSource.Manual,
            Status = ApprovalStatus.Draft,
            StudentContentJson = JsonSerializer.Serialize(new { input.Title, input.TotalMarks, input.DurationMinutes, Sections = Array.Empty<object>() }),
            SolutionContentJson = JsonSerializer.Serialize(new { input.Title, Sections = Array.Empty<object>() }),
            Notes = "Manual fallback created after AI provider failure."
        };
        paper.Versions.Add(version);
        await _db.SaveChangesAsync(cancellationToken);
        request.QuestionPaperId = paper.Id;
        request.GeneratedVersionId = version.Id;
        await _db.SaveChangesAsync(cancellationToken);
        return new AIGenerationOutcome(request.Id, paper.Id, version.Id, request.Status, null);
    }

    private async Task<QuestionPaper> ResolvePaperAsync(AIQuestionPaperGenerationInput input, CancellationToken cancellationToken)
    {
        if (input.QuestionPaperId.HasValue)
            return await _db.QuestionPapers.Include(p => p.Sections).ThenInclude(s => s.Questions)
                .SingleAsync(p => p.Id == input.QuestionPaperId, cancellationToken);
        return new QuestionPaper
        {
            TenantId = RequireTenant(), BranchId = input.BranchId, SubjectId = input.SubjectId, ExamId = input.ExamId,
            Title = input.Title, TotalMarks = input.TotalMarks, DurationMinutes = input.DurationMinutes,
            Instructions = input.Instructions, Source = GenerationSource.AiGenerated, Status = ApprovalStatus.Draft
        };
    }

    private void ValidateProvider(AIProviderSetting? setting)
    {
        if (setting is null) throw new InvalidOperationException("No enabled AI provider is configured for this tenant.");
        if (setting.DataResidencyPolicy == AIDataResidencyPolicy.LocalOnly)
            throw new InvalidOperationException("This tenant permits local-only AI processing; the configured external provider cannot be used.");
        if (setting.DataResidencyPolicy == AIDataResidencyPolicy.RequiredRegion &&
            !string.Equals(setting.Region, setting.RequiredRegion, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Provider region '{setting.Region}' does not satisfy required region '{setting.RequiredRegion}'.");
        if (!_providers.ContainsKey(setting.ProviderType)) throw new InvalidOperationException($"No adapter is registered for {setting.ProviderType}.");
    }

    private static string BuildPrompt(string template, AIQuestionPaperGenerationInput input, string subject, string? course, IReadOnlyCollection<SyllabusUnit> units)
    {
        var dimensions = JsonSerializer.Serialize(new
        {
            Subject = subject, Course = course, input.Title, input.TotalMarks, input.DurationMinutes,
            input.Language, input.Instructions,
            SyllabusUnits = units.Select(u => new { u.Title, u.Description }),
            input.BloomDistribution, input.DifficultyDistribution, input.QuestionTypeCounts
        }, JsonOptions);
        return template.Replace("{{GenerationInputJson}}", dimensions, StringComparison.Ordinal)
            .Replace("{{SubjectName}}", subject, StringComparison.Ordinal)
            .Replace("{{TotalMarks}}", input.TotalMarks.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    private Guid RequireTenant() => _tenantContext.TenantId ?? throw new InvalidOperationException("A tenant must be selected before using AI generation.");

    private static void ValidateInputDimensions(AIQuestionPaperGenerationInput input)
    {
        if (input.BloomDistribution.Count == 0 || input.BloomDistribution.Values.Sum() != 100)
            throw new InvalidOperationException("Bloom's taxonomy distribution must total 100 percent.");
        if (input.DifficultyDistribution.Count == 0 || input.DifficultyDistribution.Values.Sum() != 100)
            throw new InvalidOperationException("Difficulty distribution must total 100 percent.");
        if (input.QuestionTypeCounts.Count == 0 || input.QuestionTypeCounts.Values.Any(v => v <= 0))
            throw new InvalidOperationException("At least one positive question-type count is required.");
    }

    private static string NormalizeQuestion(string value) => string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    private static bool IsGenerationFailure(Exception exception) => exception is
        HttpRequestException or
        TaskCanceledException or
        InvalidOperationException or
        JsonException or
        System.Security.Cryptography.CryptographicException;
}
