using System.Linq.Expressions;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Interfaces;

namespace EduSphere.Application.Services;

public class CrudService<T> : ICrudService<T>
    where T : class, IGuidEntity, ISoftDeletable
{
    private readonly IGenericRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CrudService(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
        => (await _repository.GetAllAsync(predicate)).ToList();

    public Task<T?> GetAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<T> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(Guid id, Action<T> apply)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        apply(entity);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.IsDeleted = true;
        await _unitOfWork.CommitAsync();
        return true;
    }
}
