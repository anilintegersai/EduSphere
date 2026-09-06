using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class NotificationProviderSetting : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    [Required]
    [StringLength(80)]
    public string ProviderKey { get; set; } = null!;

    [Required]
    [StringLength(120)]
    public string DisplayName { get; set; } = null!;

    [StringLength(150)]
    public string? FromAddress { get; set; }

    [StringLength(150)]
    public string? FromDisplayName { get; set; }

    [StringLength(2000)]
    public string? ConfigurationJson { get; set; }

    [StringLength(200)]
    public string? SecretReference { get; set; }

    public bool IsEnabled { get; set; } = true;
}

public class NotificationTemplate : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string Code { get; set; } = null!;

    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    [StringLength(250)]
    public string? SubjectTemplate { get; set; }

    [Required]
    [StringLength(4000)]
    public string BodyTemplate { get; set; } = null!;

    [StringLength(1000)]
    public string? VariablesJson { get; set; }

    public bool IsActive { get; set; } = true;
}

public class NotificationMessage : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? NotificationTemplateId { get; set; }
    public NotificationTemplate? NotificationTemplate { get; set; }

    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    [StringLength(250)]
    public string? Subject { get; set; }

    [Required]
    [StringLength(4000)]
    public string Body { get; set; } = null!;

    public NotificationStatus Status { get; set; } = NotificationStatus.Draft;
    public DateTime? ScheduledOn { get; set; }
    public DateTime? SentOn { get; set; }

    [StringLength(80)]
    public string? ProviderKey { get; set; }

    [StringLength(160)]
    public string? ProviderMessageId { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    public Guid? CreatedForUserId { get; set; }
    public ApplicationUser? CreatedForUser { get; set; }

    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}

public class NotificationRecipient : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid NotificationMessageId { get; set; }
    public NotificationMessage? NotificationMessage { get; set; }

    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(150)]
    public string DisplayName { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string DestinationAddress { get; set; } = null!;

    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public DateTime? SentOn { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }
}

public class Announcement : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(180)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(4000)]
    public string Message { get; set; } = null!;

    public AnnouncementAudience Audience { get; set; } = AnnouncementAudience.AllUsers;
    public DateTime PublishOn { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireOn { get; set; }
    public bool IsPinned { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }
}

public class CommunicationLog : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public CommunicationDirection Direction { get; set; } = CommunicationDirection.Outbound;

    [Required]
    [StringLength(250)]
    public string Recipient { get; set; } = null!;

    [StringLength(250)]
    public string? Subject { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;

    [StringLength(80)]
    public string? ProviderKey { get; set; }

    [StringLength(160)]
    public string? ProviderMessageId { get; set; }

    [StringLength(1000)]
    public string? PayloadSummary { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}
