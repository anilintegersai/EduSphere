using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Vehicle : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(30)]
    public string RegistrationNumber { get; set; } = null!;

    [StringLength(80)]
    public string? DisplayName { get; set; }

    public VehicleType Type { get; set; } = VehicleType.Bus;
    public int SeatCapacity { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Active;
    public DateOnly? InsuranceValidUntil { get; set; }
    public DateOnly? FitnessValidUntil { get; set; }

    [StringLength(80)]
    public string? GpsDeviceId { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class TransportDriver : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [StringLength(60)]
    public string LicenseNumber { get; set; } = null!;

    public DateOnly? LicenseValidUntil { get; set; }
    public DriverStatus Status { get; set; } = DriverStatus.Active;

    [StringLength(500)]
    public string? Address { get; set; }
}

public class TransportRoute : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(50)]
    public string RouteCode { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(80)]
    public string? ShiftName { get; set; }

    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
    public decimal DistanceKm { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<TransportRouteStop> Stops { get; set; } = new List<TransportRouteStop>();
}

public class TransportRouteStop : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid TransportRouteId { get; set; }
    public TransportRoute? TransportRoute { get; set; }

    [Required]
    [StringLength(150)]
    public string StopName { get; set; } = null!;

    public int StopOrder { get; set; }
    public TimeOnly? PickupTime { get; set; }
    public TimeOnly? DropTime { get; set; }
    public decimal MonthlyFee { get; set; }

    [StringLength(200)]
    public string? Landmark { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class TransportRouteAssignment : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid TransportRouteId { get; set; }
    public TransportRoute? TransportRoute { get; set; }

    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public Guid? DriverId { get; set; }
    public TransportDriver? Driver { get; set; }

    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class StudentTransportAssignment : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid TransportRouteId { get; set; }
    public TransportRoute? TransportRoute { get; set; }

    public Guid? TransportRouteStopId { get; set; }
    public TransportRouteStop? TransportRouteStop { get; set; }

    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyFee { get; set; }
    public TransportAssignmentStatus Status { get; set; } = TransportAssignmentStatus.Active;

    [StringLength(500)]
    public string? Notes { get; set; }
}
