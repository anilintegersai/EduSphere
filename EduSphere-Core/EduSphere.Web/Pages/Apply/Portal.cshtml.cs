using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Apply;

[Authorize(AuthenticationSchemes = "Applicant")]
public sealed class PortalModel(TenantDbContext db, IAdmissionDocumentStorageService storage, IAdmissionPaymentGateway payments) : PageModel
{
    public List<AdmissionApplication> Applications { get; private set; } = [];
    public Dictionary<Guid, List<AdmissionDocument>> Documents { get; private set; } = [];
    public Dictionary<Guid, FeeInvoice?> Invoices { get; private set; } = [];
    [TempData] public string? StatusMessage { get; set; }
    private Guid AccountId => Guid.Parse(User.FindFirst(ApplicantClaimTypes.AccountId)!.Value);
    private Guid TenantId => Guid.Parse(User.FindFirst(ApplicantClaimTypes.TenantId)!.Value);
    public async Task OnGetAsync(string? payment, string? session_id)
    {
        if (payment == "success" && !string.IsNullOrWhiteSpace(session_id)) await ConfirmPaymentAsync(session_id);
        else if (payment == "cancelled") StatusMessage = "Payment checkout was cancelled; no payment was recorded.";
        await LoadAsync();
    }
    public async Task<IActionResult> OnPostReplaceDocumentAsync(Guid applicationId, Guid documentId, IFormFile replacement)
    {
        var app = await db.AdmissionApplications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == applicationId && x.ApplicantAccountId == AccountId && x.TenantId == TenantId);
        var old = await db.AdmissionDocuments.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == documentId && x.AdmissionApplicationId == applicationId);
        if (app is null || old is null || old.ReviewStatus != AdmissionDocumentReviewStatus.ReplacementRequested || replacement.Length == 0) return NotFound();
        await using var stream = replacement.OpenReadStream(); var stored = await storage.SaveAsync(app.TenantId, app.Id, stream, replacement.FileName, replacement.ContentType);
        var doc = new AdmissionDocument { TenantId = app.TenantId, AdmissionApplicationId = app.Id, DocumentType = old.DocumentType, DisplayName = old.DisplayName, FileName = stored.FileName, ContentType = stored.ContentType, StoragePath = stored.StoragePath, SizeBytes = stored.SizeBytes, UploadedOn = DateTime.UtcNow, ReplacesDocumentId = old.Id, ReviewStatus = AdmissionDocumentReviewStatus.Pending };
        db.AdmissionDocuments.Add(doc); old.ReviewStatus = AdmissionDocumentReviewStatus.Replaced; old.ReplacedByDocumentId = doc.Id; await db.SaveChangesAsync(); StatusMessage = "Replacement uploaded for review."; return RedirectToPage();
    }
    public async Task<IActionResult> OnPostCheckoutAsync(Guid applicationId)
    {
        var app = await db.AdmissionApplications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == applicationId && x.ApplicantAccountId == AccountId && x.TenantId == TenantId);
        if (app is null) return NotFound();
        var invoice = await db.FeeInvoices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.AdmissionApplicationId == app.Id);
        if (invoice is null) { StatusMessage = "No admission fee invoice is available yet."; return RedirectToPage(); }
        var success = Url.Page("Portal", null, new { payment = "success" }, Request.Scheme)!; var cancel = Url.Page("Portal", null, new { payment = "cancelled" }, Request.Scheme)!;
        var checkout = await payments.CreateCheckoutAsync(app, invoice, success, cancel);
        if (!checkout.Succeeded) { StatusMessage = checkout.Error; return RedirectToPage(); }
        db.OnlinePaymentTransactions.Add(new OnlinePaymentTransaction { TenantId = app.TenantId, BranchId = app.BranchId, FeeInvoiceId = invoice.Id, GatewayProviderKey = "stripe", GatewayOrderId = checkout.SessionId!, Amount = invoice.TotalAmount - invoice.PaidAmount, Currency = "INR", Status = PaymentGatewayStatus.Pending, InitiatedOn = DateTime.UtcNow }); await db.SaveChangesAsync();
        return Redirect(checkout.RedirectUrl!);
    }
    private async Task LoadAsync()
    {
        Applications = await db.AdmissionApplications.IgnoreQueryFilters().Where(x => x.TenantId == TenantId && x.ApplicantAccountId == AccountId && !x.IsDeleted).OrderByDescending(x => x.LastSavedOn ?? x.CreatedOn).ToListAsync();
        var ids = Applications.Select(x => x.Id).ToList(); var docs = await db.AdmissionDocuments.IgnoreQueryFilters().Where(x => ids.Contains(x.AdmissionApplicationId) && !x.IsDeleted).OrderByDescending(x => x.UploadedOn).ToListAsync(); Documents = docs.GroupBy(x => x.AdmissionApplicationId).ToDictionary(x => x.Key, x => x.ToList());
        var invoices = await db.FeeInvoices.IgnoreQueryFilters().Where(x => x.AdmissionApplicationId.HasValue && ids.Contains(x.AdmissionApplicationId.Value)).ToListAsync(); Invoices = Applications.ToDictionary(x => x.Id, x => invoices.FirstOrDefault(i => i.AdmissionApplicationId == x.Id));
    }
    private async Task ConfirmPaymentAsync(string sessionId)
    {
        var transaction = await db.OnlinePaymentTransactions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.TenantId == TenantId && x.GatewayOrderId == sessionId && x.Status == PaymentGatewayStatus.Pending);
        if (transaction is null) { StatusMessage = "Payment session was not found or was already processed."; return; }
        var confirmation = await payments.ConfirmAsync(TenantId, sessionId); if (!confirmation.Paid) { StatusMessage = confirmation.Error; return; }
        var invoice = await db.FeeInvoices.IgnoreQueryFilters().FirstAsync(x => x.Id == transaction.FeeInvoiceId); var amount = Math.Min(transaction.Amount, invoice.TotalAmount - invoice.PaidAmount);
        transaction.Status = PaymentGatewayStatus.Completed; transaction.GatewayPaymentId = confirmation.PaymentId; transaction.CompletedOn = DateTime.UtcNow;
        invoice.PaidAmount += amount; invoice.Status = invoice.PaidAmount >= invoice.TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        db.FeePayments.Add(new FeePayment { TenantId = invoice.TenantId, BranchId = invoice.BranchId, FeeInvoiceId = invoice.Id, PaymentNumber = "PAY-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"), PaidOn = DateTime.UtcNow, Amount = amount, Mode = PaymentMode.OnlineGateway, Status = PaymentStatus.Completed, TransactionReference = confirmation.PaymentId, Notes = "Verified Stripe admission checkout." });
        await db.SaveChangesAsync(); StatusMessage = "Payment completed and verified.";
    }
}
