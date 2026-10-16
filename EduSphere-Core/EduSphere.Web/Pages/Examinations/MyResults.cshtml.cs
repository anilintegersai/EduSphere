using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Examinations;

[Authorize(Roles = Roles.Student + "," + Roles.Parent)]
public sealed class MyResultsModel(TenantDbContext db) : PageModel
{
    public IReadOnlyList<StudentProfile> Students { get; private set; }=[];
    public IReadOnlyList<Result> Results { get; private set; }=[];
    public Guid? SelectedStudentId { get; private set; }
    public async Task OnGetAsync(Guid? studentId)
    {
        var userId=Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ids=User.IsInRole(Roles.Parent)
            ? await db.StudentGuardians.Where(x=>x.ParentUserId==userId&&x.HasPortalAccess).Select(x=>x.StudentProfileId).Distinct().ToListAsync()
            : await db.StudentProfiles.Where(x=>x.UserId==userId).Select(x=>x.Id).ToListAsync();
        Students=await db.StudentProfiles.Where(x=>ids.Contains(x.Id)).OrderBy(x=>x.FirstName).ToListAsync();
        SelectedStudentId=studentId.HasValue&&ids.Contains(studentId.Value)?studentId:ids.FirstOrDefault();
        if(SelectedStudentId!=Guid.Empty) Results=await db.Results.Include(x=>x.Exam).Include(x=>x.AcademicYear).Where(x=>x.StudentProfileId==SelectedStudentId&&x.Status==ResultStatus.Published).OrderByDescending(x=>x.AcademicYear!.StartDate).ThenByDescending(x=>x.Exam!.StartDate).ToListAsync();
    }
}
