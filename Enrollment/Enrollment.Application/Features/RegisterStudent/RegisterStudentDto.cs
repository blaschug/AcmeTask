namespace Enrollment.Application.Features.RegisterStudent;

public record RegisterStudentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int Age { get; init; }
}