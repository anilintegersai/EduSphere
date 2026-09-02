using EduSphere.Application.DTOs.Tenants;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TenantIdentifier)
            .NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Tenant identifier may contain only lowercase letters, digits and hyphens.");
        RuleFor(x => x.CustomDomain).MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(255);
    }
}

public class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(255);
    }
}
