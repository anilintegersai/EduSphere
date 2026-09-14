using System.Security.Claims;
using EduSphere.Application.DTOs.Operations;

namespace EduSphere.Application.Interfaces;

public interface IAdmissionWorkflowService
{
    Task<IReadOnlyList<AdmissionFormTemplateDto>> ListFormTemplatesAsync(
        ClaimsPrincipal actor,
        Guid? branchId = null,
        Guid? courseId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdmissionFormFieldDto>> ListFormFieldsAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdmissionDocumentRequirementDto>> ListDocumentRequirementsAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdmissionDocumentDto>> ListDocumentsAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdmissionReviewDto>> ListReviewsAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdmissionInterviewDto>> ListInterviewsAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionInterviewDto>> ScheduleInterviewAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        ScheduleAdmissionInterviewRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionInterviewDto>> UpdateInterviewAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        Guid interviewId,
        UpdateAdmissionInterviewRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionFinanceReadinessDto>> GetFinanceReadinessAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionFeeInvoiceDto>> GenerateAdmissionFeeInvoiceAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionDocumentDto>> UploadDocumentAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        UploadAdmissionDocumentRequest request,
        Stream content,
        string fileName,
        string? contentType,
        long sizeBytes,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionDocumentDto>> SetDocumentVerificationAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        Guid documentId,
        bool isVerified,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionApplicationDto>> ReviewAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        ReviewAdmissionApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationsWorkflowResult<AdmissionEnrollmentResultDto>> ConvertToEnrollmentAsync(
        ClaimsPrincipal actor,
        Guid admissionApplicationId,
        ConvertAdmissionToEnrollmentRequest request,
        CancellationToken cancellationToken = default);
}

public interface IAdmissionDocumentStorageService
{
    Task<StoredAdmissionDocument> SaveAsync(
        Guid tenantId,
        Guid admissionApplicationId,
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default);
}

public sealed record StoredAdmissionDocument(
    string StoragePath,
    string FileName,
    string? ContentType,
    long SizeBytes);

public class OperationsWorkflowResult
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static OperationsWorkflowResult Success(string? message = null)
        => new() { Succeeded = true, Message = message };

    public static OperationsWorkflowResult Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}

public class OperationsWorkflowResult<T> : OperationsWorkflowResult
{
    public T? Data { get; init; }

    public static OperationsWorkflowResult<T> Success(T data, string? message = null)
        => new() { Succeeded = true, Data = data, Message = message };

    public new static OperationsWorkflowResult<T> Failure(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}
