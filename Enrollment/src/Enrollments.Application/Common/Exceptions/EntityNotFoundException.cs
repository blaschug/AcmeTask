using Enrollments.Application.Constants;

namespace Enrollments.Application.Common.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(Type entity, Guid id) : base(
        ApplicationErrors.EntityNotFound(entity, id))
    {
        
    }
}