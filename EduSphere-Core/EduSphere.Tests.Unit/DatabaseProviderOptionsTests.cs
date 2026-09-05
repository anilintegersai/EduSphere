using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EduSphere.Tests.Unit;

public class DatabaseProviderOptionsTests
{
    [Theory]
    [InlineData("SqlServer", DatabaseProvider.SqlServer)]
    [InlineData("mssql", DatabaseProvider.SqlServer)]
    [InlineData("PostgreSql", DatabaseProvider.PostgreSql)]
    [InlineData("npgsql", DatabaseProvider.PostgreSql)]
    [InlineData("MySql", DatabaseProvider.MySql)]
    [InlineData("MariaDb", DatabaseProvider.MySql)]
    [InlineData("Sqlite", DatabaseProvider.Sqlite)]
    public void ParseProvider_SupportsExpectedAliases(string providerName, DatabaseProvider expected)
    {
        Assert.Equal(expected, DatabaseProviderOptions.ParseProvider(providerName));
    }

    [Theory]
    [InlineData("None", DatabaseMigrationStrategy.None)]
    [InlineData("manual", DatabaseMigrationStrategy.None)]
    [InlineData("Validate", DatabaseMigrationStrategy.Validate)]
    [InlineData("Migrate", DatabaseMigrationStrategy.Migrate)]
    [InlineData("auto", DatabaseMigrationStrategy.Migrate)]
    public void ParseMigrationStrategy_SupportsExpectedAliases(string strategy, DatabaseMigrationStrategy expected)
    {
        Assert.Equal(expected, DatabaseProviderOptions.ParseMigrationStrategy(strategy));
    }

    [Fact]
    public void FromConfiguration_ReadsProviderConnectionAndMigrationSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "MariaDb",
                ["Database:ConnectionStringName"] = "MySql",
                ["Database:MigrationStrategy"] = "Validate",
                ["Database:MySql:ServerType"] = "MariaDb",
                ["Database:MySql:ServerVersion"] = "10.11.6",
                ["ConnectionStrings:MySql"] = "Server=localhost;Database=edusphere;User=root;Password=password;"
            })
            .Build();

        var options = DatabaseProviderOptions.FromConfiguration(configuration);

        Assert.Equal(DatabaseProvider.MySql, options.Provider);
        Assert.Equal("MySql", options.ConnectionStringName);
        Assert.Equal(DatabaseMigrationStrategy.Validate, options.MigrationStrategy);
        Assert.Equal(MySqlServerKind.MariaDb, options.MySqlServerKind);
        Assert.Equal(new Version(10, 11, 6), options.MySqlServerVersion);
        Assert.Contains("Database=edusphere", options.ConnectionString);
    }

    [Theory]
    [InlineData("SqlServer", "Server=.;Database=EduSphere;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True")]
    [InlineData("PostgreSql", "Host=localhost;Database=edusphere;Username=postgres;Password=postgres")]
    [InlineData("MySql", "Server=localhost;Database=edusphere;User=root;Password=password;")]
    [InlineData("Sqlite", "Data Source=edusphere.db")]
    public void ConfigureProvider_SupportsRegisteredProviders(string providerName, string connectionString)
    {
        var builder = new DbContextOptionsBuilder<TenantDbContext>();

        DependencyInjectionExtensions.ConfigureProvider(
            builder,
            DatabaseProviderOptions.Create(connectionString, providerName));

        Assert.NotEmpty(builder.Options.Extensions);
    }
}
