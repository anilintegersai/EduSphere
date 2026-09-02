using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Application user backed by ASP.NET Core Identity. Inherits Email, UserName,
/// PasswordHash, PhoneNumber, security stamps, lockout and 2FA support from
/// <see cref="IdentityUser{TKey}"/>. Tenant-owned (carries <see cref="TenantId"/>
/// and is stamped on write), but intentionally excluded from the global tenant
/// query filter so Identity can resolve users during authentication.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>, ITenantEntity
{
    public int TenantId { get; set; }

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [PersonalData]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [PersonalData]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [NotMapped]
    public string FullName =>
        string.Join(' ', new[] { FirstName, MiddleName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));

    public UserType UserType { get; set; } = UserType.Student;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
