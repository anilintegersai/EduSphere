using EduSphere.Application.DTOs.AI;
using EduSphere.Domain.Enums;

namespace EduSphere.Application.Interfaces;

public interface IAIQuestionPaperProvider
{
    AIProviderType ProviderType { get; }
    Task<AIProviderGenerationResult> GenerateAsync(AIProviderGenerationRequest request, CancellationToken cancellationToken = default);
}

public interface IAISecretProtector
{
    string Protect(string secret);
    string Unprotect(string protectedSecret);
}

public interface IAIQuestionPaperService
{
    Task<AIGenerationOutcome> GenerateAsync(AIQuestionPaperGenerationInput input, CancellationToken cancellationToken = default);
    Task<Guid> RollbackAsync(Guid questionPaperId, Guid versionId, CancellationToken cancellationToken = default);
    Task<int> EnrichQuestionBankAsync(Guid questionPaperId, Guid versionId, CancellationToken cancellationToken = default);
}
