using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Administration;

[Authorize(Policy = AdministrationPolicies.View)]
public sealed class IndexModel : PageModel
{
    private readonly TenantDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IAuthorizationService _authorization;
    private readonly IDataProtector _secretProtector;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(TenantDbContext db, ITenantContext tenant, IAuthorizationService authorization, IDataProtectionProvider dataProtection, IWebHostEnvironment environment)
    {
        _db = db; _tenant = tenant; _authorization = authorization;
        _secretProtector = dataProtection.CreateProtector("EduSphere.Administration.IntegrationSecrets.v1");
        _environment = environment;
    }

    public bool HasTenant => _tenant.HasTenant;
    public bool CanManageTenant { get; private set; }
    public bool CanManageGlobal { get; private set; }
    public bool CanManagePermissions { get; private set; }
    public List<SubscriptionPlan> Plans { get; private set; } = [];
    public TenantSubscription? CurrentSubscription { get; private set; }
    public TenantBranding? Branding { get; private set; }
    public List<TenantDomain> Domains { get; private set; } = [];
    public List<TenantIntegrationSetting> Integrations { get; private set; } = [];
    public List<TenantFeatureFlag> Features { get; private set; } = [];
    public List<Branch> Branches { get; private set; } = [];
    public List<BranchContact> Contacts { get; private set; } = [];
    public List<BranchAcademicConfig> BranchConfigs { get; private set; } = [];
    public List<AppSetting> AppSettings { get; private set; } = [];
    public List<TenantSetting> TenantSettings { get; private set; } = [];
    public List<LookupItem> Lookups { get; private set; } = [];
    public List<AuditLog> AuditLogs { get; private set; } = [];
    public List<BackgroundJobLog> Jobs { get; private set; } = [];
    public List<PermissionDefinition> Permissions { get; private set; } = [];
    public List<ApplicationRole> Roles { get; private set; } = [];
    public List<ApplicationUser> Users { get; private set; } = [];
    public List<RolePermissionGrant> RoleGrants { get; private set; } = [];
    public List<UserPermissionGrant> UserGrants { get; private set; } = [];

    [BindProperty] public PlanForm Plan { get; set; } = new();
    [BindProperty] public SubscriptionForm Subscription { get; set; } = new();
    [BindProperty] public BrandingForm Brand { get; set; } = new();
    [BindProperty] public DomainForm Domain { get; set; } = new();
    [BindProperty] public IntegrationForm Integration { get; set; } = new();
    [BindProperty] public FeatureForm Feature { get; set; } = new();
    [BindProperty] public ContactForm Contact { get; set; } = new();
    [BindProperty] public BranchConfigForm BranchConfig { get; set; } = new();
    [BindProperty] public SettingForm Setting { get; set; } = new();
    [BindProperty] public LookupForm Lookup { get; set; } = new();
    [BindProperty] public PermissionGrantForm Grant { get; set; } = new();

    public async Task OnGetAsync(string? auditEntity, string? auditActor, BackgroundJobStatus? jobStatus)
    {
        await LoadAsync(auditEntity, auditActor, jobStatus);
    }

    public async Task<IActionResult> OnPostSavePlanAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageGlobal)) return Forbid();
        ValidateJson(Plan.IncludedFeaturesJson, nameof(Plan.IncludedFeaturesJson));
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var entity = Plan.Id.HasValue ? await _db.SubscriptionPlans.SingleAsync(p => p.Id == Plan.Id) : new SubscriptionPlan();
        if (!Plan.Id.HasValue) _db.SubscriptionPlans.Add(entity);
        entity.Code = Plan.Code.Trim().ToUpperInvariant(); entity.Name = Plan.Name; entity.Description = Plan.Description;
        entity.MonthlyPrice = Plan.MonthlyPrice; entity.AnnualPrice = Plan.AnnualPrice; entity.MaxBranches = Plan.MaxBranches;
        entity.MaxStudents = Plan.MaxStudents; entity.MaxStaff = Plan.MaxStaff; entity.IncludedFeaturesJson = Plan.IncludedFeaturesJson; entity.IsActive = Plan.IsActive;
        await _db.SaveChangesAsync(); return Success("Subscription plan saved.");
    }

    public async Task<IActionResult> OnPostAssignSubscriptionAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageGlobal) || !HasTenant) return Forbid();
        var current = await _db.TenantSubscriptions.Where(s => s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial).ToListAsync();
        foreach (var item in current) { item.Status = SubscriptionStatus.Cancelled; item.EndsOn ??= DateOnly.FromDateTime(DateTime.UtcNow); }
        _db.TenantSubscriptions.Add(new TenantSubscription
        {
            TenantId = _tenant.TenantId!.Value, SubscriptionPlanId = Subscription.SubscriptionPlanId, Status = Subscription.Status,
            StartsOn = Subscription.StartsOn, EndsOn = Subscription.EndsOn, AutoRenew = Subscription.AutoRenew,
            AgreedMonthlyPrice = Subscription.AgreedMonthlyPrice
        });
        await _db.SaveChangesAsync(); return Success("Tenant subscription assigned.");
    }

    public async Task<IActionResult> OnPostSaveBrandingAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        var entity = await _db.TenantBrandings.SingleOrDefaultAsync() ?? new TenantBranding { TenantId = _tenant.TenantId!.Value };
        if (entity.Id == Guid.Empty) _db.TenantBrandings.Add(entity);
        entity.PrimaryColor = Brand.PrimaryColor; entity.SecondaryColor = Brand.SecondaryColor; entity.AccentColor = Brand.AccentColor;
        entity.FontFamily = Brand.FontFamily; entity.EmailFromName = Brand.EmailFromName; entity.EmailHeaderHtml = Brand.EmailHeaderHtml;
        entity.EmailFooterHtml = Brand.EmailFooterHtml; entity.LoginWelcomeText = Brand.LoginWelcomeText;
        if (Brand.LogoFile is not null) entity.LogoUrl = await SaveBrandAssetAsync(Brand.LogoFile, "logo");
        if (Brand.FaviconFile is not null) entity.FaviconUrl = await SaveBrandAssetAsync(Brand.FaviconFile, "favicon");
        await _db.SaveChangesAsync(); return Success("Tenant branding saved.");
    }

    public async Task<IActionResult> OnPostSaveDomainAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        var name = Domain.DomainName.Trim().ToLowerInvariant();
        if (await _db.TenantDomains.IgnoreQueryFilters().AnyAsync(d => d.DomainName == name)) ModelState.AddModelError(nameof(Domain.DomainName), "This domain is already mapped.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        if (Domain.IsPrimary) await _db.TenantDomains.Where(d => d.IsPrimary).ExecuteUpdateAsync(s => s.SetProperty(d => d.IsPrimary, false));
        _db.TenantDomains.Add(new TenantDomain { TenantId = _tenant.TenantId!.Value, DomainName = name, IsPrimary = Domain.IsPrimary, VerificationToken = "edusphere-verify=" + Guid.NewGuid().ToString("N") });
        await _db.SaveChangesAsync(); return Success("Domain mapped. Publish the displayed verification token before marking it verified.");
    }

    public async Task<IActionResult> OnPostVerifyDomainAsync(Guid id)
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant)) return Forbid();
        var domain = await _db.TenantDomains.SingleAsync(d => d.Id == id); domain.IsVerified = true; domain.VerifiedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return Success("Domain marked as verified.");
    }

    public async Task<IActionResult> OnPostSaveIntegrationAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        ValidateJson(Integration.ConfigurationJson, nameof(Integration.ConfigurationJson));
        if (!ModelState.IsValid) { Integration.Secret = string.Empty; await LoadAsync(); return Page(); }
        var key = Integration.IntegrationKey.Trim().ToLowerInvariant();
        var entity = await _db.TenantIntegrationSettings.SingleOrDefaultAsync(i => i.IntegrationKey == key)
            ?? new TenantIntegrationSetting { TenantId = _tenant.TenantId!.Value, IntegrationKey = key };
        if (entity.Id == Guid.Empty) _db.TenantIntegrationSettings.Add(entity);
        entity.DisplayName = Integration.DisplayName; entity.Category = Integration.Category; entity.IsEnabled = Integration.IsEnabled; entity.ConfigurationJson = Integration.ConfigurationJson;
        if (!string.IsNullOrWhiteSpace(Integration.Secret)) { entity.ProtectedSecret = _secretProtector.Protect(Integration.Secret); entity.SecretHint = Integration.Secret.Length <= 4 ? "configured" : "ends " + Integration.Secret[^4..]; }
        await _db.SaveChangesAsync(); return Success("Integration setting saved; secret material is encrypted.");
    }

    public async Task<IActionResult> OnPostSaveFeatureAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        ValidateJson(Feature.ConditionsJson, nameof(Feature.ConditionsJson)); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var key = Feature.FeatureKey.Trim().ToLowerInvariant();
        var entity = await _db.TenantFeatureFlags.SingleOrDefaultAsync(f => f.FeatureKey == key) ?? new TenantFeatureFlag { TenantId = _tenant.TenantId!.Value, FeatureKey = key };
        if (entity.Id == Guid.Empty) _db.TenantFeatureFlags.Add(entity);
        entity.DisplayName = Feature.DisplayName; entity.Description = Feature.Description; entity.IsEnabled = Feature.IsEnabled;
        entity.EnabledFrom = Feature.EnabledFrom; entity.EnabledUntil = Feature.EnabledUntil; entity.ConditionsJson = Feature.ConditionsJson;
        await _db.SaveChangesAsync(); return Success("Feature flag saved.");
    }

    public async Task<IActionResult> OnPostSaveContactAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        if (Contact.IsPrimary) await _db.BranchContacts.Where(c => c.BranchId == Contact.BranchId && c.IsPrimary).ExecuteUpdateAsync(s => s.SetProperty(c => c.IsPrimary, false));
        _db.BranchContacts.Add(new BranchContact { TenantId = _tenant.TenantId!.Value, BranchId = Contact.BranchId, ContactType = Contact.ContactType, ContactName = Contact.ContactName, Designation = Contact.Designation, Email = Contact.Email, Phone = Contact.Phone, IsPrimary = Contact.IsPrimary });
        await _db.SaveChangesAsync(); return Success("Branch contact added.");
    }

    public async Task<IActionResult> OnPostSaveBranchConfigAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        ValidateJson(BranchConfig.PreferencesJson, nameof(BranchConfig.PreferencesJson)); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var entity = await _db.BranchAcademicConfigs.SingleOrDefaultAsync(c => c.BranchId == BranchConfig.BranchId) ?? new BranchAcademicConfig { TenantId = _tenant.TenantId!.Value, BranchId = BranchConfig.BranchId };
        if (entity.Id == Guid.Empty) _db.BranchAcademicConfigs.Add(entity);
        entity.TimeZoneId = BranchConfig.TimeZoneId; entity.WeekStartsOn = BranchConfig.WeekStartsOn; entity.WeekendDays = BranchConfig.WeekendDays;
        entity.DefaultPeriodMinutes = BranchConfig.DefaultPeriodMinutes; entity.WorkingDaysPerWeek = BranchConfig.WorkingDaysPerWeek;
        entity.DateFormat = BranchConfig.DateFormat; entity.DefaultLanguage = BranchConfig.DefaultLanguage;
        entity.AdmissionNumberPrefix = BranchConfig.AdmissionNumberPrefix; entity.StudentNumberPrefix = BranchConfig.StudentNumberPrefix; entity.PreferencesJson = BranchConfig.PreferencesJson;
        await _db.SaveChangesAsync(); return Success("Branch academic preferences saved.");
    }

    public async Task<IActionResult> OnPostSaveSettingAsync()
    {
        var global = Setting.Scope == "Global";
        if (!await Allowed(global ? AdministrationPolicies.ManageGlobal : AdministrationPolicies.ManageTenant)) return Forbid();
        if (Setting.ValueType == SettingValueType.Json) ValidateJson(Setting.Value, nameof(Setting.Value));
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        if (global)
        {
            var entity = await _db.AppSettings.SingleOrDefaultAsync(s => s.Key == Setting.Key) ?? new AppSetting { Key = Setting.Key };
            if (entity.Id == Guid.Empty) _db.AppSettings.Add(entity); MapSetting(entity);
        }
        else
        {
            if (!HasTenant) return Forbid();
            var entity = await _db.TenantSettings.SingleOrDefaultAsync(s => s.Key == Setting.Key) ?? new TenantSetting { TenantId = _tenant.TenantId!.Value, Key = Setting.Key };
            if (entity.Id == Guid.Empty) _db.TenantSettings.Add(entity); MapSetting(entity);
        }
        await _db.SaveChangesAsync(); return Success("Setting saved.");
    }

    public async Task<IActionResult> OnPostSaveLookupAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant) || !HasTenant) return Forbid();
        ValidateJson(Lookup.MetadataJson, nameof(Lookup.MetadataJson)); if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        var entity = await _db.LookupItems.SingleOrDefaultAsync(l => l.LookupType == Lookup.LookupType && l.Code == Lookup.Code)
            ?? new LookupItem { TenantId = _tenant.TenantId!.Value, LookupType = Lookup.LookupType, Code = Lookup.Code };
        if (entity.Id == Guid.Empty) _db.LookupItems.Add(entity);
        entity.DisplayName = Lookup.DisplayName; entity.SortOrder = Lookup.SortOrder; entity.IsActive = Lookup.IsActive; entity.MetadataJson = Lookup.MetadataJson;
        await _db.SaveChangesAsync(); return Success("Lookup item saved.");
    }

    public async Task<IActionResult> OnPostSaveGrantAsync()
    {
        if (!await Allowed(AdministrationPolicies.ManagePermissions) || !HasTenant) return Forbid();
        if (Grant.UserId.HasValue)
        {
            var entity = await _db.UserPermissionGrants.SingleOrDefaultAsync(g => g.UserId == Grant.UserId && g.PermissionDefinitionId == Grant.PermissionDefinitionId)
                ?? new UserPermissionGrant { TenantId = _tenant.TenantId!.Value, UserId = Grant.UserId.Value, PermissionDefinitionId = Grant.PermissionDefinitionId };
            if (entity.Id == Guid.Empty) _db.UserPermissionGrants.Add(entity); entity.IsAllowed = Grant.IsAllowed;
        }
        else if (Grant.RoleId.HasValue)
        {
            var entity = await _db.RolePermissionGrants.SingleOrDefaultAsync(g => g.RoleId == Grant.RoleId && g.PermissionDefinitionId == Grant.PermissionDefinitionId)
                ?? new RolePermissionGrant { TenantId = _tenant.TenantId!.Value, RoleId = Grant.RoleId.Value, PermissionDefinitionId = Grant.PermissionDefinitionId };
            if (entity.Id == Guid.Empty) _db.RolePermissionGrants.Add(entity); entity.IsAllowed = Grant.IsAllowed;
        }
        else return BadRequest();
        await _db.SaveChangesAsync(); return Success("Permission override saved.");
    }

    public async Task<IActionResult> OnPostRetryJobAsync(Guid id)
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant)) return Forbid();
        var job = await _db.BackgroundJobLogs.SingleAsync(j => j.Id == id);
        job.Status = BackgroundJobStatus.RetryScheduled; job.NextRetryOn = DateTime.UtcNow; job.LastError = null;
        await _db.SaveChangesAsync(); return Success("Job scheduled for retry.");
    }

    public async Task<IActionResult> OnPostCancelJobAsync(Guid id)
    {
        if (!await Allowed(AdministrationPolicies.ManageTenant)) return Forbid();
        var job = await _db.BackgroundJobLogs.SingleAsync(j => j.Id == id); job.Status = BackgroundJobStatus.Cancelled; job.CompletedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync(); return Success("Job cancelled.");
    }

    private async Task LoadAsync(string? auditEntity = null, string? auditActor = null, BackgroundJobStatus? jobStatus = null)
    {
        CanManageTenant = await Allowed(AdministrationPolicies.ManageTenant);
        CanManageGlobal = await Allowed(AdministrationPolicies.ManageGlobal);
        CanManagePermissions = await Allowed(AdministrationPolicies.ManagePermissions);
        Plans = await _db.SubscriptionPlans.AsNoTracking().OrderBy(p => p.MonthlyPrice).ToListAsync();
        Permissions = await _db.PermissionDefinitions.AsNoTracking().OrderBy(p => p.Module).ThenBy(p => p.DisplayName).ToListAsync();
        Roles = await _db.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync();
        if (CanManageGlobal) AppSettings = await _db.AppSettings.AsNoTracking().OrderBy(s => s.Key).ToListAsync();
        if (!HasTenant)
        {
            var globalAudit = _db.AuditLogs.AsNoTracking().Where(a => a.TenantId == null);
            if (!string.IsNullOrWhiteSpace(auditEntity)) globalAudit = globalAudit.Where(a => a.EntityType.Contains(auditEntity));
            if (!string.IsNullOrWhiteSpace(auditActor)) globalAudit = globalAudit.Where(a => a.ActorId != null && a.ActorId.Contains(auditActor));
            AuditLogs = await globalAudit.OrderByDescending(a => a.OccurredOn).Take(200).ToListAsync();
            return;
        }
        CurrentSubscription = await _db.TenantSubscriptions.AsNoTracking().Include(s => s.SubscriptionPlan).OrderByDescending(s => s.StartsOn).FirstOrDefaultAsync();
        Branding = await _db.TenantBrandings.AsNoTracking().SingleOrDefaultAsync();
        Domains = await _db.TenantDomains.AsNoTracking().OrderByDescending(d => d.IsPrimary).ThenBy(d => d.DomainName).ToListAsync();
        Integrations = await _db.TenantIntegrationSettings.AsNoTracking().OrderBy(i => i.DisplayName).ToListAsync();
        Features = await _db.TenantFeatureFlags.AsNoTracking().OrderBy(f => f.DisplayName).ToListAsync();
        Branches = await _db.Branches.AsNoTracking().OrderBy(b => b.Name).ToListAsync();
        Contacts = await _db.BranchContacts.AsNoTracking().Include(c => c.Branch).OrderBy(c => c.ContactName).ToListAsync();
        BranchConfigs = await _db.BranchAcademicConfigs.AsNoTracking().Include(c => c.Branch).ToListAsync();
        TenantSettings = await _db.TenantSettings.AsNoTracking().OrderBy(s => s.Key).ToListAsync();
        Lookups = await _db.LookupItems.AsNoTracking().OrderBy(l => l.LookupType).ThenBy(l => l.SortOrder).ToListAsync();
        Users = await _db.Users.AsNoTracking().IgnoreQueryFilters().Where(u => u.TenantId == _tenant.TenantId).OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync();
        RoleGrants = await _db.RolePermissionGrants.AsNoTracking().Include(g => g.Role).Include(g => g.PermissionDefinition).ToListAsync();
        UserGrants = await _db.UserPermissionGrants.AsNoTracking().Include(g => g.User).Include(g => g.PermissionDefinition).ToListAsync();
        var auditQuery = _db.AuditLogs.AsNoTracking().Where(a => a.TenantId == _tenant.TenantId);
        if (!string.IsNullOrWhiteSpace(auditEntity)) auditQuery = auditQuery.Where(a => a.EntityType.Contains(auditEntity));
        if (!string.IsNullOrWhiteSpace(auditActor)) auditQuery = auditQuery.Where(a => a.ActorId != null && a.ActorId.Contains(auditActor));
        AuditLogs = await auditQuery.OrderByDescending(a => a.OccurredOn).Take(200).ToListAsync();
        var jobs = _db.BackgroundJobLogs.AsNoTracking(); if (jobStatus.HasValue) jobs = jobs.Where(j => j.Status == jobStatus);
        Jobs = await jobs.OrderByDescending(j => j.ScheduledOn).Take(100).ToListAsync();
        if (Branding is not null) Brand = new BrandingForm { PrimaryColor = Branding.PrimaryColor, SecondaryColor = Branding.SecondaryColor, AccentColor = Branding.AccentColor, FontFamily = Branding.FontFamily, EmailFromName = Branding.EmailFromName, EmailHeaderHtml = Branding.EmailHeaderHtml, EmailFooterHtml = Branding.EmailFooterHtml, LoginWelcomeText = Branding.LoginWelcomeText };
    }

    private async Task<bool> Allowed(string policy) => (await _authorization.AuthorizeAsync(User, policy)).Succeeded;
    private IActionResult Success(string message) { TempData["StatusMessage"] = message; return RedirectToPage(); }
    private void ValidateJson(string value, string field) { try { JsonDocument.Parse(value); } catch (JsonException) { ModelState.AddModelError(field, "Enter valid JSON."); } }
    private void MapSetting(AppSetting entity) { entity.Value = Setting.IsSensitive ? _secretProtector.Protect(Setting.Value) : Setting.Value; entity.ValueType = Setting.ValueType; entity.Description = Setting.Description; entity.IsSensitive = Setting.IsSensitive; }
    private void MapSetting(TenantSetting entity) { entity.Value = Setting.IsSensitive ? _secretProtector.Protect(Setting.Value) : Setting.Value; entity.ValueType = Setting.ValueType; entity.Description = Setting.Description; entity.IsSensitive = Setting.IsSensitive; }
    private async Task<string> SaveBrandAssetAsync(IFormFile file, string kind)
    {
        if (file.Length is <= 0 or > 2_000_000) throw new InvalidOperationException("Brand assets must be between 1 byte and 2 MB.");
        var allowed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["image/png"] = ".png", ["image/jpeg"] = ".jpg", ["image/webp"] = ".webp", ["image/x-icon"] = ".ico" };
        if (!allowed.TryGetValue(file.ContentType, out var extension)) throw new InvalidOperationException("Only PNG, JPEG, WebP and ICO brand assets are supported.");
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "branding", _tenant.TenantId!.Value.ToString("N")); Directory.CreateDirectory(folder);
        var name = kind + "-" + Guid.NewGuid().ToString("N") + extension; var path = Path.Combine(folder, name);
        await using var stream = System.IO.File.Create(path); await file.CopyToAsync(stream);
        return $"/uploads/branding/{_tenant.TenantId.Value:N}/{name}";
    }

    public sealed class PlanForm { public Guid? Id { get; set; } [Required] public string Code { get; set; } = "CUSTOM"; [Required] public string Name { get; set; } = "Custom"; public string? Description { get; set; } public decimal MonthlyPrice { get; set; } public decimal AnnualPrice { get; set; } public int MaxBranches { get; set; } public int MaxStudents { get; set; } public int MaxStaff { get; set; } public bool IsActive { get; set; } = true; public string IncludedFeaturesJson { get; set; } = "[]"; }
    public sealed class SubscriptionForm { public Guid SubscriptionPlanId { get; set; } public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active; public DateOnly StartsOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); public DateOnly? EndsOn { get; set; } public bool AutoRenew { get; set; } public decimal AgreedMonthlyPrice { get; set; } }
    public sealed class BrandingForm { [RegularExpression("^#[0-9A-Fa-f]{6}$")] public string PrimaryColor { get; set; } = "#176B87"; [RegularExpression("^#[0-9A-Fa-f]{6}$")] public string SecondaryColor { get; set; } = "#F59E0B"; [RegularExpression("^#[0-9A-Fa-f]{6}$")] public string AccentColor { get; set; } = "#16A34A"; public string? FontFamily { get; set; } public string? EmailFromName { get; set; } public string? EmailHeaderHtml { get; set; } public string? EmailFooterHtml { get; set; } public string? LoginWelcomeText { get; set; } public IFormFile? LogoFile { get; set; } public IFormFile? FaviconFile { get; set; } }
    public sealed class DomainForm { [Required] public string DomainName { get; set; } = string.Empty; public bool IsPrimary { get; set; } }
    public sealed class IntegrationForm { [Required] public string IntegrationKey { get; set; } = string.Empty; [Required] public string DisplayName { get; set; } = string.Empty; public IntegrationCategory Category { get; set; } public bool IsEnabled { get; set; } public string ConfigurationJson { get; set; } = "{}"; [DataType(DataType.Password)] public string Secret { get; set; } = string.Empty; }
    public sealed class FeatureForm { [Required] public string FeatureKey { get; set; } = string.Empty; [Required] public string DisplayName { get; set; } = string.Empty; public string? Description { get; set; } public bool IsEnabled { get; set; } public DateTime? EnabledFrom { get; set; } public DateTime? EnabledUntil { get; set; } public string ConditionsJson { get; set; } = "{}"; }
    public sealed class ContactForm { public Guid BranchId { get; set; } public string ContactType { get; set; } = "Primary"; [Required] public string ContactName { get; set; } = string.Empty; public string? Designation { get; set; } [EmailAddress] public string? Email { get; set; } public string? Phone { get; set; } public bool IsPrimary { get; set; } }
    public sealed class BranchConfigForm { public Guid BranchId { get; set; } public string TimeZoneId { get; set; } = "Asia/Kolkata"; public string WeekStartsOn { get; set; } = "Monday"; public string WeekendDays { get; set; } = "Sunday"; public int DefaultPeriodMinutes { get; set; } = 45; public int WorkingDaysPerWeek { get; set; } = 6; public string DateFormat { get; set; } = "dd-MM-yyyy"; public string DefaultLanguage { get; set; } = "en-IN"; public string AdmissionNumberPrefix { get; set; } = "ADM"; public string StudentNumberPrefix { get; set; } = "STU"; public string PreferencesJson { get; set; } = "{}"; }
    public sealed class SettingForm { public string Scope { get; set; } = "Tenant"; [Required] public string Key { get; set; } = string.Empty; [Required] public string Value { get; set; } = string.Empty; public SettingValueType ValueType { get; set; } public string? Description { get; set; } public bool IsSensitive { get; set; } }
    public sealed class LookupForm { [Required] public string LookupType { get; set; } = string.Empty; [Required] public string Code { get; set; } = string.Empty; [Required] public string DisplayName { get; set; } = string.Empty; public int SortOrder { get; set; } public bool IsActive { get; set; } = true; public string MetadataJson { get; set; } = "{}"; }
    public sealed class PermissionGrantForm { public Guid PermissionDefinitionId { get; set; } public Guid? RoleId { get; set; } public Guid? UserId { get; set; } public bool IsAllowed { get; set; } = true; }
}
