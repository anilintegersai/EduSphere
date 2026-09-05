using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class TimetableEntry : TenantEntityBase
{
    public Guid TimetableId { get; set; }
    public Timetable? Timetable { get; set; }

    public Guid SectionId { get; set; }
    public Section? Section { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid TimeSlotId { get; set; }
    public TimeSlot? TimeSlot { get; set; }

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
