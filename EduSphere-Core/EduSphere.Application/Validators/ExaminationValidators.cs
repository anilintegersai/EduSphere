using EduSphere.Application.DTOs.Examinations;
using FluentValidation;

namespace EduSphere.Application.Validators;

public class CreateExamRequestValidator : AbstractValidator<CreateExamRequest>
{
    public CreateExamRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.WeightagePercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Instructions).MaximumLength(1000);
    }
}

public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
{
    public UpdateExamRequestValidator() => Include(new CreateExamRequestValidator());
}

public class CreateExamScheduleRequestValidator : AbstractValidator<CreateExamScheduleRequest>
{
    public CreateExamScheduleRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.MaximumMarks).GreaterThan(0);
        RuleFor(x => x.PassingMarks).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MaximumMarks);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.SeatingPlanNotes).MaximumLength(1000);
        RuleFor(x => x.Instructions).MaximumLength(500);
    }
}

public class UpdateExamScheduleRequestValidator : AbstractValidator<UpdateExamScheduleRequest>
{
    public UpdateExamScheduleRequestValidator() => Include(new CreateExamScheduleRequestValidator());
}

public class CreateGradingSchemeRequestValidator : AbstractValidator<CreateGradingSchemeRequest>
{
    public CreateGradingSchemeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateGradingSchemeRequestValidator : AbstractValidator<UpdateGradingSchemeRequest>
{
    public UpdateGradingSchemeRequestValidator() => Include(new CreateGradingSchemeRequestValidator());
}

public class CreateGradingSchemeBandRequestValidator : AbstractValidator<CreateGradingSchemeBandRequest>
{
    public CreateGradingSchemeBandRequestValidator()
    {
        RuleFor(x => x.GradingSchemeId).NotEmpty();
        RuleFor(x => x.Grade).NotEmpty().MaximumLength(20);
        RuleFor(x => x.MinimumPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.MaximumPercentage).InclusiveBetween(0, 100)
            .GreaterThanOrEqualTo(x => x.MinimumPercentage);
        RuleFor(x => x.GradePoint).InclusiveBetween(0, 10);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Remarks).MaximumLength(200);
    }
}

public class UpdateGradingSchemeBandRequestValidator : AbstractValidator<UpdateGradingSchemeBandRequest>
{
    public UpdateGradingSchemeBandRequestValidator() => Include(new CreateGradingSchemeBandRequestValidator());
}

public class CreateQuestionBankItemRequestValidator : AbstractValidator<CreateQuestionBankItemRequest>
{
    public CreateQuestionBankItemRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.QuestionType).IsInEnum();
        RuleFor(x => x.Difficulty).IsInEnum();
        RuleFor(x => x.BloomLevel).IsInEnum();
        RuleFor(x => x.Source).IsInEnum();
        RuleFor(x => x.Marks).GreaterThan(0);
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ExpectedAnswer).MaximumLength(4000);
        RuleFor(x => x.Tags).MaximumLength(500);
        RuleFor(x => x.ApprovalStatus).IsInEnum();
    }
}

public class UpdateQuestionBankItemRequestValidator : AbstractValidator<UpdateQuestionBankItemRequest>
{
    public UpdateQuestionBankItemRequestValidator() => Include(new CreateQuestionBankItemRequestValidator());
}

public class CreateQuestionPaperRequestValidator : AbstractValidator<CreateQuestionPaperRequest>
{
    public CreateQuestionPaperRequestValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.TotalMarks).GreaterThan(0);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Instructions).MaximumLength(1000);
        RuleFor(x => x.Source).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateQuestionPaperRequestValidator : AbstractValidator<UpdateQuestionPaperRequest>
{
    public UpdateQuestionPaperRequestValidator() => Include(new CreateQuestionPaperRequestValidator());
}

public class CreateQuestionPaperSectionRequestValidator : AbstractValidator<CreateQuestionPaperSectionRequest>
{
    public CreateQuestionPaperSectionRequestValidator()
    {
        RuleFor(x => x.QuestionPaperId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Marks).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Instructions).MaximumLength(500);
    }
}

public class UpdateQuestionPaperSectionRequestValidator : AbstractValidator<UpdateQuestionPaperSectionRequest>
{
    public UpdateQuestionPaperSectionRequestValidator() => Include(new CreateQuestionPaperSectionRequestValidator());
}

public class CreateQuestionPaperQuestionRequestValidator : AbstractValidator<CreateQuestionPaperQuestionRequest>
{
    public CreateQuestionPaperQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionPaperSectionId).NotEmpty();
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QuestionType).IsInEnum();
        RuleFor(x => x.Difficulty).IsInEnum();
        RuleFor(x => x.BloomLevel).IsInEnum();
        RuleFor(x => x.Marks).GreaterThan(0);
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.SolutionText).MaximumLength(4000);
    }
}

public class UpdateQuestionPaperQuestionRequestValidator : AbstractValidator<UpdateQuestionPaperQuestionRequest>
{
    public UpdateQuestionPaperQuestionRequestValidator() => Include(new CreateQuestionPaperQuestionRequestValidator());
}

public class CreateQuestionPaperVersionRequestValidator : AbstractValidator<CreateQuestionPaperVersionRequest>
{
    public CreateQuestionPaperVersionRequestValidator()
    {
        RuleFor(x => x.QuestionPaperId).NotEmpty();
        RuleFor(x => x.VersionNumber).GreaterThan(0);
        RuleFor(x => x.Source).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.StudentContentJson).NotEmpty();
        RuleFor(x => x.SolutionContentJson).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateQuestionPaperVersionRequestValidator : AbstractValidator<UpdateQuestionPaperVersionRequest>
{
    public UpdateQuestionPaperVersionRequestValidator() => Include(new CreateQuestionPaperVersionRequestValidator());
}

public class CreateMarkEntryRequestValidator : AbstractValidator<CreateMarkEntryRequest>
{
    public CreateMarkEntryRequestValidator()
    {
        RuleFor(x => x.ExamScheduleId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.MarksObtained).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Grade).MaximumLength(20);
        RuleFor(x => x.GradePoint).InclusiveBetween(0, 10).When(x => x.GradePoint.HasValue);
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}

public class UpdateMarkEntryRequestValidator : AbstractValidator<UpdateMarkEntryRequest>
{
    public UpdateMarkEntryRequestValidator() => Include(new CreateMarkEntryRequestValidator());
}

public class ComputeResultRequestValidator : AbstractValidator<ComputeResultRequest>
{
    public ComputeResultRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}

public class PublishResultRequestValidator : AbstractValidator<PublishResultRequest>
{
    public PublishResultRequestValidator()
    {
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
