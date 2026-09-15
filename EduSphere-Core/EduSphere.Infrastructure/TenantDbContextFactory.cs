using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EduSphere.Infrastructure;

public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var providerName =
            Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_PROVIDER") ??
            configuration?["Database:Provider"] ??
            "SqlServer";
        var provider = DatabaseProviderOptions.ParseProvider(providerName);
        var connectionString = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_CONNECTION")
            ?? GetConfiguredConnectionString(configuration)
            ?? GetDefaultConnectionString(provider);
        var mySqlServerVersion =
            Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_MYSQL_SERVER_VERSION") ??
            configuration?["Database:MySql:ServerVersion"] ??
            configuration?["Database:MySqlServerVersion"];
        var mySqlServerType =
            Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_MYSQL_SERVER_TYPE") ??
            configuration?["Database:MySql:ServerType"] ??
            configuration?["Database:MySqlServerType"];

        var databaseOptions = DatabaseProviderOptions.Create(
            connectionString,
            providerName,
            mySqlServerVersion: string.IsNullOrWhiteSpace(mySqlServerVersion)
                ? null
                : DatabaseProviderOptions.ParseVersion(mySqlServerVersion),
            mySqlServerKind: DatabaseProviderOptions.ParseMySqlServerKind(mySqlServerType));

        var options = new DbContextOptionsBuilder<TenantDbContext>();
        DependencyInjectionExtensions.ConfigureProvider(options, databaseOptions);

        return new TenantDbContext(options.Options, new TenantContext());
    }

    private static IConfigurationRoot? BuildConfiguration()
    {
        var contentRoot = ResolveWebContentRoot();
        if (contentRoot is null)
            return null;

        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
            "Development";

        return new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();
    }

    private static string? GetConfiguredConnectionString(IConfiguration? configuration)
    {
        if (configuration is null)
            return null;

        var connectionStringName =
            configuration["Database:ConnectionStringName"] ??
            DatabaseProviderOptions.DefaultConnectionName;
        return configuration.GetConnectionString(connectionStringName);
    }

    private static string? ResolveWebContentRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "EduSphere.Web"),
            Path.GetFullPath(Path.Combine(currentDirectory, "..", "EduSphere.Web"))
        };

        return candidates.FirstOrDefault(path =>
            File.Exists(Path.Combine(path, "appsettings.json")));
    }

    private static string GetDefaultConnectionString(DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.SqlServer =>
                "Server=(localdb)\\mssqllocaldb;Database=EduSphere;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True;",
            DatabaseProvider.PostgreSql =>
                "Host=localhost;Database=edusphere;Username=postgres;Password=postgres",
            DatabaseProvider.MySql =>
                "Server=localhost;Database=edusphere;User=root;Password=password;",
            DatabaseProvider.Sqlite =>
                "Data Source=edusphere.db",
            _ => throw new InvalidOperationException($"Unsupported database provider '{provider}'.")
        };
    }
}
