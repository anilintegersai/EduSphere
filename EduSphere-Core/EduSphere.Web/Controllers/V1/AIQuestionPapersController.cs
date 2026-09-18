using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.AI;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduSphere.Web.Security;

namespace EduSphere.Web.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ai/question-papers")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.BranchAdmin},{Roles.Principal},{Roles.DepartmentAdmin},{Roles.Teacher},{Roles.ExamController}")]
public sealed class AIQuestionPapersController(IAIQuestionPaperService service, IBranchAccessService branchAccess) : ApiControllerBase
{
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<AIGenerationOutcome>>> Generate(AIQuestionPaperGenerationInput input, CancellationToken cancellationToken)
    {
        if (!await branchAccess.CanAccessBranchAsync(User, input.BranchId)) return Forbid();
        try { return Ok(ApiResponse<AIGenerationOutcome>.Ok(await service.GenerateAsync(input, cancellationToken))); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<AIGenerationOutcome>.Fail(ex.Message)); }
    }

    [HttpPost("{paperId:guid}/versions/{versionId:guid}/rollback")]
    public async Task<ActionResult<ApiResponse<Guid>>> Rollback(Guid paperId, Guid versionId, CancellationToken cancellationToken) =>
        Ok(ApiResponse<Guid>.Ok(await service.RollbackAsync(paperId, versionId, cancellationToken)));

    [HttpPost("{paperId:guid}/versions/{versionId:guid}/enrich-question-bank")]
    public async Task<ActionResult<ApiResponse<int>>> Enrich(Guid paperId, Guid versionId, CancellationToken cancellationToken)
    {
        try { return Ok(ApiResponse<int>.Ok(await service.EnrichQuestionBankAsync(paperId, versionId, cancellationToken))); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<int>.Fail(ex.Message)); }
    }
}
