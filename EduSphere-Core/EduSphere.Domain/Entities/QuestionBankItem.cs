using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class QuestionBankItem : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid? SyllabusUnitId { get; set; }
    public SyllabusUnit? SyllabusUnit { get; set; }

    public Guid? AuthorTeacherProfileId { get; set; }
    public TeacherProfile? AuthorTeacherProfile { get; set; }

    public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
    public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
    public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public decimal Marks { get; set; } = 1;

    [Required]
    [StringLength(4000)]
    public string QuestionText { get; set; } = null!;

    [StringLength(4000)]
    public string? ExpectedAnswer { get; set; }

    [StringLength(500)]
    public string? Tags { get; set; }

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    public Guid? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateTime? ApprovedOn { get; set; }
}
