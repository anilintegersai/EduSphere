using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionInterview : TenantEntityBase
{
    public Guid AdmissionApplicationId { get; set; }
    public AdmissionApplication? AdmissionApplication { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? InterviewerUserId { get; set; }
    public ApplicationUser? InterviewerUser { get; set; }

    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public AdmissionInterviewStatus Status { get; set; } = AdmissionInterviewStatus.Scheduled;

    [StringLength(180)]
    public string? Location { get; set; }

    [StringLength(500)]
    public string? MeetingLink { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? OutcomeNotes { get; set; }
}
