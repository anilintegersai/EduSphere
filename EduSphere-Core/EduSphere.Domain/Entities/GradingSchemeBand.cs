using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class GradingSchemeBand : TenantEntityBase
{
    public Guid GradingSchemeId { get; set; }
    public GradingScheme? GradingScheme { get; set; }

    [Required]
    [StringLength(20)]
    public string Grade { get; set; } = null!;

    public decimal MinimumPercentage { get; set; }
    public decimal MaximumPercentage { get; set; }
    public decimal GradePoint { get; set; }
    public int SortOrder { get; set; }

    [StringLength(200)]
    public string? Remarks { get; set; }
}
