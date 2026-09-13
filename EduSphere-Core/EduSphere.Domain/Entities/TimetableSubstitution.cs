using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class TimetableSubstitution : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid TimetableEntryId { get; set; }
    public TimetableEntry? TimetableEntry { get; set; }

    public DateOnly SubstitutionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public Guid OriginalTeacherProfileId { get; set; }
    public TeacherProfile? OriginalTeacherProfile { get; set; }

    public Guid SubstituteTeacherProfileId { get; set; }
    public TeacherProfile? SubstituteTeacherProfile { get; set; }

    public Guid? RequestedByUserId { get; set; }
    public ApplicationUser? RequestedByUser { get; set; }

    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;

    public Guid? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }

    public DateTime? ReviewedOn { get; set; }

    public ApprovalStatus Status { get; set; } = ApprovalStatus.UnderReview;

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(500)]
    public string? ReviewNotes { get; set; }
}
