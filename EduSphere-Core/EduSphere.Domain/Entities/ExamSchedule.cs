using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ExamSchedule : TenantEntityBase
{
    public Guid ExamId { get; set; }
    public Exam? Exam { get; set; }

    public Guid SectionId { get; set; }
    public Section? Section { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid? TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }

    public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly StartsAt { get; set; } = new(9, 0);
    public TimeOnly EndsAt { get; set; } = new(12, 0);
    public int DurationMinutes { get; set; } = 180;
    public decimal MaximumMarks { get; set; } = 100;
    public decimal PassingMarks { get; set; } = 35;
    public ExamScheduleStatus Status { get; set; } = ExamScheduleStatus.Draft;

    [StringLength(1000)]
    public string? SeatingPlanNotes { get; set; }

    [StringLength(500)]
    public string? Instructions { get; set; }

    public ICollection<QuestionPaper> QuestionPapers { get; set; } = new List<QuestionPaper>();
    public ICollection<MarkEntry> MarkEntries { get; set; } = new List<MarkEntry>();
}
