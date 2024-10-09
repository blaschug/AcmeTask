namespace Enrollments.Application.Features.RegisterStudent;

public record RegisterStudentRequest(
    string Name,
    DateOnly BirthDate);
