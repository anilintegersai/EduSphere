using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class ProfileDocument : TenantEntityBase
{
    public ProfileDocumentOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }

    [Required]
    [StringLength(100)]
    public string DocumentType { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [StringLength(120)]
    public string? ContentType { get; set; }

    [Required]
    [StringLength(1000)]
    public string StoragePath { get; set; } = null!;

    public long? SizeBytes { get; set; }
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; }

    [StringLength(128)]
    public string? VerifiedBy { get; set; }

    public DateTime? VerifiedOn { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
