namespace Enrollment.Application.Features.RegisterStudent;

public record RegisterStudentRequest(
    string Name,
    DateOnly BirthDate);
