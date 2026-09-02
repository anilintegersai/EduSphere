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
public class SectionsModel : PageModel
{
    private readonly ICrudService<Section> _svc;
    private readonly ICrudService<Batch> _batches;
    private readonly ITenantContext _tenant;

    public SectionsModel(ICrudService<Section> svc, ICrudService<Batch> batches, ITenantContext tenant)
    {
        _svc = svc;
        _batches = batches;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Section> Items { get; private set; } = new List<Section>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public string BatchName(int id) => Batches.FirstOrDefault(b => b.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public int Id { get; set; }
        [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
        [Range(1, 100000)] public int Capacity { get; set; } = 30;
        [Range(1, int.MaxValue)] [Display(Name = "Batch")] public int BatchId { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, Capacity = e.Capacity, BatchId = e.BatchId };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (await _batches.GetAsync(Input.BatchId) is null)
            ModelState.AddModelError("Input.BatchId", "Selected batch was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new Section { Name = Input.Name, Capacity = Input.Capacity, BatchId = Input.BatchId });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.Capacity = Input.Capacity; e.BatchId = Input.BatchId; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Items = (await _svc.ListAsync()).OrderBy(s => s.Name).ToList();
        Batches = (await _batches.ListAsync()).OrderBy(b => b.Name).ToList();
    }
}
