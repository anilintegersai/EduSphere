using EduSphere.Application.DTOs.Enterprise;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateFeeStructureRequestValidator : AbstractValidator<CreateFeeStructureRequest>
{
    public CreateFeeStructureRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Frequency).IsInEnum();
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom).When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateFeeStructureRequestValidator : AbstractValidator<UpdateFeeStructureRequest>
{
    public UpdateFeeStructureRequestValidator() => Include(new CreateFeeStructureRequestValidator());
}

public class CreateFeeComponentRequestValidator : AbstractValidator<CreateFeeComponentRequest>
{
    public CreateFeeComponentRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FeeStructureId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDaysFromStart).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LedgerCode).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(300);
    }
}

public class UpdateFeeComponentRequestValidator : AbstractValidator<UpdateFeeComponentRequest>
{
    public UpdateFeeComponentRequestValidator() => Include(new CreateFeeComponentRequestValidator());
}

public class CreateDiscountRuleRequestValidator : AbstractValidator<CreateDiscountRuleRequest>
{
    public CreateDiscountRuleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom).When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.EligibilityCriteria).MaximumLength(500);
    }
}

public class UpdateDiscountRuleRequestValidator : AbstractValidator<UpdateDiscountRuleRequest>
{
    public UpdateDiscountRuleRequestValidator() => Include(new CreateDiscountRuleRequestValidator());
}

public class CreateScholarshipRequestValidator : AbstractValidator<CreateScholarshipRequest>
{
    public CreateScholarshipRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ValidUntil).GreaterThanOrEqualTo(x => x.AwardedOn).When(x => x.ValidUntil.HasValue);
        RuleFor(x => x.SponsorName).MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateScholarshipRequestValidator : AbstractValidator<UpdateScholarshipRequest>
{
    public UpdateScholarshipRequestValidator() => Include(new CreateScholarshipRequestValidator());
}

public class CreateStudentFeeAssignmentRequestValidator : AbstractValidator<CreateStudentFeeAssignmentRequest>
{
    public CreateStudentFeeAssignmentRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.FeeStructureId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CustomDiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateStudentFeeAssignmentRequestValidator : AbstractValidator<UpdateStudentFeeAssignmentRequest>
{
    public UpdateStudentFeeAssignmentRequestValidator() => Include(new CreateStudentFeeAssignmentRequestValidator());
}

public class CreateFeeInvoiceRequestValidator : AbstractValidator<CreateFeeInvoiceRequest>
{
    public CreateFeeInvoiceRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.StudentFeeAssignmentId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.InvoiceDate);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FineAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateFeeInvoiceRequestValidator : AbstractValidator<UpdateFeeInvoiceRequest>
{
    public UpdateFeeInvoiceRequestValidator() => Include(new CreateFeeInvoiceRequestValidator());
}

public class CreateFeeInvoiceLineRequestValidator : AbstractValidator<CreateFeeInvoiceLineRequest>
{
    public CreateFeeInvoiceLineRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FeeInvoiceId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(180);
        RuleFor(x => x.ComponentType).IsInEnum();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public class UpdateFeeInvoiceLineRequestValidator : AbstractValidator<UpdateFeeInvoiceLineRequest>
{
    public UpdateFeeInvoiceLineRequestValidator() => Include(new CreateFeeInvoiceLineRequestValidator());
}

public class CreateFeePaymentRequestValidator : AbstractValidator<CreateFeePaymentRequest>
{
    public CreateFeePaymentRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FeeInvoiceId).NotEmpty();
        RuleFor(x => x.PaymentNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.TransactionReference).MaximumLength(120);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateFeePaymentRequestValidator : AbstractValidator<UpdateFeePaymentRequest>
{
    public UpdateFeePaymentRequestValidator() => Include(new CreateFeePaymentRequestValidator());
}

public class CreateFeeReceiptRequestValidator : AbstractValidator<CreateFeeReceiptRequest>
{
    public CreateFeeReceiptRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FeePaymentId).NotEmpty();
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.PdfStoragePath).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateFeeReceiptRequestValidator : AbstractValidator<UpdateFeeReceiptRequest>
{
    public UpdateFeeReceiptRequestValidator() => Include(new CreateFeeReceiptRequestValidator());
}

public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.DisplayName).MaximumLength(80);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.SeatCapacity).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.GpsDeviceId).MaximumLength(80);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateVehicleRequestValidator : AbstractValidator<UpdateVehicleRequest>
{
    public UpdateVehicleRequestValidator() => Include(new CreateVehicleRequestValidator());
}

public class CreateTransportDriverRequestValidator : AbstractValidator<CreateTransportDriverRequest>
{
    public CreateTransportDriverRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Address).MaximumLength(500);
    }
}

public class UpdateTransportDriverRequestValidator : AbstractValidator<UpdateTransportDriverRequest>
{
    public UpdateTransportDriverRequestValidator() => Include(new CreateTransportDriverRequestValidator());
}

public class CreateTransportRouteRequestValidator : AbstractValidator<CreateTransportRouteRequest>
{
    public CreateTransportRouteRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.RouteCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ShiftName).MaximumLength(80);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt).When(x => x.StartsAt.HasValue && x.EndsAt.HasValue);
        RuleFor(x => x.DistanceKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateTransportRouteRequestValidator : AbstractValidator<UpdateTransportRouteRequest>
{
    public UpdateTransportRouteRequestValidator() => Include(new CreateTransportRouteRequestValidator());
}

public class CreateTransportRouteStopRequestValidator : AbstractValidator<CreateTransportRouteStopRequest>
{
    public CreateTransportRouteStopRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.TransportRouteId).NotEmpty();
        RuleFor(x => x.StopName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StopOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Landmark).MaximumLength(200);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
    }
}

public class UpdateTransportRouteStopRequestValidator : AbstractValidator<UpdateTransportRouteStopRequest>
{
    public UpdateTransportRouteStopRequestValidator() => Include(new CreateTransportRouteStopRequestValidator());
}

public class CreateTransportRouteAssignmentRequestValidator : AbstractValidator<CreateTransportRouteAssignmentRequest>
{
    public CreateTransportRouteAssignmentRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.TransportRouteId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom).When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateTransportRouteAssignmentRequestValidator : AbstractValidator<UpdateTransportRouteAssignmentRequest>
{
    public UpdateTransportRouteAssignmentRequestValidator() => Include(new CreateTransportRouteAssignmentRequestValidator());
}

public class CreateStudentTransportAssignmentRequestValidator : AbstractValidator<CreateStudentTransportAssignmentRequest>
{
    public CreateStudentTransportAssignmentRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.TransportRouteId).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate.HasValue);
        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateStudentTransportAssignmentRequestValidator : AbstractValidator<UpdateStudentTransportAssignmentRequest>
{
    public UpdateStudentTransportAssignmentRequestValidator() => Include(new CreateStudentTransportAssignmentRequestValidator());
}

public class CreateLibraryBookRequestValidator : AbstractValidator<CreateLibraryBookRequest>
{
    public CreateLibraryBookRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Isbn).MaximumLength(30);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Authors).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Publisher).MaximumLength(150);
        RuleFor(x => x.Category).MaximumLength(120);
        RuleFor(x => x.Edition).MaximumLength(80);
        RuleFor(x => x.PublicationYear).InclusiveBetween(1200, DateTime.UtcNow.Year + 1).When(x => x.PublicationYear.HasValue);
        RuleFor(x => x.Language).MaximumLength(50);
        RuleFor(x => x.Keywords).MaximumLength(500);
    }
}

public class UpdateLibraryBookRequestValidator : AbstractValidator<UpdateLibraryBookRequest>
{
    public UpdateLibraryBookRequestValidator() => Include(new CreateLibraryBookRequestValidator());
}

public class CreateLibraryBookCopyRequestValidator : AbstractValidator<CreateLibraryBookCopyRequest>
{
    public CreateLibraryBookCopyRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.LibraryBookId).NotEmpty();
        RuleFor(x => x.AccessionNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Barcode).MaximumLength(80);
        RuleFor(x => x.ShelfLocation).MaximumLength(120);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
    }
}

public class UpdateLibraryBookCopyRequestValidator : AbstractValidator<UpdateLibraryBookCopyRequest>
{
    public UpdateLibraryBookCopyRequestValidator() => Include(new CreateLibraryBookCopyRequestValidator());
}

public class CreateLibraryMemberRequestValidator : AbstractValidator<CreateLibraryMemberRequest>
{
    public CreateLibraryMemberRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.MemberNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.MemberType).IsInEnum();
        RuleFor(x => x.ExpiresOn).GreaterThanOrEqualTo(x => x.JoinedOn).When(x => x.ExpiresOn.HasValue);
        RuleFor(x => x.MaxBooksAllowed).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateLibraryMemberRequestValidator : AbstractValidator<UpdateLibraryMemberRequest>
{
    public UpdateLibraryMemberRequestValidator() => Include(new CreateLibraryMemberRequestValidator());
}

public class CreateLibraryBookIssueRequestValidator : AbstractValidator<CreateLibraryBookIssueRequest>
{
    public CreateLibraryBookIssueRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.LibraryBookCopyId).NotEmpty();
        RuleFor(x => x.LibraryMemberId).NotEmpty();
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.IssueDate);
        RuleFor(x => x.ReturnedOn).GreaterThanOrEqualTo(x => x.IssueDate).When(x => x.ReturnedOn.HasValue);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.FineAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateLibraryBookIssueRequestValidator : AbstractValidator<UpdateLibraryBookIssueRequest>
{
    public UpdateLibraryBookIssueRequestValidator() => Include(new CreateLibraryBookIssueRequestValidator());
}

public class CreateLibraryBookReturnRequestValidator : AbstractValidator<CreateLibraryBookReturnRequest>
{
    public CreateLibraryBookReturnRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.LibraryBookIssueId).NotEmpty();
        RuleFor(x => x.FineAssessed).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FinePaid).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ConditionNotes).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateLibraryBookReturnRequestValidator : AbstractValidator<UpdateLibraryBookReturnRequest>
{
    public UpdateLibraryBookReturnRequestValidator() => Include(new CreateLibraryBookReturnRequestValidator());
}

public class CreateLibraryFineRecordRequestValidator : AbstractValidator<CreateLibraryFineRecordRequest>
{
    public CreateLibraryFineRecordRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.LibraryMemberId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateLibraryFineRecordRequestValidator : AbstractValidator<UpdateLibraryFineRecordRequest>
{
    public UpdateLibraryFineRecordRequestValidator() => Include(new CreateLibraryFineRecordRequestValidator());
}

public class CreateHostelBlockRequestValidator : AbstractValidator<CreateHostelBlockRequest>
{
    public CreateHostelBlockRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.WardenName).MaximumLength(120);
        RuleFor(x => x.WardenPhone).MaximumLength(30);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateHostelBlockRequestValidator : AbstractValidator<UpdateHostelBlockRequest>
{
    public UpdateHostelBlockRequestValidator() => Include(new CreateHostelBlockRequestValidator());
}

public class CreateHostelRoomRequestValidator : AbstractValidator<CreateHostelRoomRequest>
{
    public CreateHostelRoomRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.HostelBlockId).NotEmpty();
        RuleFor(x => x.RoomNumber).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Floor).MaximumLength(40);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateHostelRoomRequestValidator : AbstractValidator<UpdateHostelRoomRequest>
{
    public UpdateHostelRoomRequestValidator() => Include(new CreateHostelRoomRequestValidator());
}

public class CreateHostelBedRequestValidator : AbstractValidator<CreateHostelBedRequest>
{
    public CreateHostelBedRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.HostelRoomId).NotEmpty();
        RuleFor(x => x.BedNumber).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(300);
    }
}

public class UpdateHostelBedRequestValidator : AbstractValidator<UpdateHostelBedRequest>
{
    public UpdateHostelBedRequestValidator() => Include(new CreateHostelBedRequestValidator());
}

public class CreateHostelAllocationRequestValidator : AbstractValidator<CreateHostelAllocationRequest>
{
    public CreateHostelAllocationRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.HostelRoomId).NotEmpty();
        RuleFor(x => x.ExpectedCheckoutOn).GreaterThanOrEqualTo(x => x.AllocatedOn).When(x => x.ExpectedCheckoutOn.HasValue);
        RuleFor(x => x.CheckedOutOn).GreaterThanOrEqualTo(x => x.AllocatedOn).When(x => x.CheckedOutOn.HasValue);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SecurityDeposit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateHostelAllocationRequestValidator : AbstractValidator<UpdateHostelAllocationRequest>
{
    public UpdateHostelAllocationRequestValidator() => Include(new CreateHostelAllocationRequestValidator());
}

public class CreateHostelFeeRequestValidator : AbstractValidator<CreateHostelFeeRequest>
{
    public CreateHostelFeeRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.HostelAllocationId).NotEmpty();
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateHostelFeeRequestValidator : AbstractValidator<UpdateHostelFeeRequest>
{
    public UpdateHostelFeeRequestValidator() => Include(new CreateHostelFeeRequestValidator());
}

public class CreateNotificationProviderSettingRequestValidator : AbstractValidator<CreateNotificationProviderSettingRequest>
{
    public CreateNotificationProviderSettingRequestValidator()
    {
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.ProviderKey).NotEmpty().MaximumLength(80);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.FromAddress).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.FromAddress)).MaximumLength(150);
        RuleFor(x => x.FromDisplayName).MaximumLength(150);
        RuleFor(x => x.ConfigurationJson).MaximumLength(2000);
        RuleFor(x => x.SecretReference).MaximumLength(200);
    }
}

public class UpdateNotificationProviderSettingRequestValidator : AbstractValidator<UpdateNotificationProviderSettingRequest>
{
    public UpdateNotificationProviderSettingRequestValidator() => Include(new CreateNotificationProviderSettingRequestValidator());
}

public class CreateNotificationTemplateRequestValidator : AbstractValidator<CreateNotificationTemplateRequest>
{
    public CreateNotificationTemplateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.SubjectTemplate).MaximumLength(250);
        RuleFor(x => x.BodyTemplate).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.VariablesJson).MaximumLength(1000);
    }
}

public class UpdateNotificationTemplateRequestValidator : AbstractValidator<UpdateNotificationTemplateRequest>
{
    public UpdateNotificationTemplateRequestValidator() => Include(new CreateNotificationTemplateRequestValidator());
}

public class CreateNotificationMessageRequestValidator : AbstractValidator<CreateNotificationMessageRequest>
{
    public CreateNotificationMessageRequestValidator()
    {
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.Subject).MaximumLength(250);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ProviderKey).MaximumLength(80);
        RuleFor(x => x.ProviderMessageId).MaximumLength(160);
        RuleFor(x => x.ErrorMessage).MaximumLength(1000);
    }
}

public class UpdateNotificationMessageRequestValidator : AbstractValidator<UpdateNotificationMessageRequest>
{
    public UpdateNotificationMessageRequestValidator() => Include(new CreateNotificationMessageRequestValidator());
}

public class CreateNotificationRecipientRequestValidator : AbstractValidator<CreateNotificationRecipientRequest>
{
    public CreateNotificationRecipientRequestValidator()
    {
        RuleFor(x => x.NotificationMessageId).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DestinationAddress).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ErrorMessage).MaximumLength(1000);
    }
}

public class UpdateNotificationRecipientRequestValidator : AbstractValidator<UpdateNotificationRecipientRequest>
{
    public UpdateNotificationRecipientRequestValidator() => Include(new CreateNotificationRecipientRequestValidator());
}

public class CreateAnnouncementRequestValidator : AbstractValidator<CreateAnnouncementRequest>
{
    public CreateAnnouncementRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Audience).IsInEnum();
        RuleFor(x => x.ExpireOn).GreaterThanOrEqualTo(x => x.PublishOn).When(x => x.ExpireOn.HasValue);
    }
}

public class UpdateAnnouncementRequestValidator : AbstractValidator<UpdateAnnouncementRequest>
{
    public UpdateAnnouncementRequestValidator() => Include(new CreateAnnouncementRequestValidator());
}

public class CreateCommunicationLogRequestValidator : AbstractValidator<CreateCommunicationLogRequest>
{
    public CreateCommunicationLogRequestValidator()
    {
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.Direction).IsInEnum();
        RuleFor(x => x.Recipient).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Subject).MaximumLength(250);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ProviderKey).MaximumLength(80);
        RuleFor(x => x.ProviderMessageId).MaximumLength(160);
        RuleFor(x => x.PayloadSummary).MaximumLength(1000);
        RuleFor(x => x.ErrorMessage).MaximumLength(1000);
    }
}

public class UpdateCommunicationLogRequestValidator : AbstractValidator<UpdateCommunicationLogRequest>
{
    public UpdateCommunicationLogRequestValidator() => Include(new CreateCommunicationLogRequestValidator());
}
