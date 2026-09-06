using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Pages.Enterprise;

[Authorize(Policy = AuthorizationPolicies.CommunicationManager)]
public class CommunicationsModel : EnterprisePageModel
{
    private readonly ICrudService<NotificationProviderSetting> _providers;
    private readonly ICrudService<NotificationTemplate> _templates;
    private readonly ICrudService<NotificationMessage> _messages;
    private readonly ICrudService<NotificationRecipient> _recipients;
    private readonly ICrudService<Announcement> _announcements;
    private readonly ICrudService<CommunicationLog> _logs;
    private readonly ICrudService<Branch> _branches;
    private readonly INotificationDispatcher _dispatcher;

    public CommunicationsModel(
        ICrudService<NotificationProviderSetting> providers,
        ICrudService<NotificationTemplate> templates,
        ICrudService<NotificationMessage> messages,
        ICrudService<NotificationRecipient> recipients,
        ICrudService<Announcement> announcements,
        ICrudService<CommunicationLog> logs,
        ICrudService<Branch> branches,
        INotificationDispatcher dispatcher,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
        : base(tenant, branchAccess)
    {
        _providers = providers;
        _templates = templates;
        _messages = messages;
        _recipients = recipients;
        _announcements = announcements;
        _logs = logs;
        _branches = branches;
        _dispatcher = dispatcher;
    }

    public IReadOnlyList<NotificationProviderSetting> Providers { get; private set; } = new List<NotificationProviderSetting>();
    public IReadOnlyList<NotificationTemplate> Templates { get; private set; } = new List<NotificationTemplate>();
    public IReadOnlyList<NotificationMessage> Messages { get; private set; } = new List<NotificationMessage>();
    public IReadOnlyList<NotificationRecipient> Recipients { get; private set; } = new List<NotificationRecipient>();
    public IReadOnlyList<Announcement> Announcements { get; private set; } = new List<Announcement>();
    public IReadOnlyList<CommunicationLog> Logs { get; private set; } = new List<CommunicationLog>();

    [BindProperty] public ProviderInputModel ProviderInput { get; set; } = new();
    [BindProperty] public TemplateInputModel TemplateInput { get; set; } = new();
    [BindProperty] public MessageInputModel MessageInput { get; set; } = new();
    [BindProperty] public RecipientInputModel RecipientInput { get; set; } = new();
    [BindProperty] public AnnouncementInputModel AnnouncementInput { get; set; } = new();

    public class ProviderInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
        [Required, StringLength(80), Display(Name = "Provider key")] public string ProviderKey { get; set; } = "smtp";
        [Required, StringLength(120), Display(Name = "Display name")] public string DisplayName { get; set; } = "SMTP Email";
        [EmailAddress, StringLength(150), Display(Name = "From address")] public string? FromAddress { get; set; }
        [StringLength(150), Display(Name = "From name")] public string? FromDisplayName { get; set; }
        [StringLength(200), Display(Name = "Secret reference")] public string? SecretReference { get; set; }
    }

    public class TemplateInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(80)] public string Code { get; set; } = string.Empty;
        public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
        [StringLength(250), Display(Name = "Subject")] public string? SubjectTemplate { get; set; }
        [Required, StringLength(4000), Display(Name = "Body")] public string BodyTemplate { get; set; } = string.Empty;
    }

    public class MessageInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Template")] public Guid? NotificationTemplateId { get; set; }
        public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
        [StringLength(250)] public string? Subject { get; set; }
        [Required, StringLength(4000)] public string Body { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; } = NotificationStatus.Draft;
        [Display(Name = "Schedule")] public DateTime? ScheduledOn { get; set; }
        [StringLength(80), Display(Name = "Provider")] public string? ProviderKey { get; set; }
    }

    public class RecipientInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Message")] public Guid? NotificationMessageId { get; set; }
        [Required, StringLength(150), Display(Name = "Display name")] public string DisplayName { get; set; } = string.Empty;
        [Required, StringLength(250), Display(Name = "Destination")] public string DestinationAddress { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    }

    public class AnnouncementInputModel
    {
        [Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(180)] public string Title { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string Message { get; set; } = string.Empty;
        public AnnouncementAudience Audience { get; set; } = AnnouncementAudience.AllUsers;
        [Display(Name = "Publish on")] public DateTime PublishOn { get; set; } = DateTime.UtcNow;
        [Display(Name = "Pinned")] public bool IsPinned { get; set; }
    }

    public string TemplateName(Guid? id) => Templates.FirstOrDefault(t => t.Id == id)?.Name ?? "-";
    public string MessageSubject(Guid id) => Messages.FirstOrDefault(m => m.Id == id)?.Subject ?? "-";

    public async Task OnGetAsync()
    {
        if (!HasTenant) return;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveProviderAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(ProviderInput));
        await ValidateOptionalBranchAsync("ProviderInput.BranchId", ProviderInput.BranchId);
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _providers.CreateAsync(new NotificationProviderSetting
        {
            BranchId = ProviderInput.BranchId,
            Channel = ProviderInput.Channel,
            ProviderKey = ProviderInput.ProviderKey,
            DisplayName = ProviderInput.DisplayName,
            FromAddress = ProviderInput.FromAddress,
            FromDisplayName = ProviderInput.FromDisplayName,
            SecretReference = ProviderInput.SecretReference,
            IsEnabled = true
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveTemplateAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(TemplateInput));
        await ValidateOptionalBranchAsync("TemplateInput.BranchId", TemplateInput.BranchId);
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _templates.CreateAsync(new NotificationTemplate
        {
            BranchId = TemplateInput.BranchId,
            Name = TemplateInput.Name,
            Code = TemplateInput.Code,
            Channel = TemplateInput.Channel,
            SubjectTemplate = TemplateInput.SubjectTemplate,
            BodyTemplate = TemplateInput.BodyTemplate,
            IsActive = true
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveMessageAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(MessageInput));
        await ValidateOptionalBranchAsync("MessageInput.BranchId", MessageInput.BranchId);
        if (MessageInput.NotificationTemplateId is Guid templateId && await _templates.GetAsync(templateId) is { } template && template.BranchId != MessageInput.BranchId)
            ModelState.AddModelError("MessageInput.NotificationTemplateId", "Template must match the branch scope.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _messages.CreateAsync(new NotificationMessage
        {
            BranchId = MessageInput.BranchId,
            NotificationTemplateId = MessageInput.NotificationTemplateId,
            Channel = MessageInput.Channel,
            Subject = MessageInput.Subject,
            Body = MessageInput.Body,
            Status = MessageInput.Status,
            ScheduledOn = MessageInput.ScheduledOn,
            ProviderKey = MessageInput.ProviderKey
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveRecipientAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(RecipientInput));
        await ValidateOptionalBranchAsync("RecipientInput.BranchId", RecipientInput.BranchId);
        if (RecipientInput.NotificationMessageId is not Guid messageId || await _messages.GetAsync(messageId) is not { } message || message.BranchId != RecipientInput.BranchId)
            ModelState.AddModelError("RecipientInput.NotificationMessageId", "Message must match the branch scope.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _recipients.CreateAsync(new NotificationRecipient
        {
            BranchId = RecipientInput.BranchId,
            NotificationMessageId = RecipientInput.NotificationMessageId!.Value,
            DisplayName = RecipientInput.DisplayName,
            DestinationAddress = RecipientInput.DestinationAddress,
            Status = RecipientInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveAnnouncementAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(AnnouncementInput));
        await ValidateOptionalBranchAsync("AnnouncementInput.BranchId", AnnouncementInput.BranchId);
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _announcements.CreateAsync(new Announcement
        {
            BranchId = AnnouncementInput.BranchId,
            Title = AnnouncementInput.Title,
            Message = AnnouncementInput.Message,
            Audience = AnnouncementInput.Audience,
            PublishOn = AnnouncementInput.PublishOn,
            IsPinned = AnnouncementInput.IsPinned,
            IsActive = true
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteProviderAsync(Guid id) { await DeleteIfAllowedAsync(_providers, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteTemplateAsync(Guid id) { await DeleteIfAllowedAsync(_templates, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteMessageAsync(Guid id) { await DeleteIfAllowedAsync(_messages, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteRecipientAsync(Guid id) { await DeleteIfAllowedAsync(_recipients, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteAnnouncementAsync(Guid id) { await DeleteIfAllowedAsync(_announcements, id, x => x.BranchId); return RedirectToPage(); }

    public async Task<IActionResult> OnPostDispatchMessageAsync(Guid id)
    {
        var message = await _messages.GetAsync(id);
        if (message is not null && await CanUseBranchAsync(message.BranchId))
            await _dispatcher.DispatchAsync(id, HttpContext.RequestAborted);
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        await LoadBranchesAsync(_branches);
        Providers = (await FilterBranchScopedAsync(_providers, x => x.BranchId)).OrderBy(p => p.Channel).ThenBy(p => p.ProviderKey).ToList();
        Templates = (await FilterBranchScopedAsync(_templates, x => x.BranchId)).OrderBy(t => t.Name).ToList();
        Messages = (await FilterBranchScopedAsync(_messages, x => x.BranchId)).OrderByDescending(m => m.CreatedOn).ToList();
        Recipients = (await FilterBranchScopedAsync(_recipients, x => x.BranchId)).OrderBy(r => r.DisplayName).ToList();
        Announcements = (await FilterBranchScopedAsync(_announcements, x => x.BranchId)).OrderByDescending(a => a.PublishOn).ToList();
        Logs = (await FilterBranchScopedAsync(_logs, x => x.BranchId)).OrderByDescending(l => l.OccurredOn).Take(20).ToList();

        if (await DefaultBranchIdAsync() is Guid branchId)
        {
            ProviderInput.BranchId ??= branchId;
            TemplateInput.BranchId ??= branchId;
            MessageInput.BranchId ??= branchId;
            RecipientInput.BranchId ??= branchId;
            AnnouncementInput.BranchId ??= branchId;
        }
    }

    private async Task ValidateOptionalBranchAsync(string fieldName, Guid? branchId)
    {
        if (BranchAccess.IsBranchAdminOnly(User) && !branchId.HasValue)
        {
            ModelState.AddModelError(fieldName, "Branch is required for branch-scoped users.");
            return;
        }

        if (branchId is Guid id)
            await ValidateBranchSelectionAsync(fieldName, id, _branches);
    }

    private async Task DeleteIfAllowedAsync<T>(ICrudService<T> service, Guid id, Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        if (entity is not null && await CanUseBranchAsync(branchSelector(entity)))
            await service.SoftDeleteAsync(id);
    }
}
