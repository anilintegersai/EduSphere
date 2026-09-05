using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EduSphere.Infrastructure;

public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var providerName = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_PROVIDER") ?? "SqlServer";
        var provider = DatabaseProviderOptions.ParseProvider(providerName);
        var connectionString = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_CONNECTION")
            ?? GetDefaultConnectionString(provider);
        var mySqlServerVersion = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_MYSQL_SERVER_VERSION");
        var mySqlServerType = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_MYSQL_SERVER_TYPE");

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
