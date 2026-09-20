namespace EduSphere.Domain.Enums;

public enum AcademicTermType { Term = 0, Semester = 1, Trimester = 2, Quarter = 3 }
public enum AcademicCalendarEventType { Holiday = 0, TeachingDay = 1, Examination = 2, Activity = 3, Deadline = 4 }
public enum LessonPlanStatus { Draft = 0, Submitted = 1, Approved = 2, Delivered = 3, Cancelled = 4 }
public enum AvailabilityRuleType { Available = 0, Unavailable = 1, Preferred = 2 }
public enum TimetableGenerationStatus { Draft = 0, Generated = 1, Applied = 2, Failed = 3 }
