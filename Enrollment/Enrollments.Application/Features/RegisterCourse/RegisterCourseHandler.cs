using Enrollments.Domain.Entities;
using Enrollments.Application.Common.Repositories;

namespace Enrollments.Application.Features.RegisterCourse;

public class RegisterCourseHandler : IRegisterCourseHandler
{
    private readonly ICourseRepository _courseRepository;

    public RegisterCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<RegisterCourseDto> Handle(RegisterCourseRequest request)
    {
        var course = Course.Create(
            name: request.Name,
            registrationFee: request.RegistrationFee,
            startDate: request.StartDate,
            endDate: request.EndDate);

        var savedCourse = await _courseRepository.AddAndSaveAsync(course);

        return new RegisterCourseDto
        {
            Id = savedCourse.Id,
            Name = savedCourse.Name,
            RegistrationFee = savedCourse.RegistrationFee,
            StartDate = savedCourse.StartDate,
            EndDate = savedCourse.EndDate
        };
    }
}