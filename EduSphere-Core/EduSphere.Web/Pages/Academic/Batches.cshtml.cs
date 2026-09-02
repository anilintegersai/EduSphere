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
public class BatchesModel : PageModel
{
    private readonly ICrudService<Batch> _svc;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ITenantContext _tenant;

    public BatchesModel(ICrudService<Batch> svc, ICrudService<Course> courses, ICrudService<AcademicYear> years, ITenantContext tenant)
    {
        _svc = svc;
        _courses = courses;
        _years = years;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Batch> Items { get; private set; } = new List<Batch>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != 0;

    public string CourseName(int id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "—";
    public string YearName(int id) => Years.FirstOrDefault(y => y.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [Range(1, 100000)] public int Capacity { get; set; } = 60;
        [Range(1, int.MaxValue)] [Display(Name = "Course")] public int CourseId { get; set; }
        [Range(1, int.MaxValue)] [Display(Name = "Academic year")] public int AcademicYearId { get; set; }
    }

    public async Task OnGetAsync(int? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is int id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, Capacity = e.Capacity, CourseId = e.CourseId, AcademicYearId = e.AcademicYearId };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (await _courses.GetAsync(Input.CourseId) is null)
            ModelState.AddModelError("Input.CourseId", "Selected course was not found.");
        if (await _years.GetAsync(Input.AcademicYearId) is null)
            ModelState.AddModelError("Input.AcademicYearId", "Selected academic year was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == 0)
            await _svc.CreateAsync(new Batch { Name = Input.Name, Capacity = Input.Capacity, CourseId = Input.CourseId, AcademicYearId = Input.AcademicYearId });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.Capacity = Input.Capacity; e.CourseId = Input.CourseId; e.AcademicYearId = Input.AcademicYearId; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Items = (await _svc.ListAsync()).OrderBy(b => b.Name).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
    }
}
