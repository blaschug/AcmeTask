namespace Enrollments.Application.Models;

public record CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}