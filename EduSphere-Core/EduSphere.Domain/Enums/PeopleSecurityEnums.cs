namespace EduSphere.Domain.Enums;

public enum StaffAttendanceStatus { Present = 0, Absent = 1, Late = 2, HalfDay = 3, OnLeave = 4, Remote = 5 }
public enum StaffLeaveStatus { Draft = 0, Submitted = 1, Approved = 2, Rejected = 3, Cancelled = 4 }
public enum PayrollRunStatus { Draft = 0, Calculated = 1, Approved = 2, Exported = 3, Paid = 4 }
public enum SecurityEventType { LoginSucceeded = 0, LoginFailed = 1, AccountLocked = 2, AccountUnlocked = 3, MfaEnabled = 4, MfaDisabled = 5, MfaFailed = 6, PasswordChanged = 7, PasswordResetRequested = 8 }
public enum EmailJobStatus { Queued = 0, Processing = 1, Succeeded = 2, RetryScheduled = 3, Failed = 4, Bounced = 5 }
public enum UserImportBatchStatus { Previewed = 0, Queued = 1, Processing = 2, Completed = 3, CompletedWithErrors = 4, Failed = 5 }
