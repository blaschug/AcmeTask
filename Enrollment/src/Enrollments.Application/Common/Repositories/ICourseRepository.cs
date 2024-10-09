using Enrollments.Domain.Entities;

namespace Enrollments.Application.Common.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<List<Course>> GetCoursesWithEnrollmentsBetweenDatesAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}