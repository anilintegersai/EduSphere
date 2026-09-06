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

[Authorize(Policy = AuthorizationPolicies.TransportManager)]
public class TransportModel : EnterprisePageModel
{
    private readonly ICrudService<Vehicle> _vehicles;
    private readonly ICrudService<TransportDriver> _drivers;
    private readonly ICrudService<TransportRoute> _routes;
    private readonly ICrudService<TransportRouteStop> _stops;
    private readonly ICrudService<TransportRouteAssignment> _routeAssignments;
    private readonly ICrudService<StudentTransportAssignment> _studentAssignments;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;

    public TransportModel(
        ICrudService<Vehicle> vehicles,
        ICrudService<TransportDriver> drivers,
        ICrudService<TransportRoute> routes,
        ICrudService<TransportRouteStop> stops,
        ICrudService<TransportRouteAssignment> routeAssignments,
        ICrudService<StudentTransportAssignment> studentAssignments,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
        : base(tenant, branchAccess)
    {
        _vehicles = vehicles;
        _drivers = drivers;
        _routes = routes;
        _stops = stops;
        _routeAssignments = routeAssignments;
        _studentAssignments = studentAssignments;
        _branches = branches;
        _students = students;
    }

    public IReadOnlyList<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();
    public IReadOnlyList<TransportDriver> Drivers { get; private set; } = new List<TransportDriver>();
    public IReadOnlyList<TransportRoute> Routes { get; private set; } = new List<TransportRoute>();
    public IReadOnlyList<TransportRouteStop> Stops { get; private set; } = new List<TransportRouteStop>();
    public IReadOnlyList<TransportRouteAssignment> RouteAssignments { get; private set; } = new List<TransportRouteAssignment>();
    public IReadOnlyList<StudentTransportAssignment> StudentAssignments { get; private set; } = new List<StudentTransportAssignment>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();

    [BindProperty] public VehicleInputModel VehicleInput { get; set; } = new();
    [BindProperty] public DriverInputModel DriverInput { get; set; } = new();
    [BindProperty] public RouteInputModel RouteInput { get; set; } = new();
    [BindProperty] public StopInputModel StopInput { get; set; } = new();
    [BindProperty] public StudentAssignmentInputModel StudentAssignmentInput { get; set; } = new();

    public class VehicleInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(30), Display(Name = "Registration #")] public string RegistrationNumber { get; set; } = string.Empty;
        [StringLength(80), Display(Name = "Display name")] public string? DisplayName { get; set; }
        public VehicleType Type { get; set; } = VehicleType.Bus;
        [Range(1, 120), Display(Name = "Seats")] public int SeatCapacity { get; set; } = 40;
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;
        [Display(Name = "Insurance valid until")] public DateOnly? InsuranceValidUntil { get; set; }
    }

    public class DriverInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(100), Display(Name = "First name")] public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100), Display(Name = "Last name")] public string LastName { get; set; } = string.Empty;
        [Required, StringLength(30), Display(Name = "Phone")] public string PhoneNumber { get; set; } = string.Empty;
        [Required, StringLength(60), Display(Name = "License #")] public string LicenseNumber { get; set; } = string.Empty;
        [Display(Name = "License valid until")] public DateOnly? LicenseValidUntil { get; set; }
        public DriverStatus Status { get; set; } = DriverStatus.Active;
    }

    public class RouteInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(50), Display(Name = "Route code")] public string RouteCode { get; set; } = string.Empty;
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [StringLength(80), Display(Name = "Shift")] public string? ShiftName { get; set; }
        [Display(Name = "Start")] public TimeOnly? StartsAt { get; set; }
        [Display(Name = "End")] public TimeOnly? EndsAt { get; set; }
        [Range(0, 1000), Display(Name = "Distance km")] public decimal DistanceKm { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; } = true;
    }

    public class StopInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Route")] public Guid? TransportRouteId { get; set; }
        [Required, StringLength(150), Display(Name = "Stop name")] public string StopName { get; set; } = string.Empty;
        [Range(0, 200), Display(Name = "Order")] public int StopOrder { get; set; }
        [Display(Name = "Pickup")] public TimeOnly? PickupTime { get; set; }
        [Display(Name = "Drop")] public TimeOnly? DropTime { get; set; }
        [Range(0, 100000), Display(Name = "Monthly fee")] public decimal MonthlyFee { get; set; }
        [StringLength(200)] public string? Landmark { get; set; }
    }

    public class StudentAssignmentInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, Display(Name = "Route")] public Guid? TransportRouteId { get; set; }
        [Display(Name = "Stop")] public Guid? TransportRouteStopId { get; set; }
        [Display(Name = "Vehicle")] public Guid? VehicleId { get; set; }
        [Display(Name = "Start date")] public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Range(0, 100000), Display(Name = "Monthly fee")] public decimal MonthlyFee { get; set; }
        public TransportAssignmentStatus Status { get; set; } = TransportAssignmentStatus.Active;
    }

    public string RouteName(Guid id) => Routes.FirstOrDefault(r => r.Id == id)?.Name ?? "-";
    public string VehicleName(Guid? id) => Vehicles.FirstOrDefault(v => v.Id == id)?.RegistrationNumber ?? "-";
    public string StopName(Guid? id) => Stops.FirstOrDefault(s => s.Id == id)?.StopName ?? "-";
    public string DriverName(Guid? id)
    {
        var driver = Drivers.FirstOrDefault(d => d.Id == id);
        return driver is null ? "-" : $"{driver.FirstName} {driver.LastName}";
    }
    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }

    public async Task OnGetAsync()
    {
        if (!HasTenant) return;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveVehicleAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(VehicleInput));
        if (!await ValidateBranchSelectionAsync("VehicleInput.BranchId", VehicleInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _vehicles.CreateAsync(new Vehicle
        {
            BranchId = VehicleInput.BranchId!.Value,
            RegistrationNumber = VehicleInput.RegistrationNumber,
            DisplayName = VehicleInput.DisplayName,
            Type = VehicleInput.Type,
            SeatCapacity = VehicleInput.SeatCapacity,
            Status = VehicleInput.Status,
            InsuranceValidUntil = VehicleInput.InsuranceValidUntil
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveDriverAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(DriverInput));
        if (!await ValidateBranchSelectionAsync("DriverInput.BranchId", DriverInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _drivers.CreateAsync(new TransportDriver
        {
            BranchId = DriverInput.BranchId!.Value,
            FirstName = DriverInput.FirstName,
            LastName = DriverInput.LastName,
            PhoneNumber = DriverInput.PhoneNumber,
            LicenseNumber = DriverInput.LicenseNumber,
            LicenseValidUntil = DriverInput.LicenseValidUntil,
            Status = DriverInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveRouteAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(RouteInput));
        if (!await ValidateBranchSelectionAsync("RouteInput.BranchId", RouteInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _routes.CreateAsync(new TransportRoute
        {
            BranchId = RouteInput.BranchId!.Value,
            RouteCode = RouteInput.RouteCode,
            Name = RouteInput.Name,
            ShiftName = RouteInput.ShiftName,
            StartsAt = RouteInput.StartsAt,
            EndsAt = RouteInput.EndsAt,
            DistanceKm = RouteInput.DistanceKm,
            IsActive = RouteInput.IsActive
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveStopAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(StopInput));
        if (!await ValidateBranchSelectionAsync("StopInput.BranchId", StopInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (StopInput.TransportRouteId is not Guid routeId || await _routes.GetAsync(routeId) is not { } route || route.BranchId != StopInput.BranchId)
            ModelState.AddModelError("StopInput.TransportRouteId", "Selected route was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _stops.CreateAsync(new TransportRouteStop
        {
            BranchId = StopInput.BranchId!.Value,
            TransportRouteId = StopInput.TransportRouteId!.Value,
            StopName = StopInput.StopName,
            StopOrder = StopInput.StopOrder,
            PickupTime = StopInput.PickupTime,
            DropTime = StopInput.DropTime,
            MonthlyFee = StopInput.MonthlyFee,
            Landmark = StopInput.Landmark
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveStudentAssignmentAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(StudentAssignmentInput));
        if (!await ValidateBranchSelectionAsync("StudentAssignmentInput.BranchId", StudentAssignmentInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (StudentAssignmentInput.StudentProfileId is not Guid studentId || await _students.GetAsync(studentId) is not { } student || student.BranchId != StudentAssignmentInput.BranchId)
            ModelState.AddModelError("StudentAssignmentInput.StudentProfileId", "Selected student was not found for this branch.");
        if (StudentAssignmentInput.TransportRouteId is not Guid routeId || await _routes.GetAsync(routeId) is not { } route || route.BranchId != StudentAssignmentInput.BranchId)
            ModelState.AddModelError("StudentAssignmentInput.TransportRouteId", "Selected route was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _studentAssignments.CreateAsync(new StudentTransportAssignment
        {
            BranchId = StudentAssignmentInput.BranchId!.Value,
            StudentProfileId = StudentAssignmentInput.StudentProfileId!.Value,
            TransportRouteId = StudentAssignmentInput.TransportRouteId!.Value,
            TransportRouteStopId = StudentAssignmentInput.TransportRouteStopId,
            VehicleId = StudentAssignmentInput.VehicleId,
            StartDate = StudentAssignmentInput.StartDate,
            MonthlyFee = StudentAssignmentInput.MonthlyFee,
            Status = StudentAssignmentInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteVehicleAsync(Guid id) { await DeleteIfAllowedAsync(_vehicles, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteDriverAsync(Guid id) { await DeleteIfAllowedAsync(_drivers, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteRouteAsync(Guid id) { await DeleteIfAllowedAsync(_routes, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteStopAsync(Guid id) { await DeleteIfAllowedAsync(_stops, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteStudentAssignmentAsync(Guid id) { await DeleteIfAllowedAsync(_studentAssignments, id, x => x.BranchId); return RedirectToPage(); }

    private async Task LoadAsync()
    {
        await LoadBranchesAsync(_branches);
        Vehicles = (await FilterBranchScopedAsync(_vehicles, x => x.BranchId)).OrderBy(v => v.RegistrationNumber).ToList();
        Drivers = (await FilterBranchScopedAsync(_drivers, x => x.BranchId)).OrderBy(d => d.FirstName).ToList();
        Routes = (await FilterBranchScopedAsync(_routes, x => x.BranchId)).OrderBy(r => r.RouteCode).ToList();
        Stops = (await FilterBranchScopedAsync(_stops, x => x.BranchId)).OrderBy(s => s.StopOrder).ToList();
        RouteAssignments = (await FilterBranchScopedAsync(_routeAssignments, x => x.BranchId)).OrderByDescending(a => a.EffectiveFrom).ToList();
        StudentAssignments = (await FilterBranchScopedAsync(_studentAssignments, x => x.BranchId)).OrderByDescending(a => a.StartDate).ToList();
        Students = (await FilterBranchScopedAsync(_students, x => x.BranchId)).OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();

        if (await DefaultBranchIdAsync() is Guid branchId)
        {
            VehicleInput.BranchId ??= branchId;
            DriverInput.BranchId ??= branchId;
            RouteInput.BranchId ??= branchId;
            StopInput.BranchId ??= branchId;
            StudentAssignmentInput.BranchId ??= branchId;
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
