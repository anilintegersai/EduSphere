namespace EduSphere.Domain.Enums;

public enum ProfileStatusOwnerType { Student = 1, Teacher = 2 }
public enum DocumentScanStatus { Pending = 1, Clean = 2, Infected = 3, Failed = 4, NotRequired = 5 }
public enum DocumentVerificationStatus { Pending = 1, Verified = 2, Rejected = 3, Expired = 4 }
public enum AlumniEngagementType { Event = 1, Mentoring = 2, Placement = 3, Donation = 4, Volunteering = 5, Communication = 6 }
public enum BulkLifecycleRunStatus { Draft = 1, Processing = 2, Completed = 3, CompletedWithErrors = 4, Failed = 5 }
public enum SignatureStatus { Signed = 1, Revoked = 2 }
