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
public class SyllabusUnitsModel : PageModel
{
    private readonly ICrudService<SyllabusUnit> _svc;
    private readonly ICrudService<Subject> _subjects;
    private readonly ITenantContext _tenant;

    public SyllabusUnitsModel(ICrudService<SyllabusUnit> svc, ICrudService<Subject> subjects, ITenantContext tenant)
    {
        _svc = svc;
        _subjects = subjects;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<SyllabusUnit> Items { get; private set; } = new List<SyllabusUnit>();
    public IReadOnlyList<Subject> Subjects { get; private set; } = new List<Subject>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public string SubjectName(int id) => Subjects.FirstOrDefault(s => s.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue)] [Display(Name = "Subject")] public int SubjectId { get; set; }
        [Range(0, 1000)] public int Order { get; set; }
        [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
        [StringLength(1000)] public string? Description { get; set; }
        [Range(0, 10000)] [Display(Name = "Estimated hours")] public int EstimatedHours { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, SubjectId = e.SubjectId, Order = e.Order, Title = e.Title, Description = e.Description, EstimatedHours = e.EstimatedHours };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (await _subjects.GetAsync(Input.SubjectId) is null)
            ModelState.AddModelError("Input.SubjectId", "Selected subject was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new SyllabusUnit { SubjectId = Input.SubjectId, Order = Input.Order, Title = Input.Title, Description = Input.Description, EstimatedHours = Input.EstimatedHours });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Order = Input.Order; e.Title = Input.Title; e.Description = Input.Description; e.EstimatedHours = Input.EstimatedHours; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Items = (await _svc.ListAsync()).OrderBy(u => u.SubjectId).ThenBy(u => u.Order).ToList();
        Subjects = (await _subjects.ListAsync()).OrderBy(s => s.Name).ToList();
    }
}
