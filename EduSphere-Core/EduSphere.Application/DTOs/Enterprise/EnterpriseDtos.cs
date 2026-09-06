using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.Enterprise;

public abstract class EnterpriseTenantScopedDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

public class FeeStructureDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FeeFrequency Frequency { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateFeeStructureRequest
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FeeFrequency Frequency { get; set; } = FeeFrequency.Term;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateFeeStructureRequest : CreateFeeStructureRequest { }

public class FeeComponentDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid FeeStructureId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FeeComponentType Type { get; set; }
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int DueDaysFromStart { get; set; }
    public int SortOrder { get; set; }
    public string? LedgerCode { get; set; }
    public string? Notes { get; set; }
}

public class CreateFeeComponentRequest
{
    public Guid BranchId { get; set; }
    public Guid FeeStructureId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FeeComponentType Type { get; set; } = FeeComponentType.Tuition;
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int DueDaysFromStart { get; set; }
    public int SortOrder { get; set; }
    public string? LedgerCode { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFeeComponentRequest : CreateFeeComponentRequest { }

public class DiscountRuleDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public Guid? CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? EligibilityCriteria { get; set; }
}

public class CreateDiscountRuleRequest
{
    public Guid? BranchId { get; set; }
    public Guid? CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; } = DiscountType.Amount;
    public decimal Value { get; set; }
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? EligibilityCriteria { get; set; }
}

public class UpdateDiscountRuleRequest : CreateDiscountRuleRequest { }

public class ScholarshipDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateOnly AwardedOn { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public bool IsActive { get; set; }
    public string? SponsorName { get; set; }
    public string? Notes { get; set; }
}

public class CreateScholarshipRequest
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; } = DiscountType.Amount;
    public decimal Value { get; set; }
    public DateOnly AwardedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ValidUntil { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SponsorName { get; set; }
    public string? Notes { get; set; }
}

public class UpdateScholarshipRequest : CreateScholarshipRequest { }

public class StudentFeeAssignmentDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid FeeStructureId { get; set; }
    public Guid? DiscountRuleId { get; set; }
    public DateOnly AssignedOn { get; set; }
    public FeeAssignmentStatus Status { get; set; }
    public decimal CustomDiscountAmount { get; set; }
    public string? Notes { get; set; }
}

public class CreateStudentFeeAssignmentRequest
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid FeeStructureId { get; set; }
    public Guid? DiscountRuleId { get; set; }
    public DateOnly AssignedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public FeeAssignmentStatus Status { get; set; } = FeeAssignmentStatus.Active;
    public decimal CustomDiscountAmount { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStudentFeeAssignmentRequest : CreateStudentFeeAssignmentRequest { }

public class FeeInvoiceDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid StudentFeeAssignmentId { get; set; }
    public Guid StudentProfileId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FineAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
}

public class CreateFeeInvoiceRequest
{
    public Guid BranchId { get; set; }
    public Guid StudentFeeAssignmentId { get; set; }
    public Guid StudentProfileId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15));
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FineAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFeeInvoiceRequest : CreateFeeInvoiceRequest { }

public class FeeInvoiceLineDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid FeeInvoiceId { get; set; }
    public Guid? FeeComponentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public FeeComponentType ComponentType { get; set; }
    public decimal Amount { get; set; }
    public int SortOrder { get; set; }
}

public class CreateFeeInvoiceLineRequest
{
    public Guid BranchId { get; set; }
    public Guid FeeInvoiceId { get; set; }
    public Guid? FeeComponentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public FeeComponentType ComponentType { get; set; } = FeeComponentType.Other;
    public decimal Amount { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateFeeInvoiceLineRequest : CreateFeeInvoiceLineRequest { }

public class FeePaymentDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid FeeInvoiceId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaidOn { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode Mode { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionReference { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class CreateFeePaymentRequest
{
    public Guid BranchId { get; set; }
    public Guid FeeInvoiceId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaidOn { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMode Mode { get; set; } = PaymentMode.Cash;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public string? TransactionReference { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFeePaymentRequest : CreateFeePaymentRequest { }

public class FeeReceiptDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid FeePaymentId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime IssuedOn { get; set; }
    public string? PdfStoragePath { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class CreateFeeReceiptRequest
{
    public Guid BranchId { get; set; }
    public Guid FeePaymentId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
    public string? PdfStoragePath { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFeeReceiptRequest : CreateFeeReceiptRequest { }

public class VehicleDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public VehicleType Type { get; set; }
    public int SeatCapacity { get; set; }
    public VehicleStatus Status { get; set; }
    public DateOnly? InsuranceValidUntil { get; set; }
    public DateOnly? FitnessValidUntil { get; set; }
    public string? GpsDeviceId { get; set; }
    public string? Notes { get; set; }
}

public class CreateVehicleRequest
{
    public Guid BranchId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public VehicleType Type { get; set; } = VehicleType.Bus;
    public int SeatCapacity { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Active;
    public DateOnly? InsuranceValidUntil { get; set; }
    public DateOnly? FitnessValidUntil { get; set; }
    public string? GpsDeviceId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateVehicleRequest : CreateVehicleRequest { }

public class TransportDriverDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateOnly? LicenseValidUntil { get; set; }
    public DriverStatus Status { get; set; }
    public string? Address { get; set; }
}

public class CreateTransportDriverRequest
{
    public Guid BranchId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateOnly? LicenseValidUntil { get; set; }
    public DriverStatus Status { get; set; } = DriverStatus.Active;
    public string? Address { get; set; }
}

public class UpdateTransportDriverRequest : CreateTransportDriverRequest { }

public class TransportRouteDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string RouteCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShiftName { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
    public decimal DistanceKm { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateTransportRouteRequest
{
    public Guid BranchId { get; set; }
    public string RouteCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShiftName { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
    public decimal DistanceKm { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateTransportRouteRequest : CreateTransportRouteRequest { }

public class TransportRouteStopDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid TransportRouteId { get; set; }
    public string StopName { get; set; } = string.Empty;
    public int StopOrder { get; set; }
    public TimeOnly? PickupTime { get; set; }
    public TimeOnly? DropTime { get; set; }
    public decimal MonthlyFee { get; set; }
    public string? Landmark { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class CreateTransportRouteStopRequest
{
    public Guid BranchId { get; set; }
    public Guid TransportRouteId { get; set; }
    public string StopName { get; set; } = string.Empty;
    public int StopOrder { get; set; }
    public TimeOnly? PickupTime { get; set; }
    public TimeOnly? DropTime { get; set; }
    public decimal MonthlyFee { get; set; }
    public string? Landmark { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class UpdateTransportRouteStopRequest : CreateTransportRouteStopRequest { }

public class TransportRouteAssignmentDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid TransportRouteId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateTransportRouteAssignmentRequest
{
    public Guid BranchId { get; set; }
    public Guid TransportRouteId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateTransportRouteAssignmentRequest : CreateTransportRouteAssignmentRequest { }

public class StudentTransportAssignmentDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid TransportRouteId { get; set; }
    public Guid? TransportRouteStopId { get; set; }
    public Guid? VehicleId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyFee { get; set; }
    public TransportAssignmentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateStudentTransportAssignmentRequest
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid TransportRouteId { get; set; }
    public Guid? TransportRouteStopId { get; set; }
    public Guid? VehicleId { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyFee { get; set; }
    public TransportAssignmentStatus Status { get; set; } = TransportAssignmentStatus.Active;
    public string? Notes { get; set; }
}

public class UpdateStudentTransportAssignmentRequest : CreateStudentTransportAssignmentRequest { }

public class LibraryBookDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid? SubjectId { get; set; }
    public string? Isbn { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Language { get; set; }
    public string? Keywords { get; set; }
}

public class CreateLibraryBookRequest
{
    public Guid BranchId { get; set; }
    public Guid? SubjectId { get; set; }
    public string? Isbn { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Language { get; set; } = "English";
    public string? Keywords { get; set; }
}

public class UpdateLibraryBookRequest : CreateLibraryBookRequest { }

public class LibraryBookCopyDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookId { get; set; }
    public string AccessionNumber { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? ShelfLocation { get; set; }
    public BookCopyStatus Status { get; set; }
    public DateOnly? AcquisitionDate { get; set; }
    public decimal? Price { get; set; }
}

public class CreateLibraryBookCopyRequest
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookId { get; set; }
    public string AccessionNumber { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? ShelfLocation { get; set; }
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
    public DateOnly? AcquisitionDate { get; set; }
    public decimal? Price { get; set; }
}

public class UpdateLibraryBookCopyRequest : CreateLibraryBookCopyRequest { }

public class LibraryMemberDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid? StudentProfileId { get; set; }
    public Guid? TeacherProfileId { get; set; }
    public Guid? UserId { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public LibraryMemberType MemberType { get; set; }
    public DateOnly JoinedOn { get; set; }
    public DateOnly? ExpiresOn { get; set; }
    public int MaxBooksAllowed { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateLibraryMemberRequest
{
    public Guid BranchId { get; set; }
    public Guid? StudentProfileId { get; set; }
    public Guid? TeacherProfileId { get; set; }
    public Guid? UserId { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public LibraryMemberType MemberType { get; set; } = LibraryMemberType.Student;
    public DateOnly JoinedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpiresOn { get; set; }
    public int MaxBooksAllowed { get; set; } = 2;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateLibraryMemberRequest : CreateLibraryMemberRequest { }

public class LibraryBookIssueDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookCopyId { get; set; }
    public Guid LibraryMemberId { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnedOn { get; set; }
    public LibraryIssueStatus Status { get; set; }
    public decimal FineAmount { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public Guid? ReturnReceivedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class CreateLibraryBookIssueRequest
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookCopyId { get; set; }
    public Guid LibraryMemberId { get; set; }
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public DateOnly? ReturnedOn { get; set; }
    public LibraryIssueStatus Status { get; set; } = LibraryIssueStatus.Issued;
    public decimal FineAmount { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public Guid? ReturnReceivedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateLibraryBookIssueRequest : CreateLibraryBookIssueRequest { }

public class LibraryBookReturnDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookIssueId { get; set; }
    public DateTime ReturnedOn { get; set; }
    public decimal FineAssessed { get; set; }
    public decimal FinePaid { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public string? ConditionNotes { get; set; }
    public string? Notes { get; set; }
}

public class CreateLibraryBookReturnRequest
{
    public Guid BranchId { get; set; }
    public Guid LibraryBookIssueId { get; set; }
    public DateTime ReturnedOn { get; set; } = DateTime.UtcNow;
    public decimal FineAssessed { get; set; }
    public decimal FinePaid { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public string? ConditionNotes { get; set; }
    public string? Notes { get; set; }
}

public class UpdateLibraryBookReturnRequest : CreateLibraryBookReturnRequest { }

public class LibraryFineRecordDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid LibraryMemberId { get; set; }
    public Guid? LibraryBookIssueId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LibraryFineStatus Status { get; set; }
    public DateTime AssessedOn { get; set; }
    public DateTime? PaidOn { get; set; }
    public DateTime? WaivedOn { get; set; }
    public string? Notes { get; set; }
}

public class CreateLibraryFineRecordRequest
{
    public Guid BranchId { get; set; }
    public Guid LibraryMemberId { get; set; }
    public Guid? LibraryBookIssueId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LibraryFineStatus Status { get; set; } = LibraryFineStatus.Pending;
    public DateTime AssessedOn { get; set; } = DateTime.UtcNow;
    public DateTime? PaidOn { get; set; }
    public DateTime? WaivedOn { get; set; }
    public string? Notes { get; set; }
}

public class UpdateLibraryFineRecordRequest : CreateLibraryFineRecordRequest { }

public class HostelBlockDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string? WardenName { get; set; }
    public string? WardenPhone { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateHostelBlockRequest
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public string? WardenName { get; set; }
    public string? WardenPhone { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateHostelBlockRequest : CreateHostelBlockRequest { }

public class HostelRoomDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid HostelBlockId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public int Capacity { get; set; }
    public decimal MonthlyFee { get; set; }
    public HostelRoomStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateHostelRoomRequest
{
    public Guid BranchId { get; set; }
    public Guid HostelBlockId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public int Capacity { get; set; }
    public decimal MonthlyFee { get; set; }
    public HostelRoomStatus Status { get; set; } = HostelRoomStatus.Available;
    public string? Notes { get; set; }
}

public class UpdateHostelRoomRequest : CreateHostelRoomRequest { }

public class HostelBedDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid HostelRoomId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public HostelBedStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateHostelBedRequest
{
    public Guid BranchId { get; set; }
    public Guid HostelRoomId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public HostelBedStatus Status { get; set; } = HostelBedStatus.Available;
    public string? Notes { get; set; }
}

public class UpdateHostelBedRequest : CreateHostelBedRequest { }

public class HostelAllocationDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid HostelRoomId { get; set; }
    public Guid? HostelBedId { get; set; }
    public DateOnly AllocatedOn { get; set; }
    public DateOnly? ExpectedCheckoutOn { get; set; }
    public DateOnly? CheckedOutOn { get; set; }
    public HostelAllocationStatus Status { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal SecurityDeposit { get; set; }
    public string? Notes { get; set; }
}

public class CreateHostelAllocationRequest
{
    public Guid BranchId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid HostelRoomId { get; set; }
    public Guid? HostelBedId { get; set; }
    public DateOnly AllocatedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpectedCheckoutOn { get; set; }
    public DateOnly? CheckedOutOn { get; set; }
    public HostelAllocationStatus Status { get; set; } = HostelAllocationStatus.Active;
    public decimal MonthlyFee { get; set; }
    public decimal SecurityDeposit { get; set; }
    public string? Notes { get; set; }
}

public class UpdateHostelAllocationRequest : CreateHostelAllocationRequest { }

public class HostelFeeDto : EnterpriseTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid HostelAllocationId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly BillingMonth { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateHostelFeeRequest
{
    public Guid BranchId { get; set; }
    public Guid HostelAllocationId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly BillingMonth { get; set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
    public string? Notes { get; set; }
}

public class UpdateHostelFeeRequest : CreateHostelFeeRequest { }

public class NotificationProviderSettingDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public CommunicationChannel Channel { get; set; }
    public string ProviderKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? FromAddress { get; set; }
    public string? FromDisplayName { get; set; }
    public string? ConfigurationJson { get; set; }
    public string? SecretReference { get; set; }
    public bool IsEnabled { get; set; }
}

public class CreateNotificationProviderSettingRequest
{
    public Guid? BranchId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public string ProviderKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? FromAddress { get; set; }
    public string? FromDisplayName { get; set; }
    public string? ConfigurationJson { get; set; }
    public string? SecretReference { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public class UpdateNotificationProviderSettingRequest : CreateNotificationProviderSettingRequest { }

public class NotificationTemplateDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; }
    public string? SubjectTemplate { get; set; }
    public string BodyTemplate { get; set; } = string.Empty;
    public string? VariablesJson { get; set; }
    public bool IsActive { get; set; }
}

public class CreateNotificationTemplateRequest
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public string? SubjectTemplate { get; set; }
    public string BodyTemplate { get; set; } = string.Empty;
    public string? VariablesJson { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateNotificationTemplateRequest : CreateNotificationTemplateRequest { }

public class NotificationMessageDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public Guid? NotificationTemplateId { get; set; }
    public CommunicationChannel Channel { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public DateTime? SentOn { get; set; }
    public string? ProviderKey { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? CreatedForUserId { get; set; }
}

public class CreateNotificationMessageRequest
{
    public Guid? BranchId { get; set; }
    public Guid? NotificationTemplateId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Draft;
    public DateTime? ScheduledOn { get; set; }
    public DateTime? SentOn { get; set; }
    public string? ProviderKey { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? CreatedForUserId { get; set; }
}

public class UpdateNotificationMessageRequest : CreateNotificationMessageRequest { }

public class NotificationRecipientDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public Guid NotificationMessageId { get; set; }
    public Guid? UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime? SentOn { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CreateNotificationRecipientRequest
{
    public Guid? BranchId { get; set; }
    public Guid NotificationMessageId { get; set; }
    public Guid? UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public DateTime? SentOn { get; set; }
    public string? ErrorMessage { get; set; }
}

public class UpdateNotificationRecipientRequest : CreateNotificationRecipientRequest { }

public class AnnouncementDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public AnnouncementAudience Audience { get; set; }
    public DateTime PublishOn { get; set; }
    public DateTime? ExpireOn { get; set; }
    public bool IsPinned { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

public class CreateAnnouncementRequest
{
    public Guid? BranchId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public AnnouncementAudience Audience { get; set; } = AnnouncementAudience.AllUsers;
    public DateTime PublishOn { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireOn { get; set; }
    public bool IsPinned { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedByUserId { get; set; }
}

public class UpdateAnnouncementRequest : CreateAnnouncementRequest { }

public class CommunicationLogDto : EnterpriseTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public CommunicationChannel Channel { get; set; }
    public CommunicationDirection Direction { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public NotificationStatus Status { get; set; }
    public string? ProviderKey { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? PayloadSummary { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class CreateCommunicationLogRequest
{
    public Guid? BranchId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public CommunicationDirection Direction { get; set; } = CommunicationDirection.Outbound;
    public string Recipient { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public string? ProviderKey { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? PayloadSummary { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}

public class UpdateCommunicationLogRequest : CreateCommunicationLogRequest { }
