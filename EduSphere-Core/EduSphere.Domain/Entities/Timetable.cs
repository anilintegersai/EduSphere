using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Timetable : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid SectionId { get; set; }
    public Section? Section { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EffectiveTo { get; set; }
    public TimetableStatus Status { get; set; } = TimetableStatus.Draft;

    public ICollection<TimetableEntry> Entries { get; set; } = new List<TimetableEntry>();
}
