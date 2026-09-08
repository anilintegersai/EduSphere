using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Security;

public sealed class BranchAccessService : IBranchAccessService
{
    private static readonly string[] BranchScopedRoles =
    {
        Roles.BranchAdmin,
        Roles.Principal,
        Roles.DepartmentAdmin,
        Roles.Teacher,
        Roles.Accountant,
        Roles.Librarian,
        Roles.TransportManager,
        Roles.HostelManager,
        Roles.ExamController,
        Roles.StaffAdmin
    };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TenantDbContext _dbContext;

    public BranchAccessService(
        UserManager<ApplicationUser> userManager,
        TenantDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public bool IsBranchAdminOnly(ClaimsPrincipal user)
        => BranchScopedRoles.Any(user.IsInRole) &&
           !user.IsInRole(Roles.SuperAdmin) &&
           !user.IsInRole(Roles.TenantAdmin);

    public Guid? GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        return Guid.TryParse(value, out var id) ? id : null;
    }

    public async Task<Guid?> GetAssignedBranchIdAsync(ClaimsPrincipal user)
    {
        var branchClaim = user.FindFirstValue("branch_id");
        if (Guid.TryParse(branchClaim, out var branchId) && branchId != Guid.Empty)
            return branchId;

        var userId = GetUserId(user);
        if (!userId.HasValue)
            return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var assignedBranchId = await _dbContext.UserBranchAssignments
            .IgnoreQueryFilters()
            .Where(a => a.UserId == userId.Value &&
                        a.IsActive &&
                        a.EffectiveFrom <= today &&
                        (!a.EffectiveUntil.HasValue || a.EffectiveUntil.Value >= today))
            .OrderByDescending(a => a.IsPrimary)
            .ThenByDescending(a => a.EffectiveFrom)
            .Select(a => (Guid?)a.BranchId)
            .FirstOrDefaultAsync();

        if (assignedBranchId.HasValue)
            return assignedBranchId.Value;

        var applicationUser = await _userManager.FindByIdAsync(userId.Value.ToString());
        return applicationUser?.BranchId;
    }

    public async Task<bool> CanAccessBranchAsync(ClaimsPrincipal user, Guid branchId)
    {
        if (!IsBranchAdminOnly(user))
            return true;

        var assignedBranchId = await GetAssignedBranchIdAsync(user);
        return assignedBranchId.HasValue && assignedBranchId.Value == branchId;
    }

    public async Task<IReadOnlyList<Branch>> FilterBranchesAsync(ClaimsPrincipal user, IEnumerable<Branch> branches)
    {
        var orderedBranches = branches.OrderBy(b => b.Name).ToList();
        if (!IsBranchAdminOnly(user))
            return orderedBranches;

        var assignedBranchId = await GetAssignedBranchIdAsync(user);
        return assignedBranchId.HasValue
            ? orderedBranches.Where(b => b.Id == assignedBranchId.Value).ToList()
            : new List<Branch>();
    }
}
