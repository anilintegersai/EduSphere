namespace EduSphere.Domain.Enums;

/// <summary>
/// Coarse classification of an application user, independent of Identity role
/// membership. Roles drive authorization; UserType drives which profile/domain
/// records the user is linked to (teacher, student, guardian, staff).
/// </summary>
public enum UserType
{
    SuperAdmin = 0,
    TenantAdmin = 1,
    Principal = 2,
    HeadOfDepartment = 3,
    Teacher = 4,
    Student = 5,
    Parent = 6,
    Accountant = 7,
    Librarian = 8,
    Staff = 9
}
