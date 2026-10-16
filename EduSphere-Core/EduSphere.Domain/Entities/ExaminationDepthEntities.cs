using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ExamSeatingPlan : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; } public ExamSchedule? ExamSchedule { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public Guid? GeneratedByUserId { get; set; } public ApplicationUser? GeneratedByUser { get; set; }
    public int CandidateCount { get; set; }
    [StringLength(1000)] public string? Instructions { get; set; }
    public ICollection<ExamSeatAssignment> Assignments { get; set; } = new List<ExamSeatAssignment>();
}

public class ExamSeatAssignment : TenantEntityBase
{
    public Guid ExamSeatingPlanId { get; set; } public ExamSeatingPlan? ExamSeatingPlan { get; set; }
    public Guid StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    public Guid RoomId { get; set; } public Room? Room { get; set; }
    [Required, StringLength(30)] public string SeatNumber { get; set; } = null!;
    public ExamCandidateStatus CandidateStatus { get; set; }
}

public class ExamHallTicket : TenantEntityBase
{
    public Guid ExamId { get; set; } public Exam? Exam { get; set; }
    public Guid StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    [Required, StringLength(60)] public string TicketNumber { get; set; } = null!;
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }
    [StringLength(500)] public string? Remarks { get; set; }
}

public class MarkRubric : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; } public ExamSchedule? ExamSchedule { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<MarkRubricCriterion> Criteria { get; set; } = new List<MarkRubricCriterion>();
}

public class MarkRubricCriterion : TenantEntityBase
{
    public Guid MarkRubricId { get; set; } public MarkRubric? MarkRubric { get; set; }
    [Required, StringLength(200)] public string Name { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    public decimal MaximumMarks { get; set; }
    public int SortOrder { get; set; }
}

public class MarkRubricScore : TenantEntityBase
{
    public Guid MarkEntryId { get; set; } public MarkEntry? MarkEntry { get; set; }
    public Guid MarkRubricCriterionId { get; set; } public MarkRubricCriterion? MarkRubricCriterion { get; set; }
    public decimal MarksAwarded { get; set; }
    [StringLength(500)] public string? Feedback { get; set; }
}

public class MarksImportBatch : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; } public ExamSchedule? ExamSchedule { get; set; }
    [Required, StringLength(255)] public string FileName { get; set; } = null!;
    public MarksImportStatus Status { get; set; } = MarksImportStatus.Previewed;
    public int TotalRows { get; set; } public int ValidRows { get; set; } public int ImportedRows { get; set; } public int ErrorRows { get; set; }
    public Guid UploadedByUserId { get; set; } public ApplicationUser? UploadedByUser { get; set; }
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow; public DateTime? CompletedOn { get; set; }
    public ICollection<MarksImportRow> Rows { get; set; } = new List<MarksImportRow>();
}

public class MarksImportRow : TenantEntityBase
{
    public Guid MarksImportBatchId { get; set; } public MarksImportBatch? MarksImportBatch { get; set; }
    public int RowNumber { get; set; }
    [Required, StringLength(50)] public string StudentIdentifier { get; set; } = null!;
    public Guid? StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    public decimal? MarksObtained { get; set; } public bool IsAbsent { get; set; }
    public bool IsValid { get; set; } public bool IsImported { get; set; }
    [StringLength(1000)] public string? ValidationMessage { get; set; }
}

public class ResultReviewRequest : TenantEntityBase
{
    public Guid MarkEntryId { get; set; } public MarkEntry? MarkEntry { get; set; }
    public Guid StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    public ResultReviewType Type { get; set; }
    [Required, StringLength(1000)] public string Reason { get; set; } = null!;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;
    public decimal OriginalMarks { get; set; } public decimal? RevisedMarks { get; set; }
    public Guid RequestedByUserId { get; set; } public ApplicationUser? RequestedByUser { get; set; }
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public Guid? ReviewedByUserId { get; set; } public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime? ReviewedOn { get; set; }
    [StringLength(1000)] public string? ReviewNotes { get; set; }
}

public class ReportCardTemplate : TenantEntityBase
{
    public Guid? BranchId { get; set; } public Branch? Branch { get; set; }
    public Guid? CourseId { get; set; } public Course? Course { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    [StringLength(20)] public string PrimaryColor { get; set; } = "#135d66";
    [StringLength(250)] public string? HeaderText { get; set; }
    [StringLength(500)] public string? FooterText { get; set; }
    public bool ShowRank { get; set; } = true; public bool ShowGradePoint { get; set; } = true; public bool ShowAttendance { get; set; }
    public bool IsDefault { get; set; }
}

public class QuestionPaperReviewPolicy : TenantEntityBase
{
    public Guid? BranchId { get; set; } public Branch? Branch { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    public int RequiredReviewerCount { get; set; } = 2;
    public int RequiredApprovalCount { get; set; } = 2;
    public bool RejectOnAnyRejection { get; set; } = true;
    public bool IsDefault { get; set; }
}

public class MarkingSchemeRelease : TenantEntityBase
{
    public Guid QuestionPaperVersionId { get; set; } public QuestionPaperVersion? QuestionPaperVersion { get; set; }
    public Guid ReleasedToUserId { get; set; } public ApplicationUser? ReleasedToUser { get; set; }
    public MarkingSchemeReleaseStatus Status { get; set; } = MarkingSchemeReleaseStatus.Draft;
    public DateTime? ReleasedOn { get; set; } public DateTime? ExpiresOn { get; set; }
    public Guid? ReleasedByUserId { get; set; } public ApplicationUser? ReleasedByUser { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public class ExamAttendanceRecord : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; } public ExamSchedule? ExamSchedule { get; set; }
    public Guid StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    public ExamAttendanceStatus Status { get; set; }
    public DateTime RecordedOn { get; set; } = DateTime.UtcNow;
    public Guid? RecordedByUserId { get; set; } public ApplicationUser? RecordedByUser { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public class ExamMalpracticeRecord : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; } public ExamSchedule? ExamSchedule { get; set; }
    public Guid StudentProfileId { get; set; } public StudentProfile? StudentProfile { get; set; }
    public MalpracticeSeverity Severity { get; set; }
    public MalpracticeCaseStatus Status { get; set; } = MalpracticeCaseStatus.Reported;
    [Required, StringLength(2000)] public string Description { get; set; } = null!;
    [StringLength(1000)] public string? ActionTaken { get; set; }
    public DateTime ReportedOn { get; set; } = DateTime.UtcNow;
    public Guid? ReportedByUserId { get; set; } public ApplicationUser? ReportedByUser { get; set; }
}

public class ExternalGradingRule : TenantEntityBase
{
    public Guid GradingSchemeId { get; set; } public GradingScheme? GradingScheme { get; set; }
    [Required, StringLength(120)] public string AuthorityName { get; set; } = null!;
    [StringLength(50)] public string? AuthorityCode { get; set; }
    public decimal MaximumGraceMarks { get; set; }
    public decimal MaximumGracePercentage { get; set; }
    public bool GraceOnlyForPassing { get; set; } = true;
    public bool ApplyBestOfSubjects { get; set; }
    public int? BestOfSubjectCount { get; set; }
    [StringLength(2000)] public string? RulesJson { get; set; }
}
