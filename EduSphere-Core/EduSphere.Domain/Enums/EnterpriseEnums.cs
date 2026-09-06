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
