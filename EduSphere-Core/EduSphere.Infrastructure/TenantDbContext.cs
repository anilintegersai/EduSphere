using EduSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Infrastructure;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Tenant -> Branch relationship
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Tenant)
            .WithMany(t => t.Branches)
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User -> Branch and User -> Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Branch)
            .WithMany()
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if Branch is deleted

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "System Administrator", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 2, Name = "Teacher", Description = "Class Teacher", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = "Student", Description = "Student User", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 4, Name = "Parent", Description = "Parent/Guardian", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
    }
}