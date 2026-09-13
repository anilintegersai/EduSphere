using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class AdmissionFormTemplate : TenantEntityBase
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid? AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(1000)]
    public string? Instructions { get; set; }

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

    public ICollection<AdmissionFormField> Fields { get; set; } = new List<AdmissionFormField>();
    public ICollection<AdmissionDocumentRequirement> DocumentRequirements { get; set; } = new List<AdmissionDocumentRequirement>();
}

public class AdmissionFormField : TenantEntityBase
{
    public Guid AdmissionFormTemplateId { get; set; }
    public AdmissionFormTemplate? AdmissionFormTemplate { get; set; }

    [Required]
    [StringLength(80)]
    public string FieldKey { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Label { get; set; } = null!;

    public AdmissionFormFieldType FieldType { get; set; } = AdmissionFormFieldType.Text;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }

    [StringLength(200)]
    public string? Placeholder { get; set; }

    [StringLength(500)]
    public string? HelpText { get; set; }

    [StringLength(2000)]
    public string? OptionsJson { get; set; }

    [StringLength(200)]
    public string? ValidationRegex { get; set; }

    public int? MaxLength { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdmissionDocumentRequirement : TenantEntityBase
{
    public Guid AdmissionFormTemplateId { get; set; }
    public AdmissionFormTemplate? AdmissionFormTemplate { get; set; }

    public AdmissionDocumentType DocumentType { get; set; } = AdmissionDocumentType.Other;

    [Required]
    [StringLength(150)]
    public string DisplayName { get; set; } = null!;

    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}
