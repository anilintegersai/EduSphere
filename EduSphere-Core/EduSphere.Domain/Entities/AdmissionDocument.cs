using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionDocument : TenantEntityBase
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionApplication? AdmissionApplication { get; set; }

    public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;

    [Required]
    [StringLength(150)]
    public string DisplayName { get; set; } = null!;

    [StringLength(255)]
    public string? FileName { get; set; }

    [StringLength(100)]
    public string? ContentType { get; set; }

    [StringLength(500)]
    public string? StoragePath { get; set; }

    public bool IsVerified { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public ApplicationUser? VerifiedByUser { get; set; }
    public DateTime? VerifiedOn { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
