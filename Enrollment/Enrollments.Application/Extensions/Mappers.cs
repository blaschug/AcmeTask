using Enrollments.Application.Models;
using Enrollments.Domain.Entities;

namespace Enrollments.Application.Extensions;

public static class Mappers
{
    public static StudentDto ToDto(this Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Age = student.GetAge()
        };
    }
    
    public static CourseDto ToDto(this Course student)
    {
        return new CourseDto
        {
            Id = student.Id,
            Name = student.Name
        };
    }
}