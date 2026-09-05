using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class QuestionPaperSection : TenantEntityBase
{
    public Guid QuestionPaperId { get; set; }
    public QuestionPaper? QuestionPaper { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = null!;

    public int SortOrder { get; set; }
    public decimal Marks { get; set; }

    [StringLength(500)]
    public string? Instructions { get; set; }

    public ICollection<QuestionPaperQuestion> Questions { get; set; } = new List<QuestionPaperQuestion>();
}
