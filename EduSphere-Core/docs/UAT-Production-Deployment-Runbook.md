# EduSphere UAT and Production Deployment Runbook

## Purpose

Use this runbook for every UAT or production release. Never deploy directly from a developer output folder and never run a migration without a verified backup and rollback decision.

## Release Inputs

- Tagged or otherwise immutable source revision.
- Clean `Release` publish output from `EduSphere.Web`.
- Idempotent SQL scripts from `DatabaseScripts`, in migration order.
- Environment-owned settings and secrets. Do not copy development secrets into the publish package.
- Release notes listing migrations, configuration changes, known limitations, and smoke-test owners.

## Pre-Deployment Checklist

- [ ] Confirm the target environment and maintenance window.
- [ ] Confirm the release revision and migration range.
- [ ] Build and test: `dotnet build EduSphere.sln -c Release` and `dotnet test EduSphere.sln -c Release --no-build`.
- [ ] Generate an idempotent migration script from the target database's current migration to the release migration.
- [ ] Review SQL for destructive operations, table scans, non-null columns, backfills, and long-running indexes.
- [ ] Verify the latest full database backup and test that it is readable.
- [ ] Record current application version and `__EFMigrationsHistory` rows.
- [ ] Verify available disk space for the application, database growth, logs, and uploaded documents.
- [ ] Preserve persistent upload storage outside the replaceable publish directory and verify write permissions.
- [ ] Verify SMTP/provider credentials, sender domains, payment callbacks, public admission URL, and tenant host mappings.
- [ ] Confirm `SeedData` and demo-data switches are disabled outside approved non-production environments.

## Required Environment Configuration

- `ConnectionStrings:DefaultConnection`: target SQL Server connection.
- `Database:Provider`: `SqlServer` for the current hosted environments.
- `Database:MigrationStrategy`: use `None` for controlled UAT/production promotion; run reviewed scripts separately.
- `ASPNETCORE_ENVIRONMENT`: `UAT` or `Production`.
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`: environment-specific values; keep the key in the host secret store.
- SMTP settings: host, port, TLS, username, password, and sender identity in the environment secret store.
- Upload/document root: persistent location retained across deployments.
- Public application base URL: must match activation, reset-password, admission, and notification links.

## Database Promotion

1. Put the web application into maintenance mode or stop its application pool.
2. Capture a fresh database backup and record its location and timestamp.
3. Check `SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId`.
4. Run the reviewed idempotent scripts in ascending migration order.
5. Confirm each expected migration appears exactly once in `__EFMigrationsHistory`.
6. Confirm new tables, columns, foreign keys, indexes, and required data backfills.
7. Keep the application stopped if any SQL error occurs. Do not mark a partially executed script as complete manually.

## Application Deployment

1. Publish with `dotnet publish EduSphere.Web/EduSphere.Web.csproj -c Release -o <staging-folder>`.
2. Stop the application pool/service before replacing application binaries.
3. Preserve environment settings, secret-store references, logs, data-protection keys, and persistent uploads.
4. Replace the deployed application with the complete publish output.
5. Restore environment-owned configuration without placing secrets in source-controlled files.
6. Start the application and watch startup logs for migration, configuration, key-ring, SMTP, and database errors.

## Smoke Tests

- [ ] Anonymous public admission portal loads and does not expose administrative navigation.
- [ ] SuperAdmin login and tenant switch work.
- [ ] TenantAdmin sees tenant-scoped data and can manage branch administrators but cannot create TenantAdmins.
- [ ] BranchAdmin sees only the assigned branch.
- [ ] Teacher and Student dashboards and menus are role-specific.
- [ ] Create and review a test admission; verify fee-pending enforcement before enrollment.
- [ ] Enter marks, compute a result, request/approve publication, and open the printable report card.
- [ ] Open finance, transport, library, hostel, and communication dashboards.
- [ ] Send a controlled SMTP test and verify delivery logging.
- [ ] Upload and download a controlled test document from persistent storage.
- [ ] Call one authenticated API endpoint and confirm tenant isolation.
- [ ] Review application logs for unhandled exceptions and repeated provider failures.

## Rollback

Application-only rollback is allowed when the previous binaries are compatible with the promoted schema. Otherwise:

1. Stop the application.
2. Preserve failure logs and the failed database state for diagnosis.
3. Restore the pre-deployment database backup rather than improvising reverse SQL for a partially completed release.
4. Restore the previous complete publish package and environment configuration.
5. Start the previous version and repeat its smoke tests.
6. Record the incident, failed step, error text, and recovery timestamps.

## Post-Deployment

- [ ] Remove maintenance mode.
- [ ] Monitor errors, response times, failed notifications, login failures, and database resource use.
- [ ] Confirm scheduled jobs and retry queues are processing.
- [ ] Record deployed revision, migration IDs, operator, completion time, and smoke-test evidence.
- [ ] Retain the release package, reviewed SQL, and backup according to the environment retention policy.
