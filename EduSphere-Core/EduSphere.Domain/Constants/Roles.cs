namespace EduSphere.Domain.Constants;

/// <summary>
/// Canonical platform role names and their stable seed identifiers. Kept as
/// constants so authorization policies, seeding and the model seed all agree.
/// </summary>
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string TenantAdmin = "TenantAdmin";
    public const string Principal = "Principal";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";

    public static readonly IReadOnlyDictionary<string, Guid> SeedIds = new Dictionary<string, Guid>
    {
        [SuperAdmin] = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        [TenantAdmin] = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        [Principal] = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        [Teacher] = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        [Student] = Guid.Parse("55555555-5555-5555-5555-555555555555"),
        [Parent] = Guid.Parse("66666666-6666-6666-6666-666666666666"),
    };
}
