using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class QuestionPaperQuestion : TenantEntityBase
{
    public Guid QuestionPaperSectionId { get; set; }
    public QuestionPaperSection? QuestionPaperSection { get; set; }

    public Guid? QuestionBankItemId { get; set; }
    public QuestionBankItem? QuestionBankItem { get; set; }

    public int SortOrder { get; set; }
    public QuestionType QuestionType { get; set; } = QuestionType.ShortAnswer;
    public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
    public BloomLevel BloomLevel { get; set; } = BloomLevel.Understand;
    public decimal Marks { get; set; } = 1;

    [Required]
    [StringLength(4000)]
    public string QuestionText { get; set; } = null!;

    [StringLength(4000)]
    public string? SolutionText { get; set; }
}
