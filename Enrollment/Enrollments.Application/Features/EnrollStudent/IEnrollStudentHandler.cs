namespace Enrollments.Application.Features.EnrollStudent;

public interface IEnrollStudentHandler
{
    Task<EnrollStudentDto> Handle(EnrollStudentRequest request);

}