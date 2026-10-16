namespace EduSphere.Domain.Enums;

public enum ExamCandidateStatus { Eligible = 0, Withheld = 1, Cancelled = 2 }
public enum ExamAttendanceStatus { Present = 0, Absent = 1, Late = 2, Expelled = 3 }
public enum MalpracticeSeverity { Minor = 0, Major = 1, Critical = 2 }
public enum MalpracticeCaseStatus { Reported = 0, UnderReview = 1, Confirmed = 2, Dismissed = 3, Closed = 4 }
public enum ResultReviewType { ReTotal = 0, ReEvaluation = 1 }
public enum MarksImportStatus { Previewed = 0, Processing = 1, Completed = 2, CompletedWithErrors = 3, Failed = 4 }
public enum MarkingSchemeReleaseStatus { Draft = 0, Released = 1, Revoked = 2, Expired = 3 }

