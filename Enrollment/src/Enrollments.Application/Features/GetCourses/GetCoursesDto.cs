using Enrollments.Application.Models;

namespace Enrollments.Application.Features.GetCourses;

public record GetCoursesDto
{
    public List<CourseWithRelationsDto> Courses { get; init; }
}