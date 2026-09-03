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
public class SubjectsModel : PageModel
{
    private readonly ICrudService<Subject> _svc;
    private readonly ICrudService<Course> _courses;
    private readonly ITenantContext _tenant;

    public SubjectsModel(ICrudService<Subject> svc, ICrudService<Course> courses, ITenantContext tenant)
    {
        _svc = svc;
        _courses = courses;
        _tenant = tenant;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<Subject> Items { get; private set; } = new List<Subject>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != Guid.Empty;

    public string CourseName(Guid? id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "—";

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(30)] public string Code { get; set; } = string.Empty;
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        public SubjectType Type { get; set; }
        [Range(0, 100)] public int Credits { get; set; } = 3;
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is Guid id && await _svc.GetAsync(id) is { } e)
            Input = new InputModel { Id = e.Id, Code = e.Code, Name = e.Name, Type = e.Type, Credits = e.Credits, CourseId = e.CourseId };
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (Input.CourseId is Guid courseId && await _courses.GetAsync(courseId) is null)
            ModelState.AddModelError("Input.CourseId", "Selected course was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (Input.Id == Guid.Empty)
            await _svc.CreateAsync(new Subject { Code = Input.Code, Name = Input.Name, Type = Input.Type, Credits = Input.Credits, CourseId = Input.CourseId });
        else
            await _svc.UpdateAsync(Input.Id, e => { e.Code = Input.Code; e.Name = Input.Name; e.Type = Input.Type; e.Credits = Input.Credits; e.CourseId = Input.CourseId; });

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
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
    }
}
