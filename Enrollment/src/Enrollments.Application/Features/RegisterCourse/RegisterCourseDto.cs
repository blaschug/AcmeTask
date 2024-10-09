namespace Enrollments.Application.Features.RegisterCourse;

public record RegisterCourseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal RegistrationFee { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
};