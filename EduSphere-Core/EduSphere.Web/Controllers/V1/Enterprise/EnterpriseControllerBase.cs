using EduSphere.Application.Common;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[RequireTenant]
public abstract class EnterpriseControllerBase : ApiControllerBase
{
    protected EnterpriseControllerBase(IBranchAccessService branchAccess)
    {
        BranchAccess = branchAccess;
    }

    protected IBranchAccessService BranchAccess { get; }

    protected async Task<IReadOnlyList<T>> FilterByBranchAsync<T>(
        ICrudService<T> service,
        Func<T, Guid?> branchSelector,
        Guid? branchId = null)
        where T : class, IGuidEntity, ISoftDeletable
    {
        if (branchId.HasValue && !await CanUseBranchAsync(branchId.Value))
            return new List<T>();

        var assignedBranchId = await BranchAccess.GetAssignedBranchIdAsync(User);
        return (await service.ListAsync())
            .Where(e => (!BranchAccess.IsBranchAdminOnly(User) ||
                         (assignedBranchId.HasValue && branchSelector(e) == assignedBranchId.Value)) &&
                        (!branchId.HasValue || branchSelector(e) == branchId.Value))
            .ToList();
    }

    protected Task<bool> CanUseBranchAsync(Guid branchId)
        => BranchAccess.CanAccessBranchAsync(User, branchId);

    protected async Task<bool> CanUseOptionalBranchAsync(Guid? branchId)
        => !branchId.HasValue || await CanUseBranchAsync(branchId.Value);

    protected async Task<IActionResult> DeleteBranchScopedAsync<T>(
        ICrudService<T> service,
        Guid id,
        Func<T, Guid?> branchSelector,
        string displayName)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        if (entity is null || !await CanUseOptionalBranchAsync(branchSelector(entity)))
            return NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

        return await service.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));
    }

    protected async Task<T?> GetBranchScopedAsync<T>(
        ICrudService<T> service,
        Guid id,
        Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        return entity is not null && await CanUseOptionalBranchAsync(branchSelector(entity))
            ? entity
            : null;
    }
}
