using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduSphere.Application.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BranchService> _logger;

    public BranchService(IUnitOfWork unitOfWork, ILogger<BranchService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Branch>> GetBranchesByTenantAsync(Guid tenantId)
    {
        return await _unitOfWork.BranchRepository.GetAllAsync(
            b => b.TenantId == tenantId
        );
    }

    public async Task<Branch?> GetBranchByIdAsync(Guid id)
    {
        return await _unitOfWork.BranchRepository.GetByIdAsync(id);
    }

    public async Task<Branch> CreateBranchAsync(
        Guid tenantId,
        string name,
        string code,
        string address,
        string city,
        string state,
        string country,
        string pincode)
    {
        var branch = new Branch
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            Address = address,
            City = city,
            State = state,
            Country = country,
            Pincode = pincode,
            IsActive = true
        };

        await _unitOfWork.BranchRepository.AddAsync(branch);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Branch created: {BranchName} for Tenant {TenantId}", name, tenantId);
        return branch;
    }

    public async Task<bool> UpdateBranchAsync(
        Guid id,
        string name,
        string code,
        string address,
        string city,
        string state,
        string country,
        string pincode)
    {
        var branch = await _unitOfWork.BranchRepository.GetByIdAsync(id);
        if (branch == null)
        {
            return false;
        }

        branch.Name = name;
        branch.Code = code;
        branch.Address = address;
        branch.City = city;
        branch.State = state;
        branch.Country = country;
        branch.Pincode = pincode;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Branch updated: {BranchId}", id);
        return true;
    }

    public async Task<bool> DeleteBranchAsync(Guid id)
    {
        var branch = await _unitOfWork.BranchRepository.GetByIdAsync(id);
        if (branch == null)
        {
            return false;
        }

        branch.IsDeleted = true;
        
        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Branch deleted: {BranchId}", id);
        return true;
    }
}
