using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Branches;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public class IndexModel : PageModel
{
    private readonly IBranchService _branches;
    private readonly ITenantContext _tenant;

    public IndexModel(IBranchService branches, ITenantContext tenant)
    {
        _branches = branches;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsEditing => Input.Id != Guid.Empty;

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        [StringLength(255)] public string? Address { get; set; }
        [StringLength(50)] public string? City { get; set; }
        [StringLength(50)] public string? State { get; set; }
        [StringLength(50)] public string? Country { get; set; } = "India";
        [StringLength(20)] public string? Pincode { get; set; }
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is Guid id && await _branches.GetBranchByIdAsync(id) is { } b)
            Input = Map(b);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (_tenant.TenantId is not Guid tenantId)
            return RedirectToPage();

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (Input.Id == Guid.Empty)
            await _branches.CreateBranchAsync(tenantId, Input.Name, Input.Code,
                Input.Address ?? "", Input.City ?? "", Input.State ?? "", Input.Country ?? "India", Input.Pincode ?? "");
        else
            await _branches.UpdateBranchAsync(Input.Id, Input.Name, Input.Code,
                Input.Address ?? "", Input.City ?? "", Input.State ?? "", Input.Country ?? "India", Input.Pincode ?? "");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _branches.DeleteBranchAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        if (_tenant.TenantId is Guid tenantId)
            Branches = (await _branches.GetBranchesByTenantAsync(tenantId)).OrderBy(b => b.Name).ToList();
    }

    private static InputModel Map(Branch b) => new()
    {
        Id = b.Id,
        Name = b.Name,
        Code = b.Code ?? string.Empty,
        Address = b.Address,
        City = b.City,
        State = b.State,
        Country = b.Country,
        Pincode = b.Pincode
    };
}
