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

[Authorize(Policy = AuthorizationPolicies.LibraryManager)]
public class LibraryModel : EnterprisePageModel
{
    private readonly ICrudService<LibraryBook> _books;
    private readonly ICrudService<LibraryBookCopy> _copies;
    private readonly ICrudService<LibraryMember> _members;
    private readonly ICrudService<LibraryBookIssue> _issues;
    private readonly ICrudService<LibraryBookReturn> _returns;
    private readonly ICrudService<LibraryFineRecord> _fines;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ICrudService<Subject> _subjects;

    public LibraryModel(
        ICrudService<LibraryBook> books,
        ICrudService<LibraryBookCopy> copies,
        ICrudService<LibraryMember> members,
        ICrudService<LibraryBookIssue> issues,
        ICrudService<LibraryBookReturn> returns,
        ICrudService<LibraryFineRecord> fines,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        ICrudService<TeacherProfile> teachers,
        ICrudService<Subject> subjects,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
        : base(tenant, branchAccess)
    {
        _books = books;
        _copies = copies;
        _members = members;
        _issues = issues;
        _returns = returns;
        _fines = fines;
        _branches = branches;
        _students = students;
        _teachers = teachers;
        _subjects = subjects;
    }

    public IReadOnlyList<LibraryBook> Books { get; private set; } = new List<LibraryBook>();
    public IReadOnlyList<LibraryBookCopy> Copies { get; private set; } = new List<LibraryBookCopy>();
    public IReadOnlyList<LibraryMember> Members { get; private set; } = new List<LibraryMember>();
    public IReadOnlyList<LibraryBookIssue> Issues { get; private set; } = new List<LibraryBookIssue>();
    public IReadOnlyList<LibraryBookReturn> Returns { get; private set; } = new List<LibraryBookReturn>();
    public IReadOnlyList<LibraryFineRecord> Fines { get; private set; } = new List<LibraryFineRecord>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = new List<TeacherProfile>();
    public IReadOnlyList<Subject> Subjects { get; private set; } = new List<Subject>();

    [BindProperty] public BookInputModel BookInput { get; set; } = new();
    [BindProperty] public CopyInputModel CopyInput { get; set; } = new();
    [BindProperty] public MemberInputModel MemberInput { get; set; } = new();
    [BindProperty] public IssueInputModel IssueInput { get; set; } = new();
    [BindProperty] public ReturnInputModel ReturnInput { get; set; } = new();
    [BindProperty] public FineInputModel FineInput { get; set; } = new();

    public class BookInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Subject")] public Guid? SubjectId { get; set; }
        [StringLength(30), Display(Name = "ISBN")] public string? Isbn { get; set; }
        [Required, StringLength(250)] public string Title { get; set; } = string.Empty;
        [Required, StringLength(250)] public string Authors { get; set; } = string.Empty;
        [StringLength(150)] public string? Publisher { get; set; }
        [StringLength(120)] public string? Category { get; set; }
    }

    public class CopyInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Book")] public Guid? LibraryBookId { get; set; }
        [Required, StringLength(60), Display(Name = "Accession #")] public string AccessionNumber { get; set; } = string.Empty;
        [StringLength(80)] public string? Barcode { get; set; }
        [StringLength(120), Display(Name = "Shelf")] public string? ShelfLocation { get; set; }
        public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
        [Range(0, 100000)] public decimal? Price { get; set; }
    }

    public class MemberInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Display(Name = "Teacher")] public Guid? TeacherProfileId { get; set; }
        [Required, StringLength(60), Display(Name = "Member #")] public string MemberNumber { get; set; } = string.Empty;
        [Display(Name = "Type")] public LibraryMemberType MemberType { get; set; } = LibraryMemberType.Student;
        [Range(1, 20), Display(Name = "Book limit")] public int MaxBooksAllowed { get; set; } = 2;
    }

    public class IssueInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Copy")] public Guid? LibraryBookCopyId { get; set; }
        [Required, Display(Name = "Member")] public Guid? LibraryMemberId { get; set; }
        [Display(Name = "Issue date")] public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Due date")] public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
        public LibraryIssueStatus Status { get; set; } = LibraryIssueStatus.Issued;
    }

    public class ReturnInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Issue")] public Guid? LibraryBookIssueId { get; set; }
        [Range(0, 100000), Display(Name = "Fine assessed")] public decimal FineAssessed { get; set; }
        [Range(0, 100000), Display(Name = "Fine paid")] public decimal FinePaid { get; set; }
        [StringLength(500), Display(Name = "Condition notes")] public string? ConditionNotes { get; set; }
    }

    public class FineInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Member")] public Guid? LibraryMemberId { get; set; }
        [Display(Name = "Issue")] public Guid? LibraryBookIssueId { get; set; }
        [Range(0, 100000)] public decimal Amount { get; set; }
        [Required, StringLength(250)] public string Reason { get; set; } = string.Empty;
        public LibraryFineStatus Status { get; set; } = LibraryFineStatus.Pending;
    }

    public string BookTitle(Guid id) => Books.FirstOrDefault(b => b.Id == id)?.Title ?? "-";
    public string CopyName(Guid id) => Copies.FirstOrDefault(c => c.Id == id)?.AccessionNumber ?? "-";
    public string MemberName(Guid id)
    {
        var member = Members.FirstOrDefault(m => m.Id == id);
        return member is null ? "-" : $"{member.MemberNumber} / {member.MemberType}";
    }

    public string StudentName(Guid? id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }

    public string TeacherName(Guid? id)
    {
        var teacher = Teachers.FirstOrDefault(t => t.Id == id);
        return teacher is null ? "-" : $"{teacher.FirstName} {teacher.LastName}";
    }

    public async Task OnGetAsync()
    {
        if (!HasTenant) return;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveBookAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(BookInput));
        if (!await ValidateBranchSelectionAsync("BookInput.BranchId", BookInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _books.CreateAsync(new LibraryBook
        {
            BranchId = BookInput.BranchId!.Value,
            SubjectId = BookInput.SubjectId,
            Isbn = BookInput.Isbn,
            Title = BookInput.Title,
            Authors = BookInput.Authors,
            Publisher = BookInput.Publisher,
            Category = BookInput.Category
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveCopyAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(CopyInput));
        if (!await ValidateBranchSelectionAsync("CopyInput.BranchId", CopyInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (CopyInput.LibraryBookId is not Guid bookId || await _books.GetAsync(bookId) is not { } book || book.BranchId != CopyInput.BranchId)
            ModelState.AddModelError("CopyInput.LibraryBookId", "Selected book was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _copies.CreateAsync(new LibraryBookCopy
        {
            BranchId = CopyInput.BranchId!.Value,
            LibraryBookId = CopyInput.LibraryBookId!.Value,
            AccessionNumber = CopyInput.AccessionNumber,
            Barcode = CopyInput.Barcode,
            ShelfLocation = CopyInput.ShelfLocation,
            Status = CopyInput.Status,
            AcquisitionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Price = CopyInput.Price
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveMemberAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(MemberInput));
        if (!await ValidateBranchSelectionAsync("MemberInput.BranchId", MemberInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (MemberInput.StudentProfileId is Guid studentId && (await _students.GetAsync(studentId) is not { } student || student.BranchId != MemberInput.BranchId))
            ModelState.AddModelError("MemberInput.StudentProfileId", "Selected student was not found for this branch.");
        if (MemberInput.TeacherProfileId is Guid teacherId && (await _teachers.GetAsync(teacherId) is not { } teacher || teacher.BranchId != MemberInput.BranchId))
            ModelState.AddModelError("MemberInput.TeacherProfileId", "Selected teacher was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _members.CreateAsync(new LibraryMember
        {
            BranchId = MemberInput.BranchId!.Value,
            StudentProfileId = MemberInput.StudentProfileId,
            TeacherProfileId = MemberInput.TeacherProfileId,
            MemberNumber = MemberInput.MemberNumber,
            MemberType = MemberInput.MemberType,
            JoinedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            MaxBooksAllowed = MemberInput.MaxBooksAllowed,
            IsActive = true
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveIssueAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(IssueInput));
        if (!await ValidateBranchSelectionAsync("IssueInput.BranchId", IssueInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (IssueInput.LibraryBookCopyId is not Guid copyId || await _copies.GetAsync(copyId) is not { } copy || copy.BranchId != IssueInput.BranchId)
            ModelState.AddModelError("IssueInput.LibraryBookCopyId", "Selected copy was not found for this branch.");
        if (IssueInput.LibraryMemberId is not Guid memberId || await _members.GetAsync(memberId) is not { } member || member.BranchId != IssueInput.BranchId)
            ModelState.AddModelError("IssueInput.LibraryMemberId", "Selected member was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _issues.CreateAsync(new LibraryBookIssue
        {
            BranchId = IssueInput.BranchId!.Value,
            LibraryBookCopyId = IssueInput.LibraryBookCopyId!.Value,
            LibraryMemberId = IssueInput.LibraryMemberId!.Value,
            IssueDate = IssueInput.IssueDate,
            DueDate = IssueInput.DueDate,
            Status = IssueInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveReturnAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ReturnInput));
        if (!await ValidateBranchSelectionAsync("ReturnInput.BranchId", ReturnInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (ReturnInput.LibraryBookIssueId is not Guid issueId || await _issues.GetAsync(issueId) is not { } issue || issue.BranchId != ReturnInput.BranchId)
            ModelState.AddModelError("ReturnInput.LibraryBookIssueId", "Selected issue was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _returns.CreateAsync(new LibraryBookReturn
        {
            BranchId = ReturnInput.BranchId!.Value,
            LibraryBookIssueId = ReturnInput.LibraryBookIssueId!.Value,
            ReturnedOn = DateTime.UtcNow,
            FineAssessed = ReturnInput.FineAssessed,
            FinePaid = ReturnInput.FinePaid,
            ConditionNotes = ReturnInput.ConditionNotes
        });
        await _issues.UpdateAsync(ReturnInput.LibraryBookIssueId!.Value, issue =>
        {
            issue.ReturnedOn = DateOnly.FromDateTime(DateTime.UtcNow);
            issue.Status = LibraryIssueStatus.Returned;
            issue.FineAmount = ReturnInput.FineAssessed;
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveFineAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(FineInput));
        if (!await ValidateBranchSelectionAsync("FineInput.BranchId", FineInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (FineInput.LibraryMemberId is not Guid memberId || await _members.GetAsync(memberId) is not { } member || member.BranchId != FineInput.BranchId)
            ModelState.AddModelError("FineInput.LibraryMemberId", "Selected member was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _fines.CreateAsync(new LibraryFineRecord
        {
            BranchId = FineInput.BranchId!.Value,
            LibraryMemberId = FineInput.LibraryMemberId!.Value,
            LibraryBookIssueId = FineInput.LibraryBookIssueId,
            Amount = FineInput.Amount,
            Reason = FineInput.Reason,
            Status = FineInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteBookAsync(Guid id) { await DeleteIfAllowedAsync(_books, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteCopyAsync(Guid id) { await DeleteIfAllowedAsync(_copies, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteMemberAsync(Guid id) { await DeleteIfAllowedAsync(_members, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteIssueAsync(Guid id) { await DeleteIfAllowedAsync(_issues, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteFineAsync(Guid id) { await DeleteIfAllowedAsync(_fines, id, x => x.BranchId); return RedirectToPage(); }

    private async Task LoadAsync()
    {
        await LoadBranchesAsync(_branches);
        Books = (await FilterBranchScopedAsync(_books, x => x.BranchId)).OrderBy(b => b.Title).ToList();
        Copies = (await FilterBranchScopedAsync(_copies, x => x.BranchId)).OrderBy(c => c.AccessionNumber).ToList();
        Members = (await FilterBranchScopedAsync(_members, x => x.BranchId)).OrderBy(m => m.MemberNumber).ToList();
        Issues = (await FilterBranchScopedAsync(_issues, x => x.BranchId)).OrderByDescending(i => i.IssueDate).ToList();
        Returns = (await FilterBranchScopedAsync(_returns, x => x.BranchId)).OrderByDescending(r => r.ReturnedOn).ToList();
        Fines = (await FilterBranchScopedAsync(_fines, x => x.BranchId)).OrderByDescending(f => f.AssessedOn).ToList();
        Students = (await FilterBranchScopedAsync(_students, x => x.BranchId)).OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();
        Teachers = (await FilterBranchScopedAsync(_teachers, x => x.BranchId)).OrderBy(t => t.FirstName).ThenBy(t => t.LastName).ToList();
        Subjects = (await _subjects.ListAsync()).OrderBy(s => s.Name).ToList();

        if (await DefaultBranchIdAsync() is Guid branchId)
        {
            BookInput.BranchId ??= branchId;
            CopyInput.BranchId ??= branchId;
            MemberInput.BranchId ??= branchId;
            IssueInput.BranchId ??= branchId;
            ReturnInput.BranchId ??= branchId;
            FineInput.BranchId ??= branchId;
        }
    }

    private async Task DeleteIfAllowedAsync<T>(ICrudService<T> service, Guid id, Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        if (entity is not null && await CanUseBranchAsync(branchSelector(entity)))
            await service.SoftDeleteAsync(id);
    }
}
