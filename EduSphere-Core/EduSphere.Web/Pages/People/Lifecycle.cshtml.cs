using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.People;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class LifecycleModel : PageModel
{
    private const long MaxDocumentBytes = 10 * 1024 * 1024;

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
    private readonly TenantDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

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
        IBranchAccessService branchAccess,
        TenantDbContext dbContext,
        IWebHostEnvironment environment)
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
        _dbContext = dbContext;
        _environment = environment;
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
    public IReadOnlyList<StudentLifecycleRequestDto> StudentRequests { get; private set; } = new List<StudentLifecycleRequestDto>();
    public IReadOnlyList<TeacherLifecycleRequestDto> TeacherRequests { get; private set; } = new List<TeacherLifecycleRequestDto>();
    public IReadOnlyList<StudentAlumniRecordDto> AlumniRecords { get; private set; } = new List<StudentAlumniRecordDto>();
    public IReadOnlyList<ProfileDocument> ProfileDocuments { get; private set; } = new List<ProfileDocument>();

    [BindProperty] public StudentLifecycleInput StudentInput { get; set; } = new();
    [BindProperty] public TeacherLifecycleInput TeacherInput { get; set; } = new();
    [BindProperty] public ProfileDocumentUploadInput DocumentInput { get; set; } = new();
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

    public string DocumentOwnerName(ProfileDocument document) => document.OwnerType switch
    {
        ProfileDocumentOwnerType.Student => StudentName(document.OwnerId),
        ProfileDocumentOwnerType.Teacher => TeacherName(document.OwnerId),
        _ => "-"
    };

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

        RemoveModelStateForPrefix(nameof(TeacherInput));
        RemoveModelStateForPrefix(nameof(DocumentInput));

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _lifecycle.CreateStudentRequestAsync(User, new CreateStudentLifecycleRequestRequest
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

        StatusMessage = result.Message ?? "Student lifecycle request submitted.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTeacherAsync()
    {
        if (!HasTenant)
            return RedirectToPage();

        RemoveModelStateForPrefix(nameof(StudentInput));
        RemoveModelStateForPrefix(nameof(DocumentInput));

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _lifecycle.CreateTeacherRequestAsync(User, new CreateTeacherLifecycleRequestRequest
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

        StatusMessage = result.Message ?? "Teacher lifecycle request submitted.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostApproveStudentRequestAsync(Guid id, string? decisionNotes)
    {
        var result = await _lifecycle.ApproveStudentRequestAsync(User, id, decisionNotes);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectStudentRequestAsync(Guid id, string? decisionNotes)
    {
        var result = await _lifecycle.RejectStudentRequestAsync(User, id, decisionNotes);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostApproveTeacherRequestAsync(Guid id, string? decisionNotes)
    {
        var result = await _lifecycle.ApproveTeacherRequestAsync(User, id, decisionNotes);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectTeacherRequestAsync(Guid id, string? decisionNotes)
    {
        var result = await _lifecycle.RejectTeacherRequestAsync(User, id, decisionNotes);
        StatusMessage = result.Succeeded ? result.Message : string.Join(" ", result.Errors);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUploadDocumentAsync()
    {
        if (!HasTenant)
            return RedirectToPage();

        ModelState.Clear();

        if (!DocumentInput.OwnerId.HasValue)
            ModelState.AddModelError("DocumentInput.OwnerId", "Select a student or teacher.");
        if (DocumentInput.File is null || DocumentInput.File.Length == 0)
            ModelState.AddModelError("DocumentInput.File", "Choose a document to upload.");
        else if (DocumentInput.File.Length > MaxDocumentBytes)
            ModelState.AddModelError("DocumentInput.File", "Document size must be 10 MB or smaller.");

        var ownerBranchId = DocumentInput.OwnerId.HasValue
            ? await ResolveOwnerBranchAsync(DocumentInput.OwnerType, DocumentInput.OwnerId.Value)
            : null;
        if (DocumentInput.OwnerId.HasValue && ownerBranchId is null)
            ModelState.AddModelError("DocumentInput.OwnerId", "Selected owner was not found.");
        else if (ownerBranchId.HasValue && !await _branchAccess.CanAccessBranchAsync(User, ownerBranchId.Value))
            ModelState.AddModelError("DocumentInput.OwnerId", "You cannot upload documents for this branch.");

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var file = DocumentInput.File!;
        var originalFileName = Path.GetFileName(file.FileName);
        var extension = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(
            "App_Data",
            "ProfileDocuments",
            _tenant.TenantId!.Value.ToString("N"),
            DocumentInput.OwnerType.ToString(),
            DocumentInput.OwnerId!.Value.ToString("N"),
            storedFileName);
        var absolutePath = Path.Combine(_environment.ContentRootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var stream = System.IO.File.Create(absolutePath))
        {
            await file.CopyToAsync(stream);
        }

        _dbContext.ProfileDocuments.Add(new ProfileDocument
        {
            TenantId = _tenant.TenantId.Value,
            OwnerType = DocumentInput.OwnerType,
            OwnerId = DocumentInput.OwnerId.Value,
            DocumentType = Normalize(DocumentInput.DocumentType) ?? "Lifecycle",
            DisplayName = Normalize(DocumentInput.DisplayName) ?? Path.GetFileNameWithoutExtension(originalFileName),
            FileName = originalFileName,
            ContentType = file.ContentType,
            StoragePath = relativePath,
            SizeBytes = file.Length,
            UploadedOn = DateTime.UtcNow,
            Notes = Normalize(DocumentInput.Notes)
        });
        await _dbContext.SaveChangesAsync();

        StatusMessage = "Profile document uploaded.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetDownloadDocumentAsync(Guid id)
    {
        var document = await _dbContext.ProfileDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (document is null || !await CanAccessDocumentAsync(document))
            return NotFound();

        var absolutePath = ResolveStoredPath(document.StoragePath);
        if (!System.IO.File.Exists(absolutePath))
            return NotFound();

        return PhysicalFile(
            absolutePath,
            document.ContentType ?? "application/octet-stream",
            document.FileName);
    }

    public IActionResult OnGetSampleDocument(ProfileDocumentOwnerType ownerType = ProfileDocumentOwnerType.Student)
    {
        var sample = ownerType == ProfileDocumentOwnerType.Teacher
            ? "Teacher lifecycle document sample\n\nDocument type: Relieving letter / Rehire approval / Department transfer order\nEmployee #: EMP-0001\nTeacher name: Full legal name\nEffective date: 2026-04-01\nApproved by: Principal / Tenant administrator\nNotes: Include reason, clearance summary, and next assignment when applicable.\n"
            : "Student lifecycle document sample\n\nDocument type: Transfer certificate / Promotion approval / Graduation certificate / Rejoining approval\nAdmission #: ADM-0001\nStudent name: Full legal name\nEffective date: 2026-04-01\nApproved by: Principal / Tenant administrator\nNotes: Include reason, destination class/branch, and supporting references when applicable.\n";

        return File(
            Encoding.UTF8.GetBytes(sample),
            "text/plain",
            $"{ownerType.ToString().ToLowerInvariant()}-lifecycle-document-sample.txt");
    }

    public async Task<IActionResult> OnGetExportAsync(string kind = "requests")
    {
        if (!HasTenant)
            return RedirectToPage();

        await LoadAsync();
        var csv = new StringBuilder();

        if (string.Equals(kind, "alumni", StringComparison.OrdinalIgnoreCase))
        {
            csv.AppendLine("AlumniNumber,Student,Branch,AcademicYear,Course,Batch,GraduationDate,Email,Phone,Notes");
            foreach (var item in AlumniRecords)
            {
                csv.AppendCsv(item.AlumniNumber);
                csv.AppendCsv(StudentName(item.StudentProfileId));
                csv.AppendCsv(BranchName(item.BranchId));
                csv.AppendCsv(AcademicYearName(item.AcademicYearId));
                csv.AppendCsv(CourseName(item.CourseId));
                csv.AppendCsv(BatchName(item.BatchId));
                csv.AppendCsv(item.GraduationDate.ToString("yyyy-MM-dd"));
                csv.AppendCsv(item.ContactEmail);
                csv.AppendCsv(item.ContactPhone);
                csv.AppendCsv(item.Notes, true);
            }
        }
        else
        {
            csv.AppendLine("Type,Person,Action,ApprovalStatus,TargetStatus,Branch,Target,EffectiveOn,RequestedOn,DecidedOn,Reason,DecisionNotes");
            foreach (var item in StudentRequests)
            {
                csv.AppendCsv("Student");
                csv.AppendCsv(StudentName(item.StudentProfileId));
                csv.AppendCsv(item.EventType.ToString());
                csv.AppendCsv(item.Status.ToString());
                csv.AppendCsv(item.ToStatus.ToString());
                csv.AppendCsv(BranchName(item.BranchId));
                csv.AppendCsv($"{BranchName(item.ToBranchId)} / {SectionName(item.ToSectionId)}");
                csv.AppendCsv(item.EffectiveOn.ToString("yyyy-MM-dd"));
                csv.AppendCsv(item.RequestedOn.ToString("u"));
                csv.AppendCsv(item.DecidedOn?.ToString("u"));
                csv.AppendCsv(item.Reason);
                csv.AppendCsv(item.DecisionNotes, true);
            }

            foreach (var item in TeacherRequests)
            {
                csv.AppendCsv("Teacher");
                csv.AppendCsv(TeacherName(item.TeacherProfileId));
                csv.AppendCsv(item.EventType.ToString());
                csv.AppendCsv(item.Status.ToString());
                csv.AppendCsv(item.ToStatus.ToString());
                csv.AppendCsv(BranchName(item.BranchId));
                csv.AppendCsv($"{BranchName(item.ToBranchId)} / {DepartmentName(item.ToDepartmentId)}");
                csv.AppendCsv(item.EffectiveOn.ToString("yyyy-MM-dd"));
                csv.AppendCsv(item.RequestedOn.ToString("u"));
                csv.AppendCsv(item.DecidedOn?.ToString("u"));
                csv.AppendCsv(item.Reason);
                csv.AppendCsv(item.DecisionNotes, true);
            }
        }

        var fileName = string.Equals(kind, "alumni", StringComparison.OrdinalIgnoreCase)
            ? "alumni-report.csv"
            : "lifecycle-requests-report.csv";
        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
    }

    public async Task<IActionResult> OnGetPrintStudentRequestAsync(Guid id)
    {
        await LoadAsync();
        var request = StudentRequests.FirstOrDefault(r => r.Id == id);
        if (request is null)
            return NotFound();

        var html = PrintableDocument(
            "Student Lifecycle Approval",
            StudentName(request.StudentProfileId),
            request.EventType.ToString(),
            request.Status.ToString(),
            request.EffectiveOn,
            BranchName(request.BranchId),
            $"{BranchName(request.ToBranchId)} / {SectionName(request.ToSectionId)}",
            request.Reason,
            request.DecisionNotes);
        return Content(html, "text/html", Encoding.UTF8);
    }

    public async Task<IActionResult> OnGetPrintTeacherRequestAsync(Guid id)
    {
        await LoadAsync();
        var request = TeacherRequests.FirstOrDefault(r => r.Id == id);
        if (request is null)
            return NotFound();

        var html = PrintableDocument(
            "Teacher Lifecycle Approval",
            TeacherName(request.TeacherProfileId),
            request.EventType.ToString(),
            request.Status.ToString(),
            request.EffectiveOn,
            BranchName(request.BranchId),
            $"{BranchName(request.ToBranchId)} / {DepartmentName(request.ToDepartmentId)}",
            request.Reason,
            request.DecisionNotes);
        return Content(html, "text/html", Encoding.UTF8);
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
        StudentRequests = await _lifecycle.ListStudentRequestsAsync(User, take: 50);
        TeacherRequests = await _lifecycle.ListTeacherRequestsAsync(User, take: 50);
        AlumniRecords = await _lifecycle.ListAlumniRecordsAsync(User, take: 100);
        ProfileDocuments = await LoadDocumentsAsync();

        if (_branchAccess.IsBranchAdminOnly(User) && assignedBranchId.HasValue)
        {
            StudentInput.ToBranchId ??= assignedBranchId.Value;
            TeacherInput.ToBranchId ??= assignedBranchId.Value;
        }
    }

    private async Task<IReadOnlyList<ProfileDocument>> LoadDocumentsAsync()
    {
        var studentIds = Students.Select(s => s.Id).ToList();
        var teacherIds = Teachers.Select(t => t.Id).ToList();

        return await _dbContext.ProfileDocuments
            .AsNoTracking()
            .Where(d =>
                (d.OwnerType == ProfileDocumentOwnerType.Student && studentIds.Contains(d.OwnerId)) ||
                (d.OwnerType == ProfileDocumentOwnerType.Teacher && teacherIds.Contains(d.OwnerId)))
            .OrderByDescending(d => d.UploadedOn)
            .Take(100)
            .ToListAsync();
    }

    private async Task<Guid?> ResolveOwnerBranchAsync(ProfileDocumentOwnerType ownerType, Guid ownerId)
    {
        return ownerType switch
        {
            ProfileDocumentOwnerType.Student => await _dbContext.StudentProfiles
                .AsNoTracking()
                .Where(s => s.Id == ownerId)
                .Select(s => (Guid?)s.BranchId)
                .FirstOrDefaultAsync(),
            ProfileDocumentOwnerType.Teacher => await _dbContext.TeacherProfiles
                .AsNoTracking()
                .Where(t => t.Id == ownerId)
                .Select(t => (Guid?)t.BranchId)
                .FirstOrDefaultAsync(),
            _ => null
        };
    }

    private async Task<bool> CanAccessDocumentAsync(ProfileDocument document)
    {
        var ownerBranchId = await ResolveOwnerBranchAsync(document.OwnerType, document.OwnerId);
        return ownerBranchId.HasValue && await _branchAccess.CanAccessBranchAsync(User, ownerBranchId.Value);
    }

    private string ResolveStoredPath(string storagePath)
        => Path.IsPathRooted(storagePath)
            ? storagePath
            : Path.Combine(_environment.ContentRootPath, storagePath);

    private static string PrintableDocument(
        string title,
        string person,
        string action,
        string status,
        DateOnly effectiveOn,
        string branch,
        string target,
        string? reason,
        string? decisionNotes)
    {
        static string H(string? value) => WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "-" : value);

        return $$$"""
<!doctype html>
<html>
<head>
<meta charset="utf-8" />
<title>{{{H(title)}}}</title>
<style>
body{font-family:Segoe UI,Arial,sans-serif;margin:40px;color:#172033}
.document{border:1px solid #d4dae8;padding:32px;max-width:800px;margin:auto}
h1{font-size:24px;margin:0 0 8px}
.meta{color:#697386;margin-bottom:28px}
dl{display:grid;grid-template-columns:190px 1fr;gap:10px 18px}
dt{font-weight:700;color:#384152}
dd{margin:0}
.signature{margin-top:54px;display:flex;justify-content:space-between}
.line{border-top:1px solid #172033;padding-top:8px;width:240px;text-align:center}
@media print{button{display:none}body{margin:0}.document{border:0}}
</style>
</head>
<body>
<div class="document">
<button onclick="window.print()">Print</button>
<h1>{{{H(title)}}}</h1>
<div class="meta">Generated on {{{DateTime.Now:dd MMM yyyy HH:mm}}}</div>
<dl>
<dt>Person</dt><dd>{{{H(person)}}}</dd>
<dt>Action</dt><dd>{{{H(action)}}}</dd>
<dt>Approval status</dt><dd>{{{H(status)}}}</dd>
<dt>Effective date</dt><dd>{{{effectiveOn:dd MMM yyyy}}}</dd>
<dt>Current branch</dt><dd>{{{H(branch)}}}</dd>
<dt>Target placement</dt><dd>{{{H(target)}}}</dd>
<dt>Reason</dt><dd>{{{H(reason)}}}</dd>
<dt>Decision notes</dt><dd>{{{H(decisionNotes)}}}</dd>
</dl>
<div class="signature">
<div class="line">Prepared by</div>
<div class="line">Approved by</div>
</div>
</div>
</body>
</html>
""";
    }

    private void RemoveModelStateForPrefix(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(k =>
            k.Equals(prefix, StringComparison.Ordinal) ||
            k.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList())
        {
            ModelState.Remove(key);
        }
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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

    public class ProfileDocumentUploadInput
    {
        [Display(Name = "Owner type")]
        public ProfileDocumentOwnerType OwnerType { get; set; } = ProfileDocumentOwnerType.Student;

        [Display(Name = "Owner")]
        public Guid? OwnerId { get; set; }

        [StringLength(100), Display(Name = "Document type")]
        public string? DocumentType { get; set; }

        [StringLength(200), Display(Name = "Display name")]
        public string? DisplayName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public IFormFile? File { get; set; }
    }
}

internal static class LifecycleCsvExtensions
{
    public static void AppendCsv(this StringBuilder builder, string? value, bool endOfLine = false)
    {
        var text = value ?? string.Empty;
        builder.Append('"');
        builder.Append(text.Replace("\"", "\"\""));
        builder.Append('"');
        builder.Append(endOfLine ? Environment.NewLine : ',');
    }
}
