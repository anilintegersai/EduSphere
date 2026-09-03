using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EduSphere.Infrastructure;

public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var provider = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_PROVIDER") ?? "SqlServer";
        var connectionString = Environment.GetEnvironmentVariable("EDUSPHERE_MIGRATION_CONNECTION")
            ?? "Server=(localdb)\\mssqllocaldb;Database=EduSphere;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<TenantDbContext>();
        DependencyInjectionExtensions.ConfigureProvider(options, connectionString, provider);

        return new TenantDbContext(options.Options, new TenantContext());
    }
}
