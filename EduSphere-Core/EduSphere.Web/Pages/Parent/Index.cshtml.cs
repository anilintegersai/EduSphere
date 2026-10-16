using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EduSphere.Web.Pages.Parent;

[Authorize(Roles = Roles.Parent)]
public sealed class IndexModel(TenantDbContext db) : PageModel
{
    public IReadOnlyList<StudentProfile> Students { get; private set; } = [];
    public IReadOnlyList<AttendanceRecord> Attendance { get; private set; } = [];
    public IReadOnlyList<FeeInvoice> Invoices { get; private set; } = [];
    public IReadOnlyList<Result> Results { get; private set; } = [];
    public IReadOnlyList<NotificationMessage> Notifications { get; private set; } = [];
    public IReadOnlyList<ParentAbsenceExplanation> Explanations { get; private set; } = [];
    public IReadOnlyDictionary<Guid, string> BranchNames { get; private set; } = new Dictionary<Guid, string>();
    public Guid SelectedStudentId { get; private set; }
    [BindProperty] public ExplanationInput Explanation { get; set; } = new();
    [TempData] public string? Feedback { get; set; }

    public sealed class ExplanationInput
    {
        [Required] public Guid? StudentProfileId { get; set; }
        public Guid? AttendanceRecordId { get; set; }
        public DateOnly FromDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly ToDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [Required, StringLength(1000)] public string Details { get; set; } = "";
    }

    public async Task OnGetAsync(Guid? studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;
        var studentIds = await db.StudentGuardians.Where(x => x.ParentUserId == userId && x.HasPortalAccess).Select(x => x.StudentProfileId).Distinct().ToListAsync();
        Students = await db.StudentProfiles.Where(x => studentIds.Contains(x.Id)).OrderBy(x => x.FirstName).ToListAsync();
        var selected = studentId.HasValue && studentIds.Contains(studentId.Value) ? studentId.Value : studentIds.FirstOrDefault();
        if (selected == Guid.Empty) return;
        SelectedStudentId = selected;
        Attendance = await db.AttendanceRecords.Include(x => x.AttendanceSession).Where(x => x.StudentProfileId == selected).OrderByDescending(x => x.AttendanceSession!.AttendanceDate).Take(60).ToListAsync();
        Invoices = await db.FeeInvoices.Where(x => x.StudentProfileId == selected).OrderByDescending(x => x.InvoiceDate).ToListAsync();
        Results = await db.Results.Include(x => x.Exam).Where(x => x.StudentProfileId == selected && x.Status == ResultStatus.Published).OrderByDescending(x => x.PublishedOn).ToListAsync();
        Notifications = await (from recipient in db.NotificationRecipients join message in db.NotificationMessages on recipient.NotificationMessageId equals message.Id where recipient.UserId == userId select message).Distinct().OrderByDescending(x => x.CreatedOn).Take(50).ToListAsync();
        Explanations = await db.ParentAbsenceExplanations.Where(x => x.StudentProfileId == selected && x.SubmittedByUserId == userId).OrderByDescending(x => x.SubmittedOn).Take(30).ToListAsync();
        var branchIds = Students.Select(x => x.BranchId).Distinct().ToList(); BranchNames = await db.Branches.Where(x => branchIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.Name);
    }

    public async Task<IActionResult> OnPostExplainAsync()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || !Explanation.StudentProfileId.HasValue) return Forbid();
        var linked = await db.StudentGuardians.AnyAsync(x => x.ParentUserId == userId && x.StudentProfileId == Explanation.StudentProfileId && x.HasPortalAccess);
        if (!linked) return Forbid();
        if (Explanation.ToDate < Explanation.FromDate) ModelState.AddModelError("Explanation.ToDate", "End date must not be before start date.");
        if (Explanation.AttendanceRecordId.HasValue && !await db.AttendanceRecords.AnyAsync(x => x.Id == Explanation.AttendanceRecordId && x.StudentProfileId == Explanation.StudentProfileId)) ModelState.AddModelError("Explanation.AttendanceRecordId", "Selected attendance record is invalid.");
        if (!ModelState.IsValid) { await OnGetAsync(Explanation.StudentProfileId); return Page(); }
        db.ParentAbsenceExplanations.Add(new ParentAbsenceExplanation { StudentProfileId = Explanation.StudentProfileId.Value, AttendanceRecordId = Explanation.AttendanceRecordId, FromDate = Explanation.FromDate, ToDate = Explanation.ToDate, Explanation = Explanation.Details.Trim(), SubmittedByUserId = userId });
        await db.SaveChangesAsync(); Feedback = "Absence explanation submitted to the branch team."; return RedirectToPage(new { studentId = Explanation.StudentProfileId });
    }
    public decimal Outstanding => Invoices.Sum(x => Math.Max(0, x.TotalAmount - x.PaidAmount));
    public decimal AttendancePercentage => Attendance.Count == 0 ? 0 : Math.Round(Attendance.Count(x => x.Status == AttendanceStatus.Present) * 100m / Attendance.Count, 1);
}
