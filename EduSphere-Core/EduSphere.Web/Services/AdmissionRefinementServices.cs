using System.Globalization;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class ApplicantClaimTypes
{
    public const string AccountId = "applicant_account_id";
    public const string TenantId = "applicant_tenant_id";
}

public interface IAdmissionNumberService
{
    Task<string> NextApplicationNumberAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task<string> NextAdmissionNumberAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
}

public sealed class AdmissionNumberService(TenantDbContext db) : IAdmissionNumberService
{
    public Task<string> NextApplicationNumberAsync(Guid tenantId, Guid branchId, CancellationToken token = default) => NextAsync(tenantId, branchId, true, token);
    public Task<string> NextAdmissionNumberAsync(Guid tenantId, Guid branchId, CancellationToken token = default) => NextAsync(tenantId, branchId, false, token);

    private async Task<string> NextAsync(Guid tenantId, Guid branchId, bool application, CancellationToken token)
    {
        var strategy = db.Database.CreateExecutionStrategy(); string? result = null;
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, token);
            var setting = await db.BranchAdmissionSettings.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.BranchId == branchId, token);
            if (setting is null)
            {
                setting = new BranchAdmissionSetting { TenantId = tenantId, BranchId = branchId };
                db.BranchAdmissionSettings.Add(setting);
            }
            var sequence = application ? setting.NextApplicationSequence++ : setting.NextAdmissionSequence++;
            var format = application ? setting.ApplicationNumberFormat : setting.AdmissionNumberFormat;
            result = Format(format, sequence);
            await db.SaveChangesAsync(token); await transaction.CommitAsync(token);
        });
        return result!;
    }

    public static string Format(string format, int sequence)
    {
        var value = format.Replace("{YYYY}", DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture))
            .Replace("{YY}", (DateTime.UtcNow.Year % 100).ToString("00", CultureInfo.InvariantCulture));
        var start = value.IndexOf("{SEQ:", StringComparison.OrdinalIgnoreCase);
        if (start >= 0)
        {
            var end = value.IndexOf('}', start);
            if (end > start)
            {
                var mask = value[(start + 5)..end];
                value = value[..start] + sequence.ToString(mask, CultureInfo.InvariantCulture) + value[(end + 1)..];
            }
        }
        return value.Replace("{SEQ}", sequence.ToString(CultureInfo.InvariantCulture));
    }
}

public sealed record AdmissionCheckoutResult(bool Succeeded, string? RedirectUrl, string? SessionId, string? Error);
public interface IAdmissionPaymentGateway
{
    Task<AdmissionCheckoutResult> CreateCheckoutAsync(AdmissionApplication application, FeeInvoice invoice, string successUrl, string cancelUrl, CancellationToken token = default);
    Task<(bool Paid, string? PaymentId, string? Error)> ConfirmAsync(Guid tenantId, string sessionId, CancellationToken token = default);
}

public sealed class StripeAdmissionPaymentGateway(TenantDbContext db, IHttpClientFactory clients, IDataProtectionProvider protection) : IAdmissionPaymentGateway
{
    private readonly IDataProtector _protector = protection.CreateProtector("EduSphere.Administration.IntegrationSecrets.v1");
    public async Task<AdmissionCheckoutResult> CreateCheckoutAsync(AdmissionApplication application, FeeInvoice invoice, string successUrl, string cancelUrl, CancellationToken token = default)
    {
        var setting = await db.TenantIntegrationSettings.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == application.TenantId && x.IntegrationKey == "stripe" && x.IsEnabled, token);
        if (setting?.ProtectedSecret is null) return new(false, null, null, "Stripe is not configured for this institution.");
        string secret;
        try { secret = _protector.Unprotect(setting.ProtectedSecret); } catch { return new(false, null, null, "Stripe credentials could not be decrypted."); }
        var amount = Math.Max(invoice.TotalAmount - invoice.PaidAmount, 0);
        if (amount <= 0) return new(false, null, null, "The admission invoice is already paid.");
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/checkout/sessions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["mode"] = "payment", ["success_url"] = successUrl + "&session_id={CHECKOUT_SESSION_ID}", ["cancel_url"] = cancelUrl,
            ["client_reference_id"] = invoice.Id.ToString(), ["customer_email"] = application.Email ?? string.Empty,
            ["line_items[0][price_data][currency]"] = "inr", ["line_items[0][price_data][unit_amount]"] = ((long)Math.Round(amount * 100)).ToString(),
            ["line_items[0][price_data][product_data][name]"] = $"Admission fee - {application.ApplicationNumber}", ["line_items[0][quantity]"] = "1",
            ["metadata[application_id]"] = application.Id.ToString(), ["metadata[invoice_id]"] = invoice.Id.ToString()
        });
        using var response = await clients.CreateClient().SendAsync(request, token);
        var json = await response.Content.ReadAsStringAsync(token);
        if (!response.IsSuccessStatusCode) return new(false, null, null, "Payment provider rejected the checkout request.");
        using var doc = JsonDocument.Parse(json);
        return new(true, doc.RootElement.GetProperty("url").GetString(), doc.RootElement.GetProperty("id").GetString(), null);
    }
    public async Task<(bool Paid, string? PaymentId, string? Error)> ConfirmAsync(Guid tenantId, string sessionId, CancellationToken token = default)
    {
        var setting = await db.TenantIntegrationSettings.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IntegrationKey == "stripe" && x.IsEnabled, token);
        if (setting?.ProtectedSecret is null) return (false, null, "Stripe is not configured.");
        string secret; try { secret = _protector.Unprotect(setting.ProtectedSecret); } catch { return (false, null, "Stripe credentials could not be decrypted."); }
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.stripe.com/v1/checkout/sessions/{Uri.EscapeDataString(sessionId)}"); request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);
        using var response = await clients.CreateClient().SendAsync(request, token); if (!response.IsSuccessStatusCode) return (false, null, "Payment confirmation failed.");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token)); var paid = json.RootElement.TryGetProperty("payment_status", out var status) && status.GetString() == "paid";
        return (paid, json.RootElement.TryGetProperty("payment_intent", out var id) ? id.GetString() : null, paid ? null : "Payment is not complete.");
    }
}

public interface IAdmissionAutomationService { Task<int> ProcessAsync(CancellationToken token = default); }

public interface IAdmissionCalendarService
{
    Task<(string? Provider, string? EventId, string? MeetingLink)> CreateEventAsync(AdmissionInterview interview, AdmissionApplication application, CancellationToken token = default);
}

public sealed class AdmissionCalendarService(TenantDbContext db, IHttpClientFactory clients, IDataProtectionProvider protection) : IAdmissionCalendarService
{
    private readonly IDataProtector _protector = protection.CreateProtector("EduSphere.Administration.IntegrationSecrets.v1");
    public async Task<(string?, string?, string?)> CreateEventAsync(AdmissionInterview interview, AdmissionApplication application, CancellationToken token = default)
    {
        var setting = await db.TenantIntegrationSettings.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == application.TenantId && x.IntegrationKey == "admission-calendar" && x.IsEnabled, token);
        if (setting is null) return (null, null, null);
        using var config = JsonDocument.Parse(setting.ConfigurationJson); if (!config.RootElement.TryGetProperty("endpoint", out var endpointNode)) return (null, null, null);
        var endpoint = endpointNode.GetString(); if (string.IsNullOrWhiteSpace(endpoint)) return (null, null, null);
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        if (setting.ProtectedSecret is not null) { try { request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _protector.Unprotect(setting.ProtectedSecret)); } catch { return (null, null, null); } }
        request.Content = JsonContent.Create(new { title = $"Admission interview - {application.ApplicationNumber}", startsOn = interview.StartsOn, endsOn = interview.EndsOn, applicant = $"{application.ApplicantFirstName} {application.ApplicantLastName}", email = application.Email, location = interview.Location });
        using var response = await clients.CreateClient().SendAsync(request, token); if (!response.IsSuccessStatusCode) return ("webhook", null, null);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        return ("webhook", body.RootElement.TryGetProperty("id", out var id) ? id.GetString() : null, body.RootElement.TryGetProperty("meetingLink", out var link) ? link.GetString() : null);
    }
}
public sealed class AdmissionAutomationService(TenantDbContext db) : IAdmissionAutomationService
{
    public async Task<int> ProcessAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow; var changed = 0;
        var expiredOffers = await db.AdmissionApplications.IgnoreQueryFilters().Where(x => !x.IsDeleted && x.Status == AdmissionApplicationStatus.Accepted && x.OfferExpiresOn < now).ToListAsync(token);
        foreach (var app in expiredOffers) { app.Status = AdmissionApplicationStatus.OfferExpired; Log(app, AdmissionAutomationType.OfferExpired, "Admission offer expired automatically."); changed++; }
        var overdueFees = await db.AdmissionApplications.IgnoreQueryFilters().Where(x => !x.IsDeleted && x.Status == AdmissionApplicationStatus.FeePending && x.FeeDueOn < now).ToListAsync(token);
        foreach (var app in overdueFees) { app.Status = AdmissionApplicationStatus.FeeDeadlineExpired; Log(app, AdmissionAutomationType.FeeDeadlineExpired, "Admission fee deadline expired automatically."); changed++; }
        // Capacity queries must see seats released by expired offers in this same cycle.
        if (changed > 0) await db.SaveChangesAsync(token);
        var groups = await db.AdmissionApplications.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && x.Status == AdmissionApplicationStatus.Waitlisted)
            .Select(x => new { x.TenantId, x.BranchId, x.CourseId, x.AcademicYearId, x.BatchId, x.SectionId }).Distinct().ToListAsync(token);
        foreach (var group in groups)
        {
            var settings = await db.BranchAdmissionSettings.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == group.TenantId && x.BranchId == group.BranchId, token);
            if (settings?.AutoPromoteWaitlist == false) continue;
            var capacity = group.SectionId.HasValue
                ? await db.Sections.IgnoreQueryFilters().Where(x => x.Id == group.SectionId).Select(x => (int?)x.Capacity).FirstOrDefaultAsync(token)
                : group.BatchId.HasValue ? await db.Batches.IgnoreQueryFilters().Where(x => x.Id == group.BatchId).Select(x => (int?)x.Capacity).FirstOrDefaultAsync(token) : null;
            if (!capacity.HasValue || capacity <= 0) continue;
            var enrolled = await db.Enrollments.IgnoreQueryFilters().CountAsync(x => x.TenantId == group.TenantId && x.Status == EnrollmentStatus.Active && (group.SectionId.HasValue ? x.SectionId == group.SectionId : x.BatchId == group.BatchId), token);
            var reserved = await db.AdmissionApplications.IgnoreQueryFilters().CountAsync(x => x.TenantId == group.TenantId && x.BranchId == group.BranchId && x.CourseId == group.CourseId && x.AcademicYearId == group.AcademicYearId && x.BatchId == group.BatchId && x.SectionId == group.SectionId && (x.Status == AdmissionApplicationStatus.Accepted || x.Status == AdmissionApplicationStatus.FeePending), token);
            var available = Math.Max(capacity.Value - enrolled - reserved, 0);
            var candidates = await db.AdmissionApplications.IgnoreQueryFilters().Where(x => x.TenantId == group.TenantId && x.BranchId == group.BranchId && x.CourseId == group.CourseId && x.AcademicYearId == group.AcademicYearId && x.BatchId == group.BatchId && x.SectionId == group.SectionId && x.Status == AdmissionApplicationStatus.Waitlisted).OrderBy(x => x.WaitlistPosition).ThenBy(x => x.AppliedOn).Take(available).ToListAsync(token);
            foreach (var next in candidates)
            {
                next.Status = AdmissionApplicationStatus.Accepted; next.WaitlistPosition = null; next.OfferExpiresOn = now.AddDays(settings?.OfferValidityDays ?? 7);
                Log(next, AdmissionAutomationType.WaitlistPromoted, "Automatically promoted from the waitlist because configured intake capacity became available."); changed++;
            }
        }
        if (changed > 0) await db.SaveChangesAsync(token);
        return changed;
    }
    private void Log(AdmissionApplication app, AdmissionAutomationType type, string message) => db.AdmissionAutomationLogs.Add(new AdmissionAutomationLog { TenantId = app.TenantId, AdmissionApplicationId = app.Id, Type = type, Message = message });
}

public sealed class AdmissionAutomationWorker(IServiceScopeFactory scopes, ILogger<AdmissionAutomationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { using var scope = scopes.CreateScope(); await scope.ServiceProvider.GetRequiredService<IAdmissionAutomationService>().ProcessAsync(stoppingToken); }
            catch (Exception ex) { logger.LogError(ex, "Admission automation cycle failed."); }
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}

public static class AdmissionExportBuilder
{
    public static byte[] Excel(IEnumerable<AdmissionApplication> applications)
    {
        using var output = new MemoryStream();
        using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
        {
            Write(zip, "[Content_Types].xml", """<?xml version="1.0"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/></Types>""");
            Write(zip, "_rels/.rels", """<?xml version="1.0"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>""");
            Write(zip, "xl/workbook.xml", """<?xml version="1.0"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Admissions" sheetId="1" r:id="rId1"/></sheets></workbook>""");
            Write(zip, "xl/_rels/workbook.xml.rels", """<?xml version="1.0"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/></Relationships>""");
            var rows = new List<string[]> { new[] { "Application", "Applicant", "Email", "Source", "Applied", "Status", "Waitlist position" } };
            rows.AddRange(applications.Select(x => new[] { x.ApplicationNumber, $"{x.ApplicantFirstName} {x.ApplicantLastName}", x.Email ?? "", x.ApplicationSource ?? "Direct", x.AppliedOn.ToString("yyyy-MM-dd"), x.Status.ToString(), x.WaitlistPosition?.ToString() ?? "" }));
            var xmlRows = string.Join("", rows.Select((r, i) => $"<row r=\"{i + 1}\">{string.Join("", r.Select(v => $"<c t=\"inlineStr\"><is><t>{System.Security.SecurityElement.Escape(v)}</t></is></c>"))}</row>"));
            Write(zip, "xl/worksheets/sheet1.xml", $"<?xml version=\"1.0\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>{xmlRows}</sheetData></worksheet>");
        }
        return output.ToArray();
    }
    public static byte[] Pdf(IEnumerable<AdmissionApplication> applications)
    {
        var lines = new[] { "Admission Funnel Report", $"Generated {DateTime.UtcNow:u}", "" }.Concat(applications.Take(45).Select(x => $"{x.ApplicationNumber}  {x.ApplicantFirstName} {x.ApplicantLastName}  {x.Status}"));
        var text = string.Join("\\n", lines).Replace("(", "\\(").Replace(")", "\\)");
        var stream = $"BT /F1 10 Tf 40 790 Td 13 TL ({text.Replace("\\n", ") Tj T* (")}) Tj ET";
        var objects = new[] { "<< /Type /Catalog /Pages 2 0 R >>", "<< /Type /Pages /Kids [3 0 R] /Count 1 >>", "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 842] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>", $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>" };
        var sb = new StringBuilder("%PDF-1.4\n"); var offsets = new List<int> { 0 };
        for (var i = 0; i < objects.Length; i++) { offsets.Add(Encoding.ASCII.GetByteCount(sb.ToString())); sb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n"); }
        var xref = Encoding.ASCII.GetByteCount(sb.ToString()); sb.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n"); foreach (var o in offsets.Skip(1)) sb.Append($"{o:0000000000} 00000 n \n"); sb.Append($"trailer << /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF"); return Encoding.ASCII.GetBytes(sb.ToString());
    }
    private static void Write(ZipArchive zip, string path, string text) { var entry = zip.CreateEntry(path); using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)); writer.Write(text); }
}
