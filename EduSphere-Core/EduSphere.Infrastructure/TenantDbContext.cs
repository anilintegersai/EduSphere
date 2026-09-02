using System.Reflection;
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

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
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

        // Global tenant query filter for every tenant-owned entity EXCEPT ApplicationUser.
        // Identity's UserManager/SignInManager must resolve users (including a host-level
        // SuperAdmin) independent of the resolved tenant, so users are scoped at the
        // authorization/application layer rather than by an automatic read filter.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(ITenantEntity).IsAssignableFrom(clr) && clr != typeof(ApplicationUser))
            {
                ConfigureTenantFilterMethod
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

    private static readonly MethodInfo ConfigureTenantFilterMethod =
        typeof(TenantDbContext).GetMethod(nameof(ConfigureTenantFilter),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    // Referencing the injected context instance keeps the value parameterized:
    // EF re-reads _tenantContext.TenantId per query rather than baking it into the model.
    private void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
    }

    private static void ConfigureAcademicStructure(ModelBuilder modelBuilder)
    {
        // Relationships — all Restrict so soft delete (IsActive) governs lifecycle and
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
        ApplyTenantOnSave();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantOnSave();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantOnSave()
    {
        var tenantId = _tenantContext.TenantId;

        foreach (EntityEntry<ITenantEntity> entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.TenantId == 0 && tenantId.HasValue)
                        entry.Entity.TenantId = tenantId.Value;
                    break;

                case EntityState.Modified:
                    // Never let an update move a row to a different tenant.
                    entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                    break;
            }
        }
    }
}
