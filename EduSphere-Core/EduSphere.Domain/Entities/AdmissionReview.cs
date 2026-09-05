using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionReview : TenantEntityBase
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionApplication? AdmissionApplication { get; set; }

    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }

    public AdmissionApplicationStatus FromStatus { get; set; }
    public AdmissionApplicationStatus ToStatus { get; set; }
    public DateTime ReviewedOn { get; set; } = DateTime.UtcNow;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
