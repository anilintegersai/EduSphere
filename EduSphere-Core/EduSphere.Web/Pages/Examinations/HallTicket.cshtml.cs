using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Examinations;
[Authorize]
public sealed class HallTicketModel(TenantDbContext db, IBranchAccessService branchAccess) : PageModel
{
    public ExamHallTicket Ticket { get; private set; }=null!; public IReadOnlyList<ExamSchedule> Schedules {get;private set;}=[];
    public async Task<IActionResult> OnGetAsync(Guid id){var ticket=await db.ExamHallTickets.Include(x=>x.Exam).Include(x=>x.StudentProfile).ThenInclude(x=>x!.Section).SingleOrDefaultAsync(x=>x.Id==id);if(ticket?.Exam is null||ticket.StudentProfile is null)return NotFound();var uid=Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var parsed)?parsed:Guid.Empty;var owns=ticket.StudentProfile.UserId==uid;var parent=await db.StudentGuardians.AnyAsync(x=>x.StudentProfileId==ticket.StudentProfileId&&x.ParentUserId==uid&&x.HasPortalAccess);if(!owns&&!parent&&!await branchAccess.CanAccessBranchAsync(User,ticket.Exam.BranchId))return Forbid();Ticket=ticket;Schedules=await db.ExamSchedules.Include(x=>x.Subject).Where(x=>x.ExamId==ticket.ExamId&&x.SectionId==ticket.StudentProfile.SectionId).OrderBy(x=>x.ExamDate).ThenBy(x=>x.StartsAt).ToListAsync();return Page();}
}
