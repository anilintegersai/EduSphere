using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class LeaveApplication : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public LeaveType LeaveType { get; set; } = LeaveType.Other;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public LeaveApplicationStatus Status { get; set; } = LeaveApplicationStatus.Submitted;

    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }
    public DateTime? ReviewedOn { get; set; }

    [StringLength(500)]
    public string? ReviewNotes { get; set; }
}
