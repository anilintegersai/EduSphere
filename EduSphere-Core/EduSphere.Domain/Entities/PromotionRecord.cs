using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class PromotionRecord : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid FromAcademicYearId { get; set; }
    public AcademicYear? FromAcademicYear { get; set; }

    public Guid ToAcademicYearId { get; set; }
    public AcademicYear? ToAcademicYear { get; set; }

    public Guid? FromSectionId { get; set; }
    public Section? FromSection { get; set; }

    public Guid ToSectionId { get; set; }
    public Section? ToSection { get; set; }

    public DateOnly PromotionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [StringLength(500)]
    public string? Notes { get; set; }
}
