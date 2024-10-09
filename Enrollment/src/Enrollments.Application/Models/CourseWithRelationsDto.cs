namespace Enrollments.Application.Models;

public record CourseWithRelationsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public IReadOnlyCollection<StudentDto> Students { get; init; }
}