using System.Security.Claims;
using EduSphere.Domain.Entities;

namespace EduSphere.Web.Security;

public interface IBranchAccessService
{
    bool IsBranchAdminOnly(ClaimsPrincipal user);
    Guid? GetUserId(ClaimsPrincipal user);
    Task<Guid?> GetAssignedBranchIdAsync(ClaimsPrincipal user);
    Task<bool> CanAccessBranchAsync(ClaimsPrincipal user, Guid branchId);
    Task<IReadOnlyList<Branch>> FilterBranchesAsync(ClaimsPrincipal user, IEnumerable<Branch> branches);
}
