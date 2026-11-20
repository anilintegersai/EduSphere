using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class TransportGpsProviderSetting : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    public GpsProviderType ProviderType { get; set; } = GpsProviderType.GenericWebhook;
    [StringLength(500)] public string? EndpointUrl { get; set; }
    [StringLength(2000)] public string? ProtectedApiKey { get; set; }
    [Required, StringLength(80)] public string WebhookTokenHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime? LastSuccessfulSyncOn { get; set; }
    [StringLength(1000)] public string? LastError { get; set; }
}

public class TransportCrewProfile : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public TransportCrewRole Role { get; set; }
    [Required, StringLength(160)] public string FullName { get; set; } = null!;
    [Required, StringLength(30)] public string PhoneNumber { get; set; } = null!;
    [StringLength(60)] public string? EmployeeNumber { get; set; }
    [StringLength(60)] public string? LicenseNumber { get; set; }
    public DateOnly? LicenseValidUntil { get; set; }
    [StringLength(80)] public string? RfidTag { get; set; }
    public bool IsActive { get; set; } = true;
}

public class VehicleDocument : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public VehicleDocumentType DocumentType { get; set; }
    [Required, StringLength(200)] public string FileName { get; set; } = null!;
    [Required, StringLength(500)] public string StoragePath { get; set; } = null!;
    [StringLength(120)] public string? ContentType { get; set; }
    [StringLength(100)] public string? DocumentNumber { get; set; }
    public DateOnly? IssuedOn { get; set; }
    public DateOnly? ExpiresOn { get; set; }
    public VehicleDocumentStatus Status { get; set; } = VehicleDocumentStatus.Valid;
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
}

public class TransportGeofence : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid? TransportRouteId { get; set; }
    public Guid? TransportRouteStopId { get; set; }
    [Required, StringLength(160)] public string Name { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int RadiusMeters { get; set; } = 250;
    public int ArrivalLeadMinutes { get; set; } = 5;
    public bool NotifyParents { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

public class TransportGeofenceEvent : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid TransportGeofenceId { get; set; }
    public TransportGeofence? TransportGeofence { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Guid VehicleGpsPingId { get; set; }
    public GeofenceEventType EventType { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public bool NotificationQueued { get; set; }
}

public class VehicleFuelLog : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateOnly FueledOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal Litres { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalCost { get; set; }
    public int OdometerReading { get; set; }
    [StringLength(120)] public string? Vendor { get; set; }
    [StringLength(80)] public string? ReceiptNumber { get; set; }
}

public class TransportAttendanceScan : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Guid? TransportRouteId { get; set; }
    public TransportAttendanceEvent Event { get; set; }
    public TransportScanMethod ScanMethod { get; set; }
    [Required, StringLength(160)] public string Token { get; set; } = null!;
    public DateTime ScannedOn { get; set; } = DateTime.UtcNow;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class TransportFeeCharge : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid StudentTransportAssignmentId { get; set; }
    public StudentTransportAssignment? StudentTransportAssignment { get; set; }
    public Guid? FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal Amount { get; set; }
    [StringLength(80)] public string? InvoiceNumber { get; set; }
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
}

public class RouteOptimizationRun : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid TransportRouteId { get; set; }
    public TransportRoute? TransportRoute { get; set; }
    public RouteOptimizationStatus Status { get; set; } = RouteOptimizationStatus.Requested;
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedOn { get; set; }
    public decimal? OriginalDistanceKm { get; set; }
    public decimal? OptimizedDistanceKm { get; set; }
    [StringLength(8000)] public string? ProposedStopOrderJson { get; set; }
    [StringLength(1000)] public string? ErrorMessage { get; set; }
}
