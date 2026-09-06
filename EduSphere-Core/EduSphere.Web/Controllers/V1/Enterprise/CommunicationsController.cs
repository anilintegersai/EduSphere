using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/communications")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.CommunicationManager)]
public class CommunicationsController : EnterpriseControllerBase
{
    private readonly ICrudService<NotificationProviderSetting> _providers;
    private readonly ICrudService<NotificationTemplate> _templates;
    private readonly ICrudService<NotificationMessage> _messages;
    private readonly ICrudService<NotificationRecipient> _recipients;
    private readonly ICrudService<Announcement> _announcements;
    private readonly ICrudService<CommunicationLog> _logs;
    private readonly ICrudService<Branch> _branches;
    private readonly INotificationDispatcher _dispatcher;

    public CommunicationsController(
        ICrudService<NotificationProviderSetting> providers,
        ICrudService<NotificationTemplate> templates,
        ICrudService<NotificationMessage> messages,
        ICrudService<NotificationRecipient> recipients,
        ICrudService<Announcement> announcements,
        ICrudService<CommunicationLog> logs,
        ICrudService<Branch> branches,
        INotificationDispatcher dispatcher,
        IBranchAccessService branchAccess)
        : base(branchAccess)
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

    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<NotificationProviderSettingDto>>.Ok((await FilterByBranchAsync(_providers, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("providers")]
    public async Task<IActionResult> CreateProvider([FromBody] CreateNotificationProviderSettingRequest request)
    {
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _providers.CreateAsync(Apply(new NotificationProviderSetting(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<NotificationProviderSettingDto>.Ok(created.Map()));
    }

    [HttpPut("providers/{id:guid}")]
    public async Task<IActionResult> UpdateProvider(Guid id, [FromBody] UpdateNotificationProviderSettingRequest request)
    {
        if (await GetBranchScopedAsync(_providers, id, x => x.BranchId) is null) return NotFoundResult(id, "Provider setting");
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _providers.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("providers/{id:guid}")]
    public Task<IActionResult> DeleteProvider(Guid id)
        => DeleteBranchScopedAsync(_providers, id, x => x.BranchId, "Provider setting");

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.Ok((await FilterByBranchAsync(_templates, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("templates")]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateNotificationTemplateRequest request)
    {
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _templates.CreateAsync(Apply(new NotificationTemplate(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<NotificationTemplateDto>.Ok(created.Map()));
    }

    [HttpPut("templates/{id:guid}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateNotificationTemplateRequest request)
    {
        if (await GetBranchScopedAsync(_templates, id, x => x.BranchId) is null) return NotFoundResult(id, "Notification template");
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _templates.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("templates/{id:guid}")]
    public Task<IActionResult> DeleteTemplate(Guid id)
        => DeleteBranchScopedAsync(_templates, id, x => x.BranchId, "Notification template");

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<NotificationMessageDto>>.Ok((await FilterByBranchAsync(_messages, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("messages")]
    public async Task<IActionResult> CreateMessage([FromBody] CreateNotificationMessageRequest request)
    {
        if (await ValidateMessageAsync(request.NotificationTemplateId, request.BranchId) is { } invalid) return invalid;
        var created = await _messages.CreateAsync(Apply(new NotificationMessage(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<NotificationMessageDto>.Ok(created.Map()));
    }

    [HttpPut("messages/{id:guid}")]
    public async Task<IActionResult> UpdateMessage(Guid id, [FromBody] UpdateNotificationMessageRequest request)
    {
        if (await GetBranchScopedAsync(_messages, id, x => x.BranchId) is null) return NotFoundResult(id, "Notification message");
        if (await ValidateMessageAsync(request.NotificationTemplateId, request.BranchId) is { } invalid) return invalid;
        await _messages.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("messages/{id:guid}")]
    public Task<IActionResult> DeleteMessage(Guid id)
        => DeleteBranchScopedAsync(_messages, id, x => x.BranchId, "Notification message");

    [HttpPost("messages/{id:guid}/dispatch")]
    public async Task<IActionResult> DispatchMessage(Guid id)
    {
        var message = await GetBranchScopedAsync(_messages, id, x => x.BranchId);
        if (message is null)
            return NotFoundResult(id, "Notification message");

        var result = await _dispatcher.DispatchAsync(id, HttpContext.RequestAborted);
        return result.Succeeded
            ? Ok(ApiResponse<NotificationDispatchResult>.Ok(result))
            : BadRequest(ApiResponse<NotificationDispatchResult>.Fail(result.ErrorMessage ?? "Dispatch failed."));
    }

    [HttpGet("recipients")]
    public async Task<IActionResult> GetRecipients([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<NotificationRecipientDto>>.Ok((await FilterByBranchAsync(_recipients, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("recipients")]
    public async Task<IActionResult> CreateRecipient([FromBody] CreateNotificationRecipientRequest request)
    {
        if (await ValidateRecipientAsync(request.NotificationMessageId, request.BranchId) is { } invalid) return invalid;
        var created = await _recipients.CreateAsync(Apply(new NotificationRecipient(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<NotificationRecipientDto>.Ok(created.Map()));
    }

    [HttpPut("recipients/{id:guid}")]
    public async Task<IActionResult> UpdateRecipient(Guid id, [FromBody] UpdateNotificationRecipientRequest request)
    {
        if (await GetBranchScopedAsync(_recipients, id, x => x.BranchId) is null) return NotFoundResult(id, "Notification recipient");
        if (await ValidateRecipientAsync(request.NotificationMessageId, request.BranchId) is { } invalid) return invalid;
        await _recipients.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("recipients/{id:guid}")]
    public Task<IActionResult> DeleteRecipient(Guid id)
        => DeleteBranchScopedAsync(_recipients, id, x => x.BranchId, "Notification recipient");

    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<AnnouncementDto>>.Ok((await FilterByBranchAsync(_announcements, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("announcements")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
    {
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _announcements.CreateAsync(Apply(new Announcement(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AnnouncementDto>.Ok(created.Map()));
    }

    [HttpPut("announcements/{id:guid}")]
    public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] UpdateAnnouncementRequest request)
    {
        if (await GetBranchScopedAsync(_announcements, id, x => x.BranchId) is null) return NotFoundResult(id, "Announcement");
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _announcements.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("announcements/{id:guid}")]
    public Task<IActionResult> DeleteAnnouncement(Guid id)
        => DeleteBranchScopedAsync(_announcements, id, x => x.BranchId, "Announcement");

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<CommunicationLogDto>>.Ok((await FilterByBranchAsync(_logs, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("logs")]
    public async Task<IActionResult> CreateLog([FromBody] CreateCommunicationLogRequest request)
    {
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _logs.CreateAsync(Apply(new CommunicationLog(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<CommunicationLogDto>.Ok(created.Map()));
    }

    [HttpPut("logs/{id:guid}")]
    public async Task<IActionResult> UpdateLog(Guid id, [FromBody] UpdateCommunicationLogRequest request)
    {
        if (await GetBranchScopedAsync(_logs, id, x => x.BranchId) is null) return NotFoundResult(id, "Communication log");
        if (await ValidateOptionalBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _logs.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("logs/{id:guid}")]
    public Task<IActionResult> DeleteLog(Guid id)
        => DeleteBranchScopedAsync(_logs, id, x => x.BranchId, "Communication log");

    private async Task<IActionResult?> ValidateOptionalBranchAsync(Guid? branchId)
    {
        if (BranchAccess.IsBranchAdminOnly(User) && !branchId.HasValue)
            return BadRequest(ApiResponse<object>.Fail("A branch is required for branch-scoped users."));

        if (branchId is not Guid checkedBranchId)
            return null;

        if (await _branches.GetAsync(checkedBranchId) is null)
            return BadRequest(ApiResponse<object>.Fail("Selected branch was not found in this tenant."));

        return await CanUseBranchAsync(checkedBranchId) ? null : Forbid();
    }

    private async Task<IActionResult?> ValidateMessageAsync(Guid? templateId, Guid? branchId)
    {
        if (await ValidateOptionalBranchAsync(branchId) is { } invalid) return invalid;
        if (templateId is Guid checkedTemplateId && await _templates.GetAsync(checkedTemplateId) is { } template)
        {
            if (template.BranchId != branchId)
                return BadRequest(ApiResponse<object>.Fail("Selected template must belong to the same branch scope."));
        }
        else if (templateId.HasValue)
        {
            return BadRequest(ApiResponse<object>.Fail("Selected template was not found."));
        }
        return null;
    }

    private async Task<IActionResult?> ValidateRecipientAsync(Guid messageId, Guid? branchId)
    {
        if (await ValidateOptionalBranchAsync(branchId) is { } invalid) return invalid;
        if (await _messages.GetAsync(messageId) is not { } message)
            return BadRequest(ApiResponse<object>.Fail("Selected message was not found."));
        if (message.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Recipient must belong to the same branch scope as the message."));
        return null;
    }

    private NotFoundObjectResult NotFoundResult(Guid id, string displayName)
        => NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

    private static NotificationProviderSetting Apply(NotificationProviderSetting entity, CreateNotificationProviderSettingRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Channel = request.Channel;
        entity.ProviderKey = request.ProviderKey;
        entity.DisplayName = request.DisplayName;
        entity.FromAddress = request.FromAddress;
        entity.FromDisplayName = request.FromDisplayName;
        entity.ConfigurationJson = request.ConfigurationJson;
        entity.SecretReference = request.SecretReference;
        entity.IsEnabled = request.IsEnabled;
        return entity;
    }

    private static NotificationTemplate Apply(NotificationTemplate entity, CreateNotificationTemplateRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Channel = request.Channel;
        entity.SubjectTemplate = request.SubjectTemplate;
        entity.BodyTemplate = request.BodyTemplate;
        entity.VariablesJson = request.VariablesJson;
        entity.IsActive = request.IsActive;
        return entity;
    }

    private static NotificationMessage Apply(NotificationMessage entity, CreateNotificationMessageRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.NotificationTemplateId = request.NotificationTemplateId;
        entity.Channel = request.Channel;
        entity.Subject = request.Subject;
        entity.Body = request.Body;
        entity.Status = request.Status;
        entity.ScheduledOn = request.ScheduledOn;
        entity.SentOn = request.SentOn;
        entity.ProviderKey = request.ProviderKey;
        entity.ProviderMessageId = request.ProviderMessageId;
        entity.ErrorMessage = request.ErrorMessage;
        entity.CreatedForUserId = request.CreatedForUserId;
        return entity;
    }

    private static NotificationRecipient Apply(NotificationRecipient entity, CreateNotificationRecipientRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.NotificationMessageId = request.NotificationMessageId;
        entity.UserId = request.UserId;
        entity.DisplayName = request.DisplayName;
        entity.DestinationAddress = request.DestinationAddress;
        entity.Status = request.Status;
        entity.SentOn = request.SentOn;
        entity.ErrorMessage = request.ErrorMessage;
        return entity;
    }

    private static Announcement Apply(Announcement entity, CreateAnnouncementRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Title = request.Title;
        entity.Message = request.Message;
        entity.Audience = request.Audience;
        entity.PublishOn = request.PublishOn;
        entity.ExpireOn = request.ExpireOn;
        entity.IsPinned = request.IsPinned;
        entity.IsActive = request.IsActive;
        entity.CreatedByUserId = request.CreatedByUserId;
        return entity;
    }

    private static CommunicationLog Apply(CommunicationLog entity, CreateCommunicationLogRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.Channel = request.Channel;
        entity.Direction = request.Direction;
        entity.Recipient = request.Recipient;
        entity.Subject = request.Subject;
        entity.Status = request.Status;
        entity.ProviderKey = request.ProviderKey;
        entity.ProviderMessageId = request.ProviderMessageId;
        entity.PayloadSummary = request.PayloadSummary;
        entity.ErrorMessage = request.ErrorMessage;
        entity.OccurredOn = request.OccurredOn;
        return entity;
    }
}
