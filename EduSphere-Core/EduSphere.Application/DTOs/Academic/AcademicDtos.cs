using EduSphere.Domain.Enums;

namespace EduSphere.Application.DTOs.Academic;

// ---- Academic year ----
public class AcademicYearDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
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
public class DepartmentDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDepartmentRequest : CreateDepartmentRequest { }

// ---- Course ----
public class CourseDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public int DurationMonths { get; set; }
    public int? DepartmentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCourseRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public int DurationMonths { get; set; }
    public int? DepartmentId { get; set; }
    public string? Description { get; set; }
}

public class UpdateCourseRequest : CreateCourseRequest { }

// ---- Batch ----
public class BatchDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int CourseId { get; set; }
    public int AcademicYearId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBatchRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int CourseId { get; set; }
    public int AcademicYearId { get; set; }
}

public class UpdateBatchRequest : CreateBatchRequest { }

// ---- Section ----
public class SectionDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int BatchId { get; set; }
    public Guid? ClassTeacherUserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSectionRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int BatchId { get; set; }
    public Guid? ClassTeacherUserId { get; set; }
}

public class UpdateSectionRequest : CreateSectionRequest { }

// ---- Subject ----
public class SubjectDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectType Type { get; set; }
    public int Credits { get; set; }
    public int? CourseId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSubjectRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectType Type { get; set; }
    public int Credits { get; set; }
    public int? CourseId { get; set; }
}

public class UpdateSubjectRequest : CreateSubjectRequest { }

// ---- Syllabus unit ----
public class SyllabusUnitDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int SubjectId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedHours { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSyllabusUnitRequest
{
    public int SubjectId { get; set; }
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
