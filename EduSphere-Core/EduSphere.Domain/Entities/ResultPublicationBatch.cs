using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ResultPublicationBatch : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid ExamId { get; set; }
    public Exam? Exam { get; set; }
    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;
    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public Guid? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public Guid? PublishedByUserId { get; set; }
    public ApplicationUser? PublishedByUser { get; set; }
    public DateTime? PublishedOn { get; set; }
    public int ResultCount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
