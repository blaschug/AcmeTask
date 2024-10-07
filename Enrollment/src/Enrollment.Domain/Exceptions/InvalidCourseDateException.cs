namespace Enrollment.Domain.Exceptions;

public class InvalidCourseDateException : DomainException
{
    public InvalidCourseDateException(string message) : base(message)
    {
    }
}