# User, Parent And Staff Management

## Parent Portal

Parents open **My Children** after signing in. `StudentGuardians` is the authoritative parent-to-student link and supports multiple students across branches. Only links with portal access enabled are shown. The portal exposes recent attendance, invoices and balances, published results, and notifications addressed to the parent user.

Administrators add links from **People > Staff & Family Admin**. A matching non-primary `UserBranchAssignment` is created when a parent is linked to a student in another branch.

## Staff And Teacher Records

**Staff & Family Admin** provides:

- complete non-teaching staff profiles and payroll settings;
- structured teacher qualifications and employment history;
- daily staff attendance;
- staff leave submission and approval/rejection;
- monthly payroll calculation and CSV export for an external payroll/accounting system.

## MFA And Account Security

Signed-in users open **MFA** in the account bar to enroll a standard TOTP authenticator. Enrollment verifies the first code and generates ten one-time recovery codes. Password login redirects MFA-enabled users to the authenticator challenge.

Administrators open **People > Account Security** to review lockouts, security events, account-email jobs, provider bounces and import batches. Locked accounts can be unlocked and failed/bounced messages can be queued again.

## Background Account Email

Activation and password-reset messages are persisted in `AccountEmailJobs`. `AccountEmailBackgroundWorker` processes queued jobs every ten seconds and retries failures with exponential delays, up to five attempts. The SMTP test action remains synchronous so production operators receive an immediate verification result.

Production deployments must keep at least one application instance running for the hosted worker. For multi-instance deployments, database row claiming should be upgraded to provider-specific atomic locking before enabling multiple concurrent workers.

## Bounce Handling And SMTP Verification

Configure `Notifications:BounceWebhookSecret` through environment variables or a secret manager. Email providers can POST bounce events to:

`POST /api/v1/email-events/bounce`

Send the secret in `X-EduSphere-Webhook-Secret`. The payload includes `tenantId`, optional `notificationMessageId`, `recipient`, `bounceType`, optional `providerEventId`, and optional `reason`.

Use **Users & Roles > SMTP delivery test** in each production tenant. A successful provider submission is recorded by EduSphere; final inbox receipt and bounce behavior must still be verified with the real provider and destination mailbox.

## Resumable Imports

Bulk import execution persists a `UserImportBatch` and each `UserImportRow` before creating the user. The security screen can resume unprocessed rows after interruption and download failed rows as CSV. Preview remains non-mutating and should be run before execution.

## Database Promotion

Apply migration `20260928230739_UserParentStaffManagementCompletion` or run `DatabaseScripts/20260928_UserParentStaffManagementCompletion_Promotion.sql`.
