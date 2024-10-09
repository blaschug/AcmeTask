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

    public static CourseWithRelationsDto ToDtoWithRelations(this Course course) => 
        new CourseWithRelationsDto
        {
            Id = course.Id,
            Name = course.Name,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            Students = course.Enrollments
                .Select(e => e.Student.ToDto())
                .ToList()
        };
}