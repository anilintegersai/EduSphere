using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Admissions;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public sealed class RefinementsModel(TenantDbContext db, ITenantContext tenant, IBranchAccessService branchAccess, IAdmissionAutomationService automation) : PageModel
{
    public List<Branch> Branches { get; private set; } = [];
    public List<ApplicationUser> Interviewers { get; private set; } = [];
    public List<BranchAdmissionSetting> Settings { get; private set; } = [];
    public List<AdmissionInterviewerAvailability> Availability { get; private set; } = [];
    public List<FunnelRow> Funnel { get; private set; } = [];
    public List<SourceRow> Sources { get; private set; } = [];
    [BindProperty] public SettingInput Setting { get; set; } = new();
    [BindProperty] public AvailabilityInput Slot { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }
    public async Task OnGetAsync() => await LoadAsync();
    public async Task<IActionResult> OnPostSettingAsync()
    {
        if (!await branchAccess.CanAccessBranchAsync(User, Setting.BranchId)) return Forbid();
        var item = await db.BranchAdmissionSettings.FirstOrDefaultAsync(x => x.BranchId == Setting.BranchId) ?? new BranchAdmissionSetting { TenantId = tenant.TenantId!.Value, BranchId = Setting.BranchId };
        if (item.Id == Guid.Empty) db.BranchAdmissionSettings.Add(item);
        item.ApplicationNumberFormat = Setting.ApplicationNumberFormat; item.AdmissionNumberFormat = Setting.AdmissionNumberFormat; item.OfferValidityDays = Math.Clamp(Setting.OfferValidityDays, 1, 90); item.FeeDeadlineDays = Math.Clamp(Setting.FeeDeadlineDays, 1, 90); item.AutoPromoteWaitlist = Setting.AutoPromoteWaitlist;
        await db.SaveChangesAsync(); StatusMessage = "Branch admission settings saved."; return RedirectToPage();
    }
    public async Task<IActionResult> OnPostAvailabilityAsync()
    {
        if (!await branchAccess.CanAccessBranchAsync(User, Slot.BranchId) || Slot.EndsAt <= Slot.StartsAt) return Forbid();
        db.AdmissionInterviewerAvailabilities.Add(new AdmissionInterviewerAvailability { TenantId = tenant.TenantId!.Value, BranchId = Slot.BranchId, InterviewerUserId = Slot.InterviewerUserId, DayOfWeek = Slot.DayOfWeek, StartsAt = Slot.StartsAt, EndsAt = Slot.EndsAt, EffectiveFrom = Slot.EffectiveFrom, EffectiveUntil = Slot.EffectiveUntil, Location = Slot.Location, IsActive = true });
        await db.SaveChangesAsync(); StatusMessage = "Interviewer availability added."; return RedirectToPage();
    }
    public async Task<IActionResult> OnPostRunAutomationAsync() { var count = await automation.ProcessAsync(); StatusMessage = $"Admission automation completed with {count} change(s)."; return RedirectToPage(); }
    public async Task<IActionResult> OnGetExportAsync(string format)
    {
        var branchIds = (await branchAccess.FilterBranchesAsync(User, await db.Branches.AsNoTracking().ToListAsync())).Select(x => x.Id).ToList();
        var apps = await db.AdmissionApplications.AsNoTracking().Where(x => branchIds.Contains(x.BranchId)).OrderByDescending(x => x.AppliedOn).ToListAsync();
        return string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase)
            ? File(AdmissionExportBuilder.Pdf(apps), "application/pdf", $"admission-funnel-{DateTime.UtcNow:yyyyMMdd}.pdf")
            : File(AdmissionExportBuilder.Excel(apps), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"admission-funnel-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }
    private async Task LoadAsync()
    {
        Branches = (await branchAccess.FilterBranchesAsync(User, await db.Branches.AsNoTracking().OrderBy(x => x.Name).ToListAsync())).ToList(); var branchIds = Branches.Select(x => x.Id).ToList();
        Interviewers = await db.Users.AsNoTracking().Where(x => x.TenantId == tenant.TenantId).OrderBy(x => x.Email).ToListAsync();
        Settings = await db.BranchAdmissionSettings.AsNoTracking().Where(x => branchIds.Contains(x.BranchId)).ToListAsync(); Availability = await db.AdmissionInterviewerAvailabilities.AsNoTracking().Where(x => branchIds.Contains(x.BranchId) && x.IsActive).OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartsAt).ToListAsync();
        var apps = await db.AdmissionApplications.AsNoTracking().Where(x => branchIds.Contains(x.BranchId)).ToListAsync(); var total = apps.Count;
        Funnel = apps.GroupBy(x => x.Status).OrderBy(x => (int)x.Key).Select(x => new FunnelRow(x.Key.ToString(), x.Count(), total == 0 ? 0 : Math.Round(x.Count() * 100m / total, 1))).ToList();
        Sources = apps.GroupBy(x => x.ApplicationSource ?? "Direct").Select(x => new SourceRow(x.Key, x.Count(), x.Count(a => a.Status == AdmissionApplicationStatus.Enrolled), x.Count() == 0 ? 0 : Math.Round(x.Count(a => a.Status == AdmissionApplicationStatus.Enrolled) * 100m / x.Count(), 1))).OrderByDescending(x => x.Applications).ToList();
    }
    public sealed record FunnelRow(string Stage, int Count, decimal Percentage); public sealed record SourceRow(string Source, int Applications, int Enrolled, decimal Conversion);
    public sealed class SettingInput { [Required] public Guid BranchId { get; set; } [Required] public string ApplicationNumberFormat { get; set; } = "APP-{YYYY}-{SEQ:0000}"; [Required] public string AdmissionNumberFormat { get; set; } = "ADM-{YYYY}-{SEQ:0000}"; public int OfferValidityDays { get; set; } = 7; public int FeeDeadlineDays { get; set; } = 7; public bool AutoPromoteWaitlist { get; set; } = true; }
    public sealed class AvailabilityInput { [Required] public Guid BranchId { get; set; } [Required] public Guid InterviewerUserId { get; set; } public DayOfWeek DayOfWeek { get; set; } public TimeOnly StartsAt { get; set; } = new(9,0); public TimeOnly EndsAt { get; set; } = new(17,0); public DateOnly? EffectiveFrom { get; set; } public DateOnly? EffectiveUntil { get; set; } public string? Location { get; set; } }
}
