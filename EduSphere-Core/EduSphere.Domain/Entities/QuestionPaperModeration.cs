using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class QuestionPaperModeration : TenantEntityBase
{
    public Guid QuestionPaperId { get; set; }
    public QuestionPaper? QuestionPaper { get; set; }
    public ApprovalStatus Decision { get; set; } = ApprovalStatus.UnderReview;
    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime ReviewedOn { get; set; } = DateTime.UtcNow;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
