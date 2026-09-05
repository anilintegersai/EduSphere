# Migration Strategy

Migrations are owned by `EduSphere.Infrastructure` because this project owns persistence provider registration and the `TenantDbContext`.

## Runtime Strategy

Set `Database:MigrationStrategy` to one of:

- `None`: application startup does not inspect or change the database. Use this for production when migrations are applied by release automation.
- `Validate`: startup checks for pending migrations and fails fast if the database is behind.
- `Migrate`: startup calls `Database.MigrateAsync()`. Use for local development or controlled internal deployments.

## Provider Selection

Set `Database:Provider` to `SqlServer`, `PostgreSql`, `MySql`, `MariaDb`, or `Sqlite`.

Set `Database:ConnectionStringName` to the connection string key the provider should use. The default is `DefaultConnection`.

For MySQL-compatible providers, configure:

- `Database:MySql:ServerType`: `MySql` or `MariaDb`
- `Database:MySql:ServerVersion`: for example `8.0.36` or `10.11.6`

## Design-Time Commands

The design-time factory reads:

- `EDUSPHERE_MIGRATION_PROVIDER`
- `EDUSPHERE_MIGRATION_CONNECTION`
- `EDUSPHERE_MIGRATION_MYSQL_SERVER_TYPE`
- `EDUSPHERE_MIGRATION_MYSQL_SERVER_VERSION`

Example:

```powershell
$env:EDUSPHERE_MIGRATION_PROVIDER = "SqlServer"
$env:EDUSPHERE_MIGRATION_CONNECTION = "Server=.;Database=EduSphere;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True"
dotnet ef migrations add InitialCreate --project EduSphere.Infrastructure --startup-project EduSphere.Infrastructure --context TenantDbContext
```

The existing `20260902200639_InitialCreate` migration is the project baseline; later migrations evolve that baseline.
