using Enrollments.Domain.Constants;

namespace Enrollments.Domain.Exceptions;

public class InvalidRegistrationFeeException : DomainException
{
    public InvalidRegistrationFeeException(
        ) : base(DomainErrors.InvalidRegistrationFee)
    {
    }
}