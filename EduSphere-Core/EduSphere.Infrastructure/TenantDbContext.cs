using System.Reflection;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RoleNames = EduSphere.Domain.Constants.Roles;
using System.Text.Json;
using EduSphere.Domain.Enums;

namespace EduSphere.Infrastructure;

public class TenantDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserContext _currentUserContext;

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ITenantContext tenantContext,
        ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
        _currentUserContext = currentUserContext ?? SystemCurrentUserContext.Instance;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Branch> Branches { get; set; }

    // Academic structure (Module 3)
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SyllabusUnit> SyllabusUnits { get; set; }

    // Students / Teachers (Module 4)
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<StudentGuardian> StudentGuardians { get; set; }
    public DbSet<StudentLifecycleEvent> StudentLifecycleEvents { get; set; }
    public DbSet<StudentLifecycleRequest> StudentLifecycleRequests { get; set; }
    public DbSet<StudentAlumniRecord> StudentAlumniRecords { get; set; }
    public DbSet<TeacherProfile> TeacherProfiles { get; set; }
    public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; }
    public DbSet<TeacherLifecycleEvent> TeacherLifecycleEvents { get; set; }
    public DbSet<TeacherLifecycleRequest> TeacherLifecycleRequests { get; set; }
    public DbSet<ProfileDocument> ProfileDocuments { get; set; }
    public DbSet<ParentProfile> ParentProfiles { get; set; }
    public DbSet<StaffProfile> StaffProfiles { get; set; }
    public DbSet<UserBranchAssignment> UserBranchAssignments { get; set; }
    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
    public DbSet<UserInvitation> UserInvitations { get; set; }

    // Admissions / Attendance / Timetable (Module 5-9)
    public DbSet<AdmissionFormTemplate> AdmissionFormTemplates { get; set; }
    public DbSet<AdmissionFormField> AdmissionFormFields { get; set; }
    public DbSet<AdmissionDocumentRequirement> AdmissionDocumentRequirements { get; set; }
    public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
    public DbSet<AdmissionDocument> AdmissionDocuments { get; set; }
    public DbSet<AdmissionReview> AdmissionReviews { get; set; }
    public DbSet<AdmissionInterview> AdmissionInterviews { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<PromotionRecord> PromotionRecords { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<AttendanceCorrectionRequest> AttendanceCorrectionRequests { get; set; }
    public DbSet<AttendancePolicy> AttendancePolicies { get; set; }
    public DbSet<LeaveApplication> LeaveApplications { get; set; }
    public DbSet<AttendanceAlert> AttendanceAlerts { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Timetable> Timetables { get; set; }
    public DbSet<TimetableEntry> TimetableEntries { get; set; }
    public DbSet<TimetableSubstitution> TimetableSubstitutions { get; set; }

    // Examinations / Results / Question Bank (Module 10-11)
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamSchedule> ExamSchedules { get; set; }
    public DbSet<GradingScheme> GradingSchemes { get; set; }
    public DbSet<GradingSchemeBand> GradingSchemeBands { get; set; }
    public DbSet<QuestionBankItem> QuestionBankItems { get; set; }
    public DbSet<QuestionPaper> QuestionPapers { get; set; }
    public DbSet<QuestionPaperSection> QuestionPaperSections { get; set; }
    public DbSet<QuestionPaperQuestion> QuestionPaperQuestions { get; set; }
    public DbSet<QuestionPaperVersion> QuestionPaperVersions { get; set; }
    public DbSet<QuestionPaperModeration> QuestionPaperModerations { get; set; }
    public DbSet<MarkEntry> MarkEntries { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<ResultPublicationBatch> ResultPublicationBatches { get; set; }
    public DbSet<ResultRankingRule> ResultRankingRules { get; set; }
    public DbSet<AIProviderSetting> AIProviderSettings { get; set; }
    public DbSet<AIPromptTemplate> AIPromptTemplates { get; set; }
    public DbSet<AIGenerationRequest> AIGenerationRequests { get; set; }
    public DbSet<AIGenerationResponse> AIGenerationResponses { get; set; }
    public DbSet<AIGenerationLog> AIGenerationLogs { get; set; }
    public DbSet<AIUsageRecord> AIUsageRecords { get; set; }

    // Enterprise services (Modules 14-18)
    public DbSet<FeeStructure> FeeStructures { get; set; }
    public DbSet<FeeComponent> FeeComponents { get; set; }
    public DbSet<DiscountRule> DiscountRules { get; set; }
    public DbSet<Scholarship> Scholarships { get; set; }
    public DbSet<StudentFeeAssignment> StudentFeeAssignments { get; set; }
    public DbSet<FeeInvoice> FeeInvoices { get; set; }
    public DbSet<FeeInvoiceLine> FeeInvoiceLines { get; set; }
    public DbSet<FeePayment> FeePayments { get; set; }
    public DbSet<FeeReceipt> FeeReceipts { get; set; }
    public DbSet<OnlinePaymentTransaction> OnlinePaymentTransactions { get; set; }
    public DbSet<FeeReminder> FeeReminders { get; set; }
    public DbSet<FeeRefund> FeeRefunds { get; set; }
    public DbSet<FeeConcessionRequest> FeeConcessionRequests { get; set; }
    public DbSet<LedgerExportBatch> LedgerExportBatches { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<TransportDriver> TransportDrivers { get; set; }
    public DbSet<TransportRoute> TransportRoutes { get; set; }
    public DbSet<TransportRouteStop> TransportRouteStops { get; set; }
    public DbSet<TransportRouteAssignment> TransportRouteAssignments { get; set; }
    public DbSet<StudentTransportAssignment> StudentTransportAssignments { get; set; }
    public DbSet<VehicleGpsPing> VehicleGpsPings { get; set; }
    public DbSet<TransportMaintenanceRecord> TransportMaintenanceRecords { get; set; }
    public DbSet<TransportDocumentReminder> TransportDocumentReminders { get; set; }
    public DbSet<LibraryBook> LibraryBooks { get; set; }
    public DbSet<LibraryBookCopy> LibraryBookCopies { get; set; }
    public DbSet<LibraryMember> LibraryMembers { get; set; }
    public DbSet<LibraryBookIssue> LibraryBookIssues { get; set; }
    public DbSet<LibraryBookReturn> LibraryBookReturns { get; set; }
    public DbSet<LibraryFineRecord> LibraryFineRecords { get; set; }
    public DbSet<LibraryReservation> LibraryReservations { get; set; }
    public DbSet<LibraryRenewal> LibraryRenewals { get; set; }
    public DbSet<LibraryOverdueNotification> LibraryOverdueNotifications { get; set; }
    public DbSet<LibraryBarcodeScan> LibraryBarcodeScans { get; set; }
    public DbSet<HostelBlock> HostelBlocks { get; set; }
    public DbSet<HostelRoom> HostelRooms { get; set; }
    public DbSet<HostelBed> HostelBeds { get; set; }
    public DbSet<HostelAllocation> HostelAllocations { get; set; }
    public DbSet<HostelFee> HostelFees { get; set; }
    public DbSet<HostelVisitorLog> HostelVisitorLogs { get; set; }
    public DbSet<HostelMaintenanceRequest> HostelMaintenanceRequests { get; set; }
    public DbSet<HostelAllocationTransferRequest> HostelAllocationTransferRequests { get; set; }
    public DbSet<NotificationProviderSetting> NotificationProviderSettings { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
    public DbSet<NotificationMessage> NotificationMessages { get; set; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<CommunicationLog> CommunicationLogs { get; set; }
    public DbSet<NotificationDeliveryAttempt> NotificationDeliveryAttempts { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<TenantBranding> TenantBrandings { get; set; }
    public DbSet<TenantDomain> TenantDomains { get; set; }
    public DbSet<TenantIntegrationSetting> TenantIntegrationSettings { get; set; }
    public DbSet<TenantFeatureFlag> TenantFeatureFlags { get; set; }
    public DbSet<BranchContact> BranchContacts { get; set; }
    public DbSet<BranchAcademicConfig> BranchAcademicConfigs { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }
    public DbSet<LookupItem> LookupItems { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<BackgroundJobLog> BackgroundJobLogs { get; set; }
    public DbSet<PermissionDefinition> PermissionDefinitions { get; set; }
    public DbSet<RolePermissionGrant> RolePermissionGrants { get; set; }
    public DbSet<UserPermissionGrant> UserPermissionGrants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the ASP.NET Core Identity schema (AspNetUsers, AspNetRoles, ...).
        base.OnModelCreating(modelBuilder);

        // Tenant -> Branch
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Tenant)
            .WithMany(t => t.Branches)
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicationUser -> Branch (optional; a platform/tenant admin may have no branch)
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Branch)
            .WithMany()
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lookup keys and tenant discriminator indexes
        modelBuilder.Entity<Tenant>().HasIndex(t => t.TenantIdentifier).IsUnique();
        modelBuilder.Entity<Tenant>().HasIndex(t => t.CustomDomain);
        modelBuilder.Entity<Branch>().HasIndex(b => b.TenantId);
        modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.TenantId);
        modelBuilder.Entity<ApplicationUser>().HasIndex(u => new { u.TenantId, u.BranchId });
        modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.RequiresActivation);

        ConfigureAcademicStructure(modelBuilder);
        ConfigurePeopleProfiles(modelBuilder);
        ConfigureOperations(modelBuilder);
        ConfigureExaminations(modelBuilder);
        ConfigureEnterpriseServices(modelBuilder);
        ConfigureAdministration(modelBuilder);

        // Global tenant and soft-delete query filters for tenant-owned domain entities.
        // Identity's UserManager/SignInManager must resolve users (including a host-level
        // SuperAdmin) independent of the resolved tenant, so users are scoped at the
        // authorization/application layer rather than by an automatic read filter.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(IAuditableEntity).IsAssignableFrom(clr))
            {
                ConfigureAuditMethod
                    .MakeGenericMethod(clr)
                    .Invoke(this, new object[] { modelBuilder });
            }

            if (typeof(IConcurrencyTrackedEntity).IsAssignableFrom(clr))
            {
                ConfigureConcurrencyMethod
                    .MakeGenericMethod(clr)
                    .Invoke(this, new object[] { modelBuilder });
            }

            if (typeof(ITenantEntity).IsAssignableFrom(clr) &&
                typeof(ISoftDeletable).IsAssignableFrom(clr) &&
                clr != typeof(ApplicationUser))
            {
                ConfigureTenantSoftDeleteFilterMethod
                    .MakeGenericMethod(clr)
                    .Invoke(this, new object[] { modelBuilder });
            }
            else if (typeof(ISoftDeletable).IsAssignableFrom(clr) && clr != typeof(ApplicationUser))
            {
                ConfigureSoftDeleteFilterMethod
                    .MakeGenericMethod(clr)
                    .Invoke(this, new object[] { modelBuilder });
            }
        }

        // Seed platform roles with stable identifiers (deterministic for migrations).
        modelBuilder.Entity<ApplicationRole>().HasData(
            RoleNames.SeedIds.Select(kvp => new ApplicationRole
            {
                Id = kvp.Value,
                Name = kvp.Key,
                NormalizedName = kvp.Key.ToUpperInvariant(),
                ConcurrencyStamp = kvp.Value.ToString(),
                Description = $"{kvp.Key} platform role"
            }).ToArray());
    }

    private static readonly MethodInfo ConfigureTenantSoftDeleteFilterMethod =
        typeof(TenantDbContext).GetMethod(nameof(ConfigureTenantSoftDeleteFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo ConfigureSoftDeleteFilterMethod =
        typeof(TenantDbContext).GetMethod(nameof(ConfigureSoftDeleteFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo ConfigureAuditMethod =
        typeof(TenantDbContext).GetMethod(nameof(ConfigureAudit),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo ConfigureConcurrencyMethod =
        typeof(TenantDbContext).GetMethod(nameof(ConfigureConcurrency),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    // Referencing the injected context instance keeps the value parameterized:
    // EF re-reads _tenantContext.TenantId per query rather than baking it into the model.
    private void ConfigureTenantSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            e.TenantId == _tenantContext.TenantId &&
            !e.IsDeleted);
    }

    private void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    private void ConfigureAudit<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IAuditableEntity
    {
        modelBuilder.Entity<TEntity>().Property(e => e.CreatedBy).HasMaxLength(128);
        modelBuilder.Entity<TEntity>().Property(e => e.ModifiedBy).HasMaxLength(128);
    }

    private void ConfigureConcurrency<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IConcurrencyTrackedEntity
    {
        modelBuilder.Entity<TEntity>().Property(e => e.ConcurrencyToken).IsConcurrencyToken();
    }

    private static void ConfigureAcademicStructure(ModelBuilder modelBuilder)
    {
        // Relationships — all Restrict so soft delete governs lifecycle and
        // SQL Server never sees multiple/cyclic cascade paths.
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Department).WithMany()
            .HasForeignKey(c => c.DepartmentId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Batch>()
            .HasOne(b => b.Course).WithMany()
            .HasForeignKey(b => b.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Batch>()
            .HasOne(b => b.AcademicYear).WithMany()
            .HasForeignKey(b => b.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Section>()
            .HasOne(s => s.Batch).WithMany()
            .HasForeignKey(s => s.BatchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Course).WithMany()
            .HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SyllabusUnit>()
            .HasOne(u => u.Subject).WithMany()
            .HasForeignKey(u => u.SubjectId).OnDelete(DeleteBehavior.Restrict);

        // Business-identity uniqueness (per tenant) and tenant/FK indexes.
        modelBuilder.Entity<AcademicYear>().HasIndex(a => new { a.TenantId, a.Name }).IsUnique();
        modelBuilder.Entity<Department>().HasIndex(d => new { d.TenantId, d.Code }).IsUnique();
        modelBuilder.Entity<Course>().HasIndex(c => new { c.TenantId, c.Code }).IsUnique();
        modelBuilder.Entity<Subject>().HasIndex(s => new { s.TenantId, s.Code }).IsUnique();
        modelBuilder.Entity<Batch>().HasIndex(b => b.TenantId);
        modelBuilder.Entity<Batch>().HasIndex(b => b.CourseId);
        modelBuilder.Entity<Batch>().HasIndex(b => b.AcademicYearId);
        modelBuilder.Entity<Section>().HasIndex(s => s.BatchId);
        modelBuilder.Entity<SyllabusUnit>().HasIndex(u => u.SubjectId);
    }

    private static void ConfigurePeopleProfiles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentProfile>()
            .HasOne(s => s.User).WithMany()
            .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentProfile>()
            .HasOne(s => s.Branch).WithMany()
            .HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentProfile>()
            .HasOne(s => s.Section).WithMany()
            .HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeacherProfile>()
            .HasOne(t => t.User).WithMany()
            .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherProfile>()
            .HasOne(t => t.Branch).WithMany()
            .HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherProfile>()
            .HasOne(t => t.Department).WithMany()
            .HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherProfile>()
            .Property(t => t.ExperienceYears)
            .HasPrecision(18, 2);

        modelBuilder.Entity<StudentGuardian>()
            .HasOne(g => g.StudentProfile).WithMany(s => s.Guardians)
            .HasForeignKey(g => g.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentGuardian>()
            .HasOne(g => g.ParentUser).WithMany()
            .HasForeignKey(g => g.ParentUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeacherSubjectAssignment>()
            .HasOne(a => a.TeacherProfile).WithMany(t => t.SubjectAssignments)
            .HasForeignKey(a => a.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherSubjectAssignment>()
            .HasOne(a => a.Subject).WithMany()
            .HasForeignKey(a => a.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherSubjectAssignment>()
            .HasOne(a => a.Section).WithMany()
            .HasForeignKey(a => a.SectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.StudentProfile).WithMany(s => s.LifecycleEvents)
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.FromBranch).WithMany()
            .HasForeignKey(e => e.FromBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.ToBranch).WithMany()
            .HasForeignKey(e => e.ToBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.FromAcademicYear).WithMany()
            .HasForeignKey(e => e.FromAcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.ToAcademicYear).WithMany()
            .HasForeignKey(e => e.ToAcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.FromCourse).WithMany()
            .HasForeignKey(e => e.FromCourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.ToCourse).WithMany()
            .HasForeignKey(e => e.ToCourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.FromBatch).WithMany()
            .HasForeignKey(e => e.FromBatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.ToBatch).WithMany()
            .HasForeignKey(e => e.ToBatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.FromSection).WithMany()
            .HasForeignKey(e => e.FromSectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.ToSection).WithMany()
            .HasForeignKey(e => e.ToSectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleEvent>()
            .HasOne(e => e.RecordedByUser).WithMany()
            .HasForeignKey(e => e.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.StudentProfile).WithMany(s => s.LifecycleRequests)
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.ToBranch).WithMany()
            .HasForeignKey(e => e.ToBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.ToAcademicYear).WithMany()
            .HasForeignKey(e => e.ToAcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.ToCourse).WithMany()
            .HasForeignKey(e => e.ToCourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.ToBatch).WithMany()
            .HasForeignKey(e => e.ToBatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.ToSection).WithMany()
            .HasForeignKey(e => e.ToSectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.RequestedByUser).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.DecidedByUser).WithMany()
            .HasForeignKey(e => e.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentLifecycleRequest>()
            .HasOne(e => e.AppliedStudentLifecycleEvent).WithMany()
            .HasForeignKey(e => e.AppliedStudentLifecycleEventId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentAlumniRecord>()
            .HasOne(e => e.StudentProfile).WithMany(s => s.AlumniRecords)
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentAlumniRecord>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentAlumniRecord>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentAlumniRecord>()
            .HasOne(e => e.Course).WithMany()
            .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentAlumniRecord>()
            .HasOne(e => e.Batch).WithMany()
            .HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.TeacherProfile).WithMany(t => t.LifecycleEvents)
            .HasForeignKey(e => e.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.FromBranch).WithMany()
            .HasForeignKey(e => e.FromBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.ToBranch).WithMany()
            .HasForeignKey(e => e.ToBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.FromDepartment).WithMany()
            .HasForeignKey(e => e.FromDepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.ToDepartment).WithMany()
            .HasForeignKey(e => e.ToDepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleEvent>()
            .HasOne(e => e.RecordedByUser).WithMany()
            .HasForeignKey(e => e.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.TeacherProfile).WithMany(t => t.LifecycleRequests)
            .HasForeignKey(e => e.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.ToBranch).WithMany()
            .HasForeignKey(e => e.ToBranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.ToDepartment).WithMany()
            .HasForeignKey(e => e.ToDepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.RequestedByUser).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.DecidedByUser).WithMany()
            .HasForeignKey(e => e.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeacherLifecycleRequest>()
            .HasOne(e => e.AppliedTeacherLifecycleEvent).WithMany()
            .HasForeignKey(e => e.AppliedTeacherLifecycleEventId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParentProfile>()
            .HasOne(p => p.User).WithMany()
            .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ParentProfile>()
            .HasOne(p => p.Branch).WithMany()
            .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StaffProfile>()
            .HasOne(s => s.User).WithMany()
            .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StaffProfile>()
            .HasOne(s => s.Branch).WithMany()
            .HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StaffProfile>()
            .HasOne(s => s.Department).WithMany()
            .HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StaffProfile>()
            .Property(s => s.ExperienceYears)
            .HasPrecision(18, 2);

        modelBuilder.Entity<UserBranchAssignment>()
            .HasOne(a => a.User).WithMany()
            .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserBranchAssignment>()
            .HasOne(a => a.Branch).WithMany()
            .HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(a => a.User).WithMany()
            .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(a => a.Role).WithMany()
            .HasForeignKey(a => a.RoleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(a => a.Branch).WithMany()
            .HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(a => a.AssignedByUser).WithMany()
            .HasForeignKey(a => a.AssignedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserInvitation>()
            .HasOne(i => i.User).WithMany()
            .HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserInvitation>()
            .HasOne(i => i.Branch).WithMany()
            .HasForeignKey(i => i.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserInvitation>()
            .HasOne(i => i.InvitedByUser).WithMany()
            .HasForeignKey(i => i.InvitedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentProfile>().HasIndex(s => new { s.TenantId, s.AdmissionNumber }).IsUnique();
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.BranchId);
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.SectionId);
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.UserId);

        modelBuilder.Entity<TeacherProfile>().HasIndex(t => new { t.TenantId, t.EmployeeNumber }).IsUnique();
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.BranchId);
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.DepartmentId);
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.UserId);

        modelBuilder.Entity<StudentGuardian>().HasIndex(g => g.StudentProfileId);
        modelBuilder.Entity<StudentGuardian>().HasIndex(g => g.ParentUserId);

        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.TeacherProfileId);
        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.SubjectId);
        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.SectionId);

        modelBuilder.Entity<StudentLifecycleEvent>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.EffectiveOn });
        modelBuilder.Entity<StudentLifecycleEvent>().HasIndex(e => new { e.TenantId, e.BranchId, e.EventType, e.EffectiveOn });
        modelBuilder.Entity<StudentLifecycleEvent>().HasIndex(e => e.ToSectionId);

        modelBuilder.Entity<StudentLifecycleRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.RequestedOn });
        modelBuilder.Entity<StudentLifecycleRequest>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Status });
        modelBuilder.Entity<StudentLifecycleRequest>().HasIndex(e => e.ToSectionId);
        modelBuilder.Entity<StudentLifecycleRequest>().HasIndex(e => e.AppliedStudentLifecycleEventId);

        modelBuilder.Entity<StudentAlumniRecord>().HasIndex(e => new { e.TenantId, e.AlumniNumber }).IsUnique();
        modelBuilder.Entity<StudentAlumniRecord>().HasIndex(e => new { e.TenantId, e.StudentProfileId }).IsUnique();
        modelBuilder.Entity<StudentAlumniRecord>().HasIndex(e => new { e.TenantId, e.BranchId, e.GraduationDate });

        modelBuilder.Entity<TeacherLifecycleEvent>().HasIndex(e => new { e.TenantId, e.TeacherProfileId, e.EffectiveOn });
        modelBuilder.Entity<TeacherLifecycleEvent>().HasIndex(e => new { e.TenantId, e.BranchId, e.EventType, e.EffectiveOn });
        modelBuilder.Entity<TeacherLifecycleEvent>().HasIndex(e => e.ToDepartmentId);

        modelBuilder.Entity<TeacherLifecycleRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.RequestedOn });
        modelBuilder.Entity<TeacherLifecycleRequest>().HasIndex(e => new { e.TenantId, e.TeacherProfileId, e.Status });
        modelBuilder.Entity<TeacherLifecycleRequest>().HasIndex(e => e.ToDepartmentId);
        modelBuilder.Entity<TeacherLifecycleRequest>().HasIndex(e => e.AppliedTeacherLifecycleEventId);

        modelBuilder.Entity<ProfileDocument>().HasIndex(d => new { d.OwnerType, d.OwnerId });

        modelBuilder.Entity<ParentProfile>().HasIndex(p => new { p.TenantId, p.UserId }).IsUnique();
        modelBuilder.Entity<ParentProfile>().HasIndex(p => new { p.TenantId, p.Email });
        modelBuilder.Entity<ParentProfile>().HasIndex(p => p.BranchId);

        modelBuilder.Entity<StaffProfile>().HasIndex(s => new { s.TenantId, s.UserId }).IsUnique();
        modelBuilder.Entity<StaffProfile>().HasIndex(s => new { s.TenantId, s.EmployeeNumber }).IsUnique();
        modelBuilder.Entity<StaffProfile>().HasIndex(s => s.BranchId);
        modelBuilder.Entity<StaffProfile>().HasIndex(s => s.DepartmentId);

        modelBuilder.Entity<UserBranchAssignment>().HasIndex(a => new { a.TenantId, a.UserId, a.BranchId }).IsUnique();
        modelBuilder.Entity<UserBranchAssignment>().HasIndex(a => new { a.TenantId, a.BranchId, a.IsActive });

        modelBuilder.Entity<UserRoleAssignment>().HasIndex(a => new { a.TenantId, a.UserId, a.RoleName, a.BranchId }).IsUnique();
        modelBuilder.Entity<UserRoleAssignment>().HasIndex(a => new { a.TenantId, a.RoleName, a.IsActive });

        modelBuilder.Entity<UserInvitation>().HasIndex(i => new { i.TenantId, i.Email, i.Status });
        modelBuilder.Entity<UserInvitation>().HasIndex(i => new { i.TenantId, i.UserId, i.Status });
    }

    private static void ConfigureOperations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdmissionFormTemplate>()
            .HasOne(f => f.Branch).WithMany()
            .HasForeignKey(f => f.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionFormTemplate>()
            .HasOne(f => f.AcademicYear).WithMany()
            .HasForeignKey(f => f.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionFormTemplate>()
            .HasOne(f => f.Course).WithMany()
            .HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionFormField>()
            .HasOne(f => f.AdmissionFormTemplate).WithMany(t => t.Fields)
            .HasForeignKey(f => f.AdmissionFormTemplateId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionDocumentRequirement>()
            .HasOne(r => r.AdmissionFormTemplate).WithMany(t => t.DocumentRequirements)
            .HasForeignKey(r => r.AdmissionFormTemplateId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.AdmissionFormTemplate).WithMany()
            .HasForeignKey(a => a.AdmissionFormTemplateId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.Branch).WithMany()
            .HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.AcademicYear).WithMany()
            .HasForeignKey(a => a.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.Course).WithMany()
            .HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.Batch).WithMany()
            .HasForeignKey(a => a.BatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.Section).WithMany()
            .HasForeignKey(a => a.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionApplication>()
            .HasOne(a => a.EnrolledStudentProfile).WithMany()
            .HasForeignKey(a => a.EnrolledStudentProfileId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionDocument>()
            .HasOne(d => d.AdmissionApplication).WithMany(a => a.Documents)
            .HasForeignKey(d => d.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionDocument>()
            .HasOne(d => d.UploadedByUser).WithMany()
            .HasForeignKey(d => d.UploadedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionDocument>()
            .HasOne(d => d.VerifiedByUser).WithMany()
            .HasForeignKey(d => d.VerifiedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionReview>()
            .HasOne(r => r.AdmissionApplication).WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionReview>()
            .HasOne(r => r.ReviewedByUser).WithMany()
            .HasForeignKey(r => r.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionInterview>()
            .HasOne(i => i.AdmissionApplication).WithMany(a => a.Interviews)
            .HasForeignKey(i => i.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionInterview>()
            .HasOne(i => i.Branch).WithMany()
            .HasForeignKey(i => i.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionInterview>()
            .HasOne(i => i.InterviewerUser).WithMany()
            .HasForeignKey(i => i.InterviewerUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.AdmissionApplication).WithMany()
            .HasForeignKey(e => e.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course).WithMany()
            .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Batch).WithMany()
            .HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Section).WithMany()
            .HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PromotionRecord>()
            .HasOne(p => p.StudentProfile).WithMany()
            .HasForeignKey(p => p.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionRecord>()
            .HasOne(p => p.FromAcademicYear).WithMany()
            .HasForeignKey(p => p.FromAcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionRecord>()
            .HasOne(p => p.ToAcademicYear).WithMany()
            .HasForeignKey(p => p.ToAcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionRecord>()
            .HasOne(p => p.FromSection).WithMany()
            .HasForeignKey(p => p.FromSectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionRecord>()
            .HasOne(p => p.ToSection).WithMany()
            .HasForeignKey(p => p.ToSectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.Branch).WithMany()
            .HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.Section).WithMany()
            .HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.Subject).WithMany()
            .HasForeignKey(s => s.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.TimetableEntry).WithMany()
            .HasForeignKey(s => s.TimetableEntryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.MarkedByUser).WithMany()
            .HasForeignKey(s => s.MarkedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(r => r.AttendanceSession).WithMany(s => s.Records)
            .HasForeignKey(r => r.AttendanceSessionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(r => r.StudentProfile).WithMany()
            .HasForeignKey(r => r.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(r => r.MarkedByUser).WithMany()
            .HasForeignKey(r => r.MarkedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceCorrectionRequest>()
            .HasOne(c => c.AttendanceRecord).WithMany()
            .HasForeignKey(c => c.AttendanceRecordId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceCorrectionRequest>()
            .HasOne(c => c.AttendanceSession).WithMany()
            .HasForeignKey(c => c.AttendanceSessionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceCorrectionRequest>()
            .HasOne(c => c.StudentProfile).WithMany()
            .HasForeignKey(c => c.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceCorrectionRequest>()
            .HasOne(c => c.RequestedByUser).WithMany()
            .HasForeignKey(c => c.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceCorrectionRequest>()
            .HasOne(c => c.ReviewedByUser).WithMany()
            .HasForeignKey(c => c.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendancePolicy>()
            .HasOne(p => p.Branch).WithMany()
            .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendancePolicy>()
            .Property(p => p.MinimumPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<LeaveApplication>()
            .HasOne(l => l.StudentProfile).WithMany()
            .HasForeignKey(l => l.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveApplication>()
            .HasOne(l => l.ReviewedByUser).WithMany()
            .HasForeignKey(l => l.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceAlert>()
            .HasOne(a => a.StudentProfile).WithMany()
            .HasForeignKey(a => a.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceAlert>()
            .HasOne(a => a.AttendancePolicy).WithMany()
            .HasForeignKey(a => a.AttendancePolicyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceAlert>()
            .Property(a => a.AttendancePercentage)
            .HasPrecision(5, 2);
        modelBuilder.Entity<AttendanceAlert>()
            .Property(a => a.ThresholdPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Branch).WithMany()
            .HasForeignKey(r => r.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeSlot>()
            .HasOne(t => t.Branch).WithMany()
            .HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Timetable>()
            .HasOne(t => t.Branch).WithMany()
            .HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Timetable>()
            .HasOne(t => t.AcademicYear).WithMany()
            .HasForeignKey(t => t.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Timetable>()
            .HasOne(t => t.Section).WithMany()
            .HasForeignKey(t => t.SectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Timetable).WithMany(t => t.Entries)
            .HasForeignKey(e => e.TimetableId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Section).WithMany()
            .HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Subject).WithMany()
            .HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.TeacherProfile).WithMany()
            .HasForeignKey(e => e.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.TimeSlot).WithMany()
            .HasForeignKey(e => e.TimeSlotId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Room).WithMany()
            .HasForeignKey(e => e.RoomId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.Branch).WithMany()
            .HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.TimetableEntry).WithMany()
            .HasForeignKey(s => s.TimetableEntryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.OriginalTeacherProfile).WithMany()
            .HasForeignKey(s => s.OriginalTeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.SubstituteTeacherProfile).WithMany()
            .HasForeignKey(s => s.SubstituteTeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.RequestedByUser).WithMany()
            .HasForeignKey(s => s.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.ReviewedByUser).WithMany()
            .HasForeignKey(s => s.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionFormTemplate>().HasIndex(f => new { f.TenantId, f.Name }).IsUnique();
        modelBuilder.Entity<AdmissionFormTemplate>().HasIndex(f => new { f.TenantId, f.BranchId, f.CourseId, f.AcademicYearId, f.IsActive });
        modelBuilder.Entity<AdmissionFormField>().HasIndex(f => new { f.AdmissionFormTemplateId, f.FieldKey }).IsUnique();
        modelBuilder.Entity<AdmissionFormField>().HasIndex(f => new { f.AdmissionFormTemplateId, f.SortOrder });
        modelBuilder.Entity<AdmissionDocumentRequirement>().HasIndex(r => new { r.AdmissionFormTemplateId, r.DocumentType }).IsUnique();
        modelBuilder.Entity<AdmissionDocumentRequirement>().HasIndex(r => new { r.AdmissionFormTemplateId, r.SortOrder });

        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => new { a.TenantId, a.ApplicationNumber }).IsUnique();
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.AdmissionFormTemplateId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.BranchId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.CourseId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.BatchId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.SectionId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.Status);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.EnrolledStudentProfileId);
        modelBuilder.Entity<AdmissionDocument>().HasIndex(d => d.AdmissionApplicationId);
        modelBuilder.Entity<AdmissionDocument>().HasIndex(d => d.UploadedByUserId);
        modelBuilder.Entity<AdmissionReview>().HasIndex(r => r.AdmissionApplicationId);
        modelBuilder.Entity<AdmissionInterview>().HasIndex(i => i.AdmissionApplicationId);
        modelBuilder.Entity<AdmissionInterview>().HasIndex(i => new { i.TenantId, i.BranchId, i.StartsOn, i.Status });
        modelBuilder.Entity<AdmissionInterview>().HasIndex(i => new { i.TenantId, i.InterviewerUserId, i.StartsOn, i.EndsOn });
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.TenantId, e.EnrollmentNumber }).IsUnique();
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.AcademicYearId }).IsUnique();
        modelBuilder.Entity<PromotionRecord>().HasIndex(p => p.StudentProfileId);

        modelBuilder.Entity<AttendanceSession>().HasIndex(s => new { s.TenantId, s.SectionId, s.AttendanceDate });
        modelBuilder.Entity<AttendanceSession>().HasIndex(s => s.SubjectId);
        modelBuilder.Entity<AttendanceRecord>().HasIndex(r => new { r.TenantId, r.AttendanceSessionId, r.StudentProfileId }).IsUnique();
        modelBuilder.Entity<AttendanceCorrectionRequest>().HasIndex(c => new { c.TenantId, c.AttendanceSessionId, c.Status });
        modelBuilder.Entity<AttendanceCorrectionRequest>().HasIndex(c => new { c.TenantId, c.AttendanceRecordId, c.Status });
        modelBuilder.Entity<AttendanceCorrectionRequest>().HasIndex(c => c.StudentProfileId);
        modelBuilder.Entity<AttendancePolicy>().HasIndex(p => new { p.TenantId, p.Name }).IsUnique();
        modelBuilder.Entity<LeaveApplication>().HasIndex(l => new { l.TenantId, l.StudentProfileId, l.FromDate });
        modelBuilder.Entity<AttendanceAlert>().HasIndex(a => new { a.TenantId, a.StudentProfileId, a.Status });

        modelBuilder.Entity<Room>().HasIndex(r => new { r.TenantId, r.BranchId, r.Code }).IsUnique();
        modelBuilder.Entity<TimeSlot>().HasIndex(t => new { t.TenantId, t.BranchId, t.DayOfWeek, t.PeriodNumber }).IsUnique();
        modelBuilder.Entity<Timetable>().HasIndex(t => new { t.TenantId, t.SectionId, t.EffectiveFrom });
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.SectionId, e.TimeSlotId }).IsUnique();
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.TeacherProfileId, e.TimeSlotId }).IsUnique();
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.RoomId, e.TimeSlotId });
        modelBuilder.Entity<TimetableSubstitution>().HasIndex(s => new { s.TenantId, s.BranchId, s.SubstitutionDate, s.Status });
        modelBuilder.Entity<TimetableSubstitution>().HasIndex(s => new { s.TenantId, s.TimetableEntryId, s.SubstitutionDate });
        modelBuilder.Entity<TimetableSubstitution>().HasIndex(s => s.SubstituteTeacherProfileId);
    }

    private static void ConfigureExaminations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Exam>()
            .Property(e => e.WeightagePercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.Exam).WithMany(e => e.Schedules)
            .HasForeignKey(s => s.ExamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.Section).WithMany()
            .HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.Subject).WithMany()
            .HasForeignKey(s => s.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.TeacherProfile).WithMany()
            .HasForeignKey(s => s.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.Room).WithMany()
            .HasForeignKey(s => s.RoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamSchedule>()
            .Property(s => s.MaximumMarks)
            .HasPrecision(7, 2);
        modelBuilder.Entity<ExamSchedule>()
            .Property(s => s.PassingMarks)
            .HasPrecision(7, 2);

        modelBuilder.Entity<GradingScheme>()
            .HasOne(g => g.Branch).WithMany()
            .HasForeignKey(g => g.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GradingScheme>()
            .HasOne(g => g.Course).WithMany()
            .HasForeignKey(g => g.CourseId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GradingSchemeBand>()
            .HasOne(b => b.GradingScheme).WithMany(g => g.Bands)
            .HasForeignKey(b => b.GradingSchemeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GradingSchemeBand>()
            .Property(b => b.MinimumPercentage)
            .HasPrecision(5, 2);
        modelBuilder.Entity<GradingSchemeBand>()
            .Property(b => b.MaximumPercentage)
            .HasPrecision(5, 2);
        modelBuilder.Entity<GradingSchemeBand>()
            .Property(b => b.GradePoint)
            .HasPrecision(4, 2);

        modelBuilder.Entity<QuestionBankItem>()
            .HasOne(q => q.Branch).WithMany()
            .HasForeignKey(q => q.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionBankItem>()
            .HasOne(q => q.Subject).WithMany()
            .HasForeignKey(q => q.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionBankItem>()
            .HasOne(q => q.SyllabusUnit).WithMany()
            .HasForeignKey(q => q.SyllabusUnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionBankItem>()
            .HasOne(q => q.AuthorTeacherProfile).WithMany()
            .HasForeignKey(q => q.AuthorTeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionBankItem>()
            .HasOne(q => q.ApprovedByUser).WithMany()
            .HasForeignKey(q => q.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionBankItem>()
            .Property(q => q.Marks)
            .HasPrecision(7, 2);

        modelBuilder.Entity<QuestionPaper>()
            .HasOne(p => p.Branch).WithMany()
            .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaper>()
            .HasOne(p => p.Subject).WithMany()
            .HasForeignKey(p => p.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaper>()
            .HasOne(p => p.Exam).WithMany()
            .HasForeignKey(p => p.ExamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaper>()
            .HasOne(p => p.ExamSchedule).WithMany(s => s.QuestionPapers)
            .HasForeignKey(p => p.ExamScheduleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaper>()
            .HasOne(p => p.ApprovedByUser).WithMany()
            .HasForeignKey(p => p.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaper>()
            .Property(p => p.TotalMarks)
            .HasPrecision(7, 2);

        modelBuilder.Entity<QuestionPaperSection>()
            .HasOne(s => s.QuestionPaper).WithMany(p => p.Sections)
            .HasForeignKey(s => s.QuestionPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaperSection>()
            .Property(s => s.Marks)
            .HasPrecision(7, 2);

        modelBuilder.Entity<QuestionPaperQuestion>()
            .HasOne(q => q.QuestionPaperSection).WithMany(s => s.Questions)
            .HasForeignKey(q => q.QuestionPaperSectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaperQuestion>()
            .HasOne(q => q.QuestionBankItem).WithMany()
            .HasForeignKey(q => q.QuestionBankItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaperQuestion>()
            .Property(q => q.Marks)
            .HasPrecision(7, 2);

        modelBuilder.Entity<QuestionPaperVersion>()
            .HasOne(v => v.QuestionPaper).WithMany(p => p.Versions)
            .HasForeignKey(v => v.QuestionPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaperVersion>()
            .HasOne(v => v.CreatedByUser).WithMany()
            .HasForeignKey(v => v.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionPaperModeration>()
            .HasOne(m => m.QuestionPaper).WithMany(p => p.Moderations)
            .HasForeignKey(m => m.QuestionPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QuestionPaperModeration>()
            .HasOne(m => m.ReviewedByUser).WithMany()
            .HasForeignKey(m => m.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AIProviderSetting>().Property(e => e.InputCostPerMillionTokens).HasPrecision(12, 6);
        modelBuilder.Entity<AIProviderSetting>().Property(e => e.OutputCostPerMillionTokens).HasPrecision(12, 6);
        modelBuilder.Entity<AIProviderSetting>().HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        modelBuilder.Entity<AIProviderSetting>().HasIndex(e => new { e.TenantId, e.IsEnabled, e.IsDefault });

        modelBuilder.Entity<AIPromptTemplate>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIPromptTemplate>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code, e.Version }).IsUnique();

        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.Subject).WithMany()
            .HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.QuestionPaper).WithMany()
            .HasForeignKey(e => e.QuestionPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.GeneratedVersion).WithMany()
            .HasForeignKey(e => e.GeneratedVersionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.ProviderSetting).WithMany()
            .HasForeignKey(e => e.ProviderSettingId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.PromptTemplate).WithMany()
            .HasForeignKey(e => e.PromptTemplateId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>()
            .HasOne(e => e.RegeneratedFromRequest).WithMany()
            .HasForeignKey(e => e.RegeneratedFromRequestId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.RequestedOn });

        modelBuilder.Entity<AIGenerationResponse>()
            .HasOne(e => e.AIGenerationRequest).WithOne(e => e.Response)
            .HasForeignKey<AIGenerationResponse>(e => e.AIGenerationRequestId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationResponse>().HasIndex(e => e.AIGenerationRequestId).IsUnique();

        modelBuilder.Entity<AIGenerationLog>()
            .HasOne(e => e.AIGenerationRequest).WithMany(e => e.Logs)
            .HasForeignKey(e => e.AIGenerationRequestId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIGenerationLog>().HasIndex(e => new { e.AIGenerationRequestId, e.OccurredOn });

        modelBuilder.Entity<AIUsageRecord>()
            .HasOne(e => e.AIGenerationRequest).WithMany(e => e.UsageRecords)
            .HasForeignKey(e => e.AIGenerationRequestId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AIUsageRecord>().Property(e => e.EstimatedCost).HasPrecision(18, 6);
        modelBuilder.Entity<AIUsageRecord>().HasIndex(e => new { e.TenantId, e.RecordedOn });

        modelBuilder.Entity<MarkEntry>()
            .HasOne(m => m.ExamSchedule).WithMany(s => s.MarkEntries)
            .HasForeignKey(m => m.ExamScheduleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MarkEntry>()
            .HasOne(m => m.StudentProfile).WithMany()
            .HasForeignKey(m => m.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MarkEntry>()
            .HasOne(m => m.EnteredByUser).WithMany()
            .HasForeignKey(m => m.EnteredByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MarkEntry>()
            .Property(m => m.MarksObtained)
            .HasPrecision(7, 2);
        modelBuilder.Entity<MarkEntry>()
            .Property(m => m.GradePoint)
            .HasPrecision(4, 2);

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Exam).WithMany(e => e.Results)
            .HasForeignKey(r => r.ExamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.StudentProfile).WithMany()
            .HasForeignKey(r => r.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Branch).WithMany()
            .HasForeignKey(r => r.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.AcademicYear).WithMany()
            .HasForeignKey(r => r.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Course).WithMany()
            .HasForeignKey(r => r.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Batch).WithMany()
            .HasForeignKey(r => r.BatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Section).WithMany()
            .HasForeignKey(r => r.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .HasOne(r => r.PublishedByUser).WithMany()
            .HasForeignKey(r => r.PublishedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>()
            .Property(r => r.TotalMarks)
            .HasPrecision(9, 2);
        modelBuilder.Entity<Result>()
            .Property(r => r.MarksObtained)
            .HasPrecision(9, 2);
        modelBuilder.Entity<Result>()
            .Property(r => r.Percentage)
            .HasPrecision(5, 2);
        modelBuilder.Entity<Result>()
            .Property(r => r.GradePoint)
            .HasPrecision(4, 2);
        modelBuilder.Entity<Result>()
            .Property(r => r.Percentile)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.Branch).WithMany()
            .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.Exam).WithMany()
            .HasForeignKey(p => p.ExamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.Section).WithMany()
            .HasForeignKey(p => p.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.RequestedByUser).WithMany()
            .HasForeignKey(p => p.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.ApprovedByUser).WithMany()
            .HasForeignKey(p => p.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPublicationBatch>()
            .HasOne(p => p.PublishedByUser).WithMany()
            .HasForeignKey(p => p.PublishedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ResultRankingRule>()
            .HasOne(r => r.Branch).WithMany()
            .HasForeignKey(r => r.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultRankingRule>()
            .HasOne(r => r.Exam).WithMany()
            .HasForeignKey(r => r.ExamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultRankingRule>()
            .HasOne(r => r.Section).WithMany()
            .HasForeignKey(r => r.SectionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultRankingRule>()
            .Property(r => r.MinimumPercentageToRank)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Exam>().HasIndex(e => new { e.TenantId, e.BranchId, e.AcademicYearId, e.Name }).IsUnique();
        modelBuilder.Entity<Exam>().HasIndex(e => e.Status);
        modelBuilder.Entity<ExamSchedule>().HasIndex(s => new { s.TenantId, s.ExamId, s.SectionId, s.SubjectId }).IsUnique();
        modelBuilder.Entity<ExamSchedule>().HasIndex(s => new { s.ExamDate, s.StartsAt });
        modelBuilder.Entity<ExamSchedule>().HasIndex(s => s.RoomId);
        modelBuilder.Entity<GradingScheme>().HasIndex(g => new { g.TenantId, g.BranchId, g.Name }).IsUnique();
        modelBuilder.Entity<GradingSchemeBand>().HasIndex(b => new { b.GradingSchemeId, b.Grade }).IsUnique();
        modelBuilder.Entity<GradingSchemeBand>().HasIndex(b => new { b.GradingSchemeId, b.SortOrder });
        modelBuilder.Entity<QuestionBankItem>().HasIndex(q => new { q.TenantId, q.BranchId, q.SubjectId });
        modelBuilder.Entity<QuestionBankItem>().HasIndex(q => new { q.SubjectId, q.SyllabusUnitId });
        modelBuilder.Entity<QuestionPaper>().HasIndex(p => new { p.TenantId, p.BranchId, p.SubjectId });
        modelBuilder.Entity<QuestionPaper>().HasIndex(p => p.ExamScheduleId);
        modelBuilder.Entity<QuestionPaperSection>().HasIndex(s => new { s.QuestionPaperId, s.Code }).IsUnique();
        modelBuilder.Entity<QuestionPaperSection>().HasIndex(s => new { s.QuestionPaperId, s.SortOrder });
        modelBuilder.Entity<QuestionPaperQuestion>().HasIndex(q => new { q.QuestionPaperSectionId, q.SortOrder });
        modelBuilder.Entity<QuestionPaperQuestion>().HasIndex(q => q.QuestionBankItemId);
        modelBuilder.Entity<QuestionPaperVersion>().HasIndex(v => new { v.QuestionPaperId, v.VersionNumber }).IsUnique();
        modelBuilder.Entity<QuestionPaperModeration>().HasIndex(m => new { m.TenantId, m.QuestionPaperId, m.ReviewedOn });
        modelBuilder.Entity<MarkEntry>().HasIndex(m => new { m.TenantId, m.ExamScheduleId, m.StudentProfileId }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(r => new { r.TenantId, r.ExamId, r.StudentProfileId }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(r => new { r.TenantId, r.BranchId, r.Status });
        modelBuilder.Entity<ResultPublicationBatch>().HasIndex(p => new { p.TenantId, p.BranchId, p.ExamId, p.SectionId, p.Status });
        modelBuilder.Entity<ResultRankingRule>().HasIndex(r => new { r.TenantId, r.ExamId, r.SectionId }).IsUnique();
    }

    private static void ConfigureEnterpriseServices(ModelBuilder modelBuilder)
    {
        ConfigureFinance(modelBuilder);
        ConfigureTransport(modelBuilder);
        ConfigureLibrary(modelBuilder);
        ConfigureHostel(modelBuilder);
        ConfigureCommunications(modelBuilder);
    }

    private static void ConfigureFinance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeeStructure>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeStructure>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeStructure>()
            .HasOne(e => e.Course).WithMany()
            .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeStructure>()
            .HasOne(e => e.Batch).WithMany()
            .HasForeignKey(e => e.BatchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeeComponent>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeComponent>()
            .HasOne(e => e.FeeStructure).WithMany(e => e.Components)
            .HasForeignKey(e => e.FeeStructureId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeComponent>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<DiscountRule>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DiscountRule>()
            .HasOne(e => e.Course).WithMany()
            .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DiscountRule>()
            .Property(e => e.Value).HasPrecision(12, 2);

        modelBuilder.Entity<Scholarship>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Scholarship>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Scholarship>()
            .Property(e => e.Value).HasPrecision(12, 2);

        modelBuilder.Entity<StudentFeeAssignment>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFeeAssignment>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFeeAssignment>()
            .HasOne(e => e.FeeStructure).WithMany(e => e.Assignments)
            .HasForeignKey(e => e.FeeStructureId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFeeAssignment>()
            .HasOne(e => e.DiscountRule).WithMany()
            .HasForeignKey(e => e.DiscountRuleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFeeAssignment>()
            .Property(e => e.CustomDiscountAmount).HasPrecision(12, 2);

        modelBuilder.Entity<FeeInvoice>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoice>()
            .HasOne(e => e.StudentFeeAssignment).WithMany(e => e.Invoices)
            .HasForeignKey(e => e.StudentFeeAssignmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoice>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoice>()
            .HasOne(e => e.AdmissionApplication).WithMany(e => e.FeeInvoices)
            .HasForeignKey(e => e.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoice>()
            .Property(e => e.SubTotal).HasPrecision(12, 2);
        modelBuilder.Entity<FeeInvoice>()
            .Property(e => e.DiscountAmount).HasPrecision(12, 2);
        modelBuilder.Entity<FeeInvoice>()
            .Property(e => e.FineAmount).HasPrecision(12, 2);
        modelBuilder.Entity<FeeInvoice>()
            .Property(e => e.TotalAmount).HasPrecision(12, 2);
        modelBuilder.Entity<FeeInvoice>()
            .Property(e => e.PaidAmount).HasPrecision(12, 2);

        modelBuilder.Entity<FeeInvoiceLine>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoiceLine>()
            .HasOne(e => e.FeeInvoice).WithMany(e => e.Lines)
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoiceLine>()
            .HasOne(e => e.FeeComponent).WithMany()
            .HasForeignKey(e => e.FeeComponentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeInvoiceLine>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<FeePayment>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeePayment>()
            .HasOne(e => e.FeeInvoice).WithMany(e => e.Payments)
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeePayment>()
            .HasOne(e => e.ReceivedByUser).WithMany()
            .HasForeignKey(e => e.ReceivedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeePayment>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<FeeReceipt>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeReceipt>()
            .HasOne(e => e.FeePayment).WithOne(e => e.Receipt)
            .HasForeignKey<FeeReceipt>(e => e.FeePaymentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeReceipt>()
            .HasOne(e => e.IssuedByUser).WithMany()
            .HasForeignKey(e => e.IssuedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OnlinePaymentTransaction>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OnlinePaymentTransaction>()
            .HasOne(e => e.FeeInvoice).WithMany()
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OnlinePaymentTransaction>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<FeeReminder>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeReminder>()
            .HasOne(e => e.FeeInvoice).WithMany()
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeReminder>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeeRefund>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeRefund>()
            .HasOne(e => e.FeePayment).WithMany()
            .HasForeignKey(e => e.FeePaymentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeRefund>()
            .HasOne(e => e.FeeInvoice).WithMany()
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeRefund>()
            .HasOne(e => e.RequestedByUser).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeRefund>()
            .HasOne(e => e.ApprovedByUser).WithMany()
            .HasForeignKey(e => e.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeRefund>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.FeeInvoice).WithMany()
            .HasForeignKey(e => e.FeeInvoiceId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.FeeStructure).WithMany()
            .HasForeignKey(e => e.FeeStructureId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.RequestedByUser).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .HasOne(e => e.DecidedByUser).WithMany()
            .HasForeignKey(e => e.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeConcessionRequest>()
            .Property(e => e.RequestedValue).HasPrecision(12, 2);
        modelBuilder.Entity<FeeConcessionRequest>()
            .Property(e => e.ApprovedAmount).HasPrecision(12, 2);

        modelBuilder.Entity<LedgerExportBatch>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LedgerExportBatch>()
            .HasOne(e => e.GeneratedByUser).WithMany()
            .HasForeignKey(e => e.GeneratedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeeStructure>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<FeeStructure>().HasIndex(e => new { e.TenantId, e.BranchId, e.AcademicYearId, e.CourseId, e.BatchId });
        modelBuilder.Entity<FeeComponent>().HasIndex(e => new { e.FeeStructureId, e.SortOrder });
        modelBuilder.Entity<DiscountRule>().HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        modelBuilder.Entity<Scholarship>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Code }).IsUnique();
        modelBuilder.Entity<StudentFeeAssignment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.FeeStructureId }).IsUnique();
        modelBuilder.Entity<FeeInvoice>().HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
        modelBuilder.Entity<FeeInvoice>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<FeeInvoice>().HasIndex(e => e.AdmissionApplicationId);
        modelBuilder.Entity<FeeInvoiceLine>().HasIndex(e => new { e.FeeInvoiceId, e.SortOrder });
        modelBuilder.Entity<FeePayment>().HasIndex(e => new { e.TenantId, e.PaymentNumber }).IsUnique();
        modelBuilder.Entity<FeeReceipt>().HasIndex(e => new { e.TenantId, e.ReceiptNumber }).IsUnique();
        modelBuilder.Entity<FeeReceipt>().HasIndex(e => e.FeePaymentId).IsUnique();
        modelBuilder.Entity<OnlinePaymentTransaction>().HasIndex(e => new { e.TenantId, e.GatewayProviderKey, e.GatewayOrderId }).IsUnique();
        modelBuilder.Entity<OnlinePaymentTransaction>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<FeeReminder>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.ReminderOn });
        modelBuilder.Entity<FeeRefund>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<FeeConcessionRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<LedgerExportBatch>().HasIndex(e => new { e.TenantId, e.BranchId, e.ExportType, e.RequestedOn });
    }

    private static void ConfigureTransport(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportDriver>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRoute>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteStop>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteStop>()
            .HasOne(e => e.TransportRoute).WithMany(e => e.Stops)
            .HasForeignKey(e => e.TransportRouteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteStop>()
            .Property(e => e.MonthlyFee).HasPrecision(12, 2);
        modelBuilder.Entity<TransportRouteStop>()
            .Property(e => e.Latitude).HasPrecision(10, 7);
        modelBuilder.Entity<TransportRouteStop>()
            .Property(e => e.Longitude).HasPrecision(10, 7);

        modelBuilder.Entity<TransportRouteAssignment>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteAssignment>()
            .HasOne(e => e.TransportRoute).WithMany()
            .HasForeignKey(e => e.TransportRouteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteAssignment>()
            .HasOne(e => e.Vehicle).WithMany()
            .HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportRouteAssignment>()
            .HasOne(e => e.Driver).WithMany()
            .HasForeignKey(e => e.DriverId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentTransportAssignment>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentTransportAssignment>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentTransportAssignment>()
            .HasOne(e => e.TransportRoute).WithMany()
            .HasForeignKey(e => e.TransportRouteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentTransportAssignment>()
            .HasOne(e => e.TransportRouteStop).WithMany()
            .HasForeignKey(e => e.TransportRouteStopId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentTransportAssignment>()
            .HasOne(e => e.Vehicle).WithMany()
            .HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentTransportAssignment>()
            .Property(e => e.MonthlyFee).HasPrecision(12, 2);

        modelBuilder.Entity<VehicleGpsPing>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<VehicleGpsPing>()
            .HasOne(e => e.Vehicle).WithMany()
            .HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<VehicleGpsPing>()
            .HasOne(e => e.TransportRoute).WithMany()
            .HasForeignKey(e => e.TransportRouteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<VehicleGpsPing>()
            .Property(e => e.Latitude).HasPrecision(10, 7);
        modelBuilder.Entity<VehicleGpsPing>()
            .Property(e => e.Longitude).HasPrecision(10, 7);
        modelBuilder.Entity<VehicleGpsPing>()
            .Property(e => e.SpeedKmph).HasPrecision(8, 2);
        modelBuilder.Entity<VehicleGpsPing>()
            .Property(e => e.HeadingDegrees).HasPrecision(6, 2);

        modelBuilder.Entity<TransportMaintenanceRecord>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportMaintenanceRecord>()
            .HasOne(e => e.Vehicle).WithMany()
            .HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransportDocumentReminder>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TransportDocumentReminder>()
            .HasOne(e => e.Vehicle).WithMany()
            .HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransportRoute>()
            .Property(e => e.DistanceKm).HasPrecision(8, 2);

        modelBuilder.Entity<Vehicle>().HasIndex(e => new { e.TenantId, e.BranchId, e.RegistrationNumber }).IsUnique();
        modelBuilder.Entity<TransportDriver>().HasIndex(e => new { e.TenantId, e.BranchId, e.LicenseNumber }).IsUnique();
        modelBuilder.Entity<TransportRoute>().HasIndex(e => new { e.TenantId, e.BranchId, e.RouteCode }).IsUnique();
        modelBuilder.Entity<TransportRouteStop>().HasIndex(e => new { e.TransportRouteId, e.StopOrder }).IsUnique();
        modelBuilder.Entity<TransportRouteAssignment>().HasIndex(e => new { e.TenantId, e.BranchId, e.TransportRouteId, e.EffectiveFrom });
        modelBuilder.Entity<StudentTransportAssignment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Status });
        modelBuilder.Entity<VehicleGpsPing>().HasIndex(e => new { e.TenantId, e.VehicleId, e.RecordedOn });
        modelBuilder.Entity<TransportMaintenanceRecord>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.DueOn });
        modelBuilder.Entity<TransportDocumentReminder>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.DueOn });
    }

    private static void ConfigureLibrary(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryBook>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBook>()
            .HasOne(e => e.Subject).WithMany()
            .HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookCopy>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookCopy>()
            .HasOne(e => e.LibraryBook).WithMany(e => e.Copies)
            .HasForeignKey(e => e.LibraryBookId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookCopy>()
            .Property(e => e.Price).HasPrecision(12, 2);

        modelBuilder.Entity<LibraryMember>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryMember>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryMember>()
            .HasOne(e => e.TeacherProfile).WithMany()
            .HasForeignKey(e => e.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryMember>()
            .HasOne(e => e.User).WithMany()
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryBookIssue>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookIssue>()
            .HasOne(e => e.LibraryBookCopy).WithMany()
            .HasForeignKey(e => e.LibraryBookCopyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookIssue>()
            .HasOne(e => e.LibraryMember).WithMany()
            .HasForeignKey(e => e.LibraryMemberId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookIssue>()
            .HasOne(e => e.IssuedByUser).WithMany()
            .HasForeignKey(e => e.IssuedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookIssue>()
            .HasOne(e => e.ReturnReceivedByUser).WithMany()
            .HasForeignKey(e => e.ReturnReceivedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookIssue>()
            .Property(e => e.FineAmount).HasPrecision(12, 2);

        modelBuilder.Entity<LibraryBookReturn>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookReturn>()
            .HasOne(e => e.LibraryBookIssue).WithMany(e => e.Returns)
            .HasForeignKey(e => e.LibraryBookIssueId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookReturn>()
            .HasOne(e => e.ReceivedByUser).WithMany()
            .HasForeignKey(e => e.ReceivedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBookReturn>()
            .Property(e => e.FineAssessed).HasPrecision(12, 2);
        modelBuilder.Entity<LibraryBookReturn>()
            .Property(e => e.FinePaid).HasPrecision(12, 2);

        modelBuilder.Entity<LibraryFineRecord>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryFineRecord>()
            .HasOne(e => e.LibraryMember).WithMany()
            .HasForeignKey(e => e.LibraryMemberId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryFineRecord>()
            .HasOne(e => e.LibraryBookIssue).WithMany()
            .HasForeignKey(e => e.LibraryBookIssueId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryFineRecord>()
            .Property(e => e.Amount).HasPrecision(12, 2);

        modelBuilder.Entity<LibraryReservation>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryReservation>()
            .HasOne(e => e.LibraryBook).WithMany()
            .HasForeignKey(e => e.LibraryBookId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryReservation>()
            .HasOne(e => e.LibraryBookCopy).WithMany()
            .HasForeignKey(e => e.LibraryBookCopyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryReservation>()
            .HasOne(e => e.LibraryMember).WithMany()
            .HasForeignKey(e => e.LibraryMemberId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryRenewal>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryRenewal>()
            .HasOne(e => e.LibraryBookIssue).WithMany()
            .HasForeignKey(e => e.LibraryBookIssueId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryRenewal>()
            .HasOne(e => e.RenewedByUser).WithMany()
            .HasForeignKey(e => e.RenewedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryOverdueNotification>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryOverdueNotification>()
            .HasOne(e => e.LibraryBookIssue).WithMany()
            .HasForeignKey(e => e.LibraryBookIssueId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryOverdueNotification>()
            .HasOne(e => e.LibraryMember).WithMany()
            .HasForeignKey(e => e.LibraryMemberId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryBarcodeScan>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryBarcodeScan>()
            .HasOne(e => e.LibraryBookCopy).WithMany()
            .HasForeignKey(e => e.LibraryBookCopyId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryBook>().HasIndex(e => new { e.TenantId, e.BranchId, e.Isbn });
        modelBuilder.Entity<LibraryBook>().HasIndex(e => new { e.TenantId, e.BranchId, e.Title });
        modelBuilder.Entity<LibraryBookCopy>().HasIndex(e => new { e.TenantId, e.BranchId, e.AccessionNumber }).IsUnique();
        modelBuilder.Entity<LibraryBookCopy>().HasIndex(e => new { e.TenantId, e.BranchId, e.Barcode });
        modelBuilder.Entity<LibraryMember>().HasIndex(e => new { e.TenantId, e.BranchId, e.MemberNumber }).IsUnique();
        modelBuilder.Entity<LibraryMember>().HasIndex(e => new { e.TenantId, e.StudentProfileId });
        modelBuilder.Entity<LibraryBookIssue>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<LibraryBookIssue>().HasIndex(e => new { e.LibraryBookCopyId, e.Status });
        modelBuilder.Entity<LibraryBookReturn>().HasIndex(e => e.LibraryBookIssueId);
        modelBuilder.Entity<LibraryFineRecord>().HasIndex(e => new { e.TenantId, e.LibraryMemberId, e.Status });
        modelBuilder.Entity<LibraryReservation>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.ExpiresOn });
        modelBuilder.Entity<LibraryRenewal>().HasIndex(e => new { e.TenantId, e.LibraryBookIssueId, e.RenewedOn });
        modelBuilder.Entity<LibraryOverdueNotification>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<LibraryBarcodeScan>().HasIndex(e => new { e.TenantId, e.BranchId, e.ScanCode, e.ScannedOn });
    }

    private static void ConfigureHostel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HostelBlock>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelRoom>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelRoom>()
            .HasOne(e => e.HostelBlock).WithMany(e => e.Rooms)
            .HasForeignKey(e => e.HostelBlockId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelRoom>()
            .Property(e => e.MonthlyFee).HasPrecision(12, 2);

        modelBuilder.Entity<HostelBed>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelBed>()
            .HasOne(e => e.HostelRoom).WithMany(e => e.Beds)
            .HasForeignKey(e => e.HostelRoomId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HostelAllocation>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocation>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocation>()
            .HasOne(e => e.HostelRoom).WithMany()
            .HasForeignKey(e => e.HostelRoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocation>()
            .HasOne(e => e.HostelBed).WithMany()
            .HasForeignKey(e => e.HostelBedId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocation>()
            .Property(e => e.MonthlyFee).HasPrecision(12, 2);
        modelBuilder.Entity<HostelAllocation>()
            .Property(e => e.SecurityDeposit).HasPrecision(12, 2);

        modelBuilder.Entity<HostelFee>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelFee>()
            .HasOne(e => e.HostelAllocation).WithMany(e => e.Fees)
            .HasForeignKey(e => e.HostelAllocationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelFee>()
            .Property(e => e.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<HostelFee>()
            .Property(e => e.PaidAmount).HasPrecision(12, 2);

        modelBuilder.Entity<HostelVisitorLog>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelVisitorLog>()
            .HasOne(e => e.HostelAllocation).WithMany()
            .HasForeignKey(e => e.HostelAllocationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelVisitorLog>()
            .HasOne(e => e.StudentProfile).WithMany()
            .HasForeignKey(e => e.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelVisitorLog>()
            .HasOne(e => e.ApprovedByUser).WithMany()
            .HasForeignKey(e => e.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HostelMaintenanceRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelMaintenanceRequest>()
            .HasOne(e => e.HostelBlock).WithMany()
            .HasForeignKey(e => e.HostelBlockId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelMaintenanceRequest>()
            .HasOne(e => e.HostelRoom).WithMany()
            .HasForeignKey(e => e.HostelRoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelMaintenanceRequest>()
            .HasOne(e => e.HostelBed).WithMany()
            .HasForeignKey(e => e.HostelBedId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelMaintenanceRequest>()
            .HasOne(e => e.ReportedByUser).WithMany()
            .HasForeignKey(e => e.ReportedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.HostelAllocation).WithMany()
            .HasForeignKey(e => e.HostelAllocationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.FromRoom).WithMany()
            .HasForeignKey(e => e.FromRoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.FromBed).WithMany()
            .HasForeignKey(e => e.FromBedId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.ToRoom).WithMany()
            .HasForeignKey(e => e.ToRoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.ToBed).WithMany()
            .HasForeignKey(e => e.ToBedId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.RequestedByUser).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HostelAllocationTransferRequest>()
            .HasOne(e => e.ApprovedByUser).WithMany()
            .HasForeignKey(e => e.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HostelBlock>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<HostelRoom>().HasIndex(e => new { e.TenantId, e.HostelBlockId, e.RoomNumber }).IsUnique();
        modelBuilder.Entity<HostelBed>().HasIndex(e => new { e.TenantId, e.HostelRoomId, e.BedNumber }).IsUnique();
        modelBuilder.Entity<HostelAllocation>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Status });
        modelBuilder.Entity<HostelAllocation>().HasIndex(e => new { e.TenantId, e.HostelBedId, e.Status });
        modelBuilder.Entity<HostelFee>().HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
        modelBuilder.Entity<HostelFee>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<HostelVisitorLog>().HasIndex(e => new { e.TenantId, e.BranchId, e.CheckInOn });
        modelBuilder.Entity<HostelMaintenanceRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.Priority });
        modelBuilder.Entity<HostelAllocationTransferRequest>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
    }

    private static void ConfigureCommunications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationProviderSetting>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationTemplate>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationMessage>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationMessage>()
            .HasOne(e => e.NotificationTemplate).WithMany()
            .HasForeignKey(e => e.NotificationTemplateId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationMessage>()
            .HasOne(e => e.CreatedForUser).WithMany()
            .HasForeignKey(e => e.CreatedForUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationRecipient>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationRecipient>()
            .HasOne(e => e.NotificationMessage).WithMany(e => e.Recipients)
            .HasForeignKey(e => e.NotificationMessageId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationRecipient>()
            .HasOne(e => e.User).WithMany()
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Announcement>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Announcement>()
            .HasOne(e => e.CreatedByUser).WithMany()
            .HasForeignKey(e => e.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CommunicationLog>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationDeliveryAttempt>()
            .HasOne(e => e.Branch).WithMany()
            .HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationDeliveryAttempt>()
            .HasOne(e => e.NotificationMessage).WithMany()
            .HasForeignKey(e => e.NotificationMessageId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationDeliveryAttempt>()
            .HasOne(e => e.NotificationRecipient).WithMany()
            .HasForeignKey(e => e.NotificationRecipientId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationProviderSetting>().HasIndex(e => new { e.TenantId, e.BranchId, e.Channel, e.ProviderKey }).IsUnique();
        modelBuilder.Entity<NotificationTemplate>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<NotificationMessage>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<NotificationMessage>().HasIndex(e => e.ScheduledOn);
        modelBuilder.Entity<NotificationRecipient>().HasIndex(e => new { e.NotificationMessageId, e.DestinationAddress });
        modelBuilder.Entity<Announcement>().HasIndex(e => new { e.TenantId, e.BranchId, e.Audience, e.PublishOn });
        modelBuilder.Entity<CommunicationLog>().HasIndex(e => new { e.TenantId, e.BranchId, e.Channel, e.OccurredOn });
        modelBuilder.Entity<NotificationDeliveryAttempt>().HasIndex(e => new { e.TenantId, e.NotificationMessageId, e.NotificationRecipientId, e.AttemptNumber });
        modelBuilder.Entity<NotificationDeliveryAttempt>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status, e.NextRetryOn });
    }

    public override int SaveChanges()
    {
        ApplyWritePolicies();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyWritePolicies();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyWritePolicies()
    {
        var tenantId = _tenantContext.TenantId;
        var now = DateTime.UtcNow;
        var userId = string.IsNullOrWhiteSpace(_currentUserContext.UserId)
            ? "system"
            : _currentUserContext.UserId;

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList())
        {
            if (entry.Entity is not ISoftDeletable softDeletable)
                continue;

            entry.State = EntityState.Modified;
            softDeletable.IsDeleted = true;
            softDeletable.DeletedOn = now;
            softDeletable.DeletedBy = userId;
        }

        foreach (EntityEntry<IGuidEntity> entry in ChangeTracker.Entries<IGuidEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.Id == Guid.Empty)
                entry.Entity.Id = Guid.NewGuid();
        }

        foreach (EntityEntry<ITenantEntity> entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.TenantId == Guid.Empty && tenantId.HasValue)
                        entry.Entity.TenantId = tenantId.Value;
                    break;

                case EntityState.Modified:
                    // Never let an update move a row to a different tenant.
                    entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                    break;
            }
        }

        foreach (EntityEntry<IAuditableEntity> entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = entry.Entity.CreatedOn == default ? now : entry.Entity.CreatedOn;
                    entry.Entity.CreatedBy ??= userId;
                    entry.Entity.ModifiedOn = null;
                    entry.Entity.ModifiedBy = null;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.CreatedOn)).IsModified = false;
                    entry.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                    entry.Entity.ModifiedOn = now;
                    entry.Entity.ModifiedBy = userId;
                    break;
            }
        }

        foreach (EntityEntry<IConcurrencyTrackedEntity> entry in ChangeTracker.Entries<IConcurrencyTrackedEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.ConcurrencyToken == Guid.Empty)
                        entry.Entity.ConcurrencyToken = Guid.NewGuid();
                    break;

                case EntityState.Modified:
                    entry.Entity.ConcurrencyToken = Guid.NewGuid();
                    break;
            }
        }

        AppendAuditLogs(now, userId);
    }

    private static void ConfigureAdministration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionPlan>().Property(e => e.MonthlyPrice).HasPrecision(18, 2);
        modelBuilder.Entity<SubscriptionPlan>().Property(e => e.AnnualPrice).HasPrecision(18, 2);
        modelBuilder.Entity<SubscriptionPlan>().HasIndex(e => e.Code).IsUnique();
        modelBuilder.Entity<TenantSubscription>().HasOne(e => e.SubscriptionPlan).WithMany().HasForeignKey(e => e.SubscriptionPlanId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TenantSubscription>().Property(e => e.AgreedMonthlyPrice).HasPrecision(18, 2);
        modelBuilder.Entity<TenantSubscription>().HasIndex(e => new { e.TenantId, e.Status });
        modelBuilder.Entity<TenantBranding>().HasIndex(e => e.TenantId).IsUnique();
        modelBuilder.Entity<TenantDomain>().HasIndex(e => e.DomainName).IsUnique();
        modelBuilder.Entity<TenantDomain>().HasIndex(e => new { e.TenantId, e.IsPrimary });
        modelBuilder.Entity<TenantIntegrationSetting>().HasIndex(e => new { e.TenantId, e.IntegrationKey }).IsUnique();
        modelBuilder.Entity<TenantFeatureFlag>().HasIndex(e => new { e.TenantId, e.FeatureKey }).IsUnique();
        modelBuilder.Entity<BranchContact>().HasOne(e => e.Branch).WithMany().HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BranchContact>().HasIndex(e => new { e.TenantId, e.BranchId, e.ContactType, e.ContactName });
        modelBuilder.Entity<BranchAcademicConfig>().HasOne(e => e.Branch).WithMany().HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BranchAcademicConfig>().HasIndex(e => e.BranchId).IsUnique();
        modelBuilder.Entity<AppSetting>().HasIndex(e => e.Key).IsUnique();
        modelBuilder.Entity<TenantSetting>().HasIndex(e => new { e.TenantId, e.Key }).IsUnique();
        modelBuilder.Entity<LookupItem>().HasIndex(e => new { e.TenantId, e.LookupType, e.Code }).IsUnique();
        modelBuilder.Entity<AuditLog>().HasIndex(e => new { e.TenantId, e.OccurredOn });
        modelBuilder.Entity<AuditLog>().HasIndex(e => new { e.EntityType, e.EntityId });
        modelBuilder.Entity<BackgroundJobLog>().HasIndex(e => new { e.TenantId, e.Status, e.ScheduledOn });
        modelBuilder.Entity<BackgroundJobLog>().HasIndex(e => new { e.TenantId, e.JobKey });
        modelBuilder.Entity<PermissionDefinition>().HasIndex(e => e.Key).IsUnique();
        modelBuilder.Entity<RolePermissionGrant>().HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RolePermissionGrant>().HasOne(e => e.PermissionDefinition).WithMany().HasForeignKey(e => e.PermissionDefinitionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RolePermissionGrant>().HasIndex(e => new { e.TenantId, e.RoleId, e.PermissionDefinitionId }).IsUnique();
        modelBuilder.Entity<UserPermissionGrant>().HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserPermissionGrant>().HasOne(e => e.PermissionDefinition).WithMany().HasForeignKey(e => e.PermissionDefinitionId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserPermissionGrant>().HasIndex(e => new { e.TenantId, e.UserId, e.PermissionDefinitionId }).IsUnique();
    }

    private void AppendAuditLogs(DateTime now, string userId)
    {
        var candidates = ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditLog && e.Entity is not IdentityUser<Guid> &&
                        e.State is EntityState.Added or EntityState.Modified)
            .ToList();
        foreach (var entry in candidates)
        {
            var entityId = entry.Entity is IGuidEntity guid ? guid.Id.ToString() : "unknown";
            var tenantId = entry.Entity is ITenantEntity tenantOwned ? tenantOwned.TenantId :
                entry.Entity is Tenant tenant ? tenant.Id : _tenantContext.TenantId;
            var branchProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "BranchId");
            Guid? branchId = branchProperty?.CurrentValue is Guid branch && branch != Guid.Empty ? branch : null;
            var isDelete = entry.Entity is ISoftDeletable deleted && deleted.IsDeleted &&
                           entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified;
            var action = entry.State == EntityState.Added ? AuditAction.Created : isDelete ? AuditAction.Deleted : AuditAction.Updated;
            var oldValues = entry.State == EntityState.Added ? null : SerializeAuditValues(entry, entry.Properties.Where(p => p.IsModified), false);
            var newValues = SerializeAuditValues(entry, entry.State == EntityState.Added ? entry.Properties : entry.Properties.Where(p => p.IsModified), true);
            AuditLogs.Add(new AuditLog
            {
                TenantId = tenantId == Guid.Empty ? null : tenantId,
                BranchId = branchId,
                EntityType = entry.Metadata.ClrType.Name,
                EntityId = entityId,
                Action = action,
                ActorId = userId,
                OccurredOn = now,
                OldValuesJson = oldValues,
                NewValuesJson = newValues,
                CreatedBy = userId,
                CreatedOn = now,
                ConcurrencyToken = Guid.NewGuid()
            });
        }
    }

    private static string? SerializeAuditValues(EntityEntry entry, IEnumerable<PropertyEntry> properties, bool current)
    {
        var values = properties.ToDictionary(
            p => p.Metadata.Name,
            p => IsSensitiveAuditField(entry.Entity, p.Metadata.Name) ? "[REDACTED]" : current ? p.CurrentValue : p.OriginalValue);
        return values.Count == 0 ? null : JsonSerializer.Serialize(values);
    }

    private static bool IsSensitiveAuditField(object entity, string name) =>
        (name == "Value" && (entity is AppSetting { IsSensitive: true } || entity is TenantSetting { IsSensitive: true })) ||
        name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Token", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Protected", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("SecurityStamp", StringComparison.OrdinalIgnoreCase);

    private sealed class SystemCurrentUserContext : ICurrentUserContext
    {
        public static readonly SystemCurrentUserContext Instance = new();
        public string UserId => "system";
    }
}
