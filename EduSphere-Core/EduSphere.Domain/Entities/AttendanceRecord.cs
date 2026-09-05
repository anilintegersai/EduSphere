using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AttendanceRecord : TenantEntityBase
{
    public Guid AttendanceSessionId { get; set; }
    public AttendanceSession? AttendanceSession { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    public Guid? MarkedByUserId { get; set; }
    public ApplicationUser? MarkedByUser { get; set; }
    public DateTime MarkedOn { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Remarks { get; set; }
}
