using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class StudentMedicalProfile : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    [StringLength(20)] public string? BloodGroup { get; set; }
    [StringLength(2000)] public string? Allergies { get; set; }
    [StringLength(2000)] public string? CurrentMedications { get; set; }
    [StringLength(2000)] public string? EmergencyInstructions { get; set; }
    [StringLength(150)] public string? PhysicianName { get; set; }
    [StringLength(30)] public string? PhysicianPhone { get; set; }
    [StringLength(100)] public string? InsuranceProvider { get; set; }
    [StringLength(100)] public string? InsurancePolicyNumber { get; set; }
    public bool EmergencyCareConsent { get; set; }
    public DateOnly? LastReviewedOn { get; set; }
    public ICollection<StudentMedicalCondition> Conditions { get; set; } = new List<StudentMedicalCondition>();
}

public class StudentMedicalCondition : TenantEntityBase
{
    public Guid StudentMedicalProfileId { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    [StringLength(50)] public string? Severity { get; set; }
    public DateOnly? DiagnosedOn { get; set; }
    public DateOnly? ResolvedOn { get; set; }
    [StringLength(1000)] public string? CareInstructions { get; set; }
}

public class ProfileStatusHistory : TenantEntityBase
{
    public ProfileStatusOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }
    [StringLength(50)] public string? PreviousStatus { get; set; }
    [Required, StringLength(50)] public string Status { get; set; } = null!;
    public DateOnly EffectiveOn { get; set; }
    [Required, StringLength(80)] public string Source { get; set; } = null!;
    public Guid? SourceEntityId { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
    [StringLength(128)] public string? RecordedBy { get; set; }
}

public class LifecycleDocumentTemplate : TenantEntityBase
{
    [Required, StringLength(60)] public string Code { get; set; } = null!;
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    public ProfileStatusOwnerType OwnerType { get; set; }
    [Required, StringLength(100)] public string DocumentType { get; set; } = null!;
    [Required] public string HtmlTemplate { get; set; } = null!;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool RequiresSignature { get; set; } = true;
}

public class StudentTranscript : TenantEntityBase
{
    public Guid StudentProfileId { get; set; }
    public Guid? AcademicYearId { get; set; }
    public Guid? CourseId { get; set; }
    [Required, StringLength(50)] public string TranscriptNumber { get; set; } = null!;
    public DateOnly IssuedOn { get; set; }
    public decimal? AggregatePercentage { get; set; }
    [StringLength(20)] public string? FinalGrade { get; set; }
    public bool IsFinal { get; set; }
    public ICollection<StudentTranscriptLine> Lines { get; set; } = new List<StudentTranscriptLine>();
}

public class StudentTranscriptLine : TenantEntityBase
{
    public Guid StudentTranscriptId { get; set; }
    [Required, StringLength(150)] public string AssessmentName { get; set; } = null!;
    public decimal MaximumMarks { get; set; }
    public decimal MarksObtained { get; set; }
    public decimal Percentage { get; set; }
    [StringLength(20)] public string? Grade { get; set; }
    public bool IsPassed { get; set; }
}

public class AlumniEngagement : TenantEntityBase
{
    public Guid StudentAlumniRecordId { get; set; }
    public AlumniEngagementType Type { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = null!;
    public DateTime OccurredOn { get; set; }
    [StringLength(1000)] public string? Notes { get; set; }
    public decimal? ContributionAmount { get; set; }
}

public class BulkStudentLifecycleRun : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Guid FromAcademicYearId { get; set; }
    public Guid ToAcademicYearId { get; set; }
    public Guid? FromSectionId { get; set; }
    public Guid ToSectionId { get; set; }
    public BulkLifecycleRunStatus Status { get; set; }
    public int TotalCount { get; set; }
    public int SucceededCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    [StringLength(128)] public string? RunBy { get; set; }
    public ICollection<BulkStudentLifecycleRow> Rows { get; set; } = new List<BulkStudentLifecycleRow>();
}

public class BulkStudentLifecycleRow : TenantEntityBase
{
    public Guid BulkStudentLifecycleRunId { get; set; }
    public Guid StudentProfileId { get; set; }
    public bool Succeeded { get; set; }
    [StringLength(1000)] public string? Message { get; set; }
}

public class ProfileDocumentPolicy : TenantEntityBase
{
    public ProfileDocumentOwnerType OwnerType { get; set; }
    [Required, StringLength(100)] public string DocumentType { get; set; } = null!;
    [StringLength(300)] public string AllowedExtensions { get; set; } = ".pdf,.png,.jpg,.jpeg,.doc,.docx";
    public long MaximumBytes { get; set; } = 10 * 1024 * 1024;
    public bool RequiresVirusScan { get; set; } = true;
    public bool RequiresVerification { get; set; } = true;
    public int? RetentionDays { get; set; }
    public int? ExpiryDays { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DocumentApprovalSignature : TenantEntityBase
{
    public Guid? ProfileDocumentId { get; set; }
    public Guid? LifecycleDocumentTemplateId { get; set; }
    public Guid SubjectId { get; set; }
    [Required, StringLength(150)] public string SignerName { get; set; } = null!;
    [StringLength(150)] public string? SignerTitle { get; set; }
    public DateTime SignedOn { get; set; }
    [Required, StringLength(128)] public string SignatureHash { get; set; } = null!;
    public SignatureStatus Status { get; set; } = SignatureStatus.Signed;
}
