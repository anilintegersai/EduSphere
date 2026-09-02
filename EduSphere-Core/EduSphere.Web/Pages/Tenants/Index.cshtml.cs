using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Tenants;

[Authorize(Policy = AuthorizationPolicies.SuperAdmin)]
public class IndexModel : PageModel
{
    private readonly ITenantService _tenants;

    public IndexModel(ITenantService tenants) => _tenants = tenants;

    public IReadOnlyList<Tenant> Tenants { get; private set; } = new List<Tenant>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsEditing => Input.Id != 0;

    public class InputModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Only lowercase letters, digits and hyphens.")]
        [Display(Name = "Identifier")]
        public string TenantIdentifier { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Custom domain")]
        public string? CustomDomain { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        await LoadAsync();
        if (editId is int id && await _tenants.GetTenantByIdAsync(id) is { } t)
            Input = Map(t);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        try
        {
            if (Input.Id == 0)
                await _tenants.CreateTenantAsync(Input.Name, Input.TenantIdentifier, Input.Description, Input.CustomDomain);
            else
                await _tenants.UpdateTenantAsync(Input.Id, Input.Name, Input.Description);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAsync();
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _tenants.DeactivateTenantAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostActivateAsync(int id)
    {
        await _tenants.ActivateTenantAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
        => Tenants = (await _tenants.GetAllTenantsAsync()).OrderBy(t => t.Name).ToList();

    private static InputModel Map(Tenant t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        TenantIdentifier = t.TenantIdentifier,
        CustomDomain = t.CustomDomain,
        Description = t.Description
    };
}
