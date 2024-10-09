namespace Enrollments.Application.Features.RegisterCourse;

public interface IRegisterCourseHandler
{
    Task<RegisterCourseDto> Handle(RegisterCourseRequest request);
}