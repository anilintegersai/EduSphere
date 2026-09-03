using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IBranchService
{
    Task<IEnumerable<Branch>> GetBranchesByTenantAsync(Guid tenantId);
    Task<Branch?> GetBranchByIdAsync(Guid id);
    Task<Branch> CreateBranchAsync(Guid tenantId, string name, string code, string address, string city, string state, string country, string pincode);
    Task<bool> UpdateBranchAsync(Guid id, string name, string code, string address, string city, string state, string country, string pincode);
    Task<bool> DeleteBranchAsync(Guid id);
}
