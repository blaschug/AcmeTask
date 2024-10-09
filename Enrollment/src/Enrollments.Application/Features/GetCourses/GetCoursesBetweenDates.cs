namespace Enrollments.Application.Features.GetCourses;

public record GetCoursesBetweenDates(
    DateTimeOffset FromDate,
    DateTimeOffset ToDate);