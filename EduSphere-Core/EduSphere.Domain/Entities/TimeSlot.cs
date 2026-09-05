using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class TimeSlot : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public bool IsBreak { get; set; }
}
