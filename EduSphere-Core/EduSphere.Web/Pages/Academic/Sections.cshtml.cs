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
    public bool IsEditing => Input.Id != Guid.Empty;

    public string BatchName(Guid id) => Batches.FirstOrDefault(b => b.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
        [Range(1, 100000)] public int Capacity { get; set; } = 30;
        [Required] [Display(Name = "Batch")] public Guid? BatchId { get; set; }
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is Guid id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, Capacity = e.Capacity, BatchId = e.BatchId };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        var batchId = Input.BatchId ?? Guid.Empty;

        if (batchId == Guid.Empty || await _batches.GetAsync(batchId) is null)
            ModelState.AddModelError("Input.BatchId", "Selected batch was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == Guid.Empty)
            await _svc.CreateAsync(new Section { Name = Input.Name, Capacity = Input.Capacity, BatchId = batchId });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.Capacity = Input.Capacity; e.BatchId = batchId; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
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
