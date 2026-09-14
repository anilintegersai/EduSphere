using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Apply;

[AllowAnonymous]
public class SubmittedModel : PageModel
{
    private readonly TenantDbContext _dbContext;

    public SubmittedModel(TenantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public AdmissionApplication? Application { get; private set; }
    public Tenant? Tenant { get; private set; }

    public async Task OnGetAsync(Guid tenantId, string applicationNumber)
    {
        Tenant = await _dbContext.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId);
        Application = await _dbContext.AdmissionApplications
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.ApplicationNumber == applicationNumber);
    }
}
