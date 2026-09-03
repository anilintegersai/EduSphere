using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.Academic;

public abstract class TenantScopedDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

// ---- Academic year ----
public class AcademicYearDto : TenantScopedDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class CreateAcademicYearRequest
{
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class UpdateAcademicYearRequest : CreateAcademicYearRequest { }

// ---- Department ----
public class DepartmentDto : TenantScopedDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDepartmentRequest : CreateDepartmentRequest { }

// ---- Course ----
public class CourseDto : TenantScopedDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public int DurationMonths { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Description { get; set; }
}

public class CreateCourseRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public int DurationMonths { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Description { get; set; }
}

public class UpdateCourseRequest : CreateCourseRequest { }

// ---- Batch ----
public class BatchDto : TenantScopedDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid CourseId { get; set; }
    public Guid AcademicYearId { get; set; }
}

public class CreateBatchRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid CourseId { get; set; }
    public Guid AcademicYearId { get; set; }
}

public class UpdateBatchRequest : CreateBatchRequest { }

// ---- Section ----
public class SectionDto : TenantScopedDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid BatchId { get; set; }
    public Guid? ClassTeacherUserId { get; set; }
}

public class CreateSectionRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid BatchId { get; set; }
    public Guid? ClassTeacherUserId { get; set; }
}

public class UpdateSectionRequest : CreateSectionRequest { }

// ---- Subject ----
public class SubjectDto : TenantScopedDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectType Type { get; set; }
    public int Credits { get; set; }
    public Guid? CourseId { get; set; }
}

public class CreateSubjectRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectType Type { get; set; }
    public int Credits { get; set; }
    public Guid? CourseId { get; set; }
}

public class UpdateSubjectRequest : CreateSubjectRequest { }

// ---- Syllabus unit ----
public class SyllabusUnitDto : TenantScopedDto
{
    public Guid SubjectId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedHours { get; set; }
}

public class CreateSyllabusUnitRequest
{
    public Guid SubjectId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedHours { get; set; }
}

public class UpdateSyllabusUnitRequest
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedHours { get; set; }
}
