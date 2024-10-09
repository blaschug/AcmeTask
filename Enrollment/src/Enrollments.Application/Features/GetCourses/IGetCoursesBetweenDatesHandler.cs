namespace Enrollments.Application.Features.GetCourses;

public interface IGetCoursesBetweenDatesHandler
{
    Task<GetCoursesDto> Handle(GetCoursesBetweenDates betweenDates);
}