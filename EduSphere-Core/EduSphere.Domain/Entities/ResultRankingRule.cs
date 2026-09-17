using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ResultRankingRule : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid ExamId { get; set; }
    public Exam? Exam { get; set; }
    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }
    public RankingMethod Method { get; set; } = RankingMethod.Competition;
    public bool RankByPercentage { get; set; } = true;
    public bool ExcludeFailedStudents { get; set; }
    public bool ExcludeWithheldResults { get; set; } = true;
    public decimal MinimumPercentageToRank { get; set; }
    public bool ShowRankOnReportCard { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }
}
