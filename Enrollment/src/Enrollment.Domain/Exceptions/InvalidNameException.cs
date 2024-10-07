using Enrollment.Domain.Constants;

namespace Enrollment.Domain.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException(Type entity, int minNameLenght)
        : base(Errors.InvalidName(entity, minNameLenght)) { }
}