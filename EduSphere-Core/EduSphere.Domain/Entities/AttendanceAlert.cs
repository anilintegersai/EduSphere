using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AttendanceAlert : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid? AttendancePolicyId { get; set; }
    public AttendancePolicy? AttendancePolicy { get; set; }

    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal ThresholdPercentage { get; set; }

    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public AttendanceAlertStatus Status { get; set; } = AttendanceAlertStatus.Pending;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = null!;
}
