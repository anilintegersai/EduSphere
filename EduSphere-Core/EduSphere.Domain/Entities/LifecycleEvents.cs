using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class StudentLifecycleEvent : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public StudentLifecycleEventType EventType { get; set; } = StudentLifecycleEventType.ProfileCreated;
    public StudentStatus FromStatus { get; set; } = StudentStatus.Active;
    public StudentStatus ToStatus { get; set; } = StudentStatus.Active;

    public Guid? FromBranchId { get; set; }
    public Branch? FromBranch { get; set; }

    public Guid? ToBranchId { get; set; }
    public Branch? ToBranch { get; set; }

    public Guid? FromAcademicYearId { get; set; }
    public AcademicYear? FromAcademicYear { get; set; }

    public Guid? ToAcademicYearId { get; set; }
    public AcademicYear? ToAcademicYear { get; set; }

    public Guid? FromCourseId { get; set; }
    public Course? FromCourse { get; set; }

    public Guid? ToCourseId { get; set; }
    public Course? ToCourse { get; set; }

    public Guid? FromBatchId { get; set; }
    public Batch? FromBatch { get; set; }

    public Guid? ToBatchId { get; set; }
    public Batch? ToBatch { get; set; }

    public Guid? FromSectionId { get; set; }
    public Section? FromSection { get; set; }

    public Guid? ToSectionId { get; set; }
    public Section? ToSection { get; set; }

    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTime RecordedOn { get; set; } = DateTime.UtcNow;

    public Guid? RecordedByUserId { get; set; }
    public ApplicationUser? RecordedByUser { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class StudentLifecycleRequest : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public StudentLifecycleEventType EventType { get; set; } = StudentLifecycleEventType.Promoted;
    public StudentStatus ToStatus { get; set; } = StudentStatus.Active;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;

    public Guid? ToBranchId { get; set; }
    public Branch? ToBranch { get; set; }

    public Guid? ToAcademicYearId { get; set; }
    public AcademicYear? ToAcademicYear { get; set; }

    public Guid? ToCourseId { get; set; }
    public Course? ToCourse { get; set; }

    public Guid? ToBatchId { get; set; }
    public Batch? ToBatch { get; set; }

    public Guid? ToSectionId { get; set; }
    public Section? ToSection { get; set; }

    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }

    public DateTime? DecidedOn { get; set; }

    public Guid? DecidedByUserId { get; set; }
    public ApplicationUser? DecidedByUser { get; set; }

    public Guid? AppliedStudentLifecycleEventId { get; set; }
    public StudentLifecycleEvent? AppliedStudentLifecycleEvent { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? DecisionNotes { get; set; }
}

public class TeacherLifecycleEvent : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public TeacherLifecycleEventType EventType { get; set; } = TeacherLifecycleEventType.ProfileCreated;
    public TeacherStatus FromStatus { get; set; } = TeacherStatus.Active;
    public TeacherStatus ToStatus { get; set; } = TeacherStatus.Active;

    public Guid? FromBranchId { get; set; }
    public Branch? FromBranch { get; set; }

    public Guid? ToBranchId { get; set; }
    public Branch? ToBranch { get; set; }

    public Guid? FromDepartmentId { get; set; }
    public Department? FromDepartment { get; set; }

    public Guid? ToDepartmentId { get; set; }
    public Department? ToDepartment { get; set; }

    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTime RecordedOn { get; set; } = DateTime.UtcNow;

    public Guid? RecordedByUserId { get; set; }
    public ApplicationUser? RecordedByUser { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class TeacherLifecycleRequest : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public TeacherLifecycleEventType EventType { get; set; } = TeacherLifecycleEventType.DepartmentTransfer;
    public TeacherStatus ToStatus { get; set; } = TeacherStatus.Active;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;

    public Guid? ToBranchId { get; set; }
    public Branch? ToBranch { get; set; }

    public Guid? ToDepartmentId { get; set; }
    public Department? ToDepartment { get; set; }

    public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }

    public DateTime? DecidedOn { get; set; }

    public Guid? DecidedByUserId { get; set; }
    public ApplicationUser? DecidedByUser { get; set; }

    public Guid? AppliedTeacherLifecycleEventId { get; set; }
    public TeacherLifecycleEvent? AppliedTeacherLifecycleEvent { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? DecisionNotes { get; set; }
}
