# EduSphere Release Checklist

## Build and Evidence

- [ ] Release revision identified
- [ ] Clean Release build
- [ ] Unit and integration tests passed
- [ ] Permission-matrix tests passed
- [ ] Migration script reviewed
- [ ] Release notes prepared

## Database and Storage

- [ ] Backup completed and readable
- [ ] Current migration recorded
- [ ] Promotion SQL completed without errors
- [ ] New schema and data backfills verified
- [ ] Persistent uploads retained and writable
- [ ] Data-protection keys retained

## Configuration

- [ ] Correct environment name
- [ ] Correct database/provider settings
- [ ] JWT secret supplied securely
- [ ] SMTP/provider secrets supplied securely
- [ ] Public base URL correct
- [ ] Demo seeding disabled for production
- [ ] Logging and retention configured

## Functional Smoke Test

- [ ] SuperAdmin and tenant switching
- [ ] TenantAdmin and BranchAdmin scope
- [ ] Teacher and Student dashboards
- [ ] Admissions and fee gate
- [ ] Attendance and timetable
- [ ] Exam publication and report card
- [ ] Enterprise module pages
- [ ] Email delivery and retry log
- [ ] Document upload/download
- [ ] API authentication and tenant isolation

## Sign-Off

- [ ] Product owner
- [ ] Technical owner
- [ ] Database owner
- [ ] UAT owner
- [ ] Rollback decision deadline recorded
