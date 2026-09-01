using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IBranchService
{
    Task<IEnumerable<Branch>> GetBranchesByTenantAsync(int tenantId);
    Task<Branch?> GetBranchByIdAsync(int id);
    Task<Branch> CreateBranchAsync(int tenantId, string name, string code, string address, string city, string state, string country, string pincode);
    Task<bool> UpdateBranchAsync(int id, string name, string code, string address, string city, string state, string country, string pincode);
    Task<bool> DeleteBranchAsync(int id);
}
