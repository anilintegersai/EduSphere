using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Reporting;

[Authorize(Roles = Roles.SuperAdmin + "," + Roles.TenantAdmin + "," + Roles.BranchAdmin + "," + Roles.Principal + "," + Roles.DepartmentAdmin + "," + Roles.ExamController + "," + Roles.Accountant)]
public sealed class IndexModel(TenantDbContext db, IReportingService reporting, IBranchAccessService branchAccess) : PageModel
{
    public IReadOnlyList<Branch> Branches { get; private set; } = [];
    public IReadOnlyList<StudentProfile> Students { get; private set; } = [];
    public IReadOnlyList<ReportDefinition> Definitions { get; private set; } = [];
    public IReadOnlyList<ReportExecution> Executions { get; private set; } = [];
    public IReadOnlyList<ReportSchedule> Schedules { get; private set; } = [];
    public IReadOnlyList<DashboardWidgetConfiguration> Widgets { get; private set; } = [];
    public IReadOnlyList<BiDashboard> BiDashboards { get; private set; } = [];
    public IReadOnlyList<TenantIntegrationSetting> AnalyticsIntegrations { get; private set; } = [];
    public IReadOnlyList<AnalyticsSnapshot> Snapshots { get; private set; } = [];
    public ReportData? Preview { get; private set; }
    public IReadOnlyList<string> AvailableFields => reporting.AvailableFields(Builder.DataSource);
    [TempData] public string? Feedback { get; set; }
    [BindProperty] public BuilderInput Builder { get; set; } = new();
    [BindProperty] public RunInput Run { get; set; } = new();
    [BindProperty] public ScheduleInput Schedule { get; set; } = new();
    [BindProperty] public WidgetInput Widget { get; set; } = new();
    [BindProperty] public BiInput Bi { get; set; } = new();

    public sealed class BuilderInput { public Guid? BranchId { get; set; } [Required, StringLength(80)] public string Code { get; set; } = ""; [Required, StringLength(180)] public string Name { get; set; } = ""; public ReportDataSource DataSource { get; set; } = ReportDataSource.Students; [Required] public string FieldsCsv { get; set; } = "AdmissionNumber,Student,Branch,Section,Status"; public Guid? StudentId { get; set; } [StringLength(500)] public string? Description { get; set; } }
    public sealed class RunInput { [Required] public Guid? DefinitionId { get; set; } public ReportOutputFormat Format { get; set; } = ReportOutputFormat.Excel; }
    public sealed class ScheduleInput { [Required] public Guid? DefinitionId { get; set; } public Guid? BranchId { get; set; } public ReportScheduleFrequency Frequency { get; set; } = ReportScheduleFrequency.Weekly; public TimeOnly DeliveryTime { get; set; } = new(7, 0); public ReportOutputFormat Format { get; set; } = ReportOutputFormat.Excel; [Required] public string Recipients { get; set; } = ""; }
    public sealed class WidgetInput { public Guid? BranchId { get; set; } [Required] public string RoleName { get; set; } = Roles.TenantAdmin; [Required] public string WidgetKey { get; set; } = "Students"; [Required] public string DisplayName { get; set; } = "Students"; public int DisplayOrder { get; set; } public DashboardWidgetSize Size { get; set; } = DashboardWidgetSize.Small; public bool IsVisible { get; set; } = true; }
    public sealed class BiInput { public Guid? BranchId { get; set; } [Required] public string Name { get; set; } = ""; public BiProvider Provider { get; set; } [Required, Url] public string EmbedUrl { get; set; } = ""; public string? ExternalReportId { get; set; } public Guid? IntegrationSettingId { get; set; } public string? AllowedRolesCsv { get; set; } }

    public async Task OnGetAsync(Guid? previewId) { await reporting.EnsureCatalogAsync(); await Load(); if (previewId is Guid id) { var definition = Definitions.FirstOrDefault(x => x.Id == id); if (definition is not null) Preview = await reporting.ExecuteAsync(definition); } }
    public async Task<IActionResult> OnPostBuilderAsync() { var allowed = reporting.AvailableFields(Builder.DataSource); var selected = Builder.FieldsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(allowed.Contains).Distinct().ToArray(); if (selected.Length == 0) ModelState.AddModelError("Builder.FieldsCsv", "Select at least one valid field for this data source."); if (!ModelState.IsValid) { await Load(); return Page(); } var filters = Builder.StudentId.HasValue ? JsonSerializer.Serialize(new { studentId = Builder.StudentId }) : "{}"; db.ReportDefinitions.Add(new() { BranchId = Builder.BranchId, Code = Builder.Code.Trim().ToLowerInvariant(), Name = Builder.Name, Description = Builder.Description, DataSource = Builder.DataSource, SelectedFieldsJson = JsonSerializer.Serialize(selected), FiltersJson = filters, IsPublished = true }); await db.SaveChangesAsync(); Feedback = "Report definition added to the catalog."; return RedirectToPage(); }
    public async Task<IActionResult> OnPostRunAsync() { if (Run.DefinitionId is not Guid id) return RedirectToPage(); var execution = await reporting.ExecuteAndStoreAsync(id, Run.Format); Feedback = execution.Status == ReportExecutionStatus.Succeeded ? $"Generated {execution.RowCount} rows." : execution.ErrorMessage; return RedirectToPage(); }
    public async Task<IActionResult> OnGetDownloadAsync(Guid id) { var execution = await db.ReportExecutions.SingleAsync(x => x.Id == id && x.Status == ReportExecutionStatus.Succeeded); return File(execution.OutputContent!, execution.ContentType!, execution.FileName); }
    public async Task<IActionResult> OnPostScheduleAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } var now = DateTime.UtcNow; db.ReportSchedules.Add(new() { ReportDefinitionId = Schedule.DefinitionId!.Value, BranchId = Schedule.BranchId, Frequency = Schedule.Frequency, DeliveryTime = Schedule.DeliveryTime, Format = Schedule.Format, Recipients = Schedule.Recipients, NextRunOn = now.AddMinutes(5) }); await db.SaveChangesAsync(); Feedback = "Report delivery scheduled."; return RedirectToPage(); }
    public async Task<IActionResult> OnPostWidgetAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } var existing = await db.DashboardWidgetConfigurations.FirstOrDefaultAsync(x => x.BranchId == Widget.BranchId && x.RoleName == Widget.RoleName && x.WidgetKey == Widget.WidgetKey); if (existing is null) { existing = new() { BranchId = Widget.BranchId, RoleName = Widget.RoleName, WidgetKey = Widget.WidgetKey }; db.Add(existing); } existing.DisplayName = Widget.DisplayName; existing.DisplayOrder = Widget.DisplayOrder; existing.Size = Widget.Size; existing.IsVisible = Widget.IsVisible; await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostBiAsync() { if (!ModelState.IsValid || !Uri.TryCreate(Bi.EmbedUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) { ModelState.AddModelError("Bi.EmbedUrl", "A valid HTTPS embed URL is required."); await Load(); return Page(); } db.BiDashboards.Add(new() { BranchId = Bi.BranchId, Name = Bi.Name, Provider = Bi.Provider, EmbedUrl = Bi.EmbedUrl, ExternalReportId = Bi.ExternalReportId, TenantIntegrationSettingId = Bi.IntegrationSettingId, AllowedRolesCsv = Bi.AllowedRolesCsv }); await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostSnapshotAsync() { await reporting.CaptureSnapshotsAsync(DateOnly.FromDateTime(DateTime.UtcNow)); Feedback = "Historical analytics snapshot captured."; return RedirectToPage(); }

    private async Task Load()
    {
        Branches = await branchAccess.FilterBranchesAsync(User, await db.Branches.AsNoTracking().ToListAsync()); var ids = Branches.Select(x => x.Id).ToList();
        Students = await db.StudentProfiles.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Take(2000).ToListAsync(); Definitions = await db.ReportDefinitions.Where(x => x.BranchId == null || ids.Contains(x.BranchId.Value)).OrderBy(x => x.Name).ToListAsync(); var definitionIds = Definitions.Select(x => x.Id).ToList(); Executions = await db.ReportExecutions.Include(x => x.ReportDefinition).Where(x => definitionIds.Contains(x.ReportDefinitionId)).OrderByDescending(x => x.RequestedOn).Take(50).ToListAsync(); Schedules = await db.ReportSchedules.Include(x => x.ReportDefinition).Where(x => definitionIds.Contains(x.ReportDefinitionId)).OrderBy(x => x.NextRunOn).ToListAsync(); Widgets = await db.DashboardWidgetConfigurations.Where(x => x.BranchId == null || ids.Contains(x.BranchId.Value)).OrderBy(x => x.RoleName).ThenBy(x => x.DisplayOrder).ToListAsync(); BiDashboards = await db.BiDashboards.Where(x => x.IsActive && (x.BranchId == null || ids.Contains(x.BranchId.Value))).OrderBy(x => x.Name).ToListAsync(); AnalyticsIntegrations = await db.TenantIntegrationSettings.Where(x => x.Category == IntegrationCategory.Analytics && x.IsEnabled).OrderBy(x => x.DisplayName).ToListAsync(); Snapshots = await db.AnalyticsSnapshots.Where(x => x.BranchId == null || ids.Contains(x.BranchId.Value)).OrderByDescending(x => x.SnapshotDate).Take(200).ToListAsync(); Builder.BranchId ??= Branches.FirstOrDefault()?.Id; Schedule.BranchId ??= Branches.FirstOrDefault()?.Id; Widget.BranchId ??= Branches.FirstOrDefault()?.Id; Bi.BranchId ??= Branches.FirstOrDefault()?.Id;
    }
}
