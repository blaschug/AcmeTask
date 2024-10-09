using Enrollments.Application.Constants;

namespace Enrollments.Application.Common.Exceptions;

public class AlreadyEnrolledException: ApplicationException
{
    public AlreadyEnrolledException() : base(
        ApplicationErrors.AlreadyEnrolled)
    {
        
    }
}