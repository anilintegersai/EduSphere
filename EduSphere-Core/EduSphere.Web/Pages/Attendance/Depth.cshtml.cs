using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

namespace EduSphere.Web.Pages.Attendance;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class DepthModel(TenantDbContext db, ITenantContext tenant, IBranchAccessService branches, AttendanceDepthService service, IWebHostEnvironment environment) : PageModel
{
    public IReadOnlyList<Branch> Branches { get; private set; } = [];
    public IReadOnlyList<AttendanceDevice> Devices { get; private set; } = [];
    public IReadOnlyList<AttendanceDeviceImportBatch> Imports { get; private set; } = [];
    public IReadOnlyList<AttendanceSession> Sessions { get; private set; } = [];
    public IReadOnlyList<StudentProfile> Students { get; private set; } = [];
    public IReadOnlyList<StaffProfile> Staff { get; private set; } = [];
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = [];
    public IReadOnlyList<LeaveApplication> ApprovedLeaves { get; private set; } = [];
    public IReadOnlyList<AttendanceCondonationRequest> Condonations { get; private set; } = [];
    public IReadOnlyList<ParentAbsenceExplanation> Explanations { get; private set; } = [];
    public IReadOnlyList<BusAttendanceRecord> BusRecords { get; private set; } = [];
    public IReadOnlyList<StaffAttendanceRecord> StaffRecords { get; private set; } = [];
    public IReadOnlyList<TeacherAttendanceRecord> TeacherRecords { get; private set; } = [];
    public IReadOnlyList<TransportRoute> Routes { get; private set; } = [];
    public IReadOnlyList<Vehicle> Vehicles { get; private set; } = [];
    public IReadOnlyList<AttendanceAlertSchedule> AlertSchedules { get; private set; } = [];
    [TempData] public string? Feedback { get; set; }

    [BindProperty] public DeviceInput Device { get; set; } = new();
    [BindProperty] public Guid ImportDeviceId { get; set; }
    [BindProperty] public IFormFile? ImportFile { get; set; }
    [BindProperty] public BulkInput Bulk { get; set; } = new();
    [BindProperty] public CondonationInput Condonation { get; set; } = new();
    [BindProperty] public BusInput Bus { get; set; } = new();
    [BindProperty] public StaffInput StaffAttendance { get; set; } = new();
    [BindProperty] public TeacherInput TeacherAttendance { get; set; } = new();
    [BindProperty] public AlertInput Alert { get; set; } = new();

    public sealed class DeviceInput { [Required] public Guid? BranchId { get; set; } [Required, StringLength(120)] public string Name { get; set; } = ""; [Required, StringLength(120)] public string DeviceIdentifier { get; set; } = ""; public AttendanceDeviceType DeviceType { get; set; } [Url, StringLength(500)] public string? EndpointUrl { get; set; } }
    public sealed class BulkInput { [Required] public Guid? SessionId { get; set; } public DateOnly TargetDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public AttendanceStatus Status { get; set; } = AttendanceStatus.Present; public bool Overwrite { get; set; } }
    public sealed class CondonationInput { [Required] public Guid? StudentProfileId { get; set; } public DateOnly FromDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public DateOnly ToDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); [Required, StringLength(1000)] public string Reason { get; set; } = ""; public IFormFile? Evidence { get; set; } }
    public sealed class BusInput { [Required] public Guid? StudentProfileId { get; set; } [Required] public Guid? TransportRouteId { get; set; } public Guid? VehicleId { get; set; } public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public BusAttendanceStatus Status { get; set; } [StringLength(500)] public string? Notes { get; set; } }
    public sealed class StaffInput { [Required] public Guid? StaffProfileId { get; set; } [Required] public Guid? BranchId { get; set; } public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public StaffAttendanceStatus Status { get; set; } public TimeOnly? CheckIn { get; set; } public TimeOnly? CheckOut { get; set; } }
    public sealed class TeacherInput { [Required] public Guid? TeacherProfileId { get; set; } [Required] public Guid? BranchId { get; set; } public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); public StaffAttendanceStatus Status { get; set; } public TimeOnly? CheckIn { get; set; } public TimeOnly? CheckOut { get; set; } }
    public sealed class AlertInput { public Guid? BranchId { get; set; } [Required, StringLength(120)] public string Name { get; set; } = "Low attendance daily check"; [Range(1, 100)] public decimal ThresholdPercentage { get; set; } = 75; [Range(1, 365)] public int LookbackDays { get; set; } = 30; [Range(1, 720)] public int RunEveryHours { get; set; } = 24; public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email; }

    public async Task OnGetAsync() => await LoadAsync();

    public IActionResult OnGetDeviceTemplate() => File(System.Text.Encoding.UTF8.GetBytes("ExternalRecordId,PersonCode,PunchedOn,Direction\r\nDEVICE-0001,ADM-2026-0001,2026-10-16 08:45:00,CheckIn\r\n"), "text/csv", "attendance-device-import-template.csv");

    public async Task<IActionResult> OnPostDeviceAsync()
    {
        Keep(nameof(Device));
        if (!ModelState.IsValid || !Device.BranchId.HasValue || !await CanAccessAsync(Device.BranchId.Value)) { await LoadAsync(); return Page(); }
        db.AttendanceDevices.Add(new AttendanceDevice { BranchId = Device.BranchId.Value, Name = Device.Name.Trim(), DeviceIdentifier = Device.DeviceIdentifier.Trim(), DeviceType = Device.DeviceType, EndpointUrl = Device.EndpointUrl?.Trim() });
        await db.SaveChangesAsync(); Feedback = "Attendance device registered."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostImportAsync(CancellationToken token)
    {
        if (ImportFile is null || ImportFile.Length == 0) { ModelState.AddModelError(nameof(ImportFile), "Choose a device CSV file."); await LoadAsync(); return Page(); }
        if (ImportFile.Length > 5 * 1024 * 1024 || !string.Equals(Path.GetExtension(ImportFile.FileName), ".csv", StringComparison.OrdinalIgnoreCase)) { ModelState.AddModelError(nameof(ImportFile), "Upload a CSV file no larger than 5 MB."); await LoadAsync(); return Page(); }
        var device = await db.AttendanceDevices.SingleOrDefaultAsync(x => x.Id == ImportDeviceId, token);
        if (device is null || !await CanAccessAsync(device.BranchId)) return Forbid();
        try { await using var stream = ImportFile.OpenReadStream(); var result = await service.ImportDeviceCsvAsync(device, stream, ImportFile.FileName, UserId(), token); Feedback = $"Imported {result.ImportedRows} rows; {result.RejectedRows} rejected."; }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, ex.Message); await LoadAsync(); return Page(); }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCopyAsync(CancellationToken token)
    {
        if (!Bulk.SessionId.HasValue) return RedirectToPage();
        var session = await db.AttendanceSessions.FindAsync([Bulk.SessionId.Value], token); if (session is null || !await CanAccessAsync(session.BranchId)) return Forbid();
        var count = await service.CopyPreviousDayAsync(session.Id, Bulk.TargetDate, UserId(), token); Feedback = $"Copied {count} attendance records to {Bulk.TargetDate:dd MMM yyyy}."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBulkMarkAsync(CancellationToken token)
    {
        if (!Bulk.SessionId.HasValue) return RedirectToPage();
        var session = await db.AttendanceSessions.FindAsync([Bulk.SessionId.Value], token); if (session is null || !await CanAccessAsync(session.BranchId)) return Forbid();
        var count = await service.BulkMarkAsync(session.Id, Bulk.Status, UserId(), Bulk.Overwrite, token); Feedback = $"Bulk attendance applied to {count} students."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCondonationAsync(CancellationToken token)
    {
        Keep(nameof(Condonation));
        if (Condonation.ToDate < Condonation.FromDate) ModelState.AddModelError("Condonation.ToDate", "End date must not be before start date.");
        var student = Condonation.StudentProfileId.HasValue ? await db.StudentProfiles.FindAsync([Condonation.StudentProfileId.Value], token) : null;
        if (student is null || !await CanAccessAsync(student.BranchId)) ModelState.AddModelError("Condonation.StudentProfileId", "Select an accessible student.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var request = new AttendanceCondonationRequest { StudentProfileId = student!.Id, FromDate = Condonation.FromDate, ToDate = Condonation.ToDate, Reason = Condonation.Reason.Trim(), RequestedByUserId = UserId() };
        if (Condonation.Evidence is { Length: > 0 } evidence)
        {
            var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" }; var extension = Path.GetExtension(evidence.FileName).ToLowerInvariant();
            if (evidence.Length > 10 * 1024 * 1024 || !allowed.Contains(extension)) { ModelState.AddModelError("Condonation.Evidence", "Evidence must be a PDF, JPG or PNG no larger than 10 MB."); await LoadAsync(); return Page(); }
            var folder = Path.Combine(environment.ContentRootPath, "App_Data", "AttendanceEvidence", tenant.TenantId!.Value.ToString("N")); Directory.CreateDirectory(folder);
            var stored = $"{Guid.NewGuid():N}{extension}"; await using var output = System.IO.File.Create(Path.Combine(folder, stored)); await evidence.CopyToAsync(output, token);
            request.EvidenceFileName = Path.GetFileName(evidence.FileName); request.EvidenceStoragePath = stored; request.EvidenceContentType = evidence.ContentType;
        }
        db.AttendanceCondonationRequests.Add(request); await db.SaveChangesAsync(token); Feedback = "Condonation request submitted for review."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReviewCondonationAsync(Guid id, ApprovalStatus status, string? notes, CancellationToken token)
    {
        var item = await db.AttendanceCondonationRequests.Include(x => x.StudentProfile).SingleOrDefaultAsync(x => x.Id == id, token); if (item?.StudentProfile is null || !await CanAccessAsync(item.StudentProfile.BranchId)) return Forbid();
        item.Status = status; item.ReviewNotes = notes?.Trim(); item.ReviewedByUserId = UserId(); item.ReviewedOn = DateTime.UtcNow; await db.SaveChangesAsync(token); Feedback = $"Condonation request {status.ToString().ToLowerInvariant()}."; return RedirectToPage();
    }

    public async Task<IActionResult> OnGetEvidenceAsync(Guid id, CancellationToken token)
    {
        var item = await db.AttendanceCondonationRequests.Include(x => x.StudentProfile).SingleOrDefaultAsync(x => x.Id == id, token); if (item?.StudentProfile is null || !await CanAccessAsync(item.StudentProfile.BranchId) || string.IsNullOrWhiteSpace(item.EvidenceStoragePath)) return NotFound();
        var path = Path.Combine(environment.ContentRootPath, "App_Data", "AttendanceEvidence", tenant.TenantId!.Value.ToString("N"), Path.GetFileName(item.EvidenceStoragePath)); if (!System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, item.EvidenceContentType ?? "application/octet-stream", item.EvidenceFileName ?? "evidence");
    }

    public async Task<IActionResult> OnPostReconcileAsync(Guid leaveId, CancellationToken token)
    {
        var leave = await db.LeaveApplications.Include(x => x.StudentProfile).SingleOrDefaultAsync(x => x.Id == leaveId, token); if (leave?.StudentProfile is null || !await CanAccessAsync(leave.StudentProfile.BranchId)) return Forbid();
        var count = await service.ReconcileLeaveAsync(leaveId, UserId(), token); Feedback = $"Reconciled {count} attendance records with approved leave."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBusAsync(CancellationToken token)
    {
        var student = Bus.StudentProfileId.HasValue ? await db.StudentProfiles.FindAsync([Bus.StudentProfileId.Value], token) : null;
        if (student is null || !Bus.TransportRouteId.HasValue || !await CanAccessAsync(student.BranchId)) return RedirectToPage();
        db.BusAttendanceRecords.Add(new BusAttendanceRecord { BranchId = student.BranchId, StudentProfileId = student.Id, TransportRouteId = Bus.TransportRouteId.Value, VehicleId = Bus.VehicleId, AttendanceDate = Bus.AttendanceDate, Status = Bus.Status, RecordedByUserId = UserId(), Notes = Bus.Notes?.Trim() });
        await db.SaveChangesAsync(token); Feedback = "Bus attendance recorded."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStaffAsync(CancellationToken token)
    {
        if (!StaffAttendance.StaffProfileId.HasValue || !StaffAttendance.BranchId.HasValue || !await CanAccessAsync(StaffAttendance.BranchId.Value)) return RedirectToPage();
        var record = await db.StaffAttendanceRecords.FirstOrDefaultAsync(x => x.StaffProfileId == StaffAttendance.StaffProfileId && x.AttendanceDate == StaffAttendance.AttendanceDate, token);
        if (record is null) db.StaffAttendanceRecords.Add(new StaffAttendanceRecord { StaffProfileId = StaffAttendance.StaffProfileId.Value, BranchId = StaffAttendance.BranchId.Value, AttendanceDate = StaffAttendance.AttendanceDate, Status = StaffAttendance.Status, CheckIn = StaffAttendance.CheckIn, CheckOut = StaffAttendance.CheckOut });
        else { record.Status = StaffAttendance.Status; record.CheckIn = StaffAttendance.CheckIn; record.CheckOut = StaffAttendance.CheckOut; }
        await db.SaveChangesAsync(token); Feedback = "Staff attendance saved."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTeacherAsync(CancellationToken token)
    {
        if (!TeacherAttendance.TeacherProfileId.HasValue || !TeacherAttendance.BranchId.HasValue || !await CanAccessAsync(TeacherAttendance.BranchId.Value)) return RedirectToPage();
        var teacher = await db.TeacherProfiles.FindAsync([TeacherAttendance.TeacherProfileId.Value], token); if (teacher is null || teacher.BranchId != TeacherAttendance.BranchId.Value) return RedirectToPage();
        var record = await db.TeacherAttendanceRecords.FirstOrDefaultAsync(x => x.TeacherProfileId == teacher.Id && x.AttendanceDate == TeacherAttendance.AttendanceDate, token);
        if (record is null) db.TeacherAttendanceRecords.Add(new TeacherAttendanceRecord { TeacherProfileId = teacher.Id, BranchId = teacher.BranchId, AttendanceDate = TeacherAttendance.AttendanceDate, Status = TeacherAttendance.Status, CheckIn = TeacherAttendance.CheckIn, CheckOut = TeacherAttendance.CheckOut });
        else { record.Status = TeacherAttendance.Status; record.CheckIn = TeacherAttendance.CheckIn; record.CheckOut = TeacherAttendance.CheckOut; }
        await db.SaveChangesAsync(token); Feedback = "Teacher attendance saved."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReviewExplanationAsync(Guid id, AttendanceExplanationStatus status, string? notes, CancellationToken token)
    {
        var item = await db.ParentAbsenceExplanations.Include(x => x.StudentProfile).SingleOrDefaultAsync(x => x.Id == id, token); if (item?.StudentProfile is null || !await CanAccessAsync(item.StudentProfile.BranchId)) return Forbid();
        item.Status = status; item.ReviewNotes = notes?.Trim(); item.ReviewedByUserId = UserId(); item.ReviewedOn = DateTime.UtcNow; await db.SaveChangesAsync(token); Feedback = "Absence explanation reviewed."; return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAlertAsync(CancellationToken token)
    {
        Keep(nameof(Alert)); if (Alert.BranchId.HasValue && !await CanAccessAsync(Alert.BranchId.Value)) return Forbid(); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        db.AttendanceAlertSchedules.Add(new AttendanceAlertSchedule { BranchId = Alert.BranchId, Name = Alert.Name.Trim(), ThresholdPercentage = Alert.ThresholdPercentage, LookbackDays = Alert.LookbackDays, RunEveryHours = Alert.RunEveryHours, Channel = Alert.Channel, NextRunOn = DateTime.UtcNow });
        await db.SaveChangesAsync(token); Feedback = "Low-attendance alert schedule created."; return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        if (!tenant.HasTenant) return;
        var allowed = await branches.FilterBranchesAsync(User, await db.Branches.OrderBy(x => x.Name).ToListAsync()); var ids = allowed.Select(x => x.Id).ToHashSet(); Branches = allowed;
        Devices = await db.AttendanceDevices.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.Name).ToListAsync();
        var deviceIds = Devices.Select(x => x.Id).ToList(); Imports = await db.AttendanceDeviceImportBatches.Where(x => deviceIds.Contains(x.AttendanceDeviceId)).OrderByDescending(x => x.UploadedOn).Take(30).ToListAsync();
        Sessions = await db.AttendanceSessions.Include(x => x.Section).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.AttendanceDate).Take(100).ToListAsync();
        Students = await db.StudentProfiles.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.FirstName).ToListAsync(); Staff = await db.StaffProfiles.Where(x => x.BranchId.HasValue && ids.Contains(x.BranchId.Value)).OrderBy(x => x.FirstName).ToListAsync(); Teachers = await db.TeacherProfiles.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.FirstName).ToListAsync();
        ApprovedLeaves = await db.LeaveApplications.Include(x => x.StudentProfile).Where(x => x.Status == LeaveApplicationStatus.Approved && x.StudentProfile != null && ids.Contains(x.StudentProfile.BranchId)).OrderByDescending(x => x.FromDate).Take(50).ToListAsync();
        Condonations = await db.AttendanceCondonationRequests.Include(x => x.StudentProfile).Where(x => x.StudentProfile != null && ids.Contains(x.StudentProfile.BranchId)).OrderByDescending(x => x.RequestedOn).Take(100).ToListAsync();
        Explanations = await db.ParentAbsenceExplanations.Include(x => x.StudentProfile).Where(x => x.StudentProfile != null && ids.Contains(x.StudentProfile.BranchId)).OrderByDescending(x => x.SubmittedOn).Take(100).ToListAsync();
        BusRecords = await db.BusAttendanceRecords.Include(x => x.StudentProfile).Include(x => x.TransportRoute).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.RecordedOn).Take(100).ToListAsync();
        StaffRecords = await db.StaffAttendanceRecords.Include(x => x.StaffProfile).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.AttendanceDate).Take(100).ToListAsync();
        TeacherRecords = await db.TeacherAttendanceRecords.Include(x => x.TeacherProfile).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.AttendanceDate).Take(100).ToListAsync();
        Routes = await db.TransportRoutes.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.Name).ToListAsync(); Vehicles = await db.Vehicles.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.RegistrationNumber).ToListAsync();
        AlertSchedules = await db.AttendanceAlertSchedules.Where(x => !x.BranchId.HasValue || ids.Contains(x.BranchId.Value)).OrderBy(x => x.Name).ToListAsync();
    }
    private Guid? UserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    private Task<bool> CanAccessAsync(Guid branchId) => branches.CanAccessBranchAsync(User, branchId);
    private void Keep(string prefix) { foreach (var key in ModelState.Keys.Where(x => !x.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList()) ModelState.Remove(key); }
}
