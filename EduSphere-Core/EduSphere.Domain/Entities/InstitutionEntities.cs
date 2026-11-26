using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public enum InstitutionKind { Unclassified, University, College, School, Coaching }
public enum OrganizationUnitKind { Institution, Trust, Campus, University, College, School, Department }
public enum FederationCapability { CurriculumDistribution, ResultSubmission }

public sealed class InstitutionProfile : TenantEntityBase
{
    [MaxLength(200)] public string DisplayName { get; set; } = "";
    public InstitutionKind Kind { get; set; }
    [MaxLength(100)] public string? Jurisdiction { get; set; }
}

public sealed class OrganizationUnit : TenantEntityBase
{
    public Guid? ParentId { get; set; }
    public Guid? BranchId { get; set; }
    [MaxLength(200)] public string Name { get; set; } = "";
    public OrganizationUnitKind Kind { get; set; }
}

public sealed class TenantAffiliation : TenantEntityBase
{
    public Guid AffiliateTenantId { get; set; }
    public FederationCapability Capability { get; set; }
    public DateTime? AcceptedOn { get; set; }
    public DateTime? RevokedOn { get; set; }
}

public sealed class OrganizationPermission : TenantEntityBase
{
    public Guid OrganizationUnitId { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(80)] public string Permission { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class InstitutionPolicyVersion : TenantEntityBase
{
    public Guid OrganizationUnitId { get; set; }
    [MaxLength(80)] public string PolicyKey { get; set; } = "";
    public int Version { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public string DefinitionJson { get; set; } = "{}";
}
