using EduSphere.Application.Interfaces;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ITenantContext _tenant;
    private readonly ITenantService _tenants;
    private readonly IBranchService _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Course> _courses;
    private readonly ICrudService<Subject> _subjects;

    public IndexModel(
        ITenantContext tenant,
        ITenantService tenants,
        IBranchService branches,
        ICrudService<AcademicYear> years,
        ICrudService<Course> courses,
        ICrudService<Subject> subjects)
    {
        _tenant = tenant;
        _tenants = tenants;
        _branches = branches;
        _years = years;
        _courses = courses;
        _subjects = subjects;
    }

    public bool IsAuthenticated { get; private set; }
    public bool IsSuper { get; private set; }
    public bool HasTenant => _tenant.HasTenant;
    public string? CurrentTenant => _tenant.TenantIdentifier;

    public int TenantCount { get; private set; }
    public int BranchCount { get; private set; }
    public int AcademicYearCount { get; private set; }
    public int CourseCount { get; private set; }
    public int SubjectCount { get; private set; }

    public async Task OnGetAsync()
    {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        if (!IsAuthenticated) return;

        IsSuper = User.IsInRole(Roles.SuperAdmin);
        if (IsSuper)
            TenantCount = (await _tenants.GetAllTenantsAsync()).Count();

        if (_tenant.TenantId is Guid tenantId)
        {
            BranchCount = (await _branches.GetBranchesByTenantAsync(tenantId)).Count();
            AcademicYearCount = (await _years.ListAsync()).Count;
            CourseCount = (await _courses.ListAsync()).Count;
            SubjectCount = (await _subjects.ListAsync()).Count;
        }
    }
}
