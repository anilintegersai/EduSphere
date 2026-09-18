using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AIProviderSetting : TenantEntityBase
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    public AIProviderType ProviderType { get; set; }
    [Required, StringLength(500)] public string Endpoint { get; set; } = null!;
    [Required, StringLength(150)] public string ModelOrDeployment { get; set; } = null!;
    [Required] public string ProtectedApiKey { get; set; } = null!;
    [StringLength(40)] public string? ApiVersion { get; set; }
    [StringLength(80)] public string? Region { get; set; }
    public AIDataResidencyPolicy DataResidencyPolicy { get; set; }
    [StringLength(80)] public string? RequiredRegion { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsDefault { get; set; }
    public int TimeoutSeconds { get; set; } = 90;
    public decimal InputCostPerMillionTokens { get; set; }
    public decimal OutputCostPerMillionTokens { get; set; }
}

public class AIPromptTemplate : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(80)] public string Code { get; set; } = null!;
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    [Required] public string SystemPrompt { get; set; } = null!;
    [Required] public string UserPromptTemplate { get; set; } = null!;
    [Required] public string OutputSchemaJson { get; set; } = null!;
}

public class AIGenerationRequest : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public Guid? QuestionPaperId { get; set; }
    public QuestionPaper? QuestionPaper { get; set; }
    public Guid? GeneratedVersionId { get; set; }
    public QuestionPaperVersion? GeneratedVersion { get; set; }
    public Guid? ProviderSettingId { get; set; }
    public AIProviderSetting? ProviderSetting { get; set; }
    public Guid PromptTemplateId { get; set; }
    public AIPromptTemplate? PromptTemplate { get; set; }
    public Guid? RegeneratedFromRequestId { get; set; }
    public AIGenerationRequest? RegeneratedFromRequest { get; set; }
    public AIGenerationStatus Status { get; set; } = AIGenerationStatus.Pending;
    [Required] public string InputParametersJson { get; set; } = "{}";
    [Required, StringLength(64)] public string PromptHash { get; set; } = null!;
    [StringLength(450)] public string? RequestedBy { get; set; }
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedOn { get; set; }
    [StringLength(2000)] public string? FailureMessage { get; set; }
    public bool UsedManualFallback { get; set; }
    public AIGenerationResponse? Response { get; set; }
    public ICollection<AIGenerationLog> Logs { get; set; } = new List<AIGenerationLog>();
    public ICollection<AIUsageRecord> UsageRecords { get; set; } = new List<AIUsageRecord>();
}

public class AIGenerationResponse : TenantEntityBase
{
    public Guid AIGenerationRequestId { get; set; }
    public AIGenerationRequest? AIGenerationRequest { get; set; }
    [Required] public string StudentPaperJson { get; set; } = "{}";
    [Required] public string MarkingSchemeJson { get; set; } = "{}";
    [StringLength(64)] public string ContentHash { get; set; } = null!;
    public bool SchemaValidated { get; set; }
}

public class AIGenerationLog : TenantEntityBase
{
    public Guid AIGenerationRequestId { get; set; }
    public AIGenerationRequest? AIGenerationRequest { get; set; }
    [Required, StringLength(50)] public string EventType { get; set; } = null!;
    [StringLength(1000)] public string? Message { get; set; }
    [StringLength(100)] public string? ProviderRequestId { get; set; }
    public int LatencyMilliseconds { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}

public class AIUsageRecord : TenantEntityBase
{
    public Guid AIGenerationRequestId { get; set; }
    public AIGenerationRequest? AIGenerationRequest { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    [Required, StringLength(30)] public string Currency { get; set; } = "USD";
    public int LatencyMilliseconds { get; set; }
    [Required, StringLength(150)] public string Model { get; set; } = null!;
    public DateTime RecordedOn { get; set; } = DateTime.UtcNow;
}
