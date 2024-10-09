using Enrollments.Application.Common.Repositories;
using Enrollments.Application.Extensions;

namespace Enrollments.Application.Features.GetCourses;

public class GetCoursesBetweenDatesBetweenDatesHandler : IGetCoursesBetweenDatesHandler
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesBetweenDatesBetweenDatesHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<GetCoursesDto> Handle(GetCoursesBetweenDates betweenDates)
    {
        var courses = 
            await _courseRepository.GetCoursesWithEnrollmentsBetweenDatesAsync(betweenDates.FromDate, betweenDates.ToDate);

        return new GetCoursesDto
        {
            Courses = courses
                .Select(x => x.ToDtoWithRelations())
                .ToList()
        };
    }
}