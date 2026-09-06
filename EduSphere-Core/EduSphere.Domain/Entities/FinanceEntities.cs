using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class FeeStructure : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? BatchId { get; set; }
    public Batch? Batch { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    public FeeFrequency Frequency { get; set; } = FeeFrequency.Term;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<FeeComponent> Components { get; set; } = new List<FeeComponent>();
    public ICollection<StudentFeeAssignment> Assignments { get; set; } = new List<StudentFeeAssignment>();
}

public class FeeComponent : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid FeeStructureId { get; set; }
    public FeeStructure? FeeStructure { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    public FeeComponentType Type { get; set; } = FeeComponentType.Tuition;
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int DueDaysFromStart { get; set; }
    public int SortOrder { get; set; }

    [StringLength(50)]
    public string? LedgerCode { get; set; }

    [StringLength(300)]
    public string? Notes { get; set; }
}

public class DiscountRule : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    public DiscountType Type { get; set; } = DiscountType.Amount;
    public decimal Value { get; set; }
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? EligibilityCriteria { get; set; }
}

public class Scholarship : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    public DiscountType Type { get; set; } = DiscountType.Amount;
    public decimal Value { get; set; }
    public DateOnly AwardedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ValidUntil { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(150)]
    public string? SponsorName { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class StudentFeeAssignment : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid FeeStructureId { get; set; }
    public FeeStructure? FeeStructure { get; set; }

    public Guid? DiscountRuleId { get; set; }
    public DiscountRule? DiscountRule { get; set; }

    public DateOnly AssignedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public FeeAssignmentStatus Status { get; set; } = FeeAssignmentStatus.Active;
    public decimal CustomDiscountAmount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<FeeInvoice> Invoices { get; set; } = new List<FeeInvoice>();
}

public class FeeInvoice : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid StudentFeeAssignmentId { get; set; }
    public StudentFeeAssignment? StudentFeeAssignment { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    [Required]
    [StringLength(60)]
    public string InvoiceNumber { get; set; } = null!;

    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15));
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FineAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<FeeInvoiceLine> Lines { get; set; } = new List<FeeInvoiceLine>();
    public ICollection<FeePayment> Payments { get; set; } = new List<FeePayment>();
}

public class FeeInvoiceLine : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }

    public Guid? FeeComponentId { get; set; }
    public FeeComponent? FeeComponent { get; set; }

    [Required]
    [StringLength(180)]
    public string Description { get; set; } = null!;

    public FeeComponentType ComponentType { get; set; } = FeeComponentType.Other;
    public decimal Amount { get; set; }
    public int SortOrder { get; set; }
}

public class FeePayment : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid FeeInvoiceId { get; set; }
    public FeeInvoice? FeeInvoice { get; set; }

    [Required]
    [StringLength(60)]
    public string PaymentNumber { get; set; } = null!;

    public DateTime PaidOn { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMode Mode { get; set; } = PaymentMode.Cash;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    [StringLength(120)]
    public string? TransactionReference { get; set; }

    public Guid? ReceivedByUserId { get; set; }
    public ApplicationUser? ReceivedByUser { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public FeeReceipt? Receipt { get; set; }
}

public class FeeReceipt : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid FeePaymentId { get; set; }
    public FeePayment? FeePayment { get; set; }

    [Required]
    [StringLength(60)]
    public string ReceiptNumber { get; set; } = null!;

    public DateTime IssuedOn { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? PdfStoragePath { get; set; }

    public Guid? IssuedByUserId { get; set; }
    public ApplicationUser? IssuedByUser { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
