using System.ComponentModel.DataAnnotations;
using EduSphere.Application.DTOs.AI;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EduSphere.Web.Security;
using EduSphere.Web.Services;

namespace EduSphere.Web.Pages.AI;

[Authorize(Policy = AuthorizationPolicies.AIQuestionPaperManager)]
public sealed class QuestionPapersModel : PageModel
{
    private const string DefaultSystemPrompt = AIQuestionPaperDefaultsBootstrapper.SystemPrompt;
    private const string DefaultUserPrompt = AIQuestionPaperDefaultsBootstrapper.UserPrompt;
    private const string DefaultSchema = AIQuestionPaperDefaultsBootstrapper.OutputSchema;

    private readonly TenantDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IAIQuestionPaperService _service;
    private readonly IAISecretProtector _secretProtector;
    private readonly IBranchAccessService _branchAccess;

    public QuestionPapersModel(TenantDbContext db, ITenantContext tenant, IAIQuestionPaperService service, IAISecretProtector secretProtector, IBranchAccessService branchAccess)
    {
        _db = db; _tenant = tenant; _service = service; _secretProtector = secretProtector; _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public bool CanConfigure => User.IsInRole(Roles.SuperAdmin) || User.IsInRole(Roles.TenantAdmin);
    public List<Branch> Branches { get; private set; } = [];
    public List<Subject> Subjects { get; private set; } = [];
    public List<SyllabusUnit> Units { get; private set; } = [];
    public List<AIProviderSetting> Providers { get; private set; } = [];
    public List<AIPromptTemplate> Templates { get; private set; } = [];
    public List<AIGenerationRequest> Requests { get; private set; } = [];
    public List<QuestionPaperVersion> Versions { get; private set; } = [];
    public string? CompareLeft { get; private set; }
    public string? CompareRight { get; private set; }

    [BindProperty] public GenerationForm Generate { get; set; } = new();
    [BindProperty] public ProviderForm Provider { get; set; } = new();
    [BindProperty] public TemplateForm Template { get; set; } = new();
    [BindProperty] public VersionEditForm VersionEdit { get; set; } = new();

    public async Task OnGetAsync(Guid? subjectId, Guid? leftVersionId, Guid? rightVersionId)
    {
        if (!HasTenant) return;
        await EnsureDefaultTemplateAsync();
        await LoadAsync(subjectId);
        if (leftVersionId.HasValue) CompareLeft = (await _db.QuestionPaperVersions.AsNoTracking().FirstOrDefaultAsync(v => v.Id == leftVersionId))?.StudentContentJson;
        if (rightVersionId.HasValue) CompareRight = (await _db.QuestionPaperVersions.AsNoTracking().FirstOrDefaultAsync(v => v.Id == rightVersionId))?.StudentContentJson;
    }

    public async Task<IActionResult> OnPostGenerateAsync()
    {
        if (!HasTenant) return RedirectToPage();
        if (!await _branchAccess.CanAccessBranchAsync(User, Generate.BranchId)) return Forbid();
        var unitIds = ParseGuids(Generate.SyllabusUnitIds);
        if (unitIds.Count == 0) ModelState.AddModelError(nameof(Generate.SyllabusUnitIds), "Select at least one syllabus unit.");
        if (!ModelState.IsValid) { await LoadAsync(Generate.SubjectId); return Page(); }
        try
        {
            var outcome = await _service.GenerateAsync(new AIQuestionPaperGenerationInput
            {
                BranchId = Generate.BranchId, SubjectId = Generate.SubjectId, QuestionPaperId = Generate.QuestionPaperId,
                ProviderSettingId = Generate.ProviderSettingId, PromptTemplateId = Generate.PromptTemplateId,
                RegeneratedFromRequestId = Generate.RegeneratedFromRequestId, Title = Generate.Title,
                TotalMarks = Generate.TotalMarks, DurationMinutes = Generate.DurationMinutes, Instructions = Generate.Instructions,
                SyllabusUnitIds = unitIds,
                BloomDistribution = ParseDistribution<BloomLevel>(Generate.BloomDistribution),
                DifficultyDistribution = ParseDistribution<QuestionDifficulty>(Generate.DifficultyDistribution),
                QuestionTypeCounts = ParseDistribution<QuestionType>(Generate.QuestionTypeCounts),
                Language = Generate.Language, AllowManualFallback = Generate.AllowManualFallback
            });
            TempData["StatusMessage"] = outcome.Warning ?? "AI paper generated as a reviewable draft.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { subjectId = Generate.SubjectId });
    }

    public async Task<IActionResult> OnPostSaveProviderAsync()
    {
        if (!CanConfigure) return Forbid();
        if (!ModelState.IsValid) { Provider.ApiKey = string.Empty; ModelState.Remove("Provider.ApiKey"); await LoadAsync(null); return Page(); }
        AIProviderSetting entity;
        if (Provider.Id.HasValue)
        {
            entity = await _db.AIProviderSettings.SingleAsync(p => p.Id == Provider.Id);
            if (!string.IsNullOrWhiteSpace(Provider.ApiKey)) entity.ProtectedApiKey = _secretProtector.Protect(Provider.ApiKey);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Provider.ApiKey)) { ModelState.AddModelError(nameof(Provider.ApiKey), "API key is required for a new provider."); await LoadAsync(null); return Page(); }
            entity = new AIProviderSetting { TenantId = _tenant.TenantId!.Value, ProtectedApiKey = _secretProtector.Protect(Provider.ApiKey) };
            _db.AIProviderSettings.Add(entity);
        }
        entity.Name = Provider.Name; entity.ProviderType = Provider.ProviderType; entity.Endpoint = Provider.Endpoint;
        entity.ModelOrDeployment = Provider.ModelOrDeployment; entity.ApiVersion = Provider.ApiVersion; entity.Region = Provider.Region;
        entity.DataResidencyPolicy = Provider.DataResidencyPolicy; entity.RequiredRegion = Provider.RequiredRegion;
        entity.IsEnabled = Provider.IsEnabled; entity.IsDefault = Provider.IsDefault; entity.TimeoutSeconds = Provider.TimeoutSeconds;
        entity.InputCostPerMillionTokens = Provider.InputCostPerMillionTokens; entity.OutputCostPerMillionTokens = Provider.OutputCostPerMillionTokens;
        if (entity.IsDefault) await _db.AIProviderSettings.Where(p => p.Id != entity.Id).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "AI provider configuration saved. The API key remains encrypted at rest.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRegenerateAsync(Guid requestId)
    {
        var prior = await _db.AIGenerationRequests.AsNoTracking().SingleAsync(r => r.Id == requestId);
        if (!await _branchAccess.CanAccessBranchAsync(User, prior.BranchId)) return Forbid();
        try
        {
            var input = System.Text.Json.JsonSerializer.Deserialize<AIQuestionPaperGenerationInput>(prior.InputParametersJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } })
                ?? throw new InvalidOperationException("The original generation input could not be restored.");
            input.RegeneratedFromRequestId = prior.Id;
            input.QuestionPaperId = prior.QuestionPaperId;
            var outcome = await _service.GenerateAsync(input);
            TempData["StatusMessage"] = outcome.Warning ?? "A new AI-generated version was created from the original brief.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveTemplateAsync()
    {
        if (!CanConfigure) return Forbid();
        if (!ModelState.IsValid) { await LoadAsync(null); return Page(); }
        var next = (await _db.AIPromptTemplates.Where(t => t.Code == Template.Code).MaxAsync(t => (int?)t.Version) ?? 0) + 1;
        await _db.AIPromptTemplates.Where(t => t.Code == Template.Code && t.IsActive).ExecuteUpdateAsync(s => s.SetProperty(t => t.IsActive, false));
        _db.AIPromptTemplates.Add(new AIPromptTemplate
        {
            TenantId = _tenant.TenantId!.Value, BranchId = Template.BranchId, Code = Template.Code, Name = Template.Name,
            Version = next, IsActive = true, SystemPrompt = Template.SystemPrompt,
            UserPromptTemplate = Template.UserPromptTemplate, OutputSchemaJson = Template.OutputSchemaJson
        });
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = $"Prompt template version {next} created.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(Guid paperId, Guid versionId, ApprovalStatus status)
    {
        var paper = await _db.QuestionPapers.SingleAsync(p => p.Id == paperId);
        var version = await _db.QuestionPaperVersions.SingleAsync(v => v.Id == versionId && v.QuestionPaperId == paperId);
        paper.Status = status; version.Status = status;
        if (status == ApprovalStatus.Approved) paper.ApprovedOn = DateTime.UtcNow;
        _db.QuestionPaperModerations.Add(new QuestionPaperModeration { TenantId = paper.TenantId, QuestionPaperId = paper.Id, Decision = status, Notes = "AI workflow decision." });
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = $"Paper version {version.VersionNumber} moved to {status}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditVersionAsync()
    {
        try
        {
            var version = await _db.QuestionPaperVersions.SingleAsync(v => v.Id == VersionEdit.VersionId);
            _ = System.Text.Json.JsonDocument.Parse(VersionEdit.StudentContentJson);
            _ = System.Text.Json.JsonDocument.Parse(VersionEdit.SolutionContentJson);
            version.StudentContentJson = VersionEdit.StudentContentJson;
            version.SolutionContentJson = VersionEdit.SolutionContentJson;
            version.Status = ApprovalStatus.Draft;
            version.Notes = VersionEdit.Notes;
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "Draft content updated and returned to review.";
        }
        catch (System.Text.Json.JsonException ex) { TempData["ErrorMessage"] = "The edited content is not valid JSON: " + ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRollbackAsync(Guid paperId, Guid versionId)
    {
        await _service.RollbackAsync(paperId, versionId);
        TempData["StatusMessage"] = "Selected version restored as a new draft.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEnrichAsync(Guid paperId, Guid versionId)
    {
        try { TempData["StatusMessage"] = $"Added {await _service.EnrichQuestionBankAsync(paperId, versionId)} approved questions to the bank."; }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage();
    }

    private async Task LoadAsync(Guid? subjectId)
    {
        Branches = (await _branchAccess.FilterBranchesAsync(User, await _db.Branches.AsNoTracking().ToListAsync())).ToList();
        Subjects = await _db.Subjects.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
        var selectedSubject = subjectId ?? Generate.SubjectId;
        Units = selectedSubject == Guid.Empty ? [] : await _db.SyllabusUnits.AsNoTracking().Where(u => u.SubjectId == selectedSubject).OrderBy(u => u.Order).ToListAsync();
        Providers = await _db.AIProviderSettings.AsNoTracking().OrderByDescending(p => p.IsDefault).ThenBy(p => p.Name).ToListAsync();
        Templates = await _db.AIPromptTemplates.AsNoTracking().OrderByDescending(t => t.IsActive).ThenBy(t => t.Name).ThenByDescending(t => t.Version).ToListAsync();
        Requests = await _db.AIGenerationRequests.AsNoTracking().Include(r => r.Subject).Include(r => r.QuestionPaper).Include(r => r.ProviderSetting).Include(r => r.UsageRecords)
            .OrderByDescending(r => r.RequestedOn).Take(50).ToListAsync();
        var paperIds = Requests.Where(r => r.QuestionPaperId.HasValue).Select(r => r.QuestionPaperId!.Value).Distinct().ToList();
        Versions = await _db.QuestionPaperVersions.AsNoTracking().Where(v => paperIds.Contains(v.QuestionPaperId)).OrderByDescending(v => v.VersionNumber).ToListAsync();
    }

    private async Task EnsureDefaultTemplateAsync()
    {
        if (await _db.AIPromptTemplates.AnyAsync()) return;
        _db.AIPromptTemplates.Add(new AIPromptTemplate { TenantId = _tenant.TenantId!.Value, Code = "GENERAL-QUESTION-PAPER", Name = "General question paper", Version = 1, IsActive = true, SystemPrompt = DefaultSystemPrompt, UserPromptTemplate = DefaultUserPrompt, OutputSchemaJson = DefaultSchema });
        await _db.SaveChangesAsync();
    }

    private static List<Guid> ParseGuids(string value) => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Guid.Parse).ToList();
    private static Dictionary<TEnum, int> ParseDistribution<TEnum>(string value) where TEnum : struct, Enum =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.Split(':', 2, StringSplitOptions.TrimEntries))
            .ToDictionary(x => Enum.Parse<TEnum>(x[0], true), x => int.Parse(x[1]));

    public sealed class GenerationForm
    {
        [Required] public Guid BranchId { get; set; }
        [Required] public Guid SubjectId { get; set; }
        public Guid? QuestionPaperId { get; set; }
        public Guid? ProviderSettingId { get; set; }
        [Required] public Guid PromptTemplateId { get; set; }
        public Guid? RegeneratedFromRequestId { get; set; }
        [Required, StringLength(150)] public string Title { get; set; } = "AI-generated assessment";
        [Range(1, 1000)] public decimal TotalMarks { get; set; } = 100;
        [Range(15, 600)] public int DurationMinutes { get; set; } = 180;
        public string? Instructions { get; set; }
        [Required] public string SyllabusUnitIds { get; set; } = string.Empty;
        public string BloomDistribution { get; set; } = "Remember:20,Understand:30,Apply:30,Analyze:20";
        public string DifficultyDistribution { get; set; } = "Easy:30,Medium:50,Hard:20";
        public string QuestionTypeCounts { get; set; } = "MultipleChoice:10,ShortAnswer:5,LongAnswer:3";
        public string Language { get; set; } = "English";
        public bool AllowManualFallback { get; set; } = true;
    }
    public sealed class ProviderForm
    {
        public Guid? Id { get; set; }
        [Required] public string Name { get; set; } = "Primary OpenAI";
        public AIProviderType ProviderType { get; set; }
        [Required] public string Endpoint { get; set; } = "https://api.openai.com";
        [Required] public string ModelOrDeployment { get; set; } = "gpt-4o-mini";
        [DataType(DataType.Password)] public string ApiKey { get; set; } = string.Empty;
        public string? ApiVersion { get; set; }
        public string? Region { get; set; }
        public AIDataResidencyPolicy DataResidencyPolicy { get; set; }
        public string? RequiredRegion { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsDefault { get; set; } = true;
        [Range(10, 300)] public int TimeoutSeconds { get; set; } = 90;
        public decimal InputCostPerMillionTokens { get; set; }
        public decimal OutputCostPerMillionTokens { get; set; }
    }
    public sealed class TemplateForm
    {
        public Guid? BranchId { get; set; }
        [Required] public string Code { get; set; } = "GENERAL-QUESTION-PAPER";
        [Required] public string Name { get; set; } = "General question paper";
        [Required] public string SystemPrompt { get; set; } = DefaultSystemPrompt;
        [Required] public string UserPromptTemplate { get; set; } = DefaultUserPrompt;
        [Required] public string OutputSchemaJson { get; set; } = DefaultSchema;
    }
    public sealed class VersionEditForm
    {
        public Guid VersionId { get; set; }
        [Required] public string StudentContentJson { get; set; } = "{}";
        [Required] public string SolutionContentJson { get; set; } = "{}";
        public string? Notes { get; set; }
    }
}
