using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class LibraryBook : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    [StringLength(30)]
    public string? Isbn { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string Authors { get; set; } = null!;

    [StringLength(150)]
    public string? Publisher { get; set; }

    [StringLength(120)]
    public string? Category { get; set; }

    [StringLength(80)]
    public string? Edition { get; set; }

    public int? PublicationYear { get; set; }

    [StringLength(50)]
    public string? Language { get; set; } = "English";

    [StringLength(500)]
    public string? Keywords { get; set; }

    public ICollection<LibraryBookCopy> Copies { get; set; } = new List<LibraryBookCopy>();
}

public class LibraryBookCopy : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookId { get; set; }
    public LibraryBook? LibraryBook { get; set; }

    [Required]
    [StringLength(60)]
    public string AccessionNumber { get; set; } = null!;

    [StringLength(80)]
    public string? Barcode { get; set; }

    [StringLength(120)]
    public string? ShelfLocation { get; set; }

    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
    public DateOnly? AcquisitionDate { get; set; }
    public decimal? Price { get; set; }
}

public class LibraryMember : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid? TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(60)]
    public string MemberNumber { get; set; } = null!;

    public LibraryMemberType MemberType { get; set; } = LibraryMemberType.Student;
    public DateOnly JoinedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpiresOn { get; set; }
    public int MaxBooksAllowed { get; set; } = 2;
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class LibraryBookIssue : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookCopyId { get; set; }
    public LibraryBookCopy? LibraryBookCopy { get; set; }

    public Guid LibraryMemberId { get; set; }
    public LibraryMember? LibraryMember { get; set; }

    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public DateOnly? ReturnedOn { get; set; }
    public LibraryIssueStatus Status { get; set; } = LibraryIssueStatus.Issued;
    public decimal FineAmount { get; set; }

    public Guid? IssuedByUserId { get; set; }
    public ApplicationUser? IssuedByUser { get; set; }

    public Guid? ReturnReceivedByUserId { get; set; }
    public ApplicationUser? ReturnReceivedByUser { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<LibraryBookReturn> Returns { get; set; } = new List<LibraryBookReturn>();
}

public class LibraryBookReturn : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookIssueId { get; set; }
    public LibraryBookIssue? LibraryBookIssue { get; set; }

    public DateTime ReturnedOn { get; set; } = DateTime.UtcNow;
    public decimal FineAssessed { get; set; }
    public decimal FinePaid { get; set; }

    public Guid? ReceivedByUserId { get; set; }
    public ApplicationUser? ReceivedByUser { get; set; }

    [StringLength(500)]
    public string? ConditionNotes { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class LibraryFineRecord : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryMemberId { get; set; }
    public LibraryMember? LibraryMember { get; set; }

    public Guid? LibraryBookIssueId { get; set; }
    public LibraryBookIssue? LibraryBookIssue { get; set; }

    public decimal Amount { get; set; }

    [Required]
    [StringLength(250)]
    public string Reason { get; set; } = null!;

    public LibraryFineStatus Status { get; set; } = LibraryFineStatus.Pending;
    public DateTime AssessedOn { get; set; } = DateTime.UtcNow;
    public DateTime? PaidOn { get; set; }
    public DateTime? WaivedOn { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class LibraryReservation : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookId { get; set; }
    public LibraryBook? LibraryBook { get; set; }

    public Guid? LibraryBookCopyId { get; set; }
    public LibraryBookCopy? LibraryBookCopy { get; set; }

    public Guid LibraryMemberId { get; set; }
    public LibraryMember? LibraryMember { get; set; }

    public DateTime ReservedOn { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.AddDays(3);
    public LibraryReservationStatus Status { get; set; } = LibraryReservationStatus.Requested;
    public DateTime? FulfilledOn { get; set; }
    public DateTime? CancelledOn { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class LibraryRenewal : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookIssueId { get; set; }
    public LibraryBookIssue? LibraryBookIssue { get; set; }

    public DateTime RenewedOn { get; set; } = DateTime.UtcNow;
    public DateOnly PreviousDueDate { get; set; }
    public DateOnly NewDueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public Guid? RenewedByUserId { get; set; }
    public ApplicationUser? RenewedByUser { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class LibraryOverdueNotification : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid LibraryBookIssueId { get; set; }
    public LibraryBookIssue? LibraryBookIssue { get; set; }

    public Guid LibraryMemberId { get; set; }
    public LibraryMember? LibraryMember { get; set; }

    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    [Required]
    [StringLength(250)]
    public string Recipient { get; set; } = null!;

    public DateTime? NotifiedOn { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public int AttemptCount { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }
}

public class LibraryBarcodeScan : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(120)]
    public string ScanCode { get; set; } = null!;

    public LibraryScanPurpose Purpose { get; set; } = LibraryScanPurpose.Lookup;
    public Guid? LibraryBookCopyId { get; set; }
    public LibraryBookCopy? LibraryBookCopy { get; set; }
    public DateTime ScannedOn { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? ResultMessage { get; set; }
}
