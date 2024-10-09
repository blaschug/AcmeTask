using Enrollments.Domain.Constants;

namespace Enrollments.Domain.Exceptions;

public class InvalidAgeException : DomainException
{
    public InvalidAgeException() : base(DomainErrors.StudentNotAdult)
    {
    }
}