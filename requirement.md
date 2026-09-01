# requirement.md

## Project Title
EduSphere - Multi-Tenant Educational Institution Management Platform

## Objective
Build a production-grade, multi-tenant, web-based enterprise application for Schools, Colleges, Universities, and Coaching Institutes.

The system must:
- Use ASP.NET Core MVC/Web API as backend.
- Use Razor Pages as frontend.
- Be database agnostic.
- Support SaaS-style tenancy.
- Be modular, secure, scalable, and enterprise-ready.
- Support AI-based question paper generation with separate solution output.

---

## AI Agent Implementation Goal
Implement the complete software system from requirements to deployable source code.

The AI agent should use this file as the authoritative implementation brief and should:
1. Design the architecture.
2. Create the solution structure.
3. Define domain models.
4. Build the backend APIs.
5. Build the Razor Pages frontend.
6. Implement multi-tenancy.
7. Implement authentication and authorization.
8. Implement all required modules.
9. Integrate external AI service for question paper generation.
10. Ensure database-agnostic persistence.
11. Add tests, documentation, and deployment artifacts.

---

## Delivery Expectations
The implementation should produce:
- A complete ASP.NET Core solution.
- Clean, maintainable, production-oriented code.
- Separate projects/layers for Domain, Application, Infrastructure, Web/API, and Tests.
- Migrations and provider configuration for multiple databases.
- Configurable tenant resolution.
- Enterprise-grade security, auditability, and observability.
- Seed data and setup instructions.

---

## Mandatory Technology Stack
- .NET / ASP.NET Core
- ASP.NET Core MVC Controllers and/or Web APIs for backend endpoints
- Razor Pages for frontend UI
- Entity Framework Core for ORM and persistence abstraction
- ASP.NET Core Identity for user management
- JWT and cookie/session auth as needed
- FluentValidation for request validation
- Serilog for logging
- Redis for distributed caching/session where applicable
- Hangfire or Quartz.NET for background jobs
- Polly for resilient external HTTP integrations
- Swagger / OpenAPI for API documentation
- Bootstrap for UI styling
- Optional HTMX or lightweight JS for dynamic partial interactions

---

## Architectural Constraints
The AI agent must follow these constraints:

### 1. Architecture Style
Use Clean Architecture or a similarly strict layered architecture with clear separation of concerns.

Minimum layers/projects:
- EduSphere.Domain
- EduSphere.Application
- EduSphere.Infrastructure
- EduSphere.Web
- EduSphere.Tests.Unit
- EduSphere.Tests.Integration

### 2. Backend Pattern
- Expose business functionality through versioned APIs.
- Use service/application layer abstractions.
- Avoid business logic inside controllers or Razor Page models.
- Use DTOs for API contracts.
- Use dependency injection consistently.

### 3. Database Agnostic Rule
Persistence must remain database agnostic.

Requirements:
- Support SQL Server, PostgreSQL, MySQL, and SQLite at minimum.
- Avoid embedding provider-specific SQL in core business logic.
- Use EF Core abstractions and provider-specific configuration only in Infrastructure.
- Keep model configuration portable.
- Use migrations in a maintainable way for multiple providers.

### 4. Multi-Tenancy Rule
The application must support multi-tenancy from the foundation.

Requirements:
- Every tenant represents one institution.
- A tenant can have multiple branches/campuses.
- Tenant isolation must be enforced across reads and writes.
- Support tenant resolution by subdomain, custom domain, header, and optionally route/path.
- Prefer shared database/shared schema with TenantId filtering, while keeping architecture open for separate-database tenancy later.

### 5. Security Rule
- Enforce authentication and authorization everywhere.
- Use role-based and policy-based authorization.
- Prevent tenant data leakage.
- Implement audit logs for sensitive changes.
- Store secrets outside source code.
- Protect AI integration keys and payment credentials.

---

## Core User Roles
Implement at least these roles:
- SuperAdmin
- TenantAdmin
- Principal / Director
- BranchAdmin
- HOD / DepartmentAdmin
- Teacher
- Student
- Parent
- Accountant
- Librarian
- TransportManager
- HostelManager
- ExamController
- HR / StaffAdmin

Role system requirements:
- Support role assignment per tenant.
- Support permission-based expansion later.
- Support branch-scoped access where applicable.
- Teachers should only access their assigned sections/subjects unless elevated.
- Students should only access their own records.
- Parents should only access linked student records.

---

## Functional Modules To Implement
The AI agent should implement the system module by module.

### Module 1: Tenant Management
Capabilities:
- Create, update, activate, suspend tenants.
- Manage subscription plan metadata.
- Configure branding, logos, theme colors, and tenant settings.
- Support custom domain and subdomain mapping.
- Store tenant-level integration settings.

Entities:
- Tenant
- TenantSettings
- SubscriptionPlan
- TenantDomain
- TenantFeatureFlag

### Module 2: Branch / Campus Management
Capabilities:
- Manage multiple branches under a tenant.
- Store branch code, address, contact info, timezone, academic preferences.
- Enable branch-scoped operations and reports.

Entities:
- Branch
- BranchContact
- BranchAcademicConfig

### Module 3: Academic Structure
Capabilities:
- Manage academic years, terms, semesters, sessions.
- Manage courses/programs.
- Manage batches/intakes.
- Manage classes and sections.
- Manage departments.
- Manage subjects and subject allocation.
- Manage syllabus units/topics and lesson plans.

Entities:
- AcademicYear
- AcademicTerm
- Department
- Course
- Batch
- ClassRoom or ClassGroup
- Section
- Subject
- SubjectAssignment
- Syllabus
- SyllabusUnit
- Topic
- LessonPlan

### Module 4: User and Identity Management
Capabilities:
- Register and invite users.
- Manage profiles for staff, teachers, students, parents.
- Support bulk import.
- Support activation/deactivation.
- Support password reset and MFA readiness.

Entities:
- ApplicationUser
- UserProfile
- TeacherProfile
- StudentProfile
- ParentProfile
- StaffProfile
- UserBranchAssignment
- UserRoleAssignment

### Module 5: Admissions
Capabilities:
- Online admission applications.
- Configurable forms.
- Document uploads.
- Review and approval workflow.
- Enrollment after approval.
- Admission number generation.
- Promotion/re-admission support.

Entities:
- AdmissionApplication
- AdmissionDocument
- AdmissionReview
- Enrollment
- PromotionRecord

### Module 6: Student Information System
Capabilities:
- Maintain complete student lifecycle.
- Track branch, course, batch, section, guardian, identity documents, medical details.
- Maintain academic and administrative status.

Entities:
- StudentProfile
- StudentGuardian
- StudentDocument
- StudentMedicalInfo
- StudentStatusHistory

### Module 7: Teacher and Staff Management
Capabilities:
- Teacher profile management.
- Qualification and experience tracking.
- Subject and class assignment.
- Staff records.
- Leave and attendance readiness.

Entities:
- TeacherProfile
- TeacherQualification
- TeacherSubjectMap
- StaffProfile
- StaffAttendance
- LeaveRequest

### Module 8: Attendance Management
Capabilities:
- Daily attendance.
- Period-wise attendance.
- Subject-wise attendance.
- Late/leave tracking.
- Absence alerts.
- Attendance summaries and percentages.

Entities:
- AttendanceRecord
- AttendanceSession
- AttendancePolicy
- LeaveApplication
- AttendanceAlert

### Module 9: Timetable and Scheduling
Capabilities:
- Build class timetables.
- Assign teachers, rooms, time slots.
- Prevent scheduling conflicts.
- Support exam schedule creation.

Entities:
- TimeSlot
- Timetable
- TimetableEntry
- Room
- ExamSchedule

### Module 10: Examination Management
Capabilities:
- Define exam types like mid-term, final, unit test, practical, viva.
- Create exam schedules.
- Configure grading schemes.
- Mark entry workflows.
- Publish results.
- Generate report cards and transcripts.

Entities:
- Exam
- ExamType
- ExamSchedule
- GradingScheme
- MarkEntry
- Result
- ReportCard
- Transcript

### Module 11: Question Bank and Question Paper Management
Capabilities:
- Maintain question bank by subject, topic, difficulty, Bloom's level, marks, question type.
- Build papers manually.
- Version papers.
- Support approval workflow.
- Store student version separately from solution version.

Entities:
- QuestionBankItem
- QuestionPaper
- QuestionPaperSection
- QuestionPaperVersion
- MarkingScheme
- ApprovalWorkflow

### Module 12: AI Question Paper Generation
Capabilities:
- Integrate with external AI service through configurable provider.
- Generate question paper based on subject, syllabus, exam type, total marks, difficulty mix, Bloom's taxonomy, question pattern, and instructions.
- Generate solutions separately.
- Allow teacher review/edit/approve before publication.
- Track generation logs, token usage, and cost.
- Avoid sending unnecessary personally identifiable information.

Entities:
- AIGenerationRequest
- AIGenerationResponse
- AIGenerationLog
- PromptTemplate
- AIProviderSetting
- AIUsageRecord

Implementation requirements:
- Use provider abstraction.
- Store prompts/templates in configurable form.
- Validate AI output structure.
- Fallback to manual workflow if AI fails.
- Support regeneration and version comparison.

### Module 13: Results and Analytics
Capabilities:
- Compute results by configurable grading rules.
- Show student performance over time.
- Show attendance vs performance reports.
- Show branch/course/class analytics.
- Provide downloadable reports.

Entities:
- ResultSummary
- PerformanceMetric
- DashboardWidgetConfig
- ReportDefinition

### Module 14: Fees and Finance
Capabilities:
- Configure fee structures by course/batch/branch.
- Apply scholarships, discounts, waivers.
- Record payments.
- Generate receipts.
- Track outstanding balances.
- Produce finance reports.

Entities:
- FeeStructure
- FeeComponent
- Invoice
- Payment
- Receipt
- Scholarship
- DiscountRule

### Module 15: Transport Management
Capabilities:
- Manage vehicles, routes, stops, drivers, assignments.
- Assign students to routes/stops.
- Track fees and transport records.

Entities:
- Vehicle
- Route
- Stop
- RouteAssignment
- Driver
- VehicleMaintenance

### Module 16: Library Management
Capabilities:
- Manage books and catalog.
- Issue and return.
- Fine calculation.
- Search catalog.

Entities:
- Book
- BookCopy
- BookIssue
- BookReturn
- FineRecord

### Module 17: Hostel Management
Capabilities:
- Manage hostel buildings, rooms, beds, allocations.
- Track hostel occupancy and fees.

Entities:
- Hostel
- HostelRoom
- Bed
- HostelAllocation
- HostelFeeRecord

### Module 18: Communication and Notifications
Capabilities:
- Announcements.
- Email notifications.
- SMS notifications.
- In-app notifications.
- Template-based communications.

Entities:
- Notification
- NotificationTemplate
- Announcement
- CommunicationLog

### Module 19: Reports and Dashboards
Capabilities:
- Role-based dashboards.
- Admission dashboards.
- Attendance dashboards.
- Fee dashboards.
- Exam/result dashboards.
- Export to PDF/Excel where appropriate.

### Module 20: Audit, Settings, and Administration
Capabilities:
- Audit trails for important actions.
- Global settings and tenant settings.
- Feature toggles.
- Lookup/master data management.
- Background job monitoring.

Entities:
- AuditLog
- AppSetting
- TenantSetting
- FeatureToggle
- LookupItem
- BackgroundJobLog

---

## Cross-Cutting Functional Requirements
The AI agent must ensure the following behaviors across modules:
- All tenant-owned entities must carry TenantId.
- Branch-relevant entities should carry BranchId where needed.
- Soft delete should be supported where business-safe.
- CreatedBy, CreatedOn, ModifiedBy, ModifiedOn audit fields should be standardized.
- Concurrency handling should be included for critical updates.
- Search, filtering, sorting, and paging should be available in major list pages and APIs.
- Bulk import/export should be available for major masters and operational records.
- Reports should be printable and downloadable.

---

## Non-Functional Requirements

### Performance
- Standard CRUD APIs should respond quickly under normal load.
- The system should handle multiple tenants concurrently.
- Large lists should use server-side paging.
- Expensive tasks should move to background jobs.

### Scalability
- Keep the web/API layer stateless as much as possible.
- Design for horizontal scaling.
- Use distributed cache where required.

### Reliability
- External integrations must use retries, timeouts, and circuit breakers.
- Background jobs should be retryable and observable.
- Critical workflows should log failures clearly.

### Security
- Use HTTPS everywhere.
- Hash passwords through ASP.NET Core Identity defaults.
- Protect against CSRF, XSS, SQL injection, IDOR, and broken access control.
- Enforce tenant-scoped authorization and query filtering.
- Implement audit logs for security-sensitive operations.

### Maintainability
- Write clean, modular code.
- Keep business rules testable.
- Use interfaces and abstractions wisely without overengineering.
- Keep module boundaries explicit.

### Observability
- Log structured events.
- Add correlation IDs.
- Record AI service latency, token usage, failures, and costs.
- Record authentication and authorization failures.

### Usability
- Use practical enterprise UI patterns.
- Support responsive layouts for desktop and tablet at minimum.
- Keep workflows optimized for admin users entering high volumes of data.

### Localization
- Design UI and data formatting to support future localization.
- Avoid hardcoding culture-specific formats.

---

## Required Solution Structure
The AI agent should create a solution similar to the following:

- EduSphere.sln
- src/
  - EduSphere.Domain/
  - EduSphere.Application/
  - EduSphere.Infrastructure/
  - EduSphere.Web/
- tests/
  - EduSphere.Tests.Unit/
  - EduSphere.Tests.Integration/
- docs/
- deploy/
- scripts/

Expected project responsibilities:
- Domain: entities, enums, value objects, domain rules, interfaces where appropriate.
- Application: use cases, commands, queries, DTOs, validators, mapping, service contracts.
- Infrastructure: EF Core, Identity, repository implementations, tenant resolution, integrations, storage, caching, logging wiring.
- Web: Razor Pages UI, API controllers, page models, filters, middleware, DI composition root.
- Tests: unit and integration test coverage.

---

## Implementation Blueprint
The AI agent should implement in this order:

### Phase 1: Foundation
- Create solution and projects.
- Configure DI, logging, configuration, exception handling.
- Implement Identity.
- Implement tenant resolution middleware.
- Implement base entities and EF Core DbContext.
- Implement audit fields and TenantId enforcement.
- Add provider configuration for SQL Server, PostgreSQL, MySQL, SQLite.
- Add database migration approach.

### Phase 2: Core Masters
- Tenant and branch management.
- Academic year, departments, courses, batches, classes, sections, subjects.
- User onboarding and profile management.

### Phase 3: Student and Teacher Operations
- Admissions.
- Student lifecycle.
- Teacher assignment.
- Timetable.
- Attendance.

### Phase 4: Exams and Results
- Exam setup.
- Schedule creation.
- Marks entry.
- Result computation.
- Report card generation.
- Question bank and manual question paper workflows.

### Phase 5: AI Integration
- Provider abstraction for AI generation.
- Prompt templates.
- Question paper generation endpoint.
- Response validation.
- Review, edit, approve UI.
- Solution separation.
- Usage and cost tracking.

### Phase 6: Enterprise Modules
- Fees and payments.
- Transport.
- Library.
- Hostel.
- Notifications.
- Dashboards and reporting.

### Phase 7: Hardening
- Authorization refinement.
- Audit logging.
- Caching.
- Performance tuning.
- Integration tests.
- Seed data.
- Deployment scripts.
- Documentation.

---

## API Requirements
The AI agent should expose versioned APIs for all important modules.

Minimum API expectations:
- CRUD APIs for master entities.
- Search/filter/list endpoints.
- Bulk import endpoints where relevant.
- Attendance mark/report endpoints.
- Admission workflow endpoints.
- Exam scheduling and marks endpoints.
- Result publication endpoints.
- AI generation endpoints.
- Reporting/download endpoints.

API design rules:
- Use `/api/v1/...` route versioning.
- Return consistent response envelopes or clear REST conventions.
- Validate all input.
- Return meaningful error payloads.
- Protect endpoints with authorization policies.

---

## Razor Pages Requirements
The AI agent must implement frontend workflows using Razor Pages.

UI expectations:
- Clean admin dashboard.
- Tenant-aware branding.
- Reusable layouts, partials, view components, and tag helpers where useful.
- List pages with paging/filtering.
- Create/edit/details/delete workflows.
- Wizard-like flow for AI question paper generation.
- Preview student paper and solution separately.
- Print-friendly reports.

Major page areas:
- Account
- Dashboard
- Tenant Management
- Branch Management
- Academic Management
- Students
- Teachers
- Admissions
- Attendance
- Exams
- Question Papers
- Results
- Fees
- Transport
- Library
- Hostel
- Notifications
- Reports
- Settings
- Integrations

---

## AI Integration Requirements
The AI agent must build AI generation as a configurable subsystem, not hardcoded to one vendor.

Implementation expectations:
- Create an interface such as `IAIQuestionPaperService`.
- Create provider adapters for at least one initial provider and leave extension points for others.
- Support prompt template storage and rendering.
- Support structured response validation.
- Support generation of:
  - Student-facing question paper
  - Separate solution/marking scheme
- Support manual editing after generation.
- Log token usage and cost.
- Handle provider downtime gracefully.
- Do not store raw secrets in the database without encryption strategy.

Prompt input dimensions should include:
- Institution type
- Course/class level
- Subject
- Syllabus units/topics
- Exam type
- Duration
- Total marks
- Question types and counts
- Difficulty mix
- Bloom's taxonomy distribution
- Special instructions
- Language

---

## Database Design Rules
The AI agent should apply these design rules:
- All primary keys should be consistent, preferably Guid/UUID where useful for distributed systems.
- Use normalized schema for operational data.
- Use lookup tables/enums carefully.
- Avoid tightly coupling schema to a single board/university unless configurable.
- Include indexes for tenant, branch, academic year, and common search fields.
- Include unique constraints where business identity requires it.
- Include optimistic concurrency for critical records.

---

## Testing Requirements
The AI agent must include tests.

Minimum test coverage areas:
- Tenant resolution behavior.
- Tenant isolation in queries.
- Role authorization checks.
- Admissions workflow.
- Attendance calculations.
- Result calculation rules.
- AI response parsing and validation.
- API integration tests for core modules.

Test types:
- Unit tests for domain/application logic.
- Integration tests for EF Core persistence and APIs.
- Consider provider matrix testing for database portability.

---

## DevOps and Deployment Requirements
The AI agent should produce deployment-ready artifacts.

Minimum deliverables:
- appsettings templates
- environment-specific configuration pattern
- Dockerfiles
- docker-compose for local development
- database migration scripts or automated migration approach
- README with setup steps
- deployment notes for Windows/Linux hosting

Optional but recommended:
- Kubernetes manifests or Helm chart
- CI pipeline definition
- health checks and readiness probes

---

## Coding Standards For The AI Agent
The AI agent should follow these coding principles:
- Prefer readability over cleverness.
- Keep methods small and cohesive.
- Avoid unnecessary abstractions.
- Use async I/O properly.
- Validate all external input.
- Keep null handling explicit.
- Centralize common infrastructure concerns.
- Avoid duplicate business logic between API and Razor Pages.

---

## Acceptance Criteria
The implementation is acceptable only if:
- The application is multi-tenant and tenant-safe.
- The solution builds and runs.
- The backend and frontend both exist and are wired together.
- Core academic workflows are usable end-to-end.
- Exams and results work end-to-end.
- AI question paper generation works through a configurable provider.
- Student paper and solution outputs are stored separately.
- The persistence layer can be switched across supported databases with limited infrastructure-only changes.
- Security basics, logging, validation, and tests are present.

---

## Final Instruction To AI Agent
Use this file as the implementation contract.

When generating code:
- Start with the solution skeleton.
- Implement foundation and cross-cutting concerns first.
- Then implement modules incrementally.
- Keep all business behavior tenant-aware.
- Keep the codebase extensible for future ERP modules.
- Do not simplify away the multi-tenant or database-agnostic requirements.
- Treat AI question paper generation as a first-class subsystem, not a demo feature.
