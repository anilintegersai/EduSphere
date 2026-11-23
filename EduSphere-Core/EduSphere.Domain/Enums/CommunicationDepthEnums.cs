namespace EduSphere.Domain.Enums;

public enum CommunicationConsentStatus { Pending=1, Granted=2, Withdrawn=3, Expired=4 }
public enum CommunicationPreferenceFrequency { Immediate=1, DailyDigest=2, WeeklyDigest=3, Never=4 }
public enum CampaignStatus { Draft=1, Scheduled=2, Running=3, Completed=4, Cancelled=5, Failed=6 }
public enum ParentTeacherMeetingStatus { Requested=1, Confirmed=2, Rescheduled=3, Completed=4, Cancelled=5, NoShow=6 }
public enum CircularStatus { Draft=1, Review=2, Approved=3, Published=4, Archived=5 }
public enum DeliveryEventType { Accepted=1, Delivered=2, Bounced=3, Rejected=4, Read=5, Unsubscribed=6 }
public enum DeadLetterStatus { Open=1, Requeued=2, Resolved=3, Ignored=4 }
