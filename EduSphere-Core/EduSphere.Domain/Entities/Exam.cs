using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Exam : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    public ExamType Type { get; set; } = ExamType.UnitTest;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal WeightagePercentage { get; set; } = 100;
    public ExamStatus Status { get; set; } = ExamStatus.Draft;

    [StringLength(1000)]
    public string? Instructions { get; set; }

    public ICollection<ExamSchedule> Schedules { get; set; } = new List<ExamSchedule>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
}
