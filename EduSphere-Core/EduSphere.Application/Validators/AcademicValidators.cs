using EduSphere.Application.DTOs.Academic;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateAcademicYearRequestValidator : AbstractValidator<CreateAcademicYearRequest>
{
    public CreateAcademicYearRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");
    }
}

public class UpdateAcademicYearRequestValidator : AbstractValidator<UpdateAcademicYearRequest>
{
    public UpdateAcademicYearRequestValidator() => Include(new CreateAcademicYearRequestValidator());
}

public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(255);
    }
}

public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator() => Include(new CreateDepartmentRequestValidator());
}

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DurationMonths).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator() => Include(new CreateCourseRequestValidator());
}

public class CreateBatchRequestValidator : AbstractValidator<CreateBatchRequest>
{
    public CreateBatchRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
    }
}

public class UpdateBatchRequestValidator : AbstractValidator<UpdateBatchRequest>
{
    public UpdateBatchRequestValidator() => Include(new CreateBatchRequestValidator());
}

public class CreateSectionRequestValidator : AbstractValidator<CreateSectionRequest>
{
    public CreateSectionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.BatchId).GreaterThan(0);
    }
}

public class UpdateSectionRequestValidator : AbstractValidator<UpdateSectionRequest>
{
    public UpdateSectionRequestValidator() => Include(new CreateSectionRequestValidator());
}

public class CreateSubjectRequestValidator : AbstractValidator<CreateSubjectRequest>
{
    public CreateSubjectRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Credits).GreaterThanOrEqualTo(0);
    }
}

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator() => Include(new CreateSubjectRequestValidator());
}

public class CreateSyllabusUnitRequestValidator : AbstractValidator<CreateSyllabusUnitRequest>
{
    public CreateSyllabusUnitRequestValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.EstimatedHours).GreaterThanOrEqualTo(0);
    }
}

public class UpdateSyllabusUnitRequestValidator : AbstractValidator<UpdateSyllabusUnitRequest>
{
    public UpdateSyllabusUnitRequestValidator()
    {
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.EstimatedHours).GreaterThanOrEqualTo(0);
    }
}
