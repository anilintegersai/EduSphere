using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class QuestionPaperVersion : TenantEntityBase
{
    public Guid QuestionPaperId { get; set; }
    public QuestionPaper? QuestionPaper { get; set; }

    public int VersionNumber { get; set; } = 1;
    public GenerationSource Source { get; set; } = GenerationSource.Manual;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    public Guid? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    [Required]
    public string StudentContentJson { get; set; } = "{}";

    [Required]
    public string SolutionContentJson { get; set; } = "{}";

    [StringLength(500)]
    public string? Notes { get; set; }
}
