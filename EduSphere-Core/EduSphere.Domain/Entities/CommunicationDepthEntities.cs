using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class UserCommunicationPreference:TenantEntityBase
{
 public Guid UserId{get;set;} public ApplicationUser? User{get;set;} public CommunicationChannel Channel{get;set;} public bool IsEnabled{get;set;}=true; public CommunicationPreferenceFrequency Frequency{get;set;}=CommunicationPreferenceFrequency.Immediate; [StringLength(10)]public string Locale{get;set;}="en"; public TimeOnly? QuietHoursStart{get;set;} public TimeOnly? QuietHoursEnd{get;set;} public DateTime UpdatedOn{get;set;}=DateTime.UtcNow;
}
public class CommunicationConsent:TenantEntityBase
{
 public Guid UserId{get;set;} public ApplicationUser? User{get;set;} public CommunicationChannel Channel{get;set;} public CommunicationConsentStatus Status{get;set;}=CommunicationConsentStatus.Pending; [StringLength(80)]public string Purpose{get;set;}="General"; [StringLength(80)]public string Source{get;set;}="Portal"; public DateTime CapturedOn{get;set;}=DateTime.UtcNow; public DateTime? WithdrawnOn{get;set;} [StringLength(64)]public string? EvidenceHash{get;set;}
}
public class InAppNotification:TenantEntityBase
{
 public Guid UserId{get;set;} public ApplicationUser? User{get;set;} public Guid? NotificationMessageId{get;set;} public NotificationMessage? NotificationMessage{get;set;} [StringLength(250)]public string? Title{get;set;} [Required,StringLength(4000)]public string Body{get;set;}=null!; [StringLength(500)]public string? ActionUrl{get;set;} public DateTime CreatedAt{get;set;}=DateTime.UtcNow; public DateTime? ReadOn{get;set;} public bool IsDismissed{get;set;}
}
public class PushSubscription:TenantEntityBase
{
 public Guid UserId{get;set;} public ApplicationUser? User{get;set;} [Required,StringLength(500)]public string Endpoint{get;set;}=null!; [StringLength(500)]public string? DeviceToken{get;set;} [StringLength(80)]public string Platform{get;set;}="Web"; [StringLength(200)]public string? DeviceName{get;set;} public bool IsActive{get;set;}=true; public DateTime LastSeenOn{get;set;}=DateTime.UtcNow;
}
public class CommunicationCampaign:TenantEntityBase
{
 public Guid? BranchId{get;set;} [Required,StringLength(180)]public string Name{get;set;}=null!; public AnnouncementAudience Audience{get;set;}=AnnouncementAudience.AllUsers; public CommunicationChannel Channel{get;set;}=CommunicationChannel.Email; public Guid? NotificationTemplateId{get;set;} public NotificationTemplate? NotificationTemplate{get;set;} [StringLength(250)]public string? Subject{get;set;} [Required,StringLength(4000)]public string Body{get;set;}=null!; public DateTime ScheduledOn{get;set;} public CampaignStatus Status{get;set;}=CampaignStatus.Draft; public DateTime? StartedOn{get;set;} public DateTime? CompletedOn{get;set;} public int RecipientCount{get;set;} public int SentCount{get;set;} public int FailedCount{get;set;}
}
public class ParentTeacherMeeting:TenantEntityBase
{
 public Guid BranchId{get;set;} public Guid StudentProfileId{get;set;} public StudentProfile? StudentProfile{get;set;} public Guid TeacherProfileId{get;set;} public TeacherProfile? TeacherProfile{get;set;} public Guid ParentUserId{get;set;} public ApplicationUser? ParentUser{get;set;} public DateTime StartsOn{get;set;} public DateTime EndsOn{get;set;} public ParentTeacherMeetingStatus Status{get;set;}=ParentTeacherMeetingStatus.Requested; [StringLength(500)]public string? Agenda{get;set;} [StringLength(500)]public string? MeetingUrl{get;set;} [StringLength(1000)]public string? Notes{get;set;}
}
public class Circular:TenantEntityBase
{
 public Guid? BranchId{get;set;} [Required,StringLength(180)]public string Title{get;set;}=null!; [Required,StringLength(8000)]public string ContentHtml{get;set;}=null!; [StringLength(500)]public string? Summary{get;set;} public AnnouncementAudience Audience{get;set;}=AnnouncementAudience.AllUsers; public CircularStatus Status{get;set;}=CircularStatus.Draft; public DateTime? PublishOn{get;set;} public DateTime? ExpireOn{get;set;} [StringLength(500)]public string? AttachmentPath{get;set;} public Guid? ApprovedByUserId{get;set;} public DateTime? ApprovedOn{get;set;}
}
public class NotificationProviderWebhookEvent:TenantEntityBase
{
 [Required,StringLength(80)]public string ProviderKey{get;set;}=null!; [Required,StringLength(160)]public string ProviderMessageId{get;set;}=null!; public DeliveryEventType EventType{get;set;} public DateTime OccurredOn{get;set;}=DateTime.UtcNow; [StringLength(4000)]public string? PayloadJson{get;set;} public bool SignatureVerified{get;set;} public DateTime? ProcessedOn{get;set;}
}
public class NotificationDeadLetter:TenantEntityBase
{
 public Guid NotificationMessageId{get;set;} public NotificationMessage? NotificationMessage{get;set;} public Guid? NotificationRecipientId{get;set;} public NotificationRecipient? NotificationRecipient{get;set;} public CommunicationChannel Channel{get;set;} public int AttemptCount{get;set;} [StringLength(1000)]public string LastError{get;set;}=""; public DateTime DeadLetteredOn{get;set;}=DateTime.UtcNow; public DeadLetterStatus Status{get;set;}=DeadLetterStatus.Open; public DateTime? ResolvedOn{get;set;}
}
public class NotificationTemplateVersion:TenantEntityBase
{
 public Guid NotificationTemplateId{get;set;} public NotificationTemplate? NotificationTemplate{get;set;} public int Version{get;set;} [StringLength(250)]public string? SubjectTemplate{get;set;} [Required,StringLength(4000)]public string BodyTemplate{get;set;}=null!; [StringLength(1000)]public string? VariablesJson{get;set;} public DateTime PublishedOn{get;set;}=DateTime.UtcNow; public Guid? PublishedByUserId{get;set;}
}
public class NotificationTemplateLocalization:TenantEntityBase
{
 public Guid NotificationTemplateId{get;set;} public NotificationTemplate? NotificationTemplate{get;set;} [Required,StringLength(10)]public string Locale{get;set;}=null!; [StringLength(250)]public string? SubjectTemplate{get;set;} [Required,StringLength(4000)]public string BodyTemplate{get;set;}=null!; public bool IsActive{get;set;}=true;
}
public class CommunicationUnsubscribe:TenantEntityBase
{
 public Guid? UserId{get;set;} [Required,StringLength(250)]public string DestinationAddress{get;set;}=null!; public CommunicationChannel Channel{get;set;} [Required,StringLength(80)]public string Purpose{get;set;}="Marketing"; [Required,StringLength(64)]public string TokenHash{get;set;}=null!; public DateTime UnsubscribedOn{get;set;}=DateTime.UtcNow; [StringLength(80)]public string Source{get;set;}="SelfService"; public bool IsActive{get;set;}=true;
}
