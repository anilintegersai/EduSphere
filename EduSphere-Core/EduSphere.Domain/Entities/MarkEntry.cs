using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class MarkEntry : TenantEntityBase
{
    public Guid ExamScheduleId { get; set; }
    public ExamSchedule? ExamSchedule { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public decimal MarksObtained { get; set; }
    public bool IsAbsent { get; set; }

    [StringLength(20)]
    public string? Grade { get; set; }

    public decimal? GradePoint { get; set; }
    public Guid? EnteredByUserId { get; set; }
    public ApplicationUser? EnteredByUser { get; set; }
    public DateTime EnteredOn { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Remarks { get; set; }
}
