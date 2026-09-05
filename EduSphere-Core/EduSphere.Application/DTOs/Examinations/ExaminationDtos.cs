using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.Examinations;

public abstract class ExaminationTenantScopedDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

public class ExamDto : ExaminationTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExamType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal WeightagePercentage { get; set; }
    public ExamStatus Status { get; set; }
    public string? Instructions { get; set; }
}

public class CreateExamRequest
{
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExamType Type { get; set; } = ExamType.UnitTest;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal WeightagePercentage { get; set; } = 100;
    public ExamStatus Status { get; set; } = ExamStatus.Draft;
    public string? Instructions { get; set; }
}

public class UpdateExamRequest : CreateExamRequest { }

public class ExamScheduleDto : ExaminationTenantScopedDto
{
    public Guid ExamId { get; set; }
    public Guid SectionId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? TeacherProfileId { get; set; }
    public Guid? RoomId { get; set; }
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal MaximumMarks { get; set; }
    public decimal PassingMarks { get; set; }
    public ExamScheduleStatus Status { get; set; }
    public string? SeatingPlanNotes { get; set; }
    public string? Instructions { get; set; }
}

public class CreateExamScheduleRequest
{
    public Guid ExamId { get; set; }
    public Guid SectionId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? TeacherProfileId { get; set; }
    public Guid? RoomId { get; set; }
    public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly StartsAt { get; set; } = new(9, 0);
    public TimeOnly EndsAt { get; set; } = new(12, 0);
    public int DurationMinutes { get; set; } = 180;
    public decimal MaximumMarks { get; set; } = 100;
    public decimal PassingMarks { get; set; } = 35;
    public ExamScheduleStatus Status { get; set; } = ExamScheduleStatus.Draft;
    public string? SeatingPlanNotes { get; set; }
    public string? Instructions { get; set; }
}

public class UpdateExamScheduleRequest : CreateExamScheduleRequest { }

public class GradingSchemeDto : ExaminationTenantScopedDto
{
    public Guid? BranchId { get; set; }
    public Guid? CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public GradingSchemeType Type { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public bool IsDefault { get; set; }
    public string? Notes { get; set; }
}

public class CreateGradingSchemeRequest
{
    public Guid? BranchId { get; set; }
    public Guid? CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public GradingSchemeType Type { get; set; } = GradingSchemeType.Percentage;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsDefault { get; set; }
    public string? Notes { get; set; }
}

public class UpdateGradingSchemeRequest : CreateGradingSchemeRequest { }

public class GradingSchemeBandDto : ExaminationTenantScopedDto
{
    public Guid GradingSchemeId { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal MinimumPercentage { get; set; }
    public decimal MaximumPercentage { get; set; }
    public decimal GradePoint { get; set; }
    public int SortOrder { get; set; }
    public string? Remarks { get; set; }
}

public class CreateGradingSchemeBandRequest
{
    public Guid GradingSchemeId { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal MinimumPercentage { get; set; }
    public decimal MaximumPercentage { get; set; }
    public decimal GradePoint { get; set; }
    public int SortOrder { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateGradingSchemeBandRequest : CreateGradingSchemeBandRequest { }

public class QuestionBankItemDto : ExaminationTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? SyllabusUnitId { get; set; }
    public Guid? AuthorTeacherProfileId { get; set; }
    public QuestionType QuestionType { get; set; }
    public QuestionDifficulty Difficulty { get; set; }
    public BloomLevel BloomLevel { get; set; }
    public GenerationSource Source { get; set; }
    public decimal Marks { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? ExpectedAnswer { get; set; }
    public string? Tags { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedOn { get; set; }
}

public class CreateQuestionBankItemRequest
{
    public Guid BranchId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? SyllabusUnitId { get; set; }
    public Guid? AuthorTeacherProfileId { get; set; }
    public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
    public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
    public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public decimal Marks { get; set; } = 1;
    public string QuestionText { get; set; } = string.Empty;
    public string? ExpectedAnswer { get; set; }
    public string? Tags { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedOn { get; set; }
}

public class UpdateQuestionBankItemRequest : CreateQuestionBankItemRequest { }

public class QuestionPaperDto : ExaminationTenantScopedDto
{
    public Guid BranchId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? ExamId { get; set; }
    public Guid? ExamScheduleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public int DurationMinutes { get; set; }
    public string? Instructions { get; set; }
    public GenerationSource Source { get; set; }
    public ApprovalStatus Status { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedOn { get; set; }
}

public class CreateQuestionPaperRequest
{
    public Guid BranchId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? ExamId { get; set; }
    public Guid? ExamScheduleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; } = 100;
    public int DurationMinutes { get; set; } = 180;
    public string? Instructions { get; set; }
    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedOn { get; set; }
}

public class UpdateQuestionPaperRequest : CreateQuestionPaperRequest { }

public class QuestionPaperSectionDto : ExaminationTenantScopedDto
{
    public Guid QuestionPaperId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public decimal Marks { get; set; }
    public string? Instructions { get; set; }
}

public class CreateQuestionPaperSectionRequest
{
    public Guid QuestionPaperId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public decimal Marks { get; set; }
    public string? Instructions { get; set; }
}

public class UpdateQuestionPaperSectionRequest : CreateQuestionPaperSectionRequest { }

public class QuestionPaperQuestionDto : ExaminationTenantScopedDto
{
    public Guid QuestionPaperSectionId { get; set; }
    public Guid? QuestionBankItemId { get; set; }
    public int SortOrder { get; set; }
    public QuestionType QuestionType { get; set; }
    public QuestionDifficulty Difficulty { get; set; }
    public BloomLevel BloomLevel { get; set; }
    public decimal Marks { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? SolutionText { get; set; }
}

public class CreateQuestionPaperQuestionRequest
{
    public Guid QuestionPaperSectionId { get; set; }
    public Guid? QuestionBankItemId { get; set; }
    public int SortOrder { get; set; }
    public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
    public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
    public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
    public decimal Marks { get; set; } = 1;
    public string QuestionText { get; set; } = string.Empty;
    public string? SolutionText { get; set; }
}

public class UpdateQuestionPaperQuestionRequest : CreateQuestionPaperQuestionRequest { }

public class QuestionPaperVersionDto : ExaminationTenantScopedDto
{
    public Guid QuestionPaperId { get; set; }
    public int VersionNumber { get; set; }
    public GenerationSource Source { get; set; }
    public ApprovalStatus Status { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string StudentContentJson { get; set; } = "{}";
    public string SolutionContentJson { get; set; } = "{}";
    public string? Notes { get; set; }
}

public class CreateQuestionPaperVersionRequest
{
    public Guid QuestionPaperId { get; set; }
    public int VersionNumber { get; set; } = 1;
    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    public Guid? CreatedByUserId { get; set; }
    public string StudentContentJson { get; set; } = "{}";
    public string SolutionContentJson { get; set; } = "{}";
    public string? Notes { get; set; }
}

public class UpdateQuestionPaperVersionRequest : CreateQuestionPaperVersionRequest { }

public class MarkEntryDto : ExaminationTenantScopedDto
{
    public Guid ExamScheduleId { get; set; }
    public Guid StudentProfileId { get; set; }
    public decimal MarksObtained { get; set; }
    public bool IsAbsent { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public Guid? EnteredByUserId { get; set; }
    public DateTime EnteredOn { get; set; }
    public string? Remarks { get; set; }
}

public class CreateMarkEntryRequest
{
    public Guid ExamScheduleId { get; set; }
    public Guid StudentProfileId { get; set; }
    public decimal MarksObtained { get; set; }
    public bool IsAbsent { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public Guid? EnteredByUserId { get; set; }
    public DateTime EnteredOn { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
}

public class UpdateMarkEntryRequest : CreateMarkEntryRequest { }

public class ResultDto : ExaminationTenantScopedDto
{
    public Guid ExamId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid BranchId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? SectionId { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal MarksObtained { get; set; }
    public decimal Percentage { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }
    public ResultStatus Status { get; set; }
    public DateTime ComputedOn { get; set; }
    public Guid? PublishedByUserId { get; set; }
    public DateTime? PublishedOn { get; set; }
    public string? Remarks { get; set; }
}

public class ComputeResultRequest
{
    public Guid ExamId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid? GradingSchemeId { get; set; }
    public bool Publish { get; set; }
    public string? Remarks { get; set; }
}

public class PublishResultRequest
{
    public string? Remarks { get; set; }
}
