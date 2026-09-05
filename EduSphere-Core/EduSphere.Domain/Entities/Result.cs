using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Result : TenantEntityBase
{
    public Guid ExamId { get; set; }
    public Exam? Exam { get; set; }

    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? BatchId { get; set; }
    public Batch? Batch { get; set; }

    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }

    public decimal TotalMarks { get; set; }
    public decimal MarksObtained { get; set; }
    public decimal Percentage { get; set; }

    [StringLength(20)]
    public string? Grade { get; set; }

    public decimal? GradePoint { get; set; }
    public ResultStatus Status { get; set; } = ResultStatus.Draft;
    public DateTime ComputedOn { get; set; } = DateTime.UtcNow;
    public Guid? PublishedByUserId { get; set; }
    public ApplicationUser? PublishedByUser { get; set; }
    public DateTime? PublishedOn { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }
}
