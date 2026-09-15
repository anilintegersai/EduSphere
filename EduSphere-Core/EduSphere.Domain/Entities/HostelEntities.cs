using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class HostelBlock : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(40)]
    public string Code { get; set; } = null!;

    public Gender Gender { get; set; } = Gender.NotSpecified;

    [StringLength(120)]
    public string? WardenName { get; set; }

    [StringLength(30)]
    public string? WardenPhone { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<HostelRoom> Rooms { get; set; } = new List<HostelRoom>();
}

public class HostelRoom : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid HostelBlockId { get; set; }
    public HostelBlock? HostelBlock { get; set; }

    [Required]
    [StringLength(40)]
    public string RoomNumber { get; set; } = null!;

    [StringLength(40)]
    public string? Floor { get; set; }

    public int Capacity { get; set; }
    public decimal MonthlyFee { get; set; }
    public HostelRoomStatus Status { get; set; } = HostelRoomStatus.Available;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<HostelBed> Beds { get; set; } = new List<HostelBed>();
}

public class HostelBed : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid HostelRoomId { get; set; }
    public HostelRoom? HostelRoom { get; set; }

    [Required]
    [StringLength(40)]
    public string BedNumber { get; set; } = null!;

    public HostelBedStatus Status { get; set; } = HostelBedStatus.Available;

    [StringLength(300)]
    public string? Notes { get; set; }
}

public class HostelAllocation : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid HostelRoomId { get; set; }
    public HostelRoom? HostelRoom { get; set; }

    public Guid? HostelBedId { get; set; }
    public HostelBed? HostelBed { get; set; }

    public DateOnly AllocatedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpectedCheckoutOn { get; set; }
    public DateOnly? CheckedOutOn { get; set; }
    public HostelAllocationStatus Status { get; set; } = HostelAllocationStatus.Active;
    public decimal MonthlyFee { get; set; }
    public decimal SecurityDeposit { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<HostelFee> Fees { get; set; } = new List<HostelFee>();
}

public class HostelFee : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid HostelAllocationId { get; set; }
    public HostelAllocation? HostelAllocation { get; set; }

    [Required]
    [StringLength(60)]
    public string InvoiceNumber { get; set; } = null!;

    public DateOnly BillingMonth { get; set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class HostelVisitorLog : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? HostelAllocationId { get; set; }
    public HostelAllocation? HostelAllocation { get; set; }

    public Guid? StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    [Required]
    [StringLength(120)]
    public string VisitorName { get; set; } = null!;

    [StringLength(80)]
    public string? Relationship { get; set; }

    [Required]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(250)]
    public string? Purpose { get; set; }

    public DateTime CheckInOn { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutOn { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }

    [StringLength(80)]
    public string? IdDocumentNumber { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class HostelMaintenanceRequest : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? HostelBlockId { get; set; }
    public HostelBlock? HostelBlock { get; set; }

    public Guid? HostelRoomId { get; set; }
    public HostelRoom? HostelRoom { get; set; }

    public Guid? HostelBedId { get; set; }
    public HostelBed? HostelBed { get; set; }

    public Guid? ReportedByUserId { get; set; }
    public ApplicationUser? ReportedByUser { get; set; }

    [Required]
    [StringLength(120)]
    public string Category { get; set; } = null!;

    public HostelMaintenancePriority Priority { get; set; } = HostelMaintenancePriority.Medium;
    public HostelMaintenanceStatus Status { get; set; } = HostelMaintenanceStatus.Open;
    public DateTime ReportedOn { get; set; } = DateTime.UtcNow;
    public DateTime? DueOn { get; set; }
    public DateTime? ResolvedOn { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = null!;

    [StringLength(1000)]
    public string? ResolutionNotes { get; set; }
}

public class HostelAllocationTransferRequest : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid HostelAllocationId { get; set; }
    public HostelAllocation? HostelAllocation { get; set; }

    public Guid FromRoomId { get; set; }
    public HostelRoom? FromRoom { get; set; }

    public Guid? FromBedId { get; set; }
    public HostelBed? FromBed { get; set; }

    public Guid ToRoomId { get; set; }
    public HostelRoom? ToRoom { get; set; }

    public Guid? ToBedId { get; set; }
    public HostelBed? ToBed { get; set; }

    public HostelTransferStatus Status { get; set; } = HostelTransferStatus.Requested;
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = null!;

    [StringLength(500)]
    public string? DecisionNotes { get; set; }
}
