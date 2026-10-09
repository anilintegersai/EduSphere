using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Cryptography;
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
public class LifecycleDepthModel : PageModel
{
    private readonly TenantDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;
    private readonly IStudentTeacherLifecycleService _lifecycle;

    public LifecycleDepthModel(TenantDbContext db, ITenantContext tenant, IBranchAccessService branchAccess, IStudentTeacherLifecycleService lifecycle)
    {
        _db = db; _tenant = tenant; _branchAccess = branchAccess; _lifecycle = lifecycle;
    }

    public bool HasTenant => _tenant.HasTenant;
    public List<StudentProfile> Students { get; private set; } = [];
    public List<TeacherProfile> Teachers { get; private set; } = [];
    public List<AcademicYear> AcademicYears { get; private set; } = [];
    public List<Section> Sections { get; private set; } = [];
    public List<StudentMedicalProfile> MedicalProfiles { get; private set; } = [];
    public List<ProfileStatusHistory> StatusHistory { get; private set; } = [];
    public List<LifecycleDocumentTemplate> Templates { get; private set; } = [];
    public List<StudentTranscript> Transcripts { get; private set; } = [];
    public List<StudentAlumniRecord> Alumni { get; private set; } = [];
    public List<AlumniEngagement> Engagements { get; private set; } = [];
    public List<BulkStudentLifecycleRun> BulkRuns { get; private set; } = [];
    public List<ProfileDocumentPolicy> Policies { get; private set; } = [];
    public List<ProfileDocument> Documents { get; private set; } = [];

    [BindProperty] public MedicalInput Medical { get; set; } = new();
    [BindProperty] public TemplateInput Template { get; set; } = new();
    [BindProperty] public PolicyInput Policy { get; set; } = new();
    [BindProperty] public EngagementInput Engagement { get; set; } = new();
    [BindProperty] public BulkInput Bulk { get; set; } = new();
    [BindProperty] public SignatureInput Signature { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }

    public async Task OnGetAsync() { if (HasTenant) await LoadAsync(); }

    public async Task<IActionResult> OnPostMedicalAsync()
    {
        if (!HasTenant) return RedirectToPage();
        var student = await _db.StudentProfiles.FirstOrDefaultAsync(x => x.Id == Medical.StudentProfileId);
        if (student is null || !await _branchAccess.CanAccessBranchAsync(User, student.BranchId)) return NotFound();
        var profile = await _db.StudentMedicalProfiles.Include(x => x.Conditions).FirstOrDefaultAsync(x => x.StudentProfileId == student.Id);
        if (profile is null)
        {
            profile = new StudentMedicalProfile { TenantId = student.TenantId, StudentProfileId = student.Id };
            _db.StudentMedicalProfiles.Add(profile);
        }
        profile.BloodGroup = Clean(Medical.BloodGroup); profile.Allergies = Clean(Medical.Allergies);
        profile.CurrentMedications = Clean(Medical.CurrentMedications); profile.EmergencyInstructions = Clean(Medical.EmergencyInstructions);
        profile.PhysicianName = Clean(Medical.PhysicianName); profile.PhysicianPhone = Clean(Medical.PhysicianPhone);
        profile.InsuranceProvider = Clean(Medical.InsuranceProvider); profile.InsurancePolicyNumber = Clean(Medical.InsurancePolicyNumber);
        profile.EmergencyCareConsent = Medical.EmergencyCareConsent; profile.LastReviewedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        if (!string.IsNullOrWhiteSpace(Medical.ConditionName))
            profile.Conditions.Add(new StudentMedicalCondition { TenantId = student.TenantId, Name = Medical.ConditionName.Trim(), Severity = Clean(Medical.ConditionSeverity), DiagnosedOn = Medical.DiagnosedOn, CareInstructions = Clean(Medical.CareInstructions) });
        await _db.SaveChangesAsync(); StatusMessage = "Medical profile and condition history saved."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTemplateAsync()
    {
        if (!HasTenant || !ModelState.IsValid) { await LoadAsync(); return Page(); }
        var version = (await _db.LifecycleDocumentTemplates.Where(x => x.Code == Template.Code.Trim()).MaxAsync(x => (int?)x.Version) ?? 0) + 1;
        _db.LifecycleDocumentTemplates.Add(new LifecycleDocumentTemplate { TenantId = _tenant.TenantId!.Value, Code = Template.Code.Trim().ToUpperInvariant(), Name = Template.Name.Trim(), OwnerType = Template.OwnerType, DocumentType = Template.DocumentType.Trim(), HtmlTemplate = Template.HtmlTemplate, Version = version, RequiresSignature = Template.RequiresSignature, IsActive = true });
        await _db.SaveChangesAsync(); StatusMessage = $"Template version {version} created."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPolicyAsync()
    {
        if (!HasTenant) return RedirectToPage();
        var type = Policy.DocumentType.Trim();
        var entity = await _db.ProfileDocumentPolicies.FirstOrDefaultAsync(x => x.OwnerType == Policy.OwnerType && x.DocumentType == type);
        entity ??= new ProfileDocumentPolicy { TenantId = _tenant.TenantId!.Value, OwnerType = Policy.OwnerType, DocumentType = type };
        if (entity.Id == Guid.Empty) _db.ProfileDocumentPolicies.Add(entity);
        entity.AllowedExtensions = Policy.AllowedExtensions; entity.MaximumBytes = Math.Clamp(Policy.MaximumMegabytes, 1, 100) * 1024L * 1024L;
        entity.RequiresVirusScan = Policy.RequiresVirusScan; entity.RequiresVerification = Policy.RequiresVerification;
        entity.RetentionDays = Policy.RetentionDays; entity.ExpiryDays = Policy.ExpiryDays; entity.IsActive = true;
        await _db.SaveChangesAsync(); StatusMessage = "Document policy saved."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostVerifyDocumentAsync(Guid id, bool approve)
    {
        var document = await _db.ProfileDocuments.FirstOrDefaultAsync(x => x.Id == id);
        if (document is null) return NotFound();
        document.VerificationStatus = approve ? DocumentVerificationStatus.Verified : DocumentVerificationStatus.Rejected;
        document.IsVerified = approve; document.VerifiedBy = User.Identity?.Name; document.VerifiedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync(); StatusMessage = approve ? "Document verified." : "Document rejected."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEngagementAsync()
    {
        var alumni = await _db.StudentAlumniRecords.FirstOrDefaultAsync(x => x.Id == Engagement.AlumniRecordId);
        if (alumni is null || !await _branchAccess.CanAccessBranchAsync(User, alumni.BranchId)) return NotFound();
        _db.AlumniEngagements.Add(new AlumniEngagement { TenantId = alumni.TenantId, StudentAlumniRecordId = alumni.Id, Type = Engagement.Type, Title = Engagement.Title.Trim(), OccurredOn = Engagement.OccurredOn, Notes = Clean(Engagement.Notes), ContributionAmount = Engagement.ContributionAmount });
        await _db.SaveChangesAsync(); StatusMessage = "Alumni engagement recorded."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBulkAsync()
    {
        var target = await _db.Sections.Include(x => x.Batch).FirstOrDefaultAsync(x => x.Id == Bulk.ToSectionId);
        if (target?.Batch is null || !HasTenant) return NotFound();
        var students = await _db.Enrollments.AsNoTracking()
            .Where(x => x.AcademicYearId == Bulk.FromAcademicYearId && (!Bulk.FromSectionId.HasValue || x.SectionId == Bulk.FromSectionId) && x.Status == EnrollmentStatus.Active)
            .Select(x => x.StudentProfile!).Distinct().ToListAsync();
        if (students.Count == 0)
        {
            StatusMessage = "No active students matched the selected source year and section.";
            return RedirectToPage();
        }
        var run = new BulkStudentLifecycleRun { TenantId = _tenant.TenantId!.Value, BranchId = students.FirstOrDefault()?.BranchId ?? Guid.Empty, FromAcademicYearId = Bulk.FromAcademicYearId, ToAcademicYearId = target.Batch.AcademicYearId, FromSectionId = Bulk.FromSectionId, ToSectionId = target.Id, Status = BulkLifecycleRunStatus.Processing, TotalCount = students.Count, StartedOn = DateTime.UtcNow, RunBy = User.Identity?.Name };
        _db.BulkStudentLifecycleRuns.Add(run); await _db.SaveChangesAsync();
        foreach (var student in students)
        {
            var result = await _lifecycle.RecordStudentEventAsync(User, new CreateStudentLifecycleEventRequest { StudentProfileId = student.Id, EventType = StudentLifecycleEventType.Promoted, ToStatus = StudentStatus.Active, ToBranchId = student.BranchId, ToAcademicYearId = target.Batch.AcademicYearId, ToCourseId = target.Batch.CourseId, ToBatchId = target.BatchId, ToSectionId = target.Id, EffectiveOn = Bulk.EffectiveOn, Reason = "Academic-session bulk promotion" });
            run.Rows.Add(new BulkStudentLifecycleRow { TenantId = run.TenantId, StudentProfileId = student.Id, Succeeded = result.Succeeded, Message = result.Succeeded ? result.Message : string.Join(" ", result.Errors) });
            if (result.Succeeded) run.SucceededCount++; else run.FailedCount++;
        }
        run.Status = run.FailedCount == 0 ? BulkLifecycleRunStatus.Completed : BulkLifecycleRunStatus.CompletedWithErrors; run.CompletedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync(); StatusMessage = $"Bulk rollover completed: {run.SucceededCount} succeeded, {run.FailedCount} failed."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSignAsync()
    {
        var template = await _db.LifecycleDocumentTemplates.FirstOrDefaultAsync(x => x.Id == Signature.TemplateId);
        if (template is null) return NotFound();
        var signedOn = DateTime.UtcNow;
        var payload = $"{template.Id}|{template.Version}|{Signature.SubjectId}|{Signature.SignerName}|{Signature.SignerTitle}|{signedOn:O}";
        _db.DocumentApprovalSignatures.Add(new DocumentApprovalSignature { TenantId = template.TenantId, LifecycleDocumentTemplateId = template.Id, SubjectId = Signature.SubjectId, SignerName = Signature.SignerName.Trim(), SignerTitle = Clean(Signature.SignerTitle), SignedOn = signedOn, SignatureHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload))), Status = SignatureStatus.Signed });
        await _db.SaveChangesAsync(); StatusMessage = "Formal approval signature recorded."; return RedirectToPage();
    }

    public async Task<IActionResult> OnGetPrintTemplateAsync(Guid templateId, Guid subjectId)
    {
        var template = await _db.LifecycleDocumentTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == templateId && x.IsActive);
        if (template is null) return NotFound();
        string name, number, branch;
        if (template.OwnerType == ProfileStatusOwnerType.Student)
        {
            var person = await _db.StudentProfiles.Include(x => x.Branch).AsNoTracking().FirstOrDefaultAsync(x => x.Id == subjectId);
            if (person is null) return NotFound(); name = $"{person.FirstName} {person.LastName}"; number = person.AdmissionNumber; branch = person.Branch?.Name ?? "-";
        }
        else
        {
            var person = await _db.TeacherProfiles.Include(x => x.Branch).AsNoTracking().FirstOrDefaultAsync(x => x.Id == subjectId);
            if (person is null) return NotFound(); name = $"{person.FirstName} {person.LastName}"; number = person.EmployeeNumber; branch = person.Branch?.Name ?? "-";
        }
        var signature = await _db.DocumentApprovalSignatures.AsNoTracking().Where(x => x.LifecycleDocumentTemplateId == templateId && x.SubjectId == subjectId && x.Status == SignatureStatus.Signed).OrderByDescending(x => x.SignedOn).FirstOrDefaultAsync();
        var html = template.HtmlTemplate.Replace("{{Name}}", H(name)).Replace("{{Number}}", H(number)).Replace("{{Branch}}", H(branch)).Replace("{{Date}}", DateTime.Today.ToString("dd MMM yyyy"));
        var signatureHtml = signature is null ? "<p><strong>Unsigned document</strong></p>" : $"<p><strong>Digitally approved by:</strong> {H(signature.SignerName)}, {H(signature.SignerTitle)}<br><small>{signature.SignedOn:u} | Verification: {H(signature.SignatureHash[..16])}</small></p>";
        return Content($"<!doctype html><html><head><meta charset='utf-8'><style>body{{font-family:Segoe UI;margin:40px;color:#182233}}main{{max-width:850px;margin:auto}}@media print{{button{{display:none}}}}</style></head><body><main><button onclick='print()'>Print</button>{html}<hr>{signatureHtml}</main></body></html>", "text/html", Encoding.UTF8);
    }

    public async Task<IActionResult> OnGetTranscriptAsync(Guid id)
    {
        var transcript = await _db.StudentTranscripts.Include(x => x.Lines).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (transcript is null) return NotFound();
        var student = await _db.StudentProfiles.AsNoTracking().FirstAsync(x => x.Id == transcript.StudentProfileId);
        var rows = string.Join("", transcript.Lines.Select(x => $"<tr><td>{H(x.AssessmentName)}</td><td>{x.MaximumMarks:0.##}</td><td>{x.MarksObtained:0.##}</td><td>{x.Percentage:0.##}%</td><td>{H(x.Grade)}</td></tr>"));
        return Content($"<!doctype html><html><head><style>body{{font-family:Segoe UI;margin:40px}}table{{width:100%;border-collapse:collapse}}th,td{{border:1px solid #ccd3df;padding:8px}}@media print{{button{{display:none}}}}</style></head><body><button onclick='print()'>Print</button><h1>Official Transcript</h1><p>{H(student.FirstName + " " + student.LastName)} | {H(student.AdmissionNumber)} | {H(transcript.TranscriptNumber)}</p><table><thead><tr><th>Assessment</th><th>Maximum</th><th>Obtained</th><th>Percentage</th><th>Grade</th></tr></thead><tbody>{rows}</tbody></table><p><strong>Aggregate:</strong> {transcript.AggregatePercentage:0.##}% &nbsp; <strong>Final grade:</strong> {H(transcript.FinalGrade)}</p></body></html>", "text/html", Encoding.UTF8);
    }

    private async Task LoadAsync()
    {
        var branchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Students = await _db.StudentProfiles.AsNoTracking().Where(x => !_branchAccess.IsBranchAdminOnly(User) || x.BranchId == branchId).OrderBy(x => x.AdmissionNumber).ToListAsync();
        Teachers = await _db.TeacherProfiles.AsNoTracking().Where(x => !_branchAccess.IsBranchAdminOnly(User) || x.BranchId == branchId).OrderBy(x => x.EmployeeNumber).ToListAsync();
        var studentIds = Students.Select(x => x.Id).ToList(); var teacherIds = Teachers.Select(x => x.Id).ToList();
        AcademicYears = await _db.AcademicYears.AsNoTracking().OrderByDescending(x => x.StartDate).ToListAsync();
        Sections = await _db.Sections.Include(x => x.Batch).AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        MedicalProfiles = await _db.StudentMedicalProfiles.Include(x => x.Conditions).AsNoTracking().Where(x => studentIds.Contains(x.StudentProfileId)).ToListAsync();
        StatusHistory = await _db.ProfileStatusHistories.AsNoTracking().Where(x => (x.OwnerType == ProfileStatusOwnerType.Student && studentIds.Contains(x.OwnerId)) || (x.OwnerType == ProfileStatusOwnerType.Teacher && teacherIds.Contains(x.OwnerId))).OrderByDescending(x => x.EffectiveOn).Take(100).ToListAsync();
        Templates = await _db.LifecycleDocumentTemplates.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ThenByDescending(x => x.Version).ToListAsync();
        Transcripts = await _db.StudentTranscripts.Include(x => x.Lines).AsNoTracking().Where(x => studentIds.Contains(x.StudentProfileId)).OrderByDescending(x => x.IssuedOn).ToListAsync();
        Alumni = await _db.StudentAlumniRecords.AsNoTracking().Where(x => studentIds.Contains(x.StudentProfileId)).OrderByDescending(x => x.GraduationDate).ToListAsync();
        var alumniIds = Alumni.Select(x => x.Id).ToList(); Engagements = await _db.AlumniEngagements.AsNoTracking().Where(x => alumniIds.Contains(x.StudentAlumniRecordId)).OrderByDescending(x => x.OccurredOn).Take(100).ToListAsync();
        BulkRuns = await _db.BulkStudentLifecycleRuns.Include(x => x.Rows).AsNoTracking().OrderByDescending(x => x.StartedOn).Take(20).ToListAsync();
        Policies = await _db.ProfileDocumentPolicies.AsNoTracking().OrderBy(x => x.DocumentType).ToListAsync();
        Documents = await _db.ProfileDocuments.AsNoTracking().Where(x => (x.OwnerType == ProfileDocumentOwnerType.Student && studentIds.Contains(x.OwnerId)) || (x.OwnerType == ProfileDocumentOwnerType.Teacher && teacherIds.Contains(x.OwnerId))).OrderByDescending(x => x.UploadedOn).Take(50).ToListAsync();
    }

    public string Person(ProfileStatusOwnerType type, Guid id) => type == ProfileStatusOwnerType.Student ? Students.Where(x => x.Id == id).Select(x => x.AdmissionNumber + " - " + x.FirstName + " " + x.LastName).FirstOrDefault() ?? "-" : Teachers.Where(x => x.Id == id).Select(x => x.EmployeeNumber + " - " + x.FirstName + " " + x.LastName).FirstOrDefault() ?? "-";
    public string Student(Guid id) => Person(ProfileStatusOwnerType.Student, id);
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string H(string? value) => WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "-" : value);

    public sealed class MedicalInput { [Required] public Guid StudentProfileId { get; set; } public string? BloodGroup { get; set; } public string? Allergies { get; set; } public string? CurrentMedications { get; set; } public string? EmergencyInstructions { get; set; } public string? PhysicianName { get; set; } public string? PhysicianPhone { get; set; } public string? InsuranceProvider { get; set; } public string? InsurancePolicyNumber { get; set; } public bool EmergencyCareConsent { get; set; } public string? ConditionName { get; set; } public string? ConditionSeverity { get; set; } public DateOnly? DiagnosedOn { get; set; } public string? CareInstructions { get; set; } }
    public sealed class TemplateInput { [Required] public string Code { get; set; } = "TRANSFER_CERTIFICATE"; [Required] public string Name { get; set; } = "Transfer Certificate"; public ProfileStatusOwnerType OwnerType { get; set; } = ProfileStatusOwnerType.Student; [Required] public string DocumentType { get; set; } = "Transfer Certificate"; [Required] public string HtmlTemplate { get; set; } = "<h1>Transfer Certificate</h1><p>This certifies that <strong>{{Name}}</strong> ({{Number}}) studied at {{Branch}}.</p><p>Date: {{Date}}</p>"; public bool RequiresSignature { get; set; } = true; }
    public sealed class PolicyInput { public ProfileDocumentOwnerType OwnerType { get; set; } = ProfileDocumentOwnerType.Student; [Required] public string DocumentType { get; set; } = "Lifecycle"; public string AllowedExtensions { get; set; } = ".pdf,.png,.jpg,.jpeg,.doc,.docx"; public int MaximumMegabytes { get; set; } = 10; public bool RequiresVirusScan { get; set; } = true; public bool RequiresVerification { get; set; } = true; public int? RetentionDays { get; set; } public int? ExpiryDays { get; set; } }
    public sealed class EngagementInput { [Required] public Guid AlumniRecordId { get; set; } public AlumniEngagementType Type { get; set; } [Required] public string Title { get; set; } = null!; public DateTime OccurredOn { get; set; } = DateTime.Now; public string? Notes { get; set; } public decimal? ContributionAmount { get; set; } }
    public sealed class BulkInput { [Required] public Guid FromAcademicYearId { get; set; } public Guid? FromSectionId { get; set; } [Required] public Guid ToSectionId { get; set; } public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.Today); }
    public sealed class SignatureInput { [Required] public Guid TemplateId { get; set; } [Required] public Guid SubjectId { get; set; } [Required] public string SignerName { get; set; } = null!; public string? SignerTitle { get; set; } }
}
