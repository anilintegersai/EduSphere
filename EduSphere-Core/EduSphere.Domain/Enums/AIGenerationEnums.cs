namespace EduSphere.Domain.Enums;

public enum AIProviderType
{
    OpenAI = 0,
    AzureOpenAI = 1
}

public enum AIGenerationStatus
{
    Pending = 0,
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    ManualFallback = 4
}

public enum AIDataResidencyPolicy
{
    AnyRegion = 0,
    RequiredRegion = 1,
    LocalOnly = 2
}
