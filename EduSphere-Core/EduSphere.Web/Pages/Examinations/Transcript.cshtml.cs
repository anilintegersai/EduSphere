using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Examinations;
[Authorize]
public sealed class TranscriptModel(TenantDbContext db, IBranchAccessService branchAccess) : PageModel
{
    public StudentProfile Student {get;private set;}=null!; public StudentTranscript Transcript {get;private set;}=null!;
    public async Task<IActionResult> OnGetAsync(Guid studentId){var student=await db.StudentProfiles.SingleOrDefaultAsync(x=>x.Id==studentId);if(student is null||!await branchAccess.CanAccessBranchAsync(User,student.BranchId))return NotFound();var transcript=await db.StudentTranscripts.Include(x=>x.Lines).Where(x=>x.StudentProfileId==studentId).OrderByDescending(x=>x.IssuedOn).FirstOrDefaultAsync();if(transcript is null)return NotFound();Student=student;Transcript=transcript;return Page();}
}
