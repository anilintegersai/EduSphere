using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Pages.Enterprise;

[Authorize(Policy = AuthorizationPolicies.FinanceManager)]
public class FinanceModel : EnterprisePageModel
{
    private readonly ICrudService<FeeStructure> _structures;
    private readonly ICrudService<FeeComponent> _components;
    private readonly ICrudService<DiscountRule> _discounts;
    private readonly ICrudService<Scholarship> _scholarships;
    private readonly ICrudService<StudentFeeAssignment> _assignments;
    private readonly ICrudService<FeeInvoice> _invoices;
    private readonly ICrudService<FeePayment> _payments;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Batch> _batches;
    private readonly ICrudService<StudentProfile> _students;

    public FinanceModel(
        ICrudService<FeeStructure> structures,
        ICrudService<FeeComponent> components,
        ICrudService<DiscountRule> discounts,
        ICrudService<Scholarship> scholarships,
        ICrudService<StudentFeeAssignment> assignments,
        ICrudService<FeeInvoice> invoices,
        ICrudService<FeePayment> payments,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Course> courses,
        ICrudService<Batch> batches,
        ICrudService<StudentProfile> students,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
        : base(tenant, branchAccess)
    {
        _structures = structures;
        _components = components;
        _discounts = discounts;
        _scholarships = scholarships;
        _assignments = assignments;
        _invoices = invoices;
        _payments = payments;
        _branches = branches;
        _years = years;
        _courses = courses;
        _batches = batches;
        _students = students;
    }

    public IReadOnlyList<FeeStructure> Structures { get; private set; } = new List<FeeStructure>();
    public IReadOnlyList<FeeComponent> Components { get; private set; } = new List<FeeComponent>();
    public IReadOnlyList<DiscountRule> Discounts { get; private set; } = new List<DiscountRule>();
    public IReadOnlyList<Scholarship> Scholarships { get; private set; } = new List<Scholarship>();
    public IReadOnlyList<StudentFeeAssignment> Assignments { get; private set; } = new List<StudentFeeAssignment>();
    public IReadOnlyList<FeeInvoice> Invoices { get; private set; } = new List<FeeInvoice>();
    public IReadOnlyList<FeePayment> Payments { get; private set; } = new List<FeePayment>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Course> Courses { get; private set; } = new List<Course>();
    public IReadOnlyList<Batch> Batches { get; private set; } = new List<Batch>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();

    [BindProperty] public StructureInputModel StructureInput { get; set; } = new();
    [BindProperty] public ComponentInputModel ComponentInput { get; set; } = new();
    [BindProperty] public DiscountInputModel DiscountInput { get; set; } = new();
    [BindProperty] public ScholarshipInputModel ScholarshipInput { get; set; } = new();
    [BindProperty] public AssignmentInputModel AssignmentInput { get; set; } = new();
    [BindProperty] public InvoiceInputModel InvoiceInput { get; set; } = new();
    [BindProperty] public PaymentInputModel PaymentInput { get; set; } = new();

    public class StructureInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Academic year")] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Display(Name = "Batch")] public Guid? BatchId { get; set; }
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        public FeeFrequency Frequency { get; set; } = FeeFrequency.Term;
        [Display(Name = "Effective from")] public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Effective to")] public DateOnly? EffectiveTo { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; } = true;
        [StringLength(500)] public string? Notes { get; set; }
    }

    public class ComponentInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Fee structure")] public Guid? FeeStructureId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        public FeeComponentType Type { get; set; } = FeeComponentType.Tuition;
        [Range(0, 10000000)] public decimal Amount { get; set; }
        [Display(Name = "Optional")] public bool IsOptional { get; set; }
        [Range(0, 365), Display(Name = "Due days")] public int DueDaysFromStart { get; set; }
        [Range(0, 1000), Display(Name = "Order")] public int SortOrder { get; set; }
        [StringLength(50), Display(Name = "Ledger code")] public string? LedgerCode { get; set; }
    }

    public class DiscountInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Course")] public Guid? CourseId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        public DiscountType Type { get; set; } = DiscountType.Percentage;
        [Range(0, 10000000)] public decimal Value { get; set; }
        [Display(Name = "Effective from")] public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Active")] public bool IsActive { get; set; } = true;
        [StringLength(500), Display(Name = "Eligibility")] public string? EligibilityCriteria { get; set; }
    }

    public class ScholarshipInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        public DiscountType Type { get; set; } = DiscountType.Amount;
        [Range(0, 10000000)] public decimal Value { get; set; }
        [Display(Name = "Awarded on")] public DateOnly AwardedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [StringLength(150), Display(Name = "Sponsor")] public string? SponsorName { get; set; }
    }

    public class AssignmentInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, Display(Name = "Fee structure")] public Guid? FeeStructureId { get; set; }
        [Display(Name = "Discount rule")] public Guid? DiscountRuleId { get; set; }
        [Display(Name = "Assigned on")] public DateOnly AssignedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public FeeAssignmentStatus Status { get; set; } = FeeAssignmentStatus.Active;
        [Range(0, 10000000), Display(Name = "Custom discount")] public decimal CustomDiscountAmount { get; set; }
    }

    public class InvoiceInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Assignment")] public Guid? StudentFeeAssignmentId { get; set; }
        [Required, Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, StringLength(60), Display(Name = "Invoice #")] public string InvoiceNumber { get; set; } = string.Empty;
        [Display(Name = "Invoice date")] public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Due date")] public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15));
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
        [Range(0, 10000000), Display(Name = "Subtotal")] public decimal SubTotal { get; set; }
        [Range(0, 10000000), Display(Name = "Discount")] public decimal DiscountAmount { get; set; }
        [Range(0, 10000000), Display(Name = "Fine")] public decimal FineAmount { get; set; }
    }

    public class PaymentInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Invoice")] public Guid? FeeInvoiceId { get; set; }
        [Required, StringLength(60), Display(Name = "Payment #")] public string PaymentNumber { get; set; } = string.Empty;
        [Range(0.01, 10000000)] public decimal Amount { get; set; }
        public PaymentMode Mode { get; set; } = PaymentMode.Cash;
        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
        [StringLength(120), Display(Name = "Reference")] public string? TransactionReference { get; set; }
    }

    public string YearName(Guid id) => Years.FirstOrDefault(y => y.Id == id)?.Name ?? "-";
    public string CourseName(Guid? id) => Courses.FirstOrDefault(c => c.Id == id)?.Name ?? "-";
    public string BatchName(Guid? id) => Batches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string StructureName(Guid id) => Structures.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string DiscountName(Guid? id) => Discounts.FirstOrDefault(d => d.Id == id)?.Name ?? "-";
    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }

    public string AssignmentName(Guid id)
    {
        var assignment = Assignments.FirstOrDefault(a => a.Id == id);
        return assignment is null ? "-" : $"{StudentName(assignment.StudentProfileId)} / {StructureName(assignment.FeeStructureId)}";
    }

    public string InvoiceName(Guid id) => Invoices.FirstOrDefault(i => i.Id == id)?.InvoiceNumber ?? "-";

    public async Task OnGetAsync()
    {
        if (!HasTenant) return;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveStructureAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(StructureInput));
        if (!await ValidateBranchSelectionAsync("StructureInput.BranchId", StructureInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (StructureInput.AcademicYearId is Guid yearId && await _years.GetAsync(yearId) is null)
            ModelState.AddModelError("StructureInput.AcademicYearId", "Selected academic year was not found.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _structures.CreateAsync(new FeeStructure
        {
            BranchId = StructureInput.BranchId!.Value,
            AcademicYearId = StructureInput.AcademicYearId!.Value,
            CourseId = StructureInput.CourseId,
            BatchId = StructureInput.BatchId,
            Name = StructureInput.Name,
            Code = StructureInput.Code,
            Frequency = StructureInput.Frequency,
            EffectiveFrom = StructureInput.EffectiveFrom,
            EffectiveTo = StructureInput.EffectiveTo,
            IsActive = StructureInput.IsActive,
            Notes = StructureInput.Notes
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveComponentAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ComponentInput));
        if (!await ValidateBranchSelectionAsync("ComponentInput.BranchId", ComponentInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (ComponentInput.FeeStructureId is not Guid structureId || await _structures.GetAsync(structureId) is not { } structure || structure.BranchId != ComponentInput.BranchId)
            ModelState.AddModelError("ComponentInput.FeeStructureId", "Selected fee structure was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _components.CreateAsync(new FeeComponent
        {
            BranchId = ComponentInput.BranchId!.Value,
            FeeStructureId = ComponentInput.FeeStructureId!.Value,
            Name = ComponentInput.Name,
            Type = ComponentInput.Type,
            Amount = ComponentInput.Amount,
            IsOptional = ComponentInput.IsOptional,
            DueDaysFromStart = ComponentInput.DueDaysFromStart,
            SortOrder = ComponentInput.SortOrder,
            LedgerCode = ComponentInput.LedgerCode
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveDiscountAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(DiscountInput));
        if (!await CanUseBranchAsync(DiscountInput.BranchId))
            ModelState.AddModelError("DiscountInput.BranchId", "Select your assigned branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _discounts.CreateAsync(new DiscountRule
        {
            BranchId = DiscountInput.BranchId,
            CourseId = DiscountInput.CourseId,
            Name = DiscountInput.Name,
            Code = DiscountInput.Code,
            Type = DiscountInput.Type,
            Value = DiscountInput.Value,
            EffectiveFrom = DiscountInput.EffectiveFrom,
            IsActive = DiscountInput.IsActive,
            EligibilityCriteria = DiscountInput.EligibilityCriteria
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveScholarshipAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ScholarshipInput));
        if (!await ValidateStudentBranchAsync("ScholarshipInput.StudentProfileId", ScholarshipInput.StudentProfileId, ScholarshipInput.BranchId))
            ModelState.AddModelError(string.Empty, "Fix student selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _scholarships.CreateAsync(new Scholarship
        {
            BranchId = ScholarshipInput.BranchId!.Value,
            StudentProfileId = ScholarshipInput.StudentProfileId!.Value,
            Name = ScholarshipInput.Name,
            Code = ScholarshipInput.Code,
            Type = ScholarshipInput.Type,
            Value = ScholarshipInput.Value,
            AwardedOn = ScholarshipInput.AwardedOn,
            SponsorName = ScholarshipInput.SponsorName
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveAssignmentAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(AssignmentInput));
        if (!await ValidateStudentBranchAsync("AssignmentInput.StudentProfileId", AssignmentInput.StudentProfileId, AssignmentInput.BranchId))
            ModelState.AddModelError(string.Empty, "Fix student selection.");
        if (AssignmentInput.FeeStructureId is not Guid structureId || await _structures.GetAsync(structureId) is not { } structure || structure.BranchId != AssignmentInput.BranchId)
            ModelState.AddModelError("AssignmentInput.FeeStructureId", "Selected fee structure was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _assignments.CreateAsync(new StudentFeeAssignment
        {
            BranchId = AssignmentInput.BranchId!.Value,
            StudentProfileId = AssignmentInput.StudentProfileId!.Value,
            FeeStructureId = AssignmentInput.FeeStructureId!.Value,
            DiscountRuleId = AssignmentInput.DiscountRuleId,
            AssignedOn = AssignmentInput.AssignedOn,
            Status = AssignmentInput.Status,
            CustomDiscountAmount = AssignmentInput.CustomDiscountAmount
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveInvoiceAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(InvoiceInput));
        if (!await ValidateStudentBranchAsync("InvoiceInput.StudentProfileId", InvoiceInput.StudentProfileId, InvoiceInput.BranchId))
            ModelState.AddModelError(string.Empty, "Fix student selection.");
        if (InvoiceInput.StudentFeeAssignmentId is not Guid assignmentId || await _assignments.GetAsync(assignmentId) is not { } assignment || assignment.BranchId != InvoiceInput.BranchId || assignment.StudentProfileId != InvoiceInput.StudentProfileId)
            ModelState.AddModelError("InvoiceInput.StudentFeeAssignmentId", "Selected assignment was not found for this student.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        var total = InvoiceInput.SubTotal - InvoiceInput.DiscountAmount + InvoiceInput.FineAmount;
        await _invoices.CreateAsync(new FeeInvoice
        {
            BranchId = InvoiceInput.BranchId!.Value,
            StudentFeeAssignmentId = InvoiceInput.StudentFeeAssignmentId!.Value,
            StudentProfileId = InvoiceInput.StudentProfileId!.Value,
            InvoiceNumber = InvoiceInput.InvoiceNumber,
            InvoiceDate = InvoiceInput.InvoiceDate,
            DueDate = InvoiceInput.DueDate,
            Status = InvoiceInput.Status,
            SubTotal = InvoiceInput.SubTotal,
            DiscountAmount = InvoiceInput.DiscountAmount,
            FineAmount = InvoiceInput.FineAmount,
            TotalAmount = Math.Max(total, 0)
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSavePaymentAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(PaymentInput));
        if (!await ValidateBranchSelectionAsync("PaymentInput.BranchId", PaymentInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (PaymentInput.FeeInvoiceId is not Guid invoiceId || await _invoices.GetAsync(invoiceId) is not { } invoice || invoice.BranchId != PaymentInput.BranchId)
            ModelState.AddModelError("PaymentInput.FeeInvoiceId", "Selected invoice was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _payments.CreateAsync(new FeePayment
        {
            BranchId = PaymentInput.BranchId!.Value,
            FeeInvoiceId = PaymentInput.FeeInvoiceId!.Value,
            PaymentNumber = PaymentInput.PaymentNumber,
            Amount = PaymentInput.Amount,
            PaidOn = DateTime.UtcNow,
            Mode = PaymentInput.Mode,
            Status = PaymentInput.Status,
            TransactionReference = PaymentInput.TransactionReference
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteStructureAsync(Guid id) { await DeleteIfAllowedAsync(_structures, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteComponentAsync(Guid id) { await DeleteIfAllowedAsync(_components, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteDiscountAsync(Guid id) { await DeleteIfAllowedAsync(_discounts, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteScholarshipAsync(Guid id) { await DeleteIfAllowedAsync(_scholarships, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteAssignmentAsync(Guid id) { await DeleteIfAllowedAsync(_assignments, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteInvoiceAsync(Guid id) { await DeleteIfAllowedAsync(_invoices, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeletePaymentAsync(Guid id) { await DeleteIfAllowedAsync(_payments, id, x => x.BranchId); return RedirectToPage(); }

    private async Task LoadAsync()
    {
        await LoadBranchesAsync(_branches);
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
        Courses = (await _courses.ListAsync()).OrderBy(c => c.Name).ToList();
        Batches = (await _batches.ListAsync()).OrderBy(b => b.Name).ToList();
        Students = (await FilterBranchScopedAsync(_students, x => x.BranchId)).OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();
        Structures = (await FilterBranchScopedAsync(_structures, x => x.BranchId)).OrderBy(s => s.Name).ToList();
        Components = (await FilterBranchScopedAsync(_components, x => x.BranchId)).OrderBy(c => c.SortOrder).ThenBy(c => c.Name).ToList();
        Discounts = (await FilterBranchScopedAsync(_discounts, x => x.BranchId)).OrderBy(d => d.Name).ToList();
        Scholarships = (await FilterBranchScopedAsync(_scholarships, x => x.BranchId)).OrderByDescending(s => s.AwardedOn).ToList();
        Assignments = (await FilterBranchScopedAsync(_assignments, x => x.BranchId)).OrderByDescending(a => a.AssignedOn).ToList();
        Invoices = (await FilterBranchScopedAsync(_invoices, x => x.BranchId)).OrderByDescending(i => i.InvoiceDate).ToList();
        Payments = (await FilterBranchScopedAsync(_payments, x => x.BranchId)).OrderByDescending(p => p.PaidOn).ToList();

        if (await DefaultBranchIdAsync() is Guid branchId)
        {
            StructureInput.BranchId ??= branchId;
            ComponentInput.BranchId ??= branchId;
            DiscountInput.BranchId ??= branchId;
            ScholarshipInput.BranchId ??= branchId;
            AssignmentInput.BranchId ??= branchId;
            InvoiceInput.BranchId ??= branchId;
            PaymentInput.BranchId ??= branchId;
        }
    }

    private async Task<bool> ValidateStudentBranchAsync(string fieldName, Guid? studentId, Guid? branchId)
    {
        if (!await ValidateBranchSelectionAsync(fieldName.Replace("StudentProfileId", "BranchId"), branchId, _branches))
            return false;
        if (studentId is not Guid id || await _students.GetAsync(id) is not { } student || student.BranchId != branchId)
        {
            ModelState.AddModelError(fieldName, "Selected student was not found for this branch.");
            return false;
        }
        return true;
    }

    private async Task DeleteIfAllowedAsync<T>(ICrudService<T> service, Guid id, Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        if (entity is not null && await CanUseBranchAsync(branchSelector(entity)))
            await service.SoftDeleteAsync(id);
    }
}
