using Enrollments.Domain.Constants;

namespace Enrollments.Domain.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException(Type entity, int minNameLenght)
        : base(DomainErrors.InvalidName(entity, minNameLenght)) { }
}