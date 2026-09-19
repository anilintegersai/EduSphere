using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class AdministrationDefaultsBootstrapper
{
    public static async Task ApplyAdministrationDefaultsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        try
        {
            foreach (var definition in PermissionKeys.Definitions)
                if (!await db.PermissionDefinitions.IgnoreQueryFilters().AnyAsync(p => p.Key == definition.Key))
                    db.PermissionDefinitions.Add(new PermissionDefinition
                    {
                        Key = definition.Key, DisplayName = definition.Value.Name, Module = definition.Value.Module,
                        Description = definition.Value.Description, IsActive = true
                    });
            if (!await db.SubscriptionPlans.IgnoreQueryFilters().AnyAsync())
            {
                db.SubscriptionPlans.AddRange(
                    new SubscriptionPlan { Code = "STARTER", Name = "Starter", MonthlyPrice = 4999, AnnualPrice = 49990, MaxBranches = 2, MaxStudents = 1000, MaxStaff = 100, IncludedFeaturesJson = "[\"Academics\",\"Attendance\",\"Examinations\"]" },
                    new SubscriptionPlan { Code = "PROFESSIONAL", Name = "Professional", MonthlyPrice = 12999, AnnualPrice = 129990, MaxBranches = 10, MaxStudents = 10000, MaxStaff = 1000, IncludedFeaturesJson = "[\"All core modules\",\"Enterprise modules\",\"AI question papers\"]" },
                    new SubscriptionPlan { Code = "ENTERPRISE", Name = "Enterprise", MonthlyPrice = 0, AnnualPrice = 0, MaxBranches = 0, MaxStudents = 0, MaxStaff = 0, IncludedFeaturesJson = "[\"Unlimited scale\",\"Custom integrations\",\"Priority support\"]" });
            }
            if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdministrationDefaultsBootstrapper")
                .LogWarning(ex, "Administration defaults could not be initialized. Apply the administration migration first.");
        }
    }
}
