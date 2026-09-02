using EduSphere.Application.Interfaces;
using EduSphere.Application.Services;
using EduSphere.Domain.Interfaces;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Infrastructure.Repositories;
using EduSphere.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduSphere.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services (user/role management is handled by ASP.NET Core Identity's
        // UserManager/RoleManager/SignInManager, registered by the web host).
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IBranchService, BranchService>();

        // Generic CRUD for tenant-owned auditable entities (academic structure, etc.)
        services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, string connectionString, string provider = "SqlServer")
    {
        // Request-scoped ambient tenant, populated by the tenant resolution middleware.
        services.AddScoped<ITenantContext, TenantContext>();

        // Register DbContext (scoped) so repositories and the unit of work share one instance per request.
        // The provider is chosen from configuration to keep persistence database-agnostic; provider-specific
        // wiring stays isolated here in Infrastructure.
        services.AddDbContext<TenantDbContext>(options =>
        {
            ConfigureProvider(options, connectionString, provider);
            // Branch and its principal Tenant sit on opposite sides of the tenant filter,
            // so the filtered-required-navigation heuristic warning is a false positive here.
            options.ConfigureWarnings(w =>
                w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
        });

        // Register repositories and UnitOfWork
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register tenant context resolver (future dedicated-DB / hybrid tenancy path)
        services.AddScoped<ITenantDbContextResolver, TenantDbContextResolver>();

        // JWT token issuance for API clients
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }

    /// <summary>
    /// Applies the configured EF Core provider. Keeping this the single place that
    /// selects a provider is what keeps the rest of the codebase database-agnostic.
    /// </summary>
    public static void ConfigureProvider(DbContextOptionsBuilder options, string connectionString, string provider)
    {
        switch (provider?.Trim().ToLowerInvariant())
        {
            case "sqlserver":
            case "mssql":
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
                break;

            case "postgres":
            case "postgresql":
            case "npgsql":
                options.UseNpgsql(connectionString);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider '{provider}'. Supported: SqlServer, Postgres.");
        }
    }
}
