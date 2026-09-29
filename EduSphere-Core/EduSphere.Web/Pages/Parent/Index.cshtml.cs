using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Parent;

[Authorize(Roles = Roles.Parent)]
public sealed class IndexModel(TenantDbContext db) : PageModel
{
    public IReadOnlyList<StudentProfile> Students { get; private set; } = [];
    public IReadOnlyList<AttendanceRecord> Attendance { get; private set; } = [];
    public IReadOnlyList<FeeInvoice> Invoices { get; private set; } = [];
    public IReadOnlyList<Result> Results { get; private set; } = [];
    public IReadOnlyList<NotificationMessage> Notifications { get; private set; } = [];
    public IReadOnlyDictionary<Guid, string> BranchNames { get; private set; } = new Dictionary<Guid, string>();

    public async Task OnGetAsync(Guid? studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;
        var studentIds = await db.StudentGuardians.Where(x => x.ParentUserId == userId && x.HasPortalAccess).Select(x => x.StudentProfileId).Distinct().ToListAsync();
        Students = await db.StudentProfiles.Where(x => studentIds.Contains(x.Id)).OrderBy(x => x.FirstName).ToListAsync();
        var selected = studentId.HasValue && studentIds.Contains(studentId.Value) ? studentId.Value : studentIds.FirstOrDefault();
        if (selected == Guid.Empty) return;
        Attendance = await db.AttendanceRecords.Include(x => x.AttendanceSession).Where(x => x.StudentProfileId == selected).OrderByDescending(x => x.AttendanceSession!.AttendanceDate).Take(60).ToListAsync();
        Invoices = await db.FeeInvoices.Where(x => x.StudentProfileId == selected).OrderByDescending(x => x.InvoiceDate).ToListAsync();
        Results = await db.Results.Include(x => x.Exam).Where(x => x.StudentProfileId == selected && x.Status == ResultStatus.Published).OrderByDescending(x => x.PublishedOn).ToListAsync();
        Notifications = await (from recipient in db.NotificationRecipients join message in db.NotificationMessages on recipient.NotificationMessageId equals message.Id where recipient.UserId == userId select message).Distinct().OrderByDescending(x => x.CreatedOn).Take(50).ToListAsync();
        var branchIds = Students.Select(x => x.BranchId).Distinct().ToList(); BranchNames = await db.Branches.Where(x => branchIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.Name);
    }
    public decimal Outstanding => Invoices.Sum(x => Math.Max(0, x.TotalAmount - x.PaidAmount));
    public decimal AttendancePercentage => Attendance.Count == 0 ? 0 : Math.Round(Attendance.Count(x => x.Status == AttendanceStatus.Present) * 100m / Attendance.Count, 1);
}
