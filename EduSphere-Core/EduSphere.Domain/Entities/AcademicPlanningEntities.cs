using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AcademicTerm : TenantEntityBase
{
    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    public AcademicTermType Type { get; set; }
    public int Sequence { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class AcademicCalendarEvent : TenantEntityBase
{
    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid? AcademicTermId { get; set; }
    public AcademicTerm? AcademicTerm { get; set; }
    [Required, StringLength(160)] public string Title { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    public AcademicCalendarEventType EventType { get; set; }
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public bool IsInstructionalDay { get; set; }
}

public class SyllabusTopic : TenantEntityBase
{
    public Guid SyllabusUnitId { get; set; }
    public SyllabusUnit? SyllabusUnit { get; set; }
    public int Order { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    public int EstimatedMinutes { get; set; }
}

public class SubjectReferenceBook : TenantEntityBase
{
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    [Required, StringLength(250)] public string Title { get; set; } = null!;
    [StringLength(200)] public string? Authors { get; set; }
    [StringLength(30)] public string? Isbn { get; set; }
    [StringLength(150)] public string? Publisher { get; set; }
    public int? EditionYear { get; set; }
    [StringLength(500)] public string? ResourceUrl { get; set; }
    public bool IsPrimary { get; set; }
}

public class LearningOutcome : TenantEntityBase
{
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public Guid? SyllabusUnitId { get; set; }
    public SyllabusUnit? SyllabusUnit { get; set; }
    [Required, StringLength(40)] public string Code { get; set; } = null!;
    [Required, StringLength(1000)] public string Description { get; set; } = null!;
    [StringLength(80)] public string? BloomLevel { get; set; }
}

public class TeacherLessonPlan : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
    public Guid SectionId { get; set; }
    public Section? Section { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public Guid AcademicTermId { get; set; }
    public AcademicTerm? AcademicTerm { get; set; }
    public Guid? SyllabusTopicId { get; set; }
    public SyllabusTopic? SyllabusTopic { get; set; }
    public DateOnly PlannedDate { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = null!;
    [StringLength(2000)] public string? Objectives { get; set; }
    [StringLength(2000)] public string? Activities { get; set; }
    [StringLength(1000)] public string? Resources { get; set; }
    [StringLength(1000)] public string? Assessment { get; set; }
    public LessonPlanStatus Status { get; set; }
}

public class TeacherAvailabilityRule : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public AvailabilityRuleType RuleType { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    [StringLength(300)] public string? Reason { get; set; }
}

public class TimetableTemplate : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<TimetableTemplateItem> Items { get; set; } = new List<TimetableTemplateItem>();
}

public class TimetableTemplateItem : TenantEntityBase
{
    public Guid TimetableTemplateId { get; set; }
    public TimetableTemplate? TimetableTemplate { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int PeriodsPerWeek { get; set; } = 1;
    public int MaximumPeriodsPerDay { get; set; } = 1;
    public Guid? PreferredRoomId { get; set; }
    public Room? PreferredRoom { get; set; }
    public bool RequireConsecutivePeriods { get; set; }
}

public class TimetableGenerationRun : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    public Guid SectionId { get; set; }
    public Section? Section { get; set; }
    public Guid TimetableTemplateId { get; set; }
    public TimetableTemplate? TimetableTemplate { get; set; }
    public TimetableGenerationStatus Status { get; set; }
    public DateTime StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public int ScheduledPeriods { get; set; }
    public int UnscheduledPeriods { get; set; }
    [StringLength(4000)] public string? Diagnostics { get; set; }
    public Guid? GeneratedTimetableId { get; set; }
    public Timetable? GeneratedTimetable { get; set; }
}
