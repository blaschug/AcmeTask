namespace Enrollments.Application.Models;

public record StudentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int Age { get; init; }
}    
