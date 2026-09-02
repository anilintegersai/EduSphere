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
public class AcademicYearsModel : PageModel
{
    private readonly ICrudService<AcademicYear> _svc;
    private readonly ITenantContext _tenant;

    public AcademicYearsModel(ICrudService<AcademicYear> svc, ITenantContext tenant)
    {
        _svc = svc;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<AcademicYear> Items { get; private set; } = new List<AcademicYear>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public class InputModel
    {
        public int Id { get; set; }
        [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
        [DataType(DataType.Date)] public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [DataType(DataType.Date)] public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
        [Display(Name = "Current year")] public bool IsCurrent { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrent = e.IsCurrent };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (Input.EndDate <= Input.StartDate)
            ModelState.AddModelError("Input.EndDate", "End date must be after the start date.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new AcademicYear { Name = Input.Name, StartDate = Input.StartDate, EndDate = Input.EndDate, IsCurrent = Input.IsCurrent });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.StartDate = Input.StartDate; e.EndDate = Input.EndDate; e.IsCurrent = Input.IsCurrent; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync() => Items = (await _svc.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
}
