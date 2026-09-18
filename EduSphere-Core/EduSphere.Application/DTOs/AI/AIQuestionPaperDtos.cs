using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.AI;

public sealed class AIQuestionPaperGenerationInput
{
    public Guid BranchId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? ExamId { get; set; }
    public Guid? QuestionPaperId { get; set; }
    public Guid? ProviderSettingId { get; set; }
    public Guid PromptTemplateId { get; set; }
    public Guid? RegeneratedFromRequestId { get; set; }
    [Required, StringLength(150)] public string Title { get; set; } = null!;
    [Range(1, 1000)] public decimal TotalMarks { get; set; } = 100;
    [Range(15, 600)] public int DurationMinutes { get; set; } = 180;
    [StringLength(1000)] public string? Instructions { get; set; }
    [Required] public List<Guid> SyllabusUnitIds { get; set; } = [];
    [Required] public Dictionary<BloomLevel, int> BloomDistribution { get; set; } = [];
    [Required] public Dictionary<QuestionDifficulty, int> DifficultyDistribution { get; set; } = [];
    [Required] public Dictionary<QuestionType, int> QuestionTypeCounts { get; set; } = [];
    [StringLength(20)] public string Language { get; set; } = "English";
    public bool AllowManualFallback { get; set; } = true;
}

public sealed class AIProviderGenerationRequest
{
    public string Endpoint { get; init; } = null!;
    public string ModelOrDeployment { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public string? ApiVersion { get; init; }
    public string SystemPrompt { get; init; } = null!;
    public string UserPrompt { get; init; } = null!;
    public int TimeoutSeconds { get; init; }
}

public sealed record AIProviderGenerationResult(
    string Content,
    int InputTokens,
    int OutputTokens,
    int LatencyMilliseconds,
    string? ProviderRequestId,
    string Model);

public sealed class GeneratedQuestionPaperDocument
{
    [Required] public string Title { get; set; } = null!;
    [Required] public List<GeneratedQuestionPaperSection> Sections { get; set; } = [];
}

public sealed class GeneratedQuestionPaperSection
{
    [Required] public string Code { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    [Required] public List<GeneratedQuestion> Questions { get; set; } = [];
}

public sealed class GeneratedQuestion
{
    [Required] public string Text { get; set; } = null!;
    [Required] public string Answer { get; set; } = null!;
    public decimal Marks { get; set; }
    public QuestionType QuestionType { get; set; }
    public QuestionDifficulty Difficulty { get; set; }
    public BloomLevel BloomLevel { get; set; }
    public string? SyllabusUnit { get; set; }
}

public sealed record AIGenerationOutcome(Guid RequestId, Guid QuestionPaperId, Guid VersionId, AIGenerationStatus Status, string? Warning);
