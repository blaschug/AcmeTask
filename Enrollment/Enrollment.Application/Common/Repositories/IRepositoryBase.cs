using Enrollment.Domain.Entities;

namespace Enrollment.Application.Common.Repositories;

public interface IRepositoryBase<T> where T : Entity
{
    Task<T> AddAndSaveAsync(T entity);
}