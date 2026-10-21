using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class LibraryCirculationPolicy : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public LibraryMemberType MemberType { get; set; }
    public int MaxIssues { get; set; } = 2;
    public int LoanDays { get; set; } = 14;
    public int MaxRenewals { get; set; } = 1;
    public int GraceDays { get; set; }
    public decimal FinePerDay { get; set; }
    public decimal MaximumFine { get; set; } = 1000;
    public decimal LostBookMultiplier { get; set; } = 1;
    public decimal DamageChargePercent { get; set; } = 25;
    public int ReservationHoldDays { get; set; } = 3;
    public bool IsActive { get; set; } = true;
}

public class LibraryItemIncident : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookCopyId { get; set; }
    public LibraryBookCopy? LibraryBookCopy { get; set; }
    public Guid LibraryMemberId { get; set; }
    public LibraryMember? LibraryMember { get; set; }
    public Guid? LibraryBookIssueId { get; set; }
    public LibraryBookIssue? LibraryBookIssue { get; set; }
    public LibraryItemCondition Condition { get; set; }
    [Required, StringLength(1000)] public string Details { get; set; } = null!;
    public decimal AssessedAmount { get; set; }
    public LibraryIncidentStatus Status { get; set; } = LibraryIncidentStatus.Reported;
    public Guid? FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }
    public DateTime ReportedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedOn { get; set; }
}

public class LibraryVendor : TenantEntityBase
{
    public Guid BranchId { get; set; }
    [Required, StringLength(160)] public string Name { get; set; } = null!;
    [StringLength(120)] public string? ContactPerson { get; set; }
    [StringLength(254)] public string? Email { get; set; }
    [StringLength(30)] public string? Phone { get; set; }
    [StringLength(500)] public string? Address { get; set; }
    [StringLength(40)] public string? TaxNumber { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LibraryPurchaseOrder : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid LibraryVendorId { get; set; }
    public LibraryVendor? LibraryVendor { get; set; }
    [Required, StringLength(60)] public string OrderNumber { get; set; } = null!;
    public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpectedOn { get; set; }
    public LibraryPurchaseOrderStatus Status { get; set; } = LibraryPurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
    public ICollection<LibraryPurchaseOrderLine> Lines { get; set; } = [];
}

public class LibraryPurchaseOrderLine : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid LibraryPurchaseOrderId { get; set; }
    public LibraryPurchaseOrder? LibraryPurchaseOrder { get; set; }
    public Guid? LibraryBookId { get; set; }
    public LibraryBook? LibraryBook { get; set; }
    [Required, StringLength(250)] public string Title { get; set; } = null!;
    [StringLength(30)] public string? Isbn { get; set; }
    public int Quantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class LibraryStockVerification : TenantEntityBase
{
    public Guid BranchId { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    public DateTime StartedOn { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedOn { get; set; }
    public LibraryStockVerificationStatus Status { get; set; } = LibraryStockVerificationStatus.Draft;
    public ICollection<LibraryStockVerificationItem> Items { get; set; } = [];
}

public class LibraryStockVerificationItem : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid LibraryStockVerificationId { get; set; }
    public LibraryStockVerification? LibraryStockVerification { get; set; }
    public Guid? LibraryBookCopyId { get; set; }
    public LibraryBookCopy? LibraryBookCopy { get; set; }
    [Required, StringLength(120)] public string ScanCode { get; set; } = null!;
    public LibraryStockResult Result { get; set; } = LibraryStockResult.Pending;
    public DateTime ScannedOn { get; set; } = DateTime.UtcNow;
    [StringLength(500)] public string? Notes { get; set; }
}

public class LibraryDigitalResource : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid? LibraryBookId { get; set; }
    public LibraryBook? LibraryBook { get; set; }
    [Required, StringLength(250)] public string Title { get; set; } = null!;
    public DigitalResourceType ResourceType { get; set; }
    [Required, StringLength(1000)] public string ResourceUrl { get; set; } = null!;
    [StringLength(250)] public string? LicenseName { get; set; }
    public DateOnly? LicenseExpiresOn { get; set; }
    public DigitalAccessLevel AccessLevel { get; set; } = DigitalAccessLevel.AllMembers;
    public bool IsActive { get; set; } = true;
}
