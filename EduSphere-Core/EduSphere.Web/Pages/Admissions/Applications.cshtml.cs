using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Admissions;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class ApplicationsModel : PageModel
{
    private readonly ICrudService<AdmissionApplication> _applications;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<Section> _sections;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public ApplicationsModel(
        ICrudService<AdmissionApplication> applications,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<Section> sections,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _applications = applications;
        _branches = branches;
        _years = years;
        _courses = courses;
        _batches = batches;
        _sections = sections;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<AdmissionApplication> Items { get; private set; } = new List<AdmissionApplication>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string CourseName(Guid id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "-";
    public string BatchName(Guid? id) => Batches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string ApplicantName(AdmissionApplication application) =>
        string.Join(' ', new[] { application.ApplicantFirstName, application.ApplicantMiddleName, application.ApplicantLastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    public class InputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Course"), Required] public Guid? CourseId { get; set; }
        [Display(Name = "Batch")] public Guid? BatchId { get; set; }
        [Display(Name = "Section")] public Guid? SectionId { get; set; }
        [Required, StringLength(50), Display(Name = "Application #")] public string ApplicationNumber { get; set; } = string.Empty;
        [Required, StringLength(100), Display(Name = "First name")] public string ApplicantFirstName { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "Middle name")] public string? ApplicantMiddleName { get; set; }
        [Required, StringLength(100), Display(Name = "Last name")] public string ApplicantLastName { get; set; } = string.Empty;
        [Required, Display(Name = "Date of birth")] public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [EmailAddress, StringLength(150)] public string? Email { get; set; }
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
        [StringLength(150), Display(Name = "Guardian")] public string? GuardianName { get; set; }
        [StringLength(30), Display(Name = "Guardian phone")] public string? GuardianPhone { get; set; }
        [StringLength(500)] public string? Address { get; set; }
        [Required, Display(Name = "Applied on")] public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public AdmissionApplicationStatus Status { get; set; } = AdmissionApplicationStatus.Submitted;
        [StringLength(1000), Display(Name = "Review notes")] public string? ReviewNotes { get; set; }
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (editId is Guid id && await _applications.GetAsync(id) is { } entity && await CanUseBranchAsync(entity.BranchId))
            Input = Map(entity);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();

        var branchId = Input.BranchId ?? Guid.Empty;
        var courseId = Input.CourseId ?? Guid.Empty;

        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("Input.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("Input.BranchId", "You can manage applications only for your assigned branch.");
        if (courseId == Guid.Empty || await _courses.GetAsync(courseId) is null)
            ModelState.AddModelError("Input.CourseId", "Selected course was not found.");
        if (Input.AcademicYearId is Guid yearId && await _years.GetAsync(yearId) is null)
            ModelState.AddModelError("Input.AcademicYearId", "Selected academic year was not found.");
        if (Input.BatchId is Guid batchId && await _batches.GetAsync(batchId) is null)
            ModelState.AddModelError("Input.BatchId", "Selected batch was not found.");
        if (Input.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            ModelState.AddModelError("Input.SectionId", "Selected section was not found.");
        if (Input.Id != Guid.Empty && await _applications.GetAsync(Input.Id) is { } existing && !await CanUseBranchAsync(existing.BranchId))
            ModelState.AddModelError(string.Empty, "You can update applications only in your assigned branch.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (Input.Id == Guid.Empty)
            await _applications.CreateAsync(Apply(new AdmissionApplication(), branchId, courseId));
        else
            await _applications.UpdateAsync(Input.Id, e => Apply(e, branchId, courseId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        if (await _applications.GetAsync(id) is { } application && await CanUseBranchAsync(application.BranchId))
            await _applications.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Items = (await _applications.ListAsync(a =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && a.BranchId == assignedBranchId.Value)))
            .OrderByDescending(a => a.AppliedOn)
            .ThenBy(a => a.ApplicationNumber)
            .ToList();
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Batches = (await _batches.ListAsync()).OrderBy(b => b.Name).ToList();
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue && Input.BranchId is null)
            Input.BranchId = assignedBranchId.Value;
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private AdmissionApplication Apply(AdmissionApplication entity, Guid branchId, Guid courseId)
    {
        entity.BranchId = branchId;
        entity.AcademicYearId = Input.AcademicYearId;
        entity.CourseId = courseId;
        entity.BatchId = Input.BatchId;
        entity.SectionId = Input.SectionId;
        entity.ApplicationNumber = Input.ApplicationNumber;
        entity.ApplicantFirstName = Input.ApplicantFirstName;
        entity.ApplicantMiddleName = Input.ApplicantMiddleName;
        entity.ApplicantLastName = Input.ApplicantLastName;
        entity.DateOfBirth = Input.DateOfBirth;
        entity.Gender = Input.Gender;
        entity.Email = Input.Email;
        entity.PhoneNumber = Input.PhoneNumber;
        entity.GuardianName = Input.GuardianName;
        entity.GuardianPhone = Input.GuardianPhone;
        entity.Address = Input.Address;
        entity.AppliedOn = Input.AppliedOn;
        entity.Status = Input.Status;
        entity.ReviewNotes = Input.ReviewNotes;
        return entity;
    }

    private static InputModel Map(AdmissionApplication e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        SectionId = e.SectionId,
        ApplicationNumber = e.ApplicationNumber,
        ApplicantFirstName = e.ApplicantFirstName,
        ApplicantMiddleName = e.ApplicantMiddleName,
        ApplicantLastName = e.ApplicantLastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        GuardianName = e.GuardianName,
        GuardianPhone = e.GuardianPhone,
        Address = e.Address,
        AppliedOn = e.AppliedOn,
        Status = e.Status,
        ReviewNotes = e.ReviewNotes
    };
}
