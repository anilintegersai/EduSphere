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
    }
}

public class UpdateAdmissionApplicationRequestValidator : AbstractValidator<UpdateAdmissionApplicationRequest>
{
    public UpdateAdmissionApplicationRequestValidator() => Include(new CreateAdmissionApplicationRequestValidator());
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
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateAdmissionDocumentRequestValidator : AbstractValidator<UpdateAdmissionDocumentRequest>
{
    public UpdateAdmissionDocumentRequestValidator() => Include(new CreateAdmissionDocumentRequestValidator());
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
