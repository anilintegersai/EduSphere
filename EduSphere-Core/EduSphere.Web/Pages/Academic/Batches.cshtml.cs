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
    public bool IsEditing => Input.Id != Guid.Empty;

    public string CourseName(Guid id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "—";
    public string YearName(Guid id) => Years.FirstOrDefault(y => y.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [Range(1, 100000)] public int Capacity { get; set; } = 60;
        [Required] [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Required] [Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is Guid id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Name = e.Name, Capacity = e.Capacity, CourseId = e.CourseId, AcademicYearId = e.AcademicYearId };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        var courseId = Input.CourseId ?? Guid.Empty;
        var academicYearId = Input.AcademicYearId ?? Guid.Empty;

        if (courseId == Guid.Empty || await _courses.GetAsync(courseId) is null)
            ModelState.AddModelError("Input.CourseId", "Selected course was not found.");
        if (academicYearId == Guid.Empty || await _years.GetAsync(academicYearId) is null)
            ModelState.AddModelError("Input.AcademicYearId", "Selected academic year was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == Guid.Empty)
            await _svc.CreateAsync(new Batch { Name = Input.Name, Capacity = Input.Capacity, CourseId = courseId, AcademicYearId = academicYearId });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Name = Input.Name; e.Capacity = Input.Capacity; e.CourseId = courseId; e.AcademicYearId = academicYearId; });

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
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
