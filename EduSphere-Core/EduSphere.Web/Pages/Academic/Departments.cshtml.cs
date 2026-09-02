using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Academic;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public class DepartmentsModel : PageModel
{
    private readonly ICrudService<Department> _svc;
    private readonly ITenantContext _tenant;

    public DepartmentsModel(ICrudService<Department> svc, ITenantContext tenant)
    {
        _svc = svc;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Department> Items { get; private set; } = new List<Department>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public class InputModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(30)] public string Code { get; set; } = string.Empty;
        [StringLength(255)] public string? Description { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, Code = e.Code, Description = e.Description };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new Department { Name = Input.Name, Code = Input.Code, Description = Input.Description });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.Code = Input.Code; e.Description = Input.Description; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync() => Items = (await _svc.ListAsync()).OrderBy(d => d.Name).ToList();
}
