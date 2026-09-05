using Microsoft.Extensions.Configuration;

namespace EduSphere.Infrastructure;

public enum DatabaseProvider
{
    SqlServer,
    PostgreSql,
    MySql,
    Sqlite
}

public enum MySqlServerKind
{
    MySql,
    MariaDb
}

public enum DatabaseMigrationStrategy
{
    None,
    Validate,
    Migrate
}

public sealed record DatabaseProviderOptions
{
    public const string DefaultConnectionName = "DefaultConnection";

    public DatabaseProvider Provider { get; init; } = DatabaseProvider.SqlServer;
    public string ConnectionStringName { get; init; } = DefaultConnectionName;
    public string ConnectionString { get; init; } = string.Empty;
    public DatabaseMigrationStrategy MigrationStrategy { get; init; } = DatabaseMigrationStrategy.None;
    public Version MySqlServerVersion { get; init; } = new(8, 0, 36);
    public MySqlServerKind MySqlServerKind { get; init; } = MySqlServerKind.MySql;

    public static DatabaseProviderOptions FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var databaseSection = configuration.GetSection("Database");
        var providerName = databaseSection["Provider"] ?? nameof(DatabaseProvider.SqlServer);
        var provider = ParseProvider(providerName);
        var connectionStringName = databaseSection["ConnectionStringName"] ?? DefaultConnectionName;
        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found or is empty.");
        }

        return new DatabaseProviderOptions
        {
            Provider = provider,
            ConnectionStringName = connectionStringName,
            ConnectionString = connectionString,
            MigrationStrategy = ParseMigrationStrategy(databaseSection["MigrationStrategy"] ?? "None"),
            MySqlServerVersion = ParseVersion(
                databaseSection["MySql:ServerVersion"] ??
                databaseSection["MySqlServerVersion"] ??
                "8.0.36"),
            MySqlServerKind = ParseMySqlServerKind(
                databaseSection["MySql:ServerType"] ??
                databaseSection["MySqlServerType"],
                IsMariaDbAlias(providerName) ? MySqlServerKind.MariaDb : MySqlServerKind.MySql)
        };
    }

    public static DatabaseProviderOptions Create(
        string connectionString,
        string provider,
        DatabaseMigrationStrategy migrationStrategy = DatabaseMigrationStrategy.None,
        Version? mySqlServerVersion = null,
        MySqlServerKind mySqlServerKind = MySqlServerKind.MySql)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

        return new DatabaseProviderOptions
        {
            Provider = ParseProvider(provider),
            ConnectionString = connectionString,
            MigrationStrategy = migrationStrategy,
            MySqlServerVersion = mySqlServerVersion ?? new Version(8, 0, 36),
            MySqlServerKind = IsMariaDbAlias(provider) ? MySqlServerKind.MariaDb : mySqlServerKind
        };
    }

    public DatabaseProviderOptions WithConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

        return this with { ConnectionString = connectionString };
    }

    public static DatabaseProvider ParseProvider(string provider)
    {
        return provider.Trim().ToLowerInvariant() switch
        {
            "sqlserver" or "mssql" or "sql-server" => DatabaseProvider.SqlServer,
            "postgres" or "postgresql" or "npgsql" or "postgre-sql" => DatabaseProvider.PostgreSql,
            "mysql" or "mariadb" => DatabaseProvider.MySql,
            "sqlite" or "sqlite3" => DatabaseProvider.Sqlite,
            _ => throw new InvalidOperationException(
                $"Unsupported database provider '{provider}'. Supported: SqlServer, PostgreSql, MySql, MariaDb, Sqlite.")
        };
    }

    public static DatabaseMigrationStrategy ParseMigrationStrategy(string strategy)
    {
        return strategy.Trim().ToLowerInvariant() switch
        {
            "none" or "manual" or "off" => DatabaseMigrationStrategy.None,
            "validate" or "check" => DatabaseMigrationStrategy.Validate,
            "migrate" or "apply" or "auto" or "automatic" => DatabaseMigrationStrategy.Migrate,
            _ => throw new InvalidOperationException(
                $"Unsupported migration strategy '{strategy}'. Supported: None, Validate, Migrate.")
        };
    }

    public static MySqlServerKind ParseMySqlServerKind(string? serverType, MySqlServerKind defaultValue = MySqlServerKind.MySql)
    {
        if (string.IsNullOrWhiteSpace(serverType))
            return defaultValue;

        return serverType.Trim().ToLowerInvariant() switch
        {
            "mysql" => MySqlServerKind.MySql,
            "mariadb" or "maria-db" => MySqlServerKind.MariaDb,
            _ => throw new InvalidOperationException(
                $"Unsupported MySQL server type '{serverType}'. Supported: MySql, MariaDb.")
        };
    }

    public static Version ParseVersion(string value)
    {
        if (Version.TryParse(value, out var version))
            return version;

        throw new InvalidOperationException($"Invalid MySQL server version '{value}'.");
    }

    private static bool IsMariaDbAlias(string provider)
        => provider.Trim().Equals("mariadb", StringComparison.OrdinalIgnoreCase);
}
