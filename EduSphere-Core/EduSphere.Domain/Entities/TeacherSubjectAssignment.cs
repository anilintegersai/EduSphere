using EduSphere.Domain.Common;

namespace EduSphere.Domain.Entities;

public class TeacherSubjectAssignment : TenantEntityBase
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid? SectionId { get; set; }
    public Section? Section { get; set; }

    public bool IsPrimary { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveUntil { get; set; }
}
