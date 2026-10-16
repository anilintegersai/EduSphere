using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Examinations;

[Authorize]
public sealed class SeatingPlanModel(TenantDbContext db, IBranchAccessService branchAccess) : PageModel
{
    public ExamSeatingPlan Plan { get; private set; } = null!;
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var plan = await db.ExamSeatingPlans.Include(x=>x.ExamSchedule)!.ThenInclude(x=>x!.Exam).Include(x=>x.ExamSchedule)!.ThenInclude(x=>x!.Subject).Include(x=>x.Assignments).ThenInclude(x=>x.StudentProfile).Include(x=>x.Assignments).ThenInclude(x=>x.Room).SingleOrDefaultAsync(x=>x.Id==id);
        if(plan?.ExamSchedule?.Exam is null || !await branchAccess.CanAccessBranchAsync(User,plan.ExamSchedule.Exam.BranchId)) return NotFound();
        Plan=plan; return Page();
    }
}
