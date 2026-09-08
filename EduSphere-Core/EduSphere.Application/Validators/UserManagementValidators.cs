using EduSphere.Application.DTOs.UserManagement;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateManagedUserRequestValidator : AbstractValidator<CreateManagedUserRequest>
{
    public CreateManagedUserRequestValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Designation).MaximumLength(120);
        RuleFor(x => x.EmployeeNumber).MaximumLength(50);
        RuleFor(x => x.AdmissionNumber).MaximumLength(50);
        RuleFor(x => x.RollNumber).MaximumLength(50);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.BloodGroup).IsInEnum();
        RuleFor(x => x.Qualifications).MaximumLength(500);
        RuleFor(x => x.Specializations).MaximumLength(500);
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EmploymentType).IsInEnum();
        RuleFor(x => x.Occupation).MaximumLength(120);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.GuardianRelationship).IsInEnum();
    }
}

public class BulkUserImportRequestValidator : AbstractValidator<BulkUserImportRequest>
{
    public BulkUserImportRequestValidator()
    {
        RuleFor(x => x.RoleName).MaximumLength(80);
        RuleFor(x => x.CsvText).NotEmpty().MaximumLength(100_000);
    }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}

public class ActivateAccountRequestValidator : AbstractValidator<ActivateAccountRequest>
{
    public ActivateAccountRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}
