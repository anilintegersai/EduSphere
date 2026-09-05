using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class GradingScheme : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    public GradingSchemeType Type { get; set; } = GradingSchemeType.Percentage;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsDefault { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<GradingSchemeBand> Bands { get; set; } = new List<GradingSchemeBand>();
}
