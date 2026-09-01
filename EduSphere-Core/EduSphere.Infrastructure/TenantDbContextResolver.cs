using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduSphere.Infrastructure;

public interface ITenantDbContextResolver
{
    Task<string> ResolveTenantConnectionStringAsync(string tenantIdentifier);
    DbContextOptions<TenantDbContext> ResolveDbContextOptions(string tenantIdentifier);
}

public class TenantDbContextResolver : ITenantDbContextResolver
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _tenantConnectionStrings = new();

    public TenantDbContextResolver(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
        optionsBuilder.UseNpgsql(connectionString);

        // Optional: Enable sensitive data logging for debugging only
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return optionsBuilder.Options;
    }

    private string SimulateTenantConnectionString(string mainConnectionString, string tenantIdentifier)
    {
        // Example: Host=localhost;Database=EduSphere;Username=postgres;Password=pass
        var uri = new Uri(mainConnectionString);
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(mainConnectionString);
        builder.Database = $"edusphere_{tenantIdentifier}"; // Tenant-specific database name
        return builder.ToString();
    }
}