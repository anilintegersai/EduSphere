using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Npgsql;

namespace EduSphere.Infrastructure;

public interface ITenantDbContextResolver
{
    Task<string> ResolveTenantConnectionStringAsync(string tenantIdentifier);
    DbContextOptions<TenantDbContext> ResolveDbContextOptions(string tenantIdentifier);
}

public class TenantDbContextResolver : ITenantDbContextResolver
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseProviderOptions _databaseOptions;
    private readonly Dictionary<string, string> _tenantConnectionStrings = new();

    public TenantDbContextResolver(IConfiguration configuration)
        : this(configuration, DatabaseProviderOptions.FromConfiguration(configuration))
    {
    }

    public TenantDbContextResolver(IConfiguration configuration, DatabaseProviderOptions databaseOptions)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
    }

    public async Task<string> ResolveTenantConnectionStringAsync(string tenantIdentifier)
    {
        if (string.IsNullOrWhiteSpace(tenantIdentifier))
            throw new ArgumentException("Tenant identifier cannot be null or empty.", nameof(tenantIdentifier));

        if (_tenantConnectionStrings.TryGetValue(tenantIdentifier, out var connectionString))
            return connectionString;

        // In production, this would fetch from a secure store (e.g., Azure Key Vault, AWS Secrets Manager, or a Tenant table in a master DB)
        // For now, we simulate it using the main configuration and a naming convention.
        var mainConnectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");

        // Simulate tenant-specific DB by appending tenant identifier to the database name
        var tenantConnectionString = SimulateTenantConnectionString(mainConnectionString, tenantIdentifier);
        _tenantConnectionStrings[tenantIdentifier] = tenantConnectionString;
        return tenantConnectionString;
    }

    public DbContextOptions<TenantDbContext> ResolveDbContextOptions(string tenantIdentifier)
    {
        var connectionString = ResolveTenantConnectionStringAsync(tenantIdentifier).Result;
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        DependencyInjectionExtensions.ConfigureProvider(optionsBuilder, _databaseOptions.WithConnectionString(connectionString));

        // Optional: Enable sensitive data logging for debugging only
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return optionsBuilder.Options;
    }

    private string SimulateTenantConnectionString(string mainConnectionString, string tenantIdentifier)
    {
        var tenantDatabaseName = BuildTenantDatabaseName("edusphere", tenantIdentifier);

        return _databaseOptions.Provider switch
        {
            DatabaseProvider.SqlServer => BuildSqlServerTenantConnectionString(mainConnectionString, tenantDatabaseName),
            DatabaseProvider.PostgreSql => BuildPostgreSqlTenantConnectionString(mainConnectionString, tenantDatabaseName),
            DatabaseProvider.MySql => BuildMySqlTenantConnectionString(mainConnectionString, tenantDatabaseName),
            DatabaseProvider.Sqlite => BuildSqliteTenantConnectionString(mainConnectionString, tenantIdentifier),
            _ => throw new InvalidOperationException($"Unsupported database provider '{_databaseOptions.Provider}'.")
        };
    }

    private static string BuildSqlServerTenantConnectionString(string mainConnectionString, string tenantDatabaseName)
    {
        var builder = new SqlConnectionStringBuilder(mainConnectionString)
        {
            InitialCatalog = tenantDatabaseName
        };
        return builder.ToString();
    }

    private static string BuildPostgreSqlTenantConnectionString(string mainConnectionString, string tenantDatabaseName)
    {
        var builder = new NpgsqlConnectionStringBuilder(mainConnectionString)
        {
            Database = tenantDatabaseName
        };
        return builder.ToString();
    }

    private static string BuildMySqlTenantConnectionString(string mainConnectionString, string tenantDatabaseName)
    {
        var builder = new MySqlConnectionStringBuilder(mainConnectionString)
        {
            Database = tenantDatabaseName
        };
        return builder.ToString();
    }

    private static string BuildSqliteTenantConnectionString(string mainConnectionString, string tenantIdentifier)
    {
        var builder = new SqliteConnectionStringBuilder(mainConnectionString);

        if (builder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
            return builder.ToString();

        var dataSource = string.IsNullOrWhiteSpace(builder.DataSource)
            ? "edusphere.db"
            : builder.DataSource;
        var directory = Path.GetDirectoryName(dataSource);
        var fileName = Path.GetFileNameWithoutExtension(dataSource);
        var extension = Path.GetExtension(dataSource);
        var tenantFileName = $"{fileName}_{NormalizeTenantIdentifier(tenantIdentifier)}{extension}";

        builder.DataSource = string.IsNullOrWhiteSpace(directory)
            ? tenantFileName
            : Path.Combine(directory, tenantFileName);

        return builder.ToString();
    }

    private static string BuildTenantDatabaseName(string baseName, string tenantIdentifier)
        => $"{baseName}_{NormalizeTenantIdentifier(tenantIdentifier)}";

    private static string NormalizeTenantIdentifier(string tenantIdentifier)
    {
        var normalized = new string(tenantIdentifier
            .Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '_')
            .ToArray())
            .Trim('_');

        return string.IsNullOrWhiteSpace(normalized) ? "tenant" : normalized;
    }
}
