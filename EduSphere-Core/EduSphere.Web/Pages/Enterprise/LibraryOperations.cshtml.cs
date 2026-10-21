using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Enterprise;

[Authorize(Policy = AuthorizationPolicies.LibraryManager)]
public sealed class LibraryOperationsModel(TenantDbContext db, IBranchAccessService branchAccess, ILibraryDepthService library) : PageModel
{
    public IReadOnlyList<Branch> Branches { get; private set; } = [];
    public IReadOnlyList<LibraryBook> Books { get; private set; } = [];
    public IReadOnlyList<LibraryBookCopy> Copies { get; private set; } = [];
    public IReadOnlyList<LibraryMember> Members { get; private set; } = [];
    public IReadOnlyList<LibraryCirculationPolicy> Policies { get; private set; } = [];
    public IReadOnlyList<LibraryItemIncident> Incidents { get; private set; } = [];
    public IReadOnlyList<LibraryVendor> Vendors { get; private set; } = [];
    public IReadOnlyList<LibraryPurchaseOrder> Orders { get; private set; } = [];
    public IReadOnlyList<LibraryStockVerification> Verifications { get; private set; } = [];
    public IReadOnlyList<LibraryDigitalResource> Resources { get; private set; } = [];
    [TempData] public string? Feedback { get; set; }
    [BindProperty] public PolicyInput Policy { get; set; } = new();
    [BindProperty] public IncidentInput Incident { get; set; } = new();
    [BindProperty] public VendorInput Vendor { get; set; } = new();
    [BindProperty] public OrderInput Order { get; set; } = new();
    [BindProperty] public StockInput Stock { get; set; } = new();
    [BindProperty] public ResourceInput Resource { get; set; } = new();
    [BindProperty] public ScanInput Scan { get; set; } = new();

    public sealed class PolicyInput { public Guid? BranchId { get; set; } public LibraryMemberType MemberType { get; set; } [Range(1, 50)] public int MaxIssues { get; set; } = 2; [Range(1, 365)] public int LoanDays { get; set; } = 14; [Range(0, 20)] public int MaxRenewals { get; set; } = 1; [Range(0, 30)] public int GraceDays { get; set; } [Range(0, 10000)] public decimal FinePerDay { get; set; } [Range(0, 100000)] public decimal MaximumFine { get; set; } = 1000; [Range(0, 10)] public decimal LostBookMultiplier { get; set; } = 1; [Range(0, 100)] public decimal DamageChargePercent { get; set; } = 25; [Range(1, 30)] public int ReservationHoldDays { get; set; } = 3; }
    public sealed class IncidentInput { [Required] public Guid? CopyId { get; set; } [Required] public Guid? MemberId { get; set; } public Guid? IssueId { get; set; } public LibraryItemCondition Condition { get; set; } = LibraryItemCondition.Damaged; [Required, StringLength(1000)] public string Details { get; set; } = ""; [Range(0, 100000)] public decimal? Amount { get; set; } }
    public sealed class VendorInput { [Required] public Guid? BranchId { get; set; } [Required, StringLength(160)] public string Name { get; set; } = ""; [StringLength(120)] public string? ContactPerson { get; set; } [EmailAddress] public string? Email { get; set; } public string? Phone { get; set; } }
    public sealed class OrderInput { [Required] public Guid? BranchId { get; set; } [Required] public Guid? VendorId { get; set; } [Required] public string OrderNumber { get; set; } = ""; [Required] public string Title { get; set; } = ""; public Guid? BookId { get; set; } [Range(1, 1000)] public int Quantity { get; set; } = 1; [Range(0, 100000)] public decimal UnitPrice { get; set; } public DateOnly? ExpectedOn { get; set; } }
    public sealed class StockInput { [Required] public Guid? BranchId { get; set; } public Guid? VerificationId { get; set; } [Required] public string NameOrScanCode { get; set; } = ""; }
    public sealed class ResourceInput { [Required] public Guid? BranchId { get; set; } public Guid? BookId { get; set; } [Required] public string Title { get; set; } = ""; public DigitalResourceType ResourceType { get; set; } = DigitalResourceType.EBook; [Required, Url] public string ResourceUrl { get; set; } = ""; public DigitalAccessLevel AccessLevel { get; set; } = DigitalAccessLevel.AllMembers; public string? LicenseName { get; set; } public DateOnly? LicenseExpiresOn { get; set; } }
    public sealed class ScanInput { [Required] public Guid? BranchId { get; set; } [Required, StringLength(120)] public string ScanCode { get; set; } = ""; public LibraryScanPurpose Purpose { get; set; } = LibraryScanPurpose.Lookup; }

    public async Task OnGetAsync() => await Load();
    public async Task<IActionResult> OnPostPolicyAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } var existing = await db.LibraryCirculationPolicies.FirstOrDefaultAsync(x => x.BranchId == Policy.BranchId && x.MemberType == Policy.MemberType); if (existing is null) { existing = new LibraryCirculationPolicy { BranchId = Policy.BranchId, MemberType = Policy.MemberType }; db.Add(existing); } existing.MaxIssues = Policy.MaxIssues; existing.LoanDays = Policy.LoanDays; existing.MaxRenewals = Policy.MaxRenewals; existing.GraceDays = Policy.GraceDays; existing.FinePerDay = Policy.FinePerDay; existing.MaximumFine = Policy.MaximumFine; existing.LostBookMultiplier = Policy.LostBookMultiplier; existing.DamageChargePercent = Policy.DamageChargePercent; existing.ReservationHoldDays = Policy.ReservationHoldDays; await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostIncidentAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } await library.ReportIncidentAsync(Incident.CopyId!.Value, Incident.MemberId!.Value, Incident.IssueId, Incident.Condition, Incident.Details, Incident.Amount); Feedback = "Incident recorded and Finance invoice created where applicable."; return RedirectToPage(); }
    public async Task<IActionResult> OnPostVendorAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } db.LibraryVendors.Add(new() { BranchId = Vendor.BranchId!.Value, Name = Vendor.Name, ContactPerson = Vendor.ContactPerson, Email = Vendor.Email, Phone = Vendor.Phone }); await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostOrderAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } var order = new LibraryPurchaseOrder { BranchId = Order.BranchId!.Value, LibraryVendorId = Order.VendorId!.Value, OrderNumber = Order.OrderNumber, ExpectedOn = Order.ExpectedOn, Status = LibraryPurchaseOrderStatus.Submitted, TotalAmount = Order.Quantity * Order.UnitPrice, Lines = [new() { BranchId = Order.BranchId.Value, LibraryBookId = Order.BookId, Title = Order.Title, Quantity = Order.Quantity, UnitPrice = Order.UnitPrice }] }; db.Add(order); await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostReceiveOrderAsync(Guid id) { var order = await db.LibraryPurchaseOrders.Include(x => x.Lines).SingleAsync(x => x.Id == id); foreach (var line in order.Lines) { var prior = line.ReceivedQuantity; line.ReceivedQuantity = line.Quantity; if (line.LibraryBookId is Guid bookId) for (var i = prior + 1; i <= line.Quantity; i++) db.LibraryBookCopies.Add(new() { BranchId = order.BranchId, LibraryBookId = bookId, AccessionNumber = $"{order.OrderNumber}-{line.Id.ToString("N")[..4]}-{i:000}".ToUpperInvariant(), Barcode = $"{order.OrderNumber}{line.Id.ToString("N")[..4]}{i:000}".ToUpperInvariant(), AcquisitionDate = DateOnly.FromDateTime(DateTime.UtcNow), Price = line.UnitPrice, Status = BookCopyStatus.Available }); } order.Status = LibraryPurchaseOrderStatus.Received; await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnPostScanAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } var code = Scan.ScanCode.Trim(); var copy = await db.LibraryBookCopies.FirstOrDefaultAsync(x => x.BranchId == Scan.BranchId && (x.Barcode == code || x.AccessionNumber == code)); db.LibraryBarcodeScans.Add(new() { BranchId = Scan.BranchId!.Value, ScanCode = code, Purpose = Scan.Purpose, LibraryBookCopyId = copy?.Id, ResultMessage = copy is null ? "No matching copy was found." : $"{copy.AccessionNumber} is {copy.Status}." }); await db.SaveChangesAsync(); Feedback = copy is null ? "No matching copy was found." : $"Located {copy.AccessionNumber}: {copy.Status}."; return RedirectToPage(); }
    public async Task<IActionResult> OnPostStockAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } if (Stock.VerificationId is null) { db.LibraryStockVerifications.Add(new() { BranchId = Stock.BranchId!.Value, Name = Stock.NameOrScanCode, Status = LibraryStockVerificationStatus.Draft }); await db.SaveChangesAsync(); } else await library.VerifyStockAsync(Stock.VerificationId.Value, Stock.NameOrScanCode); return RedirectToPage(); }
    public async Task<IActionResult> OnPostResourceAsync() { if (!ModelState.IsValid) { await Load(); return Page(); } db.LibraryDigitalResources.Add(new() { BranchId = Resource.BranchId!.Value, LibraryBookId = Resource.BookId, Title = Resource.Title, ResourceType = Resource.ResourceType, ResourceUrl = Resource.ResourceUrl, AccessLevel = Resource.AccessLevel, LicenseName = Resource.LicenseName, LicenseExpiresOn = Resource.LicenseExpiresOn }); await db.SaveChangesAsync(); return RedirectToPage(); }
    public async Task<IActionResult> OnGetLabelAsync(Guid id) { var copy = await db.LibraryBookCopies.Include(x => x.LibraryBook).SingleAsync(x => x.Id == id); var code = string.IsNullOrWhiteSpace(copy.Barcode) ? copy.AccessionNumber : copy.Barcode; return Content(library.CreateCode39Svg(code!, $"{copy.LibraryBook!.Title} | {copy.AccessionNumber}"), "image/svg+xml", Encoding.UTF8); }

    private async Task Load()
    {
        Branches = await branchAccess.FilterBranchesAsync(User, await db.Branches.AsNoTracking().ToListAsync()); var ids = Branches.Select(x => x.Id).ToList();
        Books = await db.LibraryBooks.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.Title).ToListAsync(); Copies = await db.LibraryBookCopies.Include(x => x.LibraryBook).Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.AccessionNumber).ToListAsync(); Members = await db.LibraryMembers.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.MemberNumber).ToListAsync(); Policies = await db.LibraryCirculationPolicies.Where(x => x.BranchId == null || ids.Contains(x.BranchId.Value)).ToListAsync(); Incidents = await db.LibraryItemIncidents.Include(x => x.LibraryBookCopy).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.ReportedOn).ToListAsync(); Vendors = await db.LibraryVendors.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.Name).ToListAsync(); Orders = await db.LibraryPurchaseOrders.Include(x => x.LibraryVendor).Include(x => x.Lines).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.OrderDate).ToListAsync(); Verifications = await db.LibraryStockVerifications.Include(x => x.Items).Where(x => ids.Contains(x.BranchId)).OrderByDescending(x => x.StartedOn).ToListAsync(); Resources = await db.LibraryDigitalResources.Where(x => ids.Contains(x.BranchId)).OrderBy(x => x.Title).ToListAsync(); Policy.BranchId ??= Branches.FirstOrDefault()?.Id; Vendor.BranchId ??= Branches.FirstOrDefault()?.Id; Order.BranchId ??= Branches.FirstOrDefault()?.Id; Stock.BranchId ??= Branches.FirstOrDefault()?.Id; Resource.BranchId ??= Branches.FirstOrDefault()?.Id; Scan.BranchId ??= Branches.FirstOrDefault()?.Id;
    }
}
