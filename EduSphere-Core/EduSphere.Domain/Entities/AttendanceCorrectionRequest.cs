using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AttendanceCorrectionRequest : TenantEntityBase
{
    public Guid AttendanceRecordId { get; set; }
    public AttendanceRecord? AttendanceRecord { get; set; }

    public Guid AttendanceSessionId { get; set; }
    public AttendanceSession? AttendanceSession { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public AttendanceStatus CurrentStatus { get; set; }
    public AttendanceStatus RequestedStatus { get; set; }

    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }

    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }

    public DateTime? ReviewedOn { get; set; }

    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(500)]
    public string? ReviewNotes { get; set; }
}
