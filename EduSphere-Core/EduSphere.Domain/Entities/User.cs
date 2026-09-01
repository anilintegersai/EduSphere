using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduSphere.Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!; // Always store hashed passwords

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Foreign key to Branch
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    // Foreign key to Role
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
