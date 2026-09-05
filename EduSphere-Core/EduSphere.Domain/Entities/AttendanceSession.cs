using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AttendanceSession : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid SectionId { get; set; }
    public Section? Section { get; set; }

    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid? TimetableEntryId { get; set; }
    public TimetableEntry? TimetableEntry { get; set; }

    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.Daily;
    public int? PeriodNumber { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }

    public Guid? MarkedByUserId { get; set; }
    public ApplicationUser? MarkedByUser { get; set; }

    public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Draft;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
}
