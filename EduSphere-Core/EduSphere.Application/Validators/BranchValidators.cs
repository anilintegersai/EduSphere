using EduSphere.Application.DTOs.Branches;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Pincode).NotEmpty().MaximumLength(20);
    }
}

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        Include(new CreateBranchRequestValidator());
    }
}
