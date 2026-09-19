namespace EduSphere.Domain.Enums;

public enum SubscriptionStatus { Trial = 0, Active = 1, Suspended = 2, Expired = 3, Cancelled = 4 }
public enum SettingValueType { String = 0, Number = 1, Boolean = 2, Json = 3 }
public enum BackgroundJobStatus { Queued = 0, Running = 1, Succeeded = 2, Failed = 3, RetryScheduled = 4, Cancelled = 5 }
public enum AuditAction { Created = 0, Updated = 1, Deleted = 2 }
public enum IntegrationCategory { General = 0, Identity = 1, Payment = 2, Communication = 3, Storage = 4, Analytics = 5, AI = 6 }
