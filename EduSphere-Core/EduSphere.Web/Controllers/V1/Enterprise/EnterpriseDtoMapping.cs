using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;

namespace EduSphere.Web.Controllers.V1.Enterprise;

internal static class EnterpriseDtoMapping
{
    public static T WithMetadata<T>(this T dto, TenantEntityBase entity)
        where T : EnterpriseTenantScopedDto
    {
        dto.Id = entity.Id;
        dto.TenantId = entity.TenantId;
        dto.IsDeleted = entity.IsDeleted;
        dto.CreatedBy = entity.CreatedBy;
        dto.CreatedOn = entity.CreatedOn;
        dto.ModifiedBy = entity.ModifiedBy;
        dto.ModifiedOn = entity.ModifiedOn;
        dto.DeletedBy = entity.DeletedBy;
        dto.DeletedOn = entity.DeletedOn;
        dto.ConcurrencyToken = entity.ConcurrencyToken;
        return dto;
    }

    public static FeeStructureDto Map(this FeeStructure e) => new FeeStructureDto
    {
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        CourseId = e.CourseId,
        BatchId = e.BatchId,
        Name = e.Name,
        Code = e.Code,
        Frequency = e.Frequency,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo,
        IsActive = e.IsActive,
        Notes = e.Notes
    }.WithMetadata(e);

    public static FeeComponentDto Map(this FeeComponent e) => new FeeComponentDto
    {
        BranchId = e.BranchId,
        FeeStructureId = e.FeeStructureId,
        Name = e.Name,
        Type = e.Type,
        Amount = e.Amount,
        IsOptional = e.IsOptional,
        DueDaysFromStart = e.DueDaysFromStart,
        SortOrder = e.SortOrder,
        LedgerCode = e.LedgerCode,
        Notes = e.Notes
    }.WithMetadata(e);

    public static DiscountRuleDto Map(this DiscountRule e) => new DiscountRuleDto
    {
        BranchId = e.BranchId,
        CourseId = e.CourseId,
        Name = e.Name,
        Code = e.Code,
        Type = e.Type,
        Value = e.Value,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo,
        IsActive = e.IsActive,
        EligibilityCriteria = e.EligibilityCriteria
    }.WithMetadata(e);

    public static ScholarshipDto Map(this Scholarship e) => new ScholarshipDto
    {
        BranchId = e.BranchId,
        StudentProfileId = e.StudentProfileId,
        Name = e.Name,
        Code = e.Code,
        Type = e.Type,
        Value = e.Value,
        AwardedOn = e.AwardedOn,
        ValidUntil = e.ValidUntil,
        IsActive = e.IsActive,
        SponsorName = e.SponsorName,
        Notes = e.Notes
    }.WithMetadata(e);

    public static StudentFeeAssignmentDto Map(this StudentFeeAssignment e) => new StudentFeeAssignmentDto
    {
        BranchId = e.BranchId,
        StudentProfileId = e.StudentProfileId,
        FeeStructureId = e.FeeStructureId,
        DiscountRuleId = e.DiscountRuleId,
        AssignedOn = e.AssignedOn,
        Status = e.Status,
        CustomDiscountAmount = e.CustomDiscountAmount,
        Notes = e.Notes
    }.WithMetadata(e);

    public static FeeInvoiceDto Map(this FeeInvoice e) => new FeeInvoiceDto
    {
        BranchId = e.BranchId,
        StudentFeeAssignmentId = e.StudentFeeAssignmentId,
        StudentProfileId = e.StudentProfileId,
        InvoiceNumber = e.InvoiceNumber,
        InvoiceDate = e.InvoiceDate,
        DueDate = e.DueDate,
        Status = e.Status,
        SubTotal = e.SubTotal,
        DiscountAmount = e.DiscountAmount,
        FineAmount = e.FineAmount,
        TotalAmount = e.TotalAmount,
        PaidAmount = e.PaidAmount,
        Notes = e.Notes
    }.WithMetadata(e);

    public static FeeInvoiceLineDto Map(this FeeInvoiceLine e) => new FeeInvoiceLineDto
    {
        BranchId = e.BranchId,
        FeeInvoiceId = e.FeeInvoiceId,
        FeeComponentId = e.FeeComponentId,
        Description = e.Description,
        ComponentType = e.ComponentType,
        Amount = e.Amount,
        SortOrder = e.SortOrder
    }.WithMetadata(e);

    public static FeePaymentDto Map(this FeePayment e) => new FeePaymentDto
    {
        BranchId = e.BranchId,
        FeeInvoiceId = e.FeeInvoiceId,
        PaymentNumber = e.PaymentNumber,
        PaidOn = e.PaidOn,
        Amount = e.Amount,
        Mode = e.Mode,
        Status = e.Status,
        TransactionReference = e.TransactionReference,
        ReceivedByUserId = e.ReceivedByUserId,
        Notes = e.Notes
    }.WithMetadata(e);

    public static FeeReceiptDto Map(this FeeReceipt e) => new FeeReceiptDto
    {
        BranchId = e.BranchId,
        FeePaymentId = e.FeePaymentId,
        ReceiptNumber = e.ReceiptNumber,
        IssuedOn = e.IssuedOn,
        PdfStoragePath = e.PdfStoragePath,
        IssuedByUserId = e.IssuedByUserId,
        Notes = e.Notes
    }.WithMetadata(e);

    public static VehicleDto Map(this Vehicle e) => new VehicleDto
    {
        BranchId = e.BranchId,
        RegistrationNumber = e.RegistrationNumber,
        DisplayName = e.DisplayName,
        Type = e.Type,
        SeatCapacity = e.SeatCapacity,
        Status = e.Status,
        InsuranceValidUntil = e.InsuranceValidUntil,
        FitnessValidUntil = e.FitnessValidUntil,
        GpsDeviceId = e.GpsDeviceId,
        Notes = e.Notes
    }.WithMetadata(e);

    public static TransportDriverDto Map(this TransportDriver e) => new TransportDriverDto
    {
        BranchId = e.BranchId,
        FirstName = e.FirstName,
        LastName = e.LastName,
        PhoneNumber = e.PhoneNumber,
        LicenseNumber = e.LicenseNumber,
        LicenseValidUntil = e.LicenseValidUntil,
        Status = e.Status,
        Address = e.Address
    }.WithMetadata(e);

    public static TransportRouteDto Map(this TransportRoute e) => new TransportRouteDto
    {
        BranchId = e.BranchId,
        RouteCode = e.RouteCode,
        Name = e.Name,
        ShiftName = e.ShiftName,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        DistanceKm = e.DistanceKm,
        IsActive = e.IsActive,
        Notes = e.Notes
    }.WithMetadata(e);

    public static TransportRouteStopDto Map(this TransportRouteStop e) => new TransportRouteStopDto
    {
        BranchId = e.BranchId,
        TransportRouteId = e.TransportRouteId,
        StopName = e.StopName,
        StopOrder = e.StopOrder,
        PickupTime = e.PickupTime,
        DropTime = e.DropTime,
        MonthlyFee = e.MonthlyFee,
        Landmark = e.Landmark,
        Latitude = e.Latitude,
        Longitude = e.Longitude
    }.WithMetadata(e);

    public static TransportRouteAssignmentDto Map(this TransportRouteAssignment e) => new TransportRouteAssignmentDto
    {
        BranchId = e.BranchId,
        TransportRouteId = e.TransportRouteId,
        VehicleId = e.VehicleId,
        DriverId = e.DriverId,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo,
        IsActive = e.IsActive,
        Notes = e.Notes
    }.WithMetadata(e);

    public static StudentTransportAssignmentDto Map(this StudentTransportAssignment e) => new StudentTransportAssignmentDto
    {
        BranchId = e.BranchId,
        StudentProfileId = e.StudentProfileId,
        TransportRouteId = e.TransportRouteId,
        TransportRouteStopId = e.TransportRouteStopId,
        VehicleId = e.VehicleId,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        MonthlyFee = e.MonthlyFee,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);

    public static LibraryBookDto Map(this LibraryBook e) => new LibraryBookDto
    {
        BranchId = e.BranchId,
        SubjectId = e.SubjectId,
        Isbn = e.Isbn,
        Title = e.Title,
        Authors = e.Authors,
        Publisher = e.Publisher,
        Category = e.Category,
        Edition = e.Edition,
        PublicationYear = e.PublicationYear,
        Language = e.Language,
        Keywords = e.Keywords
    }.WithMetadata(e);

    public static LibraryBookCopyDto Map(this LibraryBookCopy e) => new LibraryBookCopyDto
    {
        BranchId = e.BranchId,
        LibraryBookId = e.LibraryBookId,
        AccessionNumber = e.AccessionNumber,
        Barcode = e.Barcode,
        ShelfLocation = e.ShelfLocation,
        Status = e.Status,
        AcquisitionDate = e.AcquisitionDate,
        Price = e.Price
    }.WithMetadata(e);

    public static LibraryMemberDto Map(this LibraryMember e) => new LibraryMemberDto
    {
        BranchId = e.BranchId,
        StudentProfileId = e.StudentProfileId,
        TeacherProfileId = e.TeacherProfileId,
        UserId = e.UserId,
        MemberNumber = e.MemberNumber,
        MemberType = e.MemberType,
        JoinedOn = e.JoinedOn,
        ExpiresOn = e.ExpiresOn,
        MaxBooksAllowed = e.MaxBooksAllowed,
        IsActive = e.IsActive,
        Notes = e.Notes
    }.WithMetadata(e);

    public static LibraryBookIssueDto Map(this LibraryBookIssue e) => new LibraryBookIssueDto
    {
        BranchId = e.BranchId,
        LibraryBookCopyId = e.LibraryBookCopyId,
        LibraryMemberId = e.LibraryMemberId,
        IssueDate = e.IssueDate,
        DueDate = e.DueDate,
        ReturnedOn = e.ReturnedOn,
        Status = e.Status,
        FineAmount = e.FineAmount,
        IssuedByUserId = e.IssuedByUserId,
        ReturnReceivedByUserId = e.ReturnReceivedByUserId,
        Notes = e.Notes
    }.WithMetadata(e);

    public static LibraryBookReturnDto Map(this LibraryBookReturn e) => new LibraryBookReturnDto
    {
        BranchId = e.BranchId,
        LibraryBookIssueId = e.LibraryBookIssueId,
        ReturnedOn = e.ReturnedOn,
        FineAssessed = e.FineAssessed,
        FinePaid = e.FinePaid,
        ReceivedByUserId = e.ReceivedByUserId,
        ConditionNotes = e.ConditionNotes,
        Notes = e.Notes
    }.WithMetadata(e);

    public static LibraryFineRecordDto Map(this LibraryFineRecord e) => new LibraryFineRecordDto
    {
        BranchId = e.BranchId,
        LibraryMemberId = e.LibraryMemberId,
        LibraryBookIssueId = e.LibraryBookIssueId,
        Amount = e.Amount,
        Reason = e.Reason,
        Status = e.Status,
        AssessedOn = e.AssessedOn,
        PaidOn = e.PaidOn,
        WaivedOn = e.WaivedOn,
        Notes = e.Notes
    }.WithMetadata(e);

    public static HostelBlockDto Map(this HostelBlock e) => new HostelBlockDto
    {
        BranchId = e.BranchId,
        Name = e.Name,
        Code = e.Code,
        Gender = e.Gender,
        WardenName = e.WardenName,
        WardenPhone = e.WardenPhone,
        IsActive = e.IsActive,
        Notes = e.Notes
    }.WithMetadata(e);

    public static HostelRoomDto Map(this HostelRoom e) => new HostelRoomDto
    {
        BranchId = e.BranchId,
        HostelBlockId = e.HostelBlockId,
        RoomNumber = e.RoomNumber,
        Floor = e.Floor,
        Capacity = e.Capacity,
        MonthlyFee = e.MonthlyFee,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);

    public static HostelBedDto Map(this HostelBed e) => new HostelBedDto
    {
        BranchId = e.BranchId,
        HostelRoomId = e.HostelRoomId,
        BedNumber = e.BedNumber,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);

    public static HostelAllocationDto Map(this HostelAllocation e) => new HostelAllocationDto
    {
        BranchId = e.BranchId,
        StudentProfileId = e.StudentProfileId,
        HostelRoomId = e.HostelRoomId,
        HostelBedId = e.HostelBedId,
        AllocatedOn = e.AllocatedOn,
        ExpectedCheckoutOn = e.ExpectedCheckoutOn,
        CheckedOutOn = e.CheckedOutOn,
        Status = e.Status,
        MonthlyFee = e.MonthlyFee,
        SecurityDeposit = e.SecurityDeposit,
        Notes = e.Notes
    }.WithMetadata(e);

    public static HostelFeeDto Map(this HostelFee e) => new HostelFeeDto
    {
        BranchId = e.BranchId,
        HostelAllocationId = e.HostelAllocationId,
        InvoiceNumber = e.InvoiceNumber,
        BillingMonth = e.BillingMonth,
        DueDate = e.DueDate,
        Amount = e.Amount,
        PaidAmount = e.PaidAmount,
        Status = e.Status,
        Notes = e.Notes
    }.WithMetadata(e);

    public static NotificationProviderSettingDto Map(this NotificationProviderSetting e) => new NotificationProviderSettingDto
    {
        BranchId = e.BranchId,
        Channel = e.Channel,
        ProviderKey = e.ProviderKey,
        DisplayName = e.DisplayName,
        FromAddress = e.FromAddress,
        FromDisplayName = e.FromDisplayName,
        ConfigurationJson = e.ConfigurationJson,
        SecretReference = e.SecretReference,
        IsEnabled = e.IsEnabled
    }.WithMetadata(e);

    public static NotificationTemplateDto Map(this NotificationTemplate e) => new NotificationTemplateDto
    {
        BranchId = e.BranchId,
        Name = e.Name,
        Code = e.Code,
        Channel = e.Channel,
        SubjectTemplate = e.SubjectTemplate,
        BodyTemplate = e.BodyTemplate,
        VariablesJson = e.VariablesJson,
        IsActive = e.IsActive
    }.WithMetadata(e);

    public static NotificationMessageDto Map(this NotificationMessage e) => new NotificationMessageDto
    {
        BranchId = e.BranchId,
        NotificationTemplateId = e.NotificationTemplateId,
        Channel = e.Channel,
        Subject = e.Subject,
        Body = e.Body,
        Status = e.Status,
        ScheduledOn = e.ScheduledOn,
        SentOn = e.SentOn,
        ProviderKey = e.ProviderKey,
        ProviderMessageId = e.ProviderMessageId,
        ErrorMessage = e.ErrorMessage,
        CreatedForUserId = e.CreatedForUserId
    }.WithMetadata(e);

    public static NotificationRecipientDto Map(this NotificationRecipient e) => new NotificationRecipientDto
    {
        BranchId = e.BranchId,
        NotificationMessageId = e.NotificationMessageId,
        UserId = e.UserId,
        DisplayName = e.DisplayName,
        DestinationAddress = e.DestinationAddress,
        Status = e.Status,
        SentOn = e.SentOn,
        ErrorMessage = e.ErrorMessage
    }.WithMetadata(e);

    public static AnnouncementDto Map(this Announcement e) => new AnnouncementDto
    {
        BranchId = e.BranchId,
        Title = e.Title,
        Message = e.Message,
        Audience = e.Audience,
        PublishOn = e.PublishOn,
        ExpireOn = e.ExpireOn,
        IsPinned = e.IsPinned,
        IsActive = e.IsActive,
        CreatedByUserId = e.CreatedByUserId
    }.WithMetadata(e);

    public static CommunicationLogDto Map(this CommunicationLog e) => new CommunicationLogDto
    {
        BranchId = e.BranchId,
        Channel = e.Channel,
        Direction = e.Direction,
        Recipient = e.Recipient,
        Subject = e.Subject,
        Status = e.Status,
        ProviderKey = e.ProviderKey,
        ProviderMessageId = e.ProviderMessageId,
        PayloadSummary = e.PayloadSummary,
        ErrorMessage = e.ErrorMessage,
        OccurredOn = e.OccurredOn
    }.WithMetadata(e);
}
