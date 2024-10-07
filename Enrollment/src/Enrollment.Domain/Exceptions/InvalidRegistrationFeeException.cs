using Enrollment.Domain.Constants;

namespace Enrollment.Domain.Exceptions;

public class InvalidRegistrationFeeException : DomainException
{
    public InvalidRegistrationFeeException(
        ) : base(Errors.InvalidRegistrationFee)
    {
    }
}