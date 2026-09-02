using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Academic;

[Authorize(Policy = AuthorizationPolicies.TenantAdmin)]
public class CoursesModel : PageModel
{
    private readonly ICrudService<Course> _svc;
    private readonly ICrudService<Department> _departments;
    private readonly ITenantContext _tenant;

    public CoursesModel(ICrudService<Course> svc, ICrudService<Department> departments, ITenantContext tenant)
    {
        _svc = svc;
        _departments = departments;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Course> Items { get; private set; } = new List<Course>();
    public IReadOnlyList<Department> Departments { get; private set; } = new List<Department>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public string DepartmentName(int? id) => Departments.FirstOrDefault(d => d.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public int Id { get; set; }
        [Required, StringLength(30)] public string Code { get; set; } = string.Empty;
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        public CourseType Type { get; set; }
        [Range(1, 120)] [Display(Name = "Duration (months)")] public int DurationMonths { get; set; } = 12;
        [Display(Name = "Department")] public int? DepartmentId { get; set; }
        [StringLength(500)] public string? Description { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Code = e.Code, Name = e.Name, Type = e.Type, DurationMonths = e.DurationMonths, DepartmentId = e.DepartmentId, Description = e.Description };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (Input.DepartmentId is int deptId && await _departments.GetAsync(deptId) is null)
            ModelState.AddModelError("Input.DepartmentId", "Selected department was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new Course { Code = Input.Code, Name = Input.Name, Type = Input.Type, DurationMonths = Input.DurationMonths, DepartmentId = Input.DepartmentId, Description = Input.Description });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Code = Input.Code; e.Name = Input.Name; e.Type = Input.Type; e.DurationMonths = Input.DurationMonths; e.DepartmentId = Input.DepartmentId; e.Description = Input.Description; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Items = (await _svc.ListAsync()).OrderBy(c => c.Name).ToList();
        Departments = (await _departments.ListAsync()).OrderBy(d => d.Name).ToList();
    }
}
