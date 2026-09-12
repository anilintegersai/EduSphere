using System.ComponentModel.DataAnnotations;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.People;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class LifecycleModel : PageModel
{
    private readonly IStudentTeacherLifecycleService _lifecycle;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Department> _departments;
    private readonly ICrudService<AcademicYear> _academicYears;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public LifecycleModel(
        IStudentTeacherLifecycleService lifecycle,
        ICrudService<StudentProfile> students,
        ICrudService<TeacherProfile> teachers,
        ICrudService<Branch> branches,
        ICrudService<Department> departments,
        ICrudService<AcademicYear> academicYears,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _lifecycle = lifecycle;
        _students = students;
        _teachers = teachers;
        _branches = branches;
        _departments = departments;
        _academicYears = academicYears;
        _courses = courses;
        _batches = batches;
        _sections = sections;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = new List<TeacherProfile>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<Department> Departments { get; private set; } = new List<Department>();
    public IReadOnlyList<AcademicYear> AcademicYears { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<StudentLifecycleEventDto> StudentEvents { get; private set; } = new List<StudentLifecycleEventDto>();
    public IReadOnlyList<TeacherLifecycleEventDto> TeacherEvents { get; private set; } = new List<TeacherLifecycleEventDto>();

    [BindProperty] public StudentLifecycleInput StudentInput { get; set; } = new();
    [BindProperty] public TeacherLifecycleInput TeacherInput { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }

    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null
            ? "-"
            : $"{student.AdmissionNumber} - {student.FirstName} {student.LastName}";
    }

    public string TeacherName(Guid id)
    {
        var teacher = Teachers.FirstOrDefault(t => t.Id == id);
        return teacher is null
            ? "-"
            : $"{teacher.EmployeeNumber} - {teacher.FirstName} {teacher.LastName}";
    }

    public string BranchName(Guid? id) => id.HasValue
        ? Branches.FirstOrDefault(b => b.Id == id.Value)?.Name ?? "-"
        : "-";

    public string SectionName(Guid? id) => id.HasValue
        ? Sections.FirstOrDefault(s => s.Id == id.Value)?.Name ?? "-"
        : "-";

    public string DepartmentName(Guid? id) => id.HasValue
        ? Departments.FirstOrDefault(d => d.Id == id.Value)?.Name ?? "-"
        : "-";

    public string AcademicYearName(Guid? id) => id.HasValue
        ? AcademicYears.FirstOrDefault(a => a.Id == id.Value)?.Name ?? "-"
        : "-";

    public string CourseName(Guid? id) => id.HasValue
        ? Courses.FirstOrDefault(c => c.Id == id.Value)?.Name ?? "-"
        : "-";

    public string BatchName(Guid? id) => id.HasValue
        ? Batches.FirstOrDefault(b => b.Id == id.Value)?.Name ?? "-"
        : "-";

    public async Task OnGetAsync()
    {
        if (!HasTenant)
            return;

        await LoadAsync();
    }

    public async Task<IActionResult> OnPostStudentAsync()
    {
        if (!HasTenant)
            return RedirectToPage();

        ModelState.Remove("TeacherInput.TeacherProfileId");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _lifecycle.RecordStudentEventAsync(User, new CreateStudentLifecycleEventRequest
        {
            StudentProfileId = StudentInput.StudentProfileId!.Value,
            EventType = StudentInput.EventType,
            ToStatus = StudentInput.ToStatus,
            ToBranchId = StudentInput.ToBranchId,
            ToAcademicYearId = StudentInput.ToAcademicYearId,
            ToCourseId = StudentInput.ToCourseId,
            ToBatchId = StudentInput.ToBatchId,
            ToSectionId = StudentInput.ToSectionId,
            EffectiveOn = StudentInput.EffectiveOn,
            Reason = StudentInput.Reason,
            Notes = StudentInput.Notes
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            await LoadAsync();
            return Page();
        }

        StatusMessage = result.Message ?? "Student lifecycle event recorded.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTeacherAsync()
    {
        if (!HasTenant)
            return RedirectToPage();

        ModelState.Remove("StudentInput.StudentProfileId");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _lifecycle.RecordTeacherEventAsync(User, new CreateTeacherLifecycleEventRequest
        {
            TeacherProfileId = TeacherInput.TeacherProfileId!.Value,
            EventType = TeacherInput.EventType,
            ToStatus = TeacherInput.ToStatus,
            ToBranchId = TeacherInput.ToBranchId,
            ToDepartmentId = TeacherInput.ToDepartmentId,
            EffectiveOn = TeacherInput.EffectiveOn,
            Reason = TeacherInput.Reason,
            Notes = TeacherInput.Notes
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            await LoadAsync();
            return Page();
        }

        StatusMessage = result.Message ?? "Teacher lifecycle event recorded.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Students = (await _students.ListAsync(s =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)))
            .OrderBy(s => s.AdmissionNumber)
            .ToList();
        Teachers = (await _teachers.ListAsync(t =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)))
            .OrderBy(t => t.EmployeeNumber)
            .ToList();
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Departments = (await _departments.ListAsync()).OrderBy(d => d.Name).ToList();
        AcademicYears = (await _academicYears.ListAsync()).OrderByDescending(a => a.StartDate).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Batches = (await _batches.ListAsync()).OrderBy(b => b.Name).ToList();
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        StudentEvents = await _lifecycle.ListStudentEventsAsync(User, take: 50);
        TeacherEvents = await _lifecycle.ListTeacherEventsAsync(User, take: 50);

        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue)
        {
            StudentInput.ToBranchId ??= assignedBranchId.Value;
            TeacherInput.ToBranchId ??= assignedBranchId.Value;
        }
    }

    public class StudentLifecycleInput
    {
        [Required, Display(Name = "Student")]
        public Guid? StudentProfileId { get; set; }

        [Display(Name = "Action")]
        public StudentLifecycleEventType EventType { get; set; } = StudentLifecycleEventType.SectionTransfer;

        [Display(Name = "New status")]
        public StudentStatus ToStatus { get; set; } = StudentStatus.Active;

        [Display(Name = "Target branch")]
        public Guid? ToBranchId { get; set; }

        [Display(Name = "Academic year")]
        public Guid? ToAcademicYearId { get; set; }

        [Display(Name = "Course")]
        public Guid? ToCourseId { get; set; }

        [Display(Name = "Batch")]
        public Guid? ToBatchId { get; set; }

        [Display(Name = "Section")]
        public Guid? ToSectionId { get; set; }

        [Required, Display(Name = "Effective date")]
        public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class TeacherLifecycleInput
    {
        [Required, Display(Name = "Teacher")]
        public Guid? TeacherProfileId { get; set; }

        [Display(Name = "Action")]
        public TeacherLifecycleEventType EventType { get; set; } = TeacherLifecycleEventType.DepartmentTransfer;

        [Display(Name = "New status")]
        public TeacherStatus ToStatus { get; set; } = TeacherStatus.Active;

        [Display(Name = "Target branch")]
        public Guid? ToBranchId { get; set; }

        [Display(Name = "Department")]
        public Guid? ToDepartmentId { get; set; }

        [Required, Display(Name = "Effective date")]
        public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
