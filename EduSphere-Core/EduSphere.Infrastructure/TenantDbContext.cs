using System.Reflection;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.MultiTenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RoleNames = EduSphere.Domain.Constants.Roles;

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
    public DbSet<TeacherProfile> TeacherProfiles { get; set; }
    public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; }
    public DbSet<ProfileDocument> ProfileDocuments { get; set; }

    // Admissions / Attendance / Timetable (Module 5-9)
    public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
    public DbSet<AdmissionDocument> AdmissionDocuments { get; set; }
    public DbSet<AdmissionReview> AdmissionReviews { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<PromotionRecord> PromotionRecords { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<AttendancePolicy> AttendancePolicies { get; set; }
    public DbSet<LeaveApplication> LeaveApplications { get; set; }
    public DbSet<AttendanceAlert> AttendanceAlerts { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Timetable> Timetables { get; set; }
    public DbSet<TimetableEntry> TimetableEntries { get; set; }

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
    public DbSet<MarkEntry> MarkEntries { get; set; }
    public DbSet<Result> Results { get; set; }

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
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<TransportDriver> TransportDrivers { get; set; }
    public DbSet<TransportRoute> TransportRoutes { get; set; }
    public DbSet<TransportRouteStop> TransportRouteStops { get; set; }
    public DbSet<TransportRouteAssignment> TransportRouteAssignments { get; set; }
    public DbSet<StudentTransportAssignment> StudentTransportAssignments { get; set; }
    public DbSet<LibraryBook> LibraryBooks { get; set; }
    public DbSet<LibraryBookCopy> LibraryBookCopies { get; set; }
    public DbSet<LibraryMember> LibraryMembers { get; set; }
    public DbSet<LibraryBookIssue> LibraryBookIssues { get; set; }
    public DbSet<LibraryBookReturn> LibraryBookReturns { get; set; }
    public DbSet<LibraryFineRecord> LibraryFineRecords { get; set; }
    public DbSet<HostelBlock> HostelBlocks { get; set; }
    public DbSet<HostelRoom> HostelRooms { get; set; }
    public DbSet<HostelBed> HostelBeds { get; set; }
    public DbSet<HostelAllocation> HostelAllocations { get; set; }
    public DbSet<HostelFee> HostelFees { get; set; }
    public DbSet<NotificationProviderSetting> NotificationProviderSettings { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
    public DbSet<NotificationMessage> NotificationMessages { get; set; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<CommunicationLog> CommunicationLogs { get; set; }

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

        ConfigureAcademicStructure(modelBuilder);
        ConfigurePeopleProfiles(modelBuilder);
        ConfigureOperations(modelBuilder);
        ConfigureExaminations(modelBuilder);
        ConfigureEnterpriseServices(modelBuilder);

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

        modelBuilder.Entity<StudentProfile>().HasIndex(s => new { s.TenantId, s.AdmissionNumber }).IsUnique();
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.BranchId);
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.SectionId);
        modelBuilder.Entity<StudentProfile>().HasIndex(s => s.UserId);

        modelBuilder.Entity<TeacherProfile>().HasIndex(t => new { t.TenantId, t.EmployeeNumber }).IsUnique();
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.BranchId);
        modelBuilder.Entity<TeacherProfile>().HasIndex(t => t.UserId);

        modelBuilder.Entity<StudentGuardian>().HasIndex(g => g.StudentProfileId);
        modelBuilder.Entity<StudentGuardian>().HasIndex(g => g.ParentUserId);

        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.TeacherProfileId);
        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.SubjectId);
        modelBuilder.Entity<TeacherSubjectAssignment>().HasIndex(a => a.SectionId);

        modelBuilder.Entity<ProfileDocument>().HasIndex(d => new { d.OwnerType, d.OwnerId });
    }

    private static void ConfigureOperations(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<AdmissionDocument>()
            .HasOne(d => d.AdmissionApplication).WithMany(a => a.Documents)
            .HasForeignKey(d => d.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionDocument>()
            .HasOne(d => d.VerifiedByUser).WithMany()
            .HasForeignKey(d => d.VerifiedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdmissionReview>()
            .HasOne(r => r.AdmissionApplication).WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AdmissionApplicationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AdmissionReview>()
            .HasOne(r => r.ReviewedByUser).WithMany()
            .HasForeignKey(r => r.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => new { a.TenantId, a.ApplicationNumber }).IsUnique();
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.BranchId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.CourseId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.BatchId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.SectionId);
        modelBuilder.Entity<AdmissionApplication>().HasIndex(a => a.Status);
        modelBuilder.Entity<AdmissionDocument>().HasIndex(d => d.AdmissionApplicationId);
        modelBuilder.Entity<AdmissionReview>().HasIndex(r => r.AdmissionApplicationId);
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.TenantId, e.EnrollmentNumber }).IsUnique();
        modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.AcademicYearId }).IsUnique();
        modelBuilder.Entity<PromotionRecord>().HasIndex(p => p.StudentProfileId);

        modelBuilder.Entity<AttendanceSession>().HasIndex(s => new { s.TenantId, s.SectionId, s.AttendanceDate });
        modelBuilder.Entity<AttendanceSession>().HasIndex(s => s.SubjectId);
        modelBuilder.Entity<AttendanceRecord>().HasIndex(r => new { r.TenantId, r.AttendanceSessionId, r.StudentProfileId }).IsUnique();
        modelBuilder.Entity<AttendancePolicy>().HasIndex(p => new { p.TenantId, p.Name }).IsUnique();
        modelBuilder.Entity<LeaveApplication>().HasIndex(l => new { l.TenantId, l.StudentProfileId, l.FromDate });
        modelBuilder.Entity<AttendanceAlert>().HasIndex(a => new { a.TenantId, a.StudentProfileId, a.Status });

        modelBuilder.Entity<Room>().HasIndex(r => new { r.TenantId, r.BranchId, r.Code }).IsUnique();
        modelBuilder.Entity<TimeSlot>().HasIndex(t => new { t.TenantId, t.BranchId, t.DayOfWeek, t.PeriodNumber }).IsUnique();
        modelBuilder.Entity<Timetable>().HasIndex(t => new { t.TenantId, t.SectionId, t.EffectiveFrom });
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.SectionId, e.TimeSlotId }).IsUnique();
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.TeacherProfileId, e.TimeSlotId }).IsUnique();
        modelBuilder.Entity<TimetableEntry>().HasIndex(e => new { e.TimetableId, e.RoomId, e.TimeSlotId });
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
        modelBuilder.Entity<MarkEntry>().HasIndex(m => new { m.TenantId, m.ExamScheduleId, m.StudentProfileId }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(r => new { r.TenantId, r.ExamId, r.StudentProfileId }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(r => new { r.TenantId, r.BranchId, r.Status });
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

        modelBuilder.Entity<FeeStructure>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<FeeStructure>().HasIndex(e => new { e.TenantId, e.BranchId, e.AcademicYearId, e.CourseId, e.BatchId });
        modelBuilder.Entity<FeeComponent>().HasIndex(e => new { e.FeeStructureId, e.SortOrder });
        modelBuilder.Entity<DiscountRule>().HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        modelBuilder.Entity<Scholarship>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Code }).IsUnique();
        modelBuilder.Entity<StudentFeeAssignment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.FeeStructureId }).IsUnique();
        modelBuilder.Entity<FeeInvoice>().HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
        modelBuilder.Entity<FeeInvoice>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<FeeInvoiceLine>().HasIndex(e => new { e.FeeInvoiceId, e.SortOrder });
        modelBuilder.Entity<FeePayment>().HasIndex(e => new { e.TenantId, e.PaymentNumber }).IsUnique();
        modelBuilder.Entity<FeeReceipt>().HasIndex(e => new { e.TenantId, e.ReceiptNumber }).IsUnique();
        modelBuilder.Entity<FeeReceipt>().HasIndex(e => e.FeePaymentId).IsUnique();
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

        modelBuilder.Entity<TransportRoute>()
            .Property(e => e.DistanceKm).HasPrecision(8, 2);

        modelBuilder.Entity<Vehicle>().HasIndex(e => new { e.TenantId, e.BranchId, e.RegistrationNumber }).IsUnique();
        modelBuilder.Entity<TransportDriver>().HasIndex(e => new { e.TenantId, e.BranchId, e.LicenseNumber }).IsUnique();
        modelBuilder.Entity<TransportRoute>().HasIndex(e => new { e.TenantId, e.BranchId, e.RouteCode }).IsUnique();
        modelBuilder.Entity<TransportRouteStop>().HasIndex(e => new { e.TransportRouteId, e.StopOrder }).IsUnique();
        modelBuilder.Entity<TransportRouteAssignment>().HasIndex(e => new { e.TenantId, e.BranchId, e.TransportRouteId, e.EffectiveFrom });
        modelBuilder.Entity<StudentTransportAssignment>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Status });
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

        modelBuilder.Entity<LibraryBook>().HasIndex(e => new { e.TenantId, e.BranchId, e.Isbn });
        modelBuilder.Entity<LibraryBook>().HasIndex(e => new { e.TenantId, e.BranchId, e.Title });
        modelBuilder.Entity<LibraryBookCopy>().HasIndex(e => new { e.TenantId, e.BranchId, e.AccessionNumber }).IsUnique();
        modelBuilder.Entity<LibraryMember>().HasIndex(e => new { e.TenantId, e.BranchId, e.MemberNumber }).IsUnique();
        modelBuilder.Entity<LibraryMember>().HasIndex(e => new { e.TenantId, e.StudentProfileId });
        modelBuilder.Entity<LibraryBookIssue>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<LibraryBookIssue>().HasIndex(e => new { e.LibraryBookCopyId, e.Status });
        modelBuilder.Entity<LibraryBookReturn>().HasIndex(e => e.LibraryBookIssueId);
        modelBuilder.Entity<LibraryFineRecord>().HasIndex(e => new { e.TenantId, e.LibraryMemberId, e.Status });
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

        modelBuilder.Entity<HostelBlock>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<HostelRoom>().HasIndex(e => new { e.TenantId, e.HostelBlockId, e.RoomNumber }).IsUnique();
        modelBuilder.Entity<HostelBed>().HasIndex(e => new { e.TenantId, e.HostelRoomId, e.BedNumber }).IsUnique();
        modelBuilder.Entity<HostelAllocation>().HasIndex(e => new { e.TenantId, e.StudentProfileId, e.Status });
        modelBuilder.Entity<HostelAllocation>().HasIndex(e => new { e.TenantId, e.HostelBedId, e.Status });
        modelBuilder.Entity<HostelFee>().HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
        modelBuilder.Entity<HostelFee>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
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

        modelBuilder.Entity<NotificationProviderSetting>().HasIndex(e => new { e.TenantId, e.BranchId, e.Channel, e.ProviderKey }).IsUnique();
        modelBuilder.Entity<NotificationTemplate>().HasIndex(e => new { e.TenantId, e.BranchId, e.Code }).IsUnique();
        modelBuilder.Entity<NotificationMessage>().HasIndex(e => new { e.TenantId, e.BranchId, e.Status });
        modelBuilder.Entity<NotificationMessage>().HasIndex(e => e.ScheduledOn);
        modelBuilder.Entity<NotificationRecipient>().HasIndex(e => new { e.NotificationMessageId, e.DestinationAddress });
        modelBuilder.Entity<Announcement>().HasIndex(e => new { e.TenantId, e.BranchId, e.Audience, e.PublishOn });
        modelBuilder.Entity<CommunicationLog>().HasIndex(e => new { e.TenantId, e.BranchId, e.Channel, e.OccurredOn });
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
    }

    private sealed class SystemCurrentUserContext : ICurrentUserContext
    {
        public static readonly SystemCurrentUserContext Instance = new();
        public string UserId => "system";
    }
}
