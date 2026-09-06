using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Enterprise;

public abstract class EnterprisePageModel : PageModel
{
    protected EnterprisePageModel(ITenantContext tenant, IBranchAccessService branchAccess)
    {
        Tenant = tenant;
        BranchAccess = branchAccess;
    }

    protected ITenantContext Tenant { get; }
    protected IBranchAccessService BranchAccess { get; }

    public bool HasTenant => Tenant.HasTenant;
    public IReadOnlyList<Branch> Branches { get; protected set; } = new List<Branch>();

    public string BranchName(Guid? id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";

    protected async Task LoadBranchesAsync(ICrudService<Branch> branches)
        => Branches = await BranchAccess.FilterBranchesAsync(User, await branches.ListAsync());

    protected async Task<IReadOnlyList<T>> FilterBranchScopedAsync<T>(ICrudService<T> service, Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var assignedBranchId = await BranchAccess.GetAssignedBranchIdAsync(User);
        return (await service.ListAsync())
            .Where(e => !BranchAccess.IsBranchAdminOnly(User) ||
                        (assignedBranchId.HasValue && branchSelector(e) == assignedBranchId.Value))
            .ToList();
    }

    protected async Task<bool> CanUseBranchAsync(Guid? branchId)
    {
        if (branchId is not Guid id)
            return !BranchAccess.IsBranchAdminOnly(User);
        return await BranchAccess.CanAccessBranchAsync(User, id);
    }

    protected async Task<Guid?> DefaultBranchIdAsync()
    {
        if (!BranchAccess.IsBranchAdminOnly(User))
            return null;
        return await BranchAccess.GetAssignedBranchIdAsync(User);
    }

    protected void KeepOnlyModelStateFor(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(k => !k.StartsWith(prefix, StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);
    }

    protected async Task<bool> ValidateBranchSelectionAsync(string fieldName, Guid? branchId, ICrudService<Branch> branches)
    {
        if (branchId is not Guid id)
        {
            ModelState.AddModelError(fieldName, "Branch is required.");
            return false;
        }

        if (await branches.GetAsync(id) is null)
        {
            ModelState.AddModelError(fieldName, "Selected branch was not found.");
            return false;
        }

        if (!await BranchAccess.CanAccessBranchAsync(User, id))
        {
            ModelState.AddModelError(fieldName, "You can manage records only for your assigned branch.");
            return false;
        }

        return true;
    }

    protected static string Brief(string? value, int length = 80)
        => string.IsNullOrWhiteSpace(value)
            ? "-"
            : value.Length <= length
                ? value
                : value[..length] + "...";
}
