namespace EduSphere.Domain.Enums;

public enum FeeComponentType
{
    Tuition = 1,
    Admission = 2,
    Examination = 3,
    Transport = 4,
    Hostel = 5,
    Library = 6,
    Laboratory = 7,
    Activity = 8,
    Other = 99
}

public enum FeeFrequency
{
    OneTime = 1,
    Monthly = 2,
    Quarterly = 3,
    Term = 4,
    Annual = 5
}

public enum DiscountType
{
    Amount = 1,
    Percentage = 2
}

public enum FeeAssignmentStatus
{
    Draft = 1,
    Active = 2,
    Closed = 3,
    Cancelled = 4
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6
}

public enum PaymentMode
{
    Cash = 1,
    Cheque = 2,
    BankTransfer = 3,
    Card = 4,
    Upi = 5,
    OnlineGateway = 6,
    Wallet = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum PaymentGatewayStatus
{
    Initiated = 1,
    Pending = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum FeeReminderStatus
{
    Scheduled = 1,
    Queued = 2,
    Sent = 3,
    Failed = 4,
    Cancelled = 5
}

public enum FinanceApprovalStatus
{
    Requested = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Processed = 5,
    Cancelled = 6
}

public enum LedgerExportType
{
    Receivables = 1,
    Payments = 2,
    Refunds = 3,
    Concessions = 4,
    TrialBalance = 5,
    GeneralLedger = 6,
    Tally = 7,
    QuickBooks = 8
}

public enum LedgerExportFormat
{
    Csv = 1,
    Excel = 2,
    Json = 3
}

public enum LedgerExportStatus
{
    Requested = 1,
    Generated = 2,
    Failed = 3
}

public enum VehicleType
{
    Bus = 1,
    MiniBus = 2,
    Van = 3,
    Car = 4,
    Other = 99
}

public enum VehicleStatus
{
    Active = 1,
    Maintenance = 2,
    Retired = 3
}

public enum DriverStatus
{
    Active = 1,
    OnLeave = 2,
    Inactive = 3
}

public enum TransportAssignmentStatus
{
    Active = 1,
    Paused = 2,
    Ended = 3,
    Cancelled = 4
}

public enum TransportMaintenanceCategory
{
    Service = 1,
    Repair = 2,
    Inspection = 3,
    Fuel = 4,
    Cleaning = 5,
    Other = 99
}

public enum TransportReminderType
{
    Insurance = 1,
    Fitness = 2,
    Permit = 3,
    PollutionCertificate = 4,
    DriverLicense = 5,
    Service = 6
}

public enum OperationalReminderStatus
{
    Scheduled = 1,
    DueSoon = 2,
    Sent = 3,
    Completed = 4,
    Overdue = 5,
    Cancelled = 6
}

public enum LibraryMemberType
{
    Student = 1,
    Teacher = 2,
    Staff = 3,
    External = 4
}

public enum BookCopyStatus
{
    Available = 1,
    Issued = 2,
    Reserved = 3,
    Lost = 4,
    Damaged = 5,
    Retired = 6
}

public enum LibraryIssueStatus
{
    Issued = 1,
    Returned = 2,
    Overdue = 3,
    Lost = 4
}

public enum LibraryFineStatus
{
    Pending = 1,
    Paid = 2,
    Waived = 3,
    Cancelled = 4
}

public enum LibraryReservationStatus
{
    Requested = 1,
    Active = 2,
    Fulfilled = 3,
    Expired = 4,
    Cancelled = 5
}

public enum LibraryScanPurpose
{
    Lookup = 1,
    Issue = 2,
    Return = 3,
    Inventory = 4,
    Reservation = 5
}

public enum LibraryItemCondition { Good = 1, Worn = 2, Damaged = 3, Lost = 4 }
public enum LibraryIncidentStatus { Reported = 1, Assessed = 2, Invoiced = 3, Settled = 4, Waived = 5 }
public enum LibraryPurchaseOrderStatus { Draft = 1, Submitted = 2, Approved = 3, PartiallyReceived = 4, Received = 5, Cancelled = 6 }
public enum LibraryStockVerificationStatus { Draft = 1, InProgress = 2, Completed = 3, Reconciled = 4 }
public enum LibraryStockResult { Pending = 1, Found = 2, Missing = 3, Damaged = 4, Unexpected = 5 }
public enum DigitalResourceType { EBook = 1, Journal = 2, Video = 3, Database = 4, Document = 5, Link = 6 }
public enum DigitalAccessLevel { AllMembers = 1, Students = 2, Teachers = 3, Staff = 4 }

public enum HostelRoomStatus
{
    Available = 1,
    Full = 2,
    Maintenance = 3,
    Closed = 4
}

public enum HostelBedStatus
{
    Available = 1,
    Occupied = 2,
    Reserved = 3,
    Maintenance = 4
}

public enum HostelAllocationStatus
{
    Active = 1,
    CheckedOut = 2,
    Cancelled = 3
}

public enum HostelMaintenancePriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum HostelMaintenanceStatus
{
    Open = 1,
    Assigned = 2,
    InProgress = 3,
    Resolved = 4,
    Closed = 5,
    Cancelled = 6
}

public enum HostelTransferStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    Completed = 4,
    Cancelled = 5
}

public enum HostelMealType { Breakfast=1, Lunch=2, Snacks=3, Dinner=4 }
public enum MessAttendanceStatus { Present=1, Absent=2, Leave=3, Guest=4 }
public enum HostelVisitorStatus { Requested=1, Approved=2, Rejected=3, CheckedIn=4, CheckedOut=5 }
public enum GatePassStatus { Requested=1, WardenApproved=2, ParentApproved=3, Rejected=4, CheckedOut=5, Returned=6, Overdue=7 }
public enum HostelInspectionStatus { Scheduled=1, Completed=2, FollowUpRequired=3, Closed=4 }
public enum HostelClearanceStatus { Requested=1, InProgress=2, Cleared=3, Blocked=4, Completed=5 }
public enum NoDueArea { HostelFee=1, Mess=2, RoomDamage=3, Keys=4, Inventory=5, Library=6, Finance=7 }

public enum CommunicationChannel
{
    Email = 1,
    WhatsApp = 2,
    Telegram = 3,
    Sms = 4,
    Push = 5,
    InApp = 6
}

public enum NotificationStatus
{
    Draft = 1,
    Queued = 2,
    Sent = 3,
    Failed = 4,
    Cancelled = 5
}

public enum AnnouncementAudience
{
    AllUsers = 1,
    Branch = 2,
    Teachers = 3,
    Students = 4,
    Parents = 5,
    Staff = 6
}

public enum CommunicationDirection
{
    Outbound = 1,
    Inbound = 2
}

public enum NotificationAttemptStatus
{
    Pending = 1,
    Succeeded = 2,
    Failed = 3,
    Skipped = 4
}
