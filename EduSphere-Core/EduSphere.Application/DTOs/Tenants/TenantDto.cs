namespace EduSphere.Application.DTOs.Tenants;

public class TenantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TenantIdentifier { get; set; } = string.Empty;
    public string? CustomDomain { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string TenantIdentifier { get; set; } = string.Empty;
    public string? CustomDomain { get; set; }
    public string? Description { get; set; }
}

public class UpdateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
