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
public class StudentsModel : PageModel
{
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<Section> _sections;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public StudentsModel(
        ICrudService<StudentProfile> students,
        ICrudService<Branch> branches,
        ICrudService<Section> sections,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _students = students;
        _branches = branches;
        _sections = sections;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public IReadOnlyList<StudentProfile> Items { get; private set; } = new List<StudentProfile>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();

    [BindProperty] public InputModel Input { get; set; } = new();
    public bool IsEditing => Input.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string SectionName(Guid? id) => Sections.FirstOrDefault(s => s.Id == id)?.Name ?? "-";

    public class InputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Section")] public Guid? SectionId { get; set; }
        [Required, StringLength(50), Display(Name = "Admission #")] public string AdmissionNumber { get; set; } = string.Empty;
        [StringLength(50), Display(Name = "Roll #")] public string? RollNumber { get; set; }
        [Required, StringLength(100), Display(Name = "First name")] public string FirstName { get; set; } = string.Empty;
        [StringLength(100), Display(Name = "Middle name")] public string? MiddleName { get; set; }
        [Required, StringLength(100), Display(Name = "Last name")] public string LastName { get; set; } = string.Empty;
        [Required, Display(Name = "Date of birth")] public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [Display(Name = "Blood group")] public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;
        [Required, Display(Name = "Admission date")] public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [EmailAddress, StringLength(150)] public string? Email { get; set; }
        [StringLength(30), Display(Name = "Phone")] public string? PhoneNumber { get; set; }
        [StringLength(150), Display(Name = "Emergency contact")] public string? EmergencyContactName { get; set; }
        [StringLength(30), Display(Name = "Emergency phone")] public string? EmergencyContactPhone { get; set; }
        [StringLength(500)] public string? Address { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
    }

    public async Task OnGetAsync(Guid? editId)
    {
        if (!HasTenant) return;

        await LoadAsync();
        if (editId is Guid id && await _students.GetAsync(id) is { } e && await CanUseBranchAsync(e.BranchId))
            Input = Map(e);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HasTenant) return RedirectToPage();

        var branchId = Input.BranchId ?? Guid.Empty;
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("Input.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("Input.BranchId", "You can manage students only for your assigned branch.");

        if (Input.SectionId is Guid sectionId && await _sections.GetAsync(sectionId) is null)
            ModelState.AddModelError("Input.SectionId", "Selected section was not found.");
        if (Input.Id != Guid.Empty && await _students.GetAsync(Input.Id) is { } existing && !await CanUseBranchAsync(existing.BranchId))
            ModelState.AddModelError(string.Empty, "You can update students only in your assigned branch.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        if (Input.Id == Guid.Empty)
            await _students.CreateAsync(Apply(new StudentProfile(), branchId));
        else
            await _students.UpdateAsync(Input.Id, e => Apply(e, branchId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        if (await _students.GetAsync(id) is { } student && await CanUseBranchAsync(student.BranchId))
            await _students.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Items = (await _students.ListAsync(s =>
                !_branchAccess.IsBranchAdminOnly(User) ||
                (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)))
            .OrderBy(s => s.AdmissionNumber)
            .ToList();
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue && Input.BranchId is null)
            Input.BranchId = assignedBranchId.Value;
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private StudentProfile Apply(StudentProfile entity, Guid branchId)
    {
        entity.BranchId = branchId;
        entity.SectionId = Input.SectionId;
        entity.AdmissionNumber = Input.AdmissionNumber;
        entity.RollNumber = Input.RollNumber;
        entity.FirstName = Input.FirstName;
        entity.MiddleName = Input.MiddleName;
        entity.LastName = Input.LastName;
        entity.DateOfBirth = Input.DateOfBirth;
        entity.Gender = Input.Gender;
        entity.BloodGroup = Input.BloodGroup;
        entity.AdmissionDate = Input.AdmissionDate;
        entity.Email = Input.Email;
        entity.PhoneNumber = Input.PhoneNumber;
        entity.EmergencyContactName = Input.EmergencyContactName;
        entity.EmergencyContactPhone = Input.EmergencyContactPhone;
        entity.Address = Input.Address;
        entity.Status = Input.Status;
        return entity;
    }

    private static InputModel Map(StudentProfile e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        SectionId = e.SectionId,
        AdmissionNumber = e.AdmissionNumber,
        RollNumber = e.RollNumber,
        FirstName = e.FirstName,
        MiddleName = e.MiddleName,
        LastName = e.LastName,
        DateOfBirth = e.DateOfBirth,
        Gender = e.Gender,
        BloodGroup = e.BloodGroup,
        AdmissionDate = e.AdmissionDate,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        EmergencyContactName = e.EmergencyContactName,
        EmergencyContactPhone = e.EmergencyContactPhone,
        Address = e.Address,
        Status = e.Status
    };
}
