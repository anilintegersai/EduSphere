using EduSphere.Application.DTOs.Operations;
using EduSphere.Domain.Enums;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateAdmissionApplicationRequestValidator : AbstractValidator<CreateAdmissionApplicationRequest>
{
    public CreateAdmissionApplicationRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ApplicationNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ApplicantFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ApplicantMiddleName).MaximumLength(100);
        RuleFor(x => x.ApplicantLastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).NotEmpty();
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.GuardianName).MaximumLength(150);
        RuleFor(x => x.GuardianPhone).MaximumLength(30);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ReviewNotes).MaximumLength(1000);
        RuleFor(x => x.FormResponseJson).MaximumLength(4000);
    }
}

public class UpdateAdmissionApplicationRequestValidator : AbstractValidator<UpdateAdmissionApplicationRequest>
{
    public UpdateAdmissionApplicationRequestValidator() => Include(new CreateAdmissionApplicationRequestValidator());
}

public class CreateAdmissionFormTemplateRequestValidator : AbstractValidator<CreateAdmissionFormTemplateRequest>
{
    public CreateAdmissionFormTemplateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Instructions).MaximumLength(1000);
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveFrom.HasValue && x.EffectiveTo.HasValue)
            .WithMessage("Effective to must be on or after effective from.");
    }
}

public class UpdateAdmissionFormTemplateRequestValidator : AbstractValidator<UpdateAdmissionFormTemplateRequest>
{
    public UpdateAdmissionFormTemplateRequestValidator() => Include(new CreateAdmissionFormTemplateRequestValidator());
}

public class CreateAdmissionFormFieldRequestValidator : AbstractValidator<CreateAdmissionFormFieldRequest>
{
    public CreateAdmissionFormFieldRequestValidator()
    {
        RuleFor(x => x.AdmissionFormTemplateId).NotEmpty();
        RuleFor(x => x.FieldKey)
            .NotEmpty()
            .MaximumLength(80)
            .Matches("^[a-zA-Z][a-zA-Z0-9_]*$")
            .WithMessage("Field key must start with a letter and contain only letters, numbers, and underscores.");
        RuleFor(x => x.Label).NotEmpty().MaximumLength(150);
        RuleFor(x => x.FieldType).IsInEnum();
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Placeholder).MaximumLength(200);
        RuleFor(x => x.HelpText).MaximumLength(500);
        RuleFor(x => x.OptionsJson).MaximumLength(2000);
        RuleFor(x => x.ValidationRegex).MaximumLength(200);
        RuleFor(x => x.MaxLength).GreaterThan(0).When(x => x.MaxLength.HasValue);
    }
}

public class UpdateAdmissionFormFieldRequestValidator : AbstractValidator<UpdateAdmissionFormFieldRequest>
{
    public UpdateAdmissionFormFieldRequestValidator() => Include(new CreateAdmissionFormFieldRequestValidator());
}

public class CreateAdmissionDocumentRequirementRequestValidator : AbstractValidator<CreateAdmissionDocumentRequirementRequest>
{
    public CreateAdmissionDocumentRequirementRequestValidator()
    {
        RuleFor(x => x.AdmissionFormTemplateId).NotEmpty();
        RuleFor(x => x.DocumentType).IsInEnum();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateAdmissionDocumentRequirementRequestValidator : AbstractValidator<UpdateAdmissionDocumentRequirementRequest>
{
    public UpdateAdmissionDocumentRequirementRequestValidator() => Include(new CreateAdmissionDocumentRequirementRequestValidator());
}

public class CreateAdmissionDocumentRequestValidator : AbstractValidator<CreateAdmissionDocumentRequest>
{
    public CreateAdmissionDocumentRequestValidator()
    {
        RuleFor(x => x.AdmissionApplicationId).NotEmpty();
        RuleFor(x => x.DocumentType).IsInEnum();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.FileName).MaximumLength(255);
        RuleFor(x => x.ContentType).MaximumLength(100);
        RuleFor(x => x.StoragePath).MaximumLength(500);
        RuleFor(x => x.SizeBytes).GreaterThanOrEqualTo(0).When(x => x.SizeBytes.HasValue);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateAdmissionDocumentRequestValidator : AbstractValidator<UpdateAdmissionDocumentRequest>
{
    public UpdateAdmissionDocumentRequestValidator() => Include(new CreateAdmissionDocumentRequestValidator());
}

public class UploadAdmissionDocumentRequestValidator : AbstractValidator<UploadAdmissionDocumentRequest>
{
    public UploadAdmissionDocumentRequestValidator()
    {
        RuleFor(x => x.DocumentType).IsInEnum();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class CreateAdmissionReviewRequestValidator : AbstractValidator<CreateAdmissionReviewRequest>
{
    public CreateAdmissionReviewRequestValidator()
    {
        RuleFor(x => x.AdmissionApplicationId).NotEmpty();
        RuleFor(x => x.FromStatus).IsInEnum();
        RuleFor(x => x.ToStatus).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class ReviewAdmissionApplicationRequestValidator : AbstractValidator<ReviewAdmissionApplicationRequest>
{
    public ReviewAdmissionApplicationRequestValidator()
    {
        RuleFor(x => x.ToStatus).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class ConvertAdmissionToEnrollmentRequestValidator : AbstractValidator<ConvertAdmissionToEnrollmentRequest>
{
    public ConvertAdmissionToEnrollmentRequestValidator()
    {
        RuleFor(x => x.AdmissionNumber).MaximumLength(50);
        RuleFor(x => x.EnrollmentNumber).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.BatchId).NotEmpty();
        RuleFor(x => x.EnrollmentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateEnrollmentRequestValidator : AbstractValidator<UpdateEnrollmentRequest>
{
    public UpdateEnrollmentRequestValidator() => Include(new CreateEnrollmentRequestValidator());
}

public class CreateAttendanceSessionRequestValidator : AbstractValidator<CreateAttendanceSessionRequest>
{
    public CreateAttendanceSessionRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.SessionType).IsInEnum();
        RuleFor(x => x.PeriodNumber).GreaterThan(0).When(x => x.PeriodNumber.HasValue);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt)
            .When(x => x.StartsAt.HasValue && x.EndsAt.HasValue)
            .WithMessage("End time must be after start time.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateAttendanceSessionRequestValidator : AbstractValidator<UpdateAttendanceSessionRequest>
{
    public UpdateAttendanceSessionRequestValidator() => Include(new CreateAttendanceSessionRequestValidator());
}

public class CreateAttendanceRecordRequestValidator : AbstractValidator<CreateAttendanceRecordRequest>
{
    public CreateAttendanceRecordRequestValidator()
    {
        RuleFor(x => x.AttendanceSessionId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}

public class UpdateAttendanceRecordRequestValidator : AbstractValidator<UpdateAttendanceRecordRequest>
{
    public UpdateAttendanceRecordRequestValidator() => Include(new CreateAttendanceRecordRequestValidator());
}

public class CreateAttendanceCorrectionRequestValidator : AbstractValidator<CreateAttendanceCorrectionRequest>
{
    public CreateAttendanceCorrectionRequestValidator()
    {
        RuleFor(x => x.AttendanceRecordId).NotEmpty();
        RuleFor(x => x.RequestedStatus).IsInEnum();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}

public class ReviewAttendanceCorrectionRequestValidator : AbstractValidator<ReviewAttendanceCorrectionRequest>
{
    public ReviewAttendanceCorrectionRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is ApprovalStatus.Approved or ApprovalStatus.Rejected)
            .WithMessage("Correction requests can only be approved or rejected.");
        RuleFor(x => x.ReviewNotes).MaximumLength(500);
    }
}

public class QueueAttendanceNotificationsRequestValidator : AbstractValidator<QueueAttendanceNotificationsRequest>
{
    public QueueAttendanceNotificationsRequestValidator()
    {
        RuleFor(x => x.AttendanceSessionId).NotEmpty();
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.Subject).MaximumLength(250);
    }
}

public class CreateAttendancePolicyRequestValidator : AbstractValidator<CreateAttendancePolicyRequest>
{
    public CreateAttendancePolicyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.MinimumPercentage).InclusiveBetween(0, 100);
    }
}

public class UpdateAttendancePolicyRequestValidator : AbstractValidator<UpdateAttendancePolicyRequest>
{
    public UpdateAttendancePolicyRequestValidator() => Include(new CreateAttendancePolicyRequestValidator());
}

public class CreateLeaveApplicationRequestValidator : AbstractValidator<CreateLeaveApplicationRequest>
{
    public CreateLeaveApplicationRequestValidator()
    {
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.LeaveType).IsInEnum();
        RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("To date must be on or after from date.");
        RuleFor(x => x.Reason).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ReviewNotes).MaximumLength(500);
    }
}

public class UpdateLeaveApplicationRequestValidator : AbstractValidator<UpdateLeaveApplicationRequest>
{
    public UpdateLeaveApplicationRequestValidator() => Include(new CreateLeaveApplicationRequestValidator());
}

public class CreateAttendanceAlertRequestValidator : AbstractValidator<CreateAttendanceAlertRequest>
{
    public CreateAttendanceAlertRequestValidator()
    {
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodStart);
        RuleFor(x => x.AttendancePercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.ThresholdPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(500);
    }
}

public class UpdateAttendanceAlertRequestValidator : AbstractValidator<UpdateAttendanceAlertRequest>
{
    public UpdateAttendanceAlertRequestValidator() => Include(new CreateAttendanceAlertRequestValidator());
}

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator() => Include(new CreateRoomRequestValidator());
}

public class CreateTimeSlotRequestValidator : AbstractValidator<CreateTimeSlotRequest>
{
    public CreateTimeSlotRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.DayOfWeek).IsInEnum();
        RuleFor(x => x.PeriodNumber).GreaterThan(0);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt)
            .WithMessage("End time must be after start time.");
    }
}

public class UpdateTimeSlotRequestValidator : AbstractValidator<UpdateTimeSlotRequest>
{
    public UpdateTimeSlotRequestValidator() => Include(new CreateTimeSlotRequestValidator());
}

public class CreateTimetableRequestValidator : AbstractValidator<CreateTimetableRequest>
{
    public CreateTimetableRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EffectiveTo).GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateTimetableRequestValidator : AbstractValidator<UpdateTimetableRequest>
{
    public UpdateTimetableRequestValidator() => Include(new CreateTimetableRequestValidator());
}

public class CreateTimetableEntryRequestValidator : AbstractValidator<CreateTimetableEntryRequest>
{
    public CreateTimetableEntryRequestValidator()
    {
        RuleFor(x => x.TimetableId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.TeacherProfileId).NotEmpty();
        RuleFor(x => x.TimeSlotId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateTimetableEntryRequestValidator : AbstractValidator<UpdateTimetableEntryRequest>
{
    public UpdateTimetableEntryRequestValidator() => Include(new CreateTimetableEntryRequestValidator());
}

public class CreateTimetableSubstitutionRequestValidator : AbstractValidator<CreateTimetableSubstitutionRequest>
{
    public CreateTimetableSubstitutionRequestValidator()
    {
        RuleFor(x => x.TimetableEntryId).NotEmpty();
        RuleFor(x => x.OriginalTeacherProfileId).NotEmpty();
        RuleFor(x => x.SubstituteTeacherProfileId).NotEmpty();
        RuleFor(x => x.SubstituteTeacherProfileId)
            .NotEqual(x => x.OriginalTeacherProfileId)
            .WithMessage("The relief teacher must be different from the original teacher.");
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}

public class ReviewTimetableSubstitutionRequestValidator : AbstractValidator<ReviewTimetableSubstitutionRequest>
{
    public ReviewTimetableSubstitutionRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is ApprovalStatus.Approved or ApprovalStatus.Rejected)
            .WithMessage("Substitution requests can only be approved or rejected.");
        RuleFor(x => x.ReviewNotes).MaximumLength(500);
    }
}
