using EduSphere.Application.Interfaces;
using EduSphere.Application.Services;
using EduSphere.Domain.Interfaces;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Infrastructure.Repositories;
using EduSphere.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = DatabaseProviderOptions.FromConfiguration(configuration);
        return services.AddInfrastructureServices(databaseOptions);
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, string connectionString, string provider = "SqlServer")
    {
        var databaseOptions = DatabaseProviderOptions.Create(connectionString, provider);
        return services.AddInfrastructureServices(databaseOptions);
    }

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, DatabaseProviderOptions databaseOptions)
    {
        services.AddSingleton(databaseOptions);

        // Request-scoped ambient tenant, populated by the tenant resolution middleware.
        services.AddScoped<ITenantContext, TenantContext>();

        // Register DbContext (scoped) so repositories and the unit of work share one instance per request.
        // The provider is chosen from configuration to keep persistence database-agnostic; provider-specific
        // wiring stays isolated here in Infrastructure.
        services.AddDbContext<TenantDbContext>(options =>
        {
            ConfigureProvider(options, databaseOptions);
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

        // Centralized migration strategy: None, Validate, or Migrate.
        services.AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

        return services;
    }

    /// <summary>
    /// Applies the configured EF Core provider. Keeping this the single place that
    /// selects a provider is what keeps the rest of the codebase database-agnostic.
    /// </summary>
    public static void ConfigureProvider(DbContextOptionsBuilder options, string connectionString, string provider)
        => ConfigureProvider(options, DatabaseProviderOptions.Create(connectionString, provider));

    public static void ConfigureProvider(DbContextOptionsBuilder options, DatabaseProviderOptions databaseOptions)
    {
        switch (databaseOptions.Provider)
        {
            case DatabaseProvider.SqlServer:
                options.UseSqlServer(databaseOptions.ConnectionString, sql => sql.EnableRetryOnFailure());
                break;

            case DatabaseProvider.PostgreSql:
                options.UseNpgsql(databaseOptions.ConnectionString, sql => sql.EnableRetryOnFailure());
                break;

            case DatabaseProvider.MySql:
                options.UseMySql(
                    databaseOptions.ConnectionString,
                    ResolveMySqlServerVersion(databaseOptions),
                    sql => sql.EnableRetryOnFailure());
                break;

            case DatabaseProvider.Sqlite:
                options.UseSqlite(databaseOptions.ConnectionString);
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider '{databaseOptions.Provider}'.");
        }
    }

    private static ServerVersion ResolveMySqlServerVersion(DatabaseProviderOptions databaseOptions)
    {
        return databaseOptions.MySqlServerKind == MySqlServerKind.MariaDb
            ? new MariaDbServerVersion(databaseOptions.MySqlServerVersion)
            : new MySqlServerVersion(databaseOptions.MySqlServerVersion);
    }
}
