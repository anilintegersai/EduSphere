namespace EduSphere.Domain.Enums;

public enum ReportDataSource { Student360 = 1, TeacherWorkload = 2, AttendancePerformance = 3, BranchComparison = 4, CourseComparison = 5, ClassComparison = 6, Students = 7, Teachers = 8, Attendance = 9, Results = 10, Finance = 11 }
public enum ReportOutputFormat { Excel = 1, Pdf = 2, Csv = 3 }
public enum ReportExecutionStatus { Queued = 1, Running = 2, Succeeded = 3, Failed = 4 }
public enum ReportScheduleFrequency { Daily = 1, Weekly = 2, Monthly = 3 }
public enum BiProvider { PowerBI = 1, Metabase = 2 }
public enum DashboardWidgetSize { Small = 1, Medium = 2, Large = 3 }
