using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class QuestionPaper : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid? ExamId { get; set; }
    public Exam? Exam { get; set; }

    public Guid? ExamScheduleId { get; set; }
    public ExamSchedule? ExamSchedule { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = null!;

    public decimal TotalMarks { get; set; } = 100;
    public int DurationMinutes { get; set; } = 180;

    [StringLength(1000)]
    public string? Instructions { get; set; }

    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    public Guid? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateTime? ApprovedOn { get; set; }

    public ICollection<QuestionPaperSection> Sections { get; set; } = new List<QuestionPaperSection>();
    public ICollection<QuestionPaperVersion> Versions { get; set; } = new List<QuestionPaperVersion>();
}
