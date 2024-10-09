namespace Enrollments.Application.Constants;

public static class ApplicationErrors
{
    public static string EntityNotFound(Type entity, Guid id) => $"{nameof(entity)} Not found by Id: {id}";
    public static string AlreadyEnrolled => "Student is already enrolled in this Course";
}