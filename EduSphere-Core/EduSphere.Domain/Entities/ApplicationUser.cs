using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduSphere.Domain.Common;
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
public class ApplicationUser : IdentityUser<Guid>, ITenantEntity, IAuditableEntity, IConcurrencyTrackedEntity
{
    public Guid TenantId { get; set; }

    public Guid? BranchId { get; set; }
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

    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();
    public DateTime? LastLoginAt { get; set; }
}
