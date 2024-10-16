using Enrollments.Domain.Entities;
using Enrollments.Application.Common.Repositories;

namespace Enrollments.Application.Features.RegisterStudent;

public class RegisterStudentHandler : IRegisterStudentHandler
{
    private readonly IStudentRepository _studentRepository;

    public RegisterStudentHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<RegisterStudentDto> Handle(RegisterStudentRequest request)
    {
        var student = Student.Create(name: request.Name, birthDate: request.BirthDate);

        var savedEntity = await _studentRepository.AddAndSaveAsync(student);

        return new RegisterStudentDto
        {
            Id = savedEntity.Id,
            Name = savedEntity.Name,
            Age = savedEntity.GetAge()
        };
    }
}