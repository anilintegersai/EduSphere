using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EduSphere.Web.Data;

/// <summary>
/// First-run convenience seeder. Creates the schema (dev-only EnsureCreated),
/// then ensures a SuperAdmin account exists. Platform roles are seeded through
/// the model (see <see cref="TenantDbContext"/>), so this only provisions the user.
/// </summary>
public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var db = services.GetRequiredService<TenantDbContext>();

        // Apply any pending EF Core migrations (creates the database + schema, including
        // seeded roles) before provisioning the first user.
        await db.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = configuration["Seed:AdminEmail"] ?? "superadmin@edusphere.local";
        var password = configuration["Seed:AdminPassword"] ?? "ChangeMe!123";

        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Super",
            LastName = "Admin",
            UserType = UserType.SuperAdmin,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.SuperAdmin);
        }
        else
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed SuperAdmin user: {errors}");
        }
    }
}
