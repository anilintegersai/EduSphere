using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class AttendancePolicy : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    public decimal MinimumPercentage { get; set; } = 75;
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsDefault { get; set; }
}
