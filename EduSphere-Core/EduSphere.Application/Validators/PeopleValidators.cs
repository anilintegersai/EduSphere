using EduSphere.Application.DTOs.People;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateStudentProfileRequestValidator : AbstractValidator<CreateStudentProfileRequest>
{
    public CreateStudentProfileRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.AdmissionNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.RollNumber).MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.BloodGroup).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.EmergencyContactName).MaximumLength(150);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AdmissionDate).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}

public class UpdateStudentProfileRequestValidator : AbstractValidator<UpdateStudentProfileRequest>
{
    public UpdateStudentProfileRequestValidator() => Include(new CreateStudentProfileRequestValidator());
}

public class CreateTeacherProfileRequestValidator : AbstractValidator<CreateTeacherProfileRequest>
{
    public CreateTeacherProfileRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Designation).MaximumLength(120);
        RuleFor(x => x.Qualifications).MaximumLength(500);
        RuleFor(x => x.Specializations).MaximumLength(500);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0);
        RuleFor(x => x.JoiningDate).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateTeacherProfileRequestValidator : AbstractValidator<UpdateTeacherProfileRequest>
{
    public UpdateTeacherProfileRequestValidator() => Include(new CreateTeacherProfileRequestValidator());
}

public class CreateStudentGuardianRequestValidator : AbstractValidator<CreateStudentGuardianRequest>
{
    public CreateStudentGuardianRequestValidator()
    {
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.Relationship).IsInEnum();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Occupation).MaximumLength(120);
    }
}

public class UpdateStudentGuardianRequestValidator : AbstractValidator<UpdateStudentGuardianRequest>
{
    public UpdateStudentGuardianRequestValidator() => Include(new CreateStudentGuardianRequestValidator());
}

public class CreateTeacherSubjectAssignmentRequestValidator : AbstractValidator<CreateTeacherSubjectAssignmentRequest>
{
    public CreateTeacherSubjectAssignmentRequestValidator()
    {
        RuleFor(x => x.TeacherProfileId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.EffectiveUntil)
            .GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveFrom.HasValue && x.EffectiveUntil.HasValue)
            .WithMessage("Effective until must be on or after effective from.");
    }
}

public class UpdateTeacherSubjectAssignmentRequestValidator : AbstractValidator<UpdateTeacherSubjectAssignmentRequest>
{
    public UpdateTeacherSubjectAssignmentRequestValidator() => Include(new CreateTeacherSubjectAssignmentRequestValidator());
}

public class CreateProfileDocumentRequestValidator : AbstractValidator<CreateProfileDocumentRequest>
{
    public CreateProfileDocumentRequestValidator()
    {
        RuleFor(x => x.OwnerType).IsInEnum();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).MaximumLength(120);
        RuleFor(x => x.StoragePath).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.SizeBytes).GreaterThanOrEqualTo(0).When(x => x.SizeBytes.HasValue);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateProfileDocumentRequestValidator : AbstractValidator<UpdateProfileDocumentRequest>
{
    public UpdateProfileDocumentRequestValidator()
    {
        Include(new CreateProfileDocumentRequestValidator());
        RuleFor(x => x.VerifiedBy).MaximumLength(128);
    }
}
