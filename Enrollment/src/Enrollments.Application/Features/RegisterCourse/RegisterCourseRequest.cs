namespace Enrollments.Application.Features.RegisterCourse;

public record RegisterCourseRequest(
    string Name,
    decimal RegistrationFee,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
