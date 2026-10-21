using System.Security.Claims;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Library;

[Authorize]
public sealed class OpacModel(TenantDbContext db, ILibraryDepthService library) : PageModel
{
    public IReadOnlyList<LibraryBook> Books { get; private set; } = [];
    public IReadOnlyList<LibraryDigitalResource> Resources { get; private set; } = [];
    public IReadOnlyList<LibraryBookIssue> Issues { get; private set; } = [];
    public IReadOnlyList<LibraryReservation> Reservations { get; private set; } = [];
    public LibraryMember? Member { get; private set; }
    [BindProperty(SupportsGet = true)] public string? Query { get; set; }
    [TempData] public string? Feedback { get; set; }
    public async Task OnGetAsync() => await Load();
    public async Task<IActionResult> OnPostReserveAsync(Guid bookId) { await LoadMember(); if (Member is null) { Feedback = "No active library membership is linked to your account."; return RedirectToPage(); } try { var r = await library.ReserveAsync(bookId, Member.Id); Feedback = $"Reservation added at queue position {r.QueuePosition}."; } catch (InvalidOperationException ex) { Feedback = ex.Message; } return RedirectToPage(new { Query }); }
    private async Task Load() { await LoadMember(); var query = Query?.Trim(); Books = await db.LibraryBooks.Include(x => x.Copies).Where(x => string.IsNullOrEmpty(query) || x.Title.Contains(query) || x.Authors.Contains(query) || (x.Isbn != null && x.Isbn.Contains(query))).OrderBy(x => x.Title).Take(100).ToListAsync(); var levels = new List<DigitalAccessLevel> { DigitalAccessLevel.AllMembers }; if (User.IsInRole(EduSphere.Domain.Constants.Roles.Student)) levels.Add(DigitalAccessLevel.Students); if (User.IsInRole(EduSphere.Domain.Constants.Roles.Teacher)) levels.Add(DigitalAccessLevel.Teachers); Resources = await db.LibraryDigitalResources.Where(x => x.IsActive && levels.Contains(x.AccessLevel) && (!x.LicenseExpiresOn.HasValue || x.LicenseExpiresOn >= DateOnly.FromDateTime(DateTime.UtcNow))).OrderBy(x => x.Title).ToListAsync(); if (Member is not null) { Issues = await db.LibraryBookIssues.Include(x => x.LibraryBookCopy)!.ThenInclude(x => x!.LibraryBook).Where(x => x.LibraryMemberId == Member.Id).OrderByDescending(x => x.IssueDate).ToListAsync(); Reservations = await db.LibraryReservations.Include(x => x.LibraryBook).Where(x => x.LibraryMemberId == Member.Id).OrderByDescending(x => x.ReservedOn).ToListAsync(); } }
    private async Task LoadMember() { var idText = User.FindFirstValue(ClaimTypes.NameIdentifier); if (Guid.TryParse(idText, out var userId)) Member = await db.LibraryMembers.FirstOrDefaultAsync(x => x.UserId == userId || x.StudentProfile!.UserId == userId || x.TeacherProfile!.UserId == userId); }
}
