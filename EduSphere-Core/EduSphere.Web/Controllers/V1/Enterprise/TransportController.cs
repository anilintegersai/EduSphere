using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/transport")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TransportManager)]
public class TransportController : EnterpriseControllerBase
{
    private readonly ICrudService<Vehicle> _vehicles;
    private readonly ICrudService<TransportDriver> _drivers;
    private readonly ICrudService<TransportRoute> _routes;
    private readonly ICrudService<TransportRouteStop> _stops;
    private readonly ICrudService<TransportRouteAssignment> _routeAssignments;
    private readonly ICrudService<StudentTransportAssignment> _studentAssignments;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;

    public TransportController(
        ICrudService<Vehicle> vehicles,
        ICrudService<TransportDriver> drivers,
        ICrudService<TransportRoute> routes,
        ICrudService<TransportRouteStop> stops,
        ICrudService<TransportRouteAssignment> routeAssignments,
        ICrudService<StudentTransportAssignment> studentAssignments,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        IBranchAccessService branchAccess)
        : base(branchAccess)
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

    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehicles([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<VehicleDto>>.Ok((await FilterByBranchAsync(_vehicles, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("vehicles")]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _vehicles.CreateAsync(Apply(new Vehicle(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<VehicleDto>.Ok(created.Map()));
    }

    [HttpPut("vehicles/{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        if (await GetBranchScopedAsync(_vehicles, id, x => x.BranchId) is null) return NotFoundResult(id, "Vehicle");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _vehicles.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("vehicles/{id:guid}")]
    public Task<IActionResult> DeleteVehicle(Guid id)
        => DeleteBranchScopedAsync(_vehicles, id, x => x.BranchId, "Vehicle");

    [HttpGet("drivers")]
    public async Task<IActionResult> GetDrivers([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<TransportDriverDto>>.Ok((await FilterByBranchAsync(_drivers, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("drivers")]
    public async Task<IActionResult> CreateDriver([FromBody] CreateTransportDriverRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _drivers.CreateAsync(Apply(new TransportDriver(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TransportDriverDto>.Ok(created.Map()));
    }

    [HttpPut("drivers/{id:guid}")]
    public async Task<IActionResult> UpdateDriver(Guid id, [FromBody] UpdateTransportDriverRequest request)
    {
        if (await GetBranchScopedAsync(_drivers, id, x => x.BranchId) is null) return NotFoundResult(id, "Driver");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _drivers.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("drivers/{id:guid}")]
    public Task<IActionResult> DeleteDriver(Guid id)
        => DeleteBranchScopedAsync(_drivers, id, x => x.BranchId, "Driver");

    [HttpGet("routes")]
    public async Task<IActionResult> GetRoutes([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<TransportRouteDto>>.Ok((await FilterByBranchAsync(_routes, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("routes")]
    public async Task<IActionResult> CreateRoute([FromBody] CreateTransportRouteRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _routes.CreateAsync(Apply(new TransportRoute(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TransportRouteDto>.Ok(created.Map()));
    }

    [HttpPut("routes/{id:guid}")]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateTransportRouteRequest request)
    {
        if (await GetBranchScopedAsync(_routes, id, x => x.BranchId) is null) return NotFoundResult(id, "Route");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _routes.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("routes/{id:guid}")]
    public Task<IActionResult> DeleteRoute(Guid id)
        => DeleteBranchScopedAsync(_routes, id, x => x.BranchId, "Route");

    [HttpGet("stops")]
    public async Task<IActionResult> GetStops([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<TransportRouteStopDto>>.Ok((await FilterByBranchAsync(_stops, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("stops")]
    public async Task<IActionResult> CreateStop([FromBody] CreateTransportRouteStopRequest request)
    {
        if (await ValidateRouteChildAsync(request.TransportRouteId, request.BranchId) is { } invalid) return invalid;
        var created = await _stops.CreateAsync(Apply(new TransportRouteStop(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TransportRouteStopDto>.Ok(created.Map()));
    }

    [HttpPut("stops/{id:guid}")]
    public async Task<IActionResult> UpdateStop(Guid id, [FromBody] UpdateTransportRouteStopRequest request)
    {
        if (await GetBranchScopedAsync(_stops, id, x => x.BranchId) is null) return NotFoundResult(id, "Route stop");
        if (await ValidateRouteChildAsync(request.TransportRouteId, request.BranchId) is { } invalid) return invalid;
        await _stops.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("stops/{id:guid}")]
    public Task<IActionResult> DeleteStop(Guid id)
        => DeleteBranchScopedAsync(_stops, id, x => x.BranchId, "Route stop");

    [HttpGet("route-assignments")]
    public async Task<IActionResult> GetRouteAssignments([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<TransportRouteAssignmentDto>>.Ok((await FilterByBranchAsync(_routeAssignments, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("route-assignments")]
    public async Task<IActionResult> CreateRouteAssignment([FromBody] CreateTransportRouteAssignmentRequest request)
    {
        if (await ValidateRouteAssignmentAsync(request.BranchId, request.TransportRouteId, request.VehicleId, request.DriverId) is { } invalid) return invalid;
        var created = await _routeAssignments.CreateAsync(Apply(new TransportRouteAssignment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TransportRouteAssignmentDto>.Ok(created.Map()));
    }

    [HttpPut("route-assignments/{id:guid}")]
    public async Task<IActionResult> UpdateRouteAssignment(Guid id, [FromBody] UpdateTransportRouteAssignmentRequest request)
    {
        if (await GetBranchScopedAsync(_routeAssignments, id, x => x.BranchId) is null) return NotFoundResult(id, "Route assignment");
        if (await ValidateRouteAssignmentAsync(request.BranchId, request.TransportRouteId, request.VehicleId, request.DriverId) is { } invalid) return invalid;
        await _routeAssignments.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("route-assignments/{id:guid}")]
    public Task<IActionResult> DeleteRouteAssignment(Guid id)
        => DeleteBranchScopedAsync(_routeAssignments, id, x => x.BranchId, "Route assignment");

    [HttpGet("student-assignments")]
    public async Task<IActionResult> GetStudentAssignments([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<StudentTransportAssignmentDto>>.Ok((await FilterByBranchAsync(_studentAssignments, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("student-assignments")]
    public async Task<IActionResult> CreateStudentAssignment([FromBody] CreateStudentTransportAssignmentRequest request)
    {
        if (await ValidateStudentAssignmentAsync(request) is { } invalid) return invalid;
        var created = await _studentAssignments.CreateAsync(Apply(new StudentTransportAssignment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<StudentTransportAssignmentDto>.Ok(created.Map()));
    }

    [HttpPut("student-assignments/{id:guid}")]
    public async Task<IActionResult> UpdateStudentAssignment(Guid id, [FromBody] UpdateStudentTransportAssignmentRequest request)
    {
        if (await GetBranchScopedAsync(_studentAssignments, id, x => x.BranchId) is null) return NotFoundResult(id, "Student transport assignment");
        if (await ValidateStudentAssignmentAsync(request) is { } invalid) return invalid;
        await _studentAssignments.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("student-assignments/{id:guid}")]
    public Task<IActionResult> DeleteStudentAssignment(Guid id)
        => DeleteBranchScopedAsync(_studentAssignments, id, x => x.BranchId, "Student transport assignment");

    private async Task<IActionResult?> ValidateBranchAsync(Guid branchId)
    {
        if (await _branches.GetAsync(branchId) is null)
            return BadRequest(ApiResponse<object>.Fail("Selected branch was not found in this tenant."));
        return await CanUseBranchAsync(branchId) ? null : Forbid();
    }

    private async Task<IActionResult?> ValidateRouteChildAsync(Guid routeId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _routes.GetAsync(routeId) is not { } route || route.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected route was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateRouteAssignmentAsync(Guid branchId, Guid routeId, Guid vehicleId, Guid? driverId)
    {
        if (await ValidateRouteChildAsync(routeId, branchId) is { } invalid) return invalid;
        if (await _vehicles.GetAsync(vehicleId) is not { } vehicle || vehicle.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected vehicle was not found for the branch."));
        if (driverId is Guid checkedDriverId && (await _drivers.GetAsync(checkedDriverId) is not { } driver || driver.BranchId != branchId))
            return BadRequest(ApiResponse<object>.Fail("Selected driver was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateStudentAssignmentAsync(CreateStudentTransportAssignmentRequest request)
    {
        if (await ValidateRouteChildAsync(request.TransportRouteId, request.BranchId) is { } invalid) return invalid;
        if (await _students.GetAsync(request.StudentProfileId) is not { } student || student.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected student was not found for the branch."));
        if (request.TransportRouteStopId is Guid stopId && (await _stops.GetAsync(stopId) is not { } stop || stop.BranchId != request.BranchId || stop.TransportRouteId != request.TransportRouteId))
            return BadRequest(ApiResponse<object>.Fail("Selected route stop was not found for the route and branch."));
        if (request.VehicleId is Guid vehicleId && (await _vehicles.GetAsync(vehicleId) is not { } vehicle || vehicle.BranchId != request.BranchId))
            return BadRequest(ApiResponse<object>.Fail("Selected vehicle was not found for the branch."));
        return null;
    }

    private NotFoundObjectResult NotFoundResult(Guid id, string displayName)
        => NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

    private static Vehicle Apply(Vehicle entity, CreateVehicleRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.RegistrationNumber = request.RegistrationNumber;
        entity.DisplayName = request.DisplayName;
        entity.Type = request.Type;
        entity.SeatCapacity = request.SeatCapacity;
        entity.Status = request.Status;
        entity.InsuranceValidUntil = request.InsuranceValidUntil;
        entity.FitnessValidUntil = request.FitnessValidUntil;
        entity.GpsDeviceId = request.GpsDeviceId;
        entity.Notes = request.Notes;
        return entity;
    }

    private static TransportDriver Apply(TransportDriver entity, CreateTransportDriverRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.PhoneNumber = request.PhoneNumber;
        entity.LicenseNumber = request.LicenseNumber;
        entity.LicenseValidUntil = request.LicenseValidUntil;
        entity.Status = request.Status;
        entity.Address = request.Address;
        return entity;
    }

    private static TransportRoute Apply(TransportRoute entity, CreateTransportRouteRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.RouteCode = request.RouteCode;
        entity.Name = request.Name;
        entity.ShiftName = request.ShiftName;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.DistanceKm = request.DistanceKm;
        entity.IsActive = request.IsActive;
        entity.Notes = request.Notes;
        return entity;
    }

    private static TransportRouteStop Apply(TransportRouteStop entity, CreateTransportRouteStopRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.TransportRouteId = request.TransportRouteId;
        entity.StopName = request.StopName;
        entity.StopOrder = request.StopOrder;
        entity.PickupTime = request.PickupTime;
        entity.DropTime = request.DropTime;
        entity.MonthlyFee = request.MonthlyFee;
        entity.Landmark = request.Landmark;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        return entity;
    }

    private static TransportRouteAssignment Apply(TransportRouteAssignment entity, CreateTransportRouteAssignmentRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.TransportRouteId = request.TransportRouteId;
        entity.VehicleId = request.VehicleId;
        entity.DriverId = request.DriverId;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.IsActive = request.IsActive;
        entity.Notes = request.Notes;
        return entity;
    }

    private static StudentTransportAssignment Apply(StudentTransportAssignment entity, CreateStudentTransportAssignmentRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.TransportRouteId = request.TransportRouteId;
        entity.TransportRouteStopId = request.TransportRouteStopId;
        entity.VehicleId = request.VehicleId;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.MonthlyFee = request.MonthlyFee;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        return entity;
    }
}
