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

namespace EduSphere.Web.Pages.People;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class TeachersModel : PageModel
{
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Branch> _branches;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public TeachersModel(
        ICrudService<TeacherProfile> teachers,
        ICrudService<Branch> branches,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _teachers = teachers;
        _branches = branches;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<TeacherProfile> Items { get; private set; } = new List<TeacherProfile>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";

    public class InputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Required, StringLength(50), Display(Name = "Employee #")] public string EmployeeNumber { get; set; } = string.Empty;
        [Required, StringLength(100), Display(Name = "First name")] public string FirstName { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "Middle name")] public string? MiddleName { get; set; }
        [Required, StringLength(100), Display(Name = "Last name")] public string LastName { get; set; } = string.Empty;
        [Display(Name = "Date of birth")] public DateOnly? DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [EmailAddress, StringLength(150)] public string? Email { get; set; }
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
        [StringLength(120)] public string? Designation { get; set; }
        [StringLength(500)] public string? Qualifications { get; set; }
        [StringLength(500)] public string? Specializations { get; set; }
        [Range(0, 80), Display(Name = "Experience")] public decimal ExperienceYears { get; set; }
        [Required, Display(Name = "Joining date")] public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;

        await LoadAsync();
        if (editId is Guid id && await _teachers.GetAsync(id) is { } e && await CanUseBranchAsync(e.BranchId))
            Input = Map(e);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();

        var branchId = Input.BranchId ?? Guid.Empty;
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("Input.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("Input.BranchId", "You can manage teachers only for your assigned branch.");
        if (Input.Id != Guid.Empty && await _teachers.GetAsync(Input.Id) is { } existing && !await CanUseBranchAsync(existing.BranchId))
            ModelState.AddModelError(string.Empty, "You can update teachers only in your assigned branch.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (Input.Id == Guid.Empty)
            await _teachers.CreateAsync(Apply(new TeacherProfile(), branchId));
        else
            await _teachers.UpdateAsync(Input.Id, e => Apply(e, branchId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        if (await _teachers.GetAsync(id) is { } teacher && await CanUseBranchAsync(teacher.BranchId))
            await _teachers.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Items = (await _teachers.ListAsync(t =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)))
            .OrderBy(t => t.EmployeeNumber)
            .ToList();
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue && Input.BranchId is null)
            Input.BranchId = assignedBranchId.Value;
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private TeacherProfile Apply(TeacherProfile entity, Guid branchId)
    {
        entity.BranchId = branchId;
        entity.EmployeeNumber = Input.EmployeeNumber;
        entity.FirstName = Input.FirstName;
        entity.MiddleName = Input.MiddleName;
        entity.LastName = Input.LastName;
        entity.DateOfBirth = Input.DateOfBirth;
        entity.Gender = Input.Gender;
        entity.Email = Input.Email;
        entity.PhoneNumber = Input.PhoneNumber;
        entity.Designation = Input.Designation;
        entity.Qualifications = Input.Qualifications;
        entity.Specializations = Input.Specializations;
        entity.ExperienceYears = Input.ExperienceYears;
        entity.JoiningDate = Input.JoiningDate;
        entity.Status = Input.Status;
        return entity;
    }

    private static InputModel Map(TeacherProfile e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        EmployeeNumber = e.EmployeeNumber,
        FirstName = e.FirstName,
        MiddleName = e.MiddleName,
        LastName = e.LastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Designation = e.Designation,
        Qualifications = e.Qualifications,
        Specializations = e.Specializations,
        ExperienceYears = e.ExperienceYears,
        JoiningDate = e.JoiningDate,
        Status = e.Status
    };
}
