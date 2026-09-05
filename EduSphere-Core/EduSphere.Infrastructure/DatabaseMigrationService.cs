using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduSphere.Infrastructure;

public interface IDatabaseMigrationService
{
    Task ApplyConfiguredStrategyAsync(CancellationToken cancellationToken = default);
}

internal sealed class DatabaseMigrationService : IDatabaseMigrationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DatabaseProviderOptions _databaseOptions;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(
        IServiceScopeFactory scopeFactory,
        DatabaseProviderOptions databaseOptions,
        ILogger<DatabaseMigrationService> logger)
    {
        _scopeFactory = scopeFactory;
        _databaseOptions = databaseOptions;
        _logger = logger;
    }

    public async Task ApplyConfiguredStrategyAsync(CancellationToken cancellationToken = default)
    {
        switch (_databaseOptions.MigrationStrategy)
        {
            case DatabaseMigrationStrategy.None:
                _logger.LogInformation("Database migration strategy is None. Skipping migration check.");
                return;

            case DatabaseMigrationStrategy.Validate:
                await ValidateNoPendingMigrationsAsync(cancellationToken);
                return;

            case DatabaseMigrationStrategy.Migrate:
                await MigrateAsync(cancellationToken);
                return;

            default:
                throw new InvalidOperationException(
                    $"Unsupported migration strategy '{_databaseOptions.MigrationStrategy}'.");
        }
    }

    private async Task ValidateNoPendingMigrationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();

        if (pendingMigrations.Length == 0)
        {
            _logger.LogInformation(
                "Database migration validation passed for {Provider}; no pending migrations.",
                _databaseOptions.Provider);
            return;
        }

        throw new InvalidOperationException(
            $"Database has pending migrations for {_databaseOptions.Provider}: {string.Join(", ", pendingMigrations)}.");
    }

    private async Task MigrateAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        _logger.LogInformation("Applying EF Core migrations for {Provider}.", _databaseOptions.Provider);
        await context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("EF Core migrations applied for {Provider}.", _databaseOptions.Provider);
    }
}

public static class DatabaseMigrationServiceProviderExtensions
{
    public static Task ApplyDatabaseMigrationStrategyAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        return serviceProvider
            .GetRequiredService<IDatabaseMigrationService>()
            .ApplyConfiguredStrategyAsync(cancellationToken);
    }
}
