namespace Enrollments.Application.Features.RegisterStudent;

public interface IRegisterStudentHandler
{
    Task<RegisterStudentDto> Handle(RegisterStudentRequest request);
}