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
