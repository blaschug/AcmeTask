using Enrollments.Domain.Entities;

namespace Enrollments.Application.Common.Repositories;

public interface IRepositoryBase<T> where T : Entity
{
    Task<T> AddAndSaveAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
}