# Tenant, Settings And Administration

## Access

Open **Administration** from the authenticated navigation. Access is evaluated through permission definitions rather than menu-only role checks.

Default decisions:

- `SuperAdmin`: all global and tenant administration permissions.
- `TenantAdmin`: tenant settings and permission overrides.
- `BranchAdmin`: read-only administration oversight.

Tenant-scoped role and user overrides can grant or deny individual permissions. A user override wins over role decisions; an explicit role deny wins over another role allow.

## Permission Keys

- `administration.view`
- `administration.manage-tenant`
- `administration.manage-global`
- `administration.manage-permissions`
- `tenants.manage`
- `tenant-workspace.manage`
- `branch-workspace.manage`
- `attendance.mark`
- `finance.manage`
- `transport.manage`
- `library.manage`
- `hostel.manage`
- `communications.manage`
- `users.manage`
- `ai-question-papers.manage`

Existing page and API policies now resolve through these permission keys. Navigation uses the same authorization policies, so an explicit deny removes the menu entry and blocks direct requests.

## Feature Keys

These keys control both navigation and direct page/API access:

- `module.ai-question-papers`
- `module.finance`
- `module.transport`
- `module.library`
- `module.hostel`
- `module.communications`

An undefined flag defaults to enabled to preserve existing tenant behavior. Enabled-from and enabled-until windows are evaluated in UTC.

## Secrets And Branding

Integration secrets and settings marked sensitive are encrypted with ASP.NET Core Data Protection. Production instances must share a durable `DataProtection:KeysPath` and protect its access and backup.

Brand assets are restricted to PNG, JPEG, WebP and ICO files up to 2 MB. Assets are stored under `wwwroot/uploads/branding/{tenant-id}`. For multi-instance deployment, map that directory to shared durable storage or replace it with object storage.

## Domain Mapping

A mapped domain receives a verification token. Publish that token using the institution's domain validation process, then mark the mapping verified. Only verified mappings participate in tenant resolution. TLS certificates and reverse-proxy bindings remain deployment responsibilities.

## Audit And Jobs

The DbContext write pipeline records create, update and soft-delete activity for domain entities. Password, secret, protected-value, token and sensitive-setting fields are redacted. The job monitor contract records queueing, attempts, retry scheduling, failures and completion for background processors.

## Database Promotion

Apply migration `20260919101602_TenantSettingsAdministration` or run `DatabaseScripts/20260919_TenantSettingsAdministration_Promotion.sql`. The promotion script also inserts three subscription plans and all permission definitions idempotently. It inserts no environment secrets.
