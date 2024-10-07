using Enrollment.Domain.Constants;

namespace Enrollment.Domain.Exceptions;

public class InvalidAgeException : DomainException
{
    public InvalidAgeException() : base(Errors.StudentNotAdult)
    {
    }
}