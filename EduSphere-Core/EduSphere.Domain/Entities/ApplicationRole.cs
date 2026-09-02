using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EduSphere.Domain.Entities;

/// <summary>
/// Application role backed by ASP.NET Core Identity. Roles are a global platform
/// lookup (SuperAdmin, TenantAdmin, Teacher, ...); tenant-scoping of access is
/// enforced by the tenant context and per-entity authorization, not by the role row.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(255)]
    public string? Description { get; set; }

    public ApplicationRole() { }

    public ApplicationRole(string roleName) : base(roleName) { }
}
