using Enrollment.Application.Common.Repositories;
using Enrollment.Application.Features.RegisterStudent;
using Enrollment.Domain.Entities;
using Enrollment.Domain.Exceptions;
using Enrollment.UnitTests.Builders;
using Enrollment.UnitTests.Helpers;
using Moq;

namespace Enrollment.UnitTests.Core;

public class RegisterStudent
{
    private RegisterStudentHandler Sut;
    private readonly Mock<IStudentRepository> _studentRepository;

    public RegisterStudent()
    {
        _studentRepository = new Mock<IStudentRepository>();
        Sut = new RegisterStudentHandler(_studentRepository.Object);
    }

    [Fact]
    public async Task HandleShouldReturnDtoIfRequestIsValid()
    {
        // Arrange
        var request = new RegisterStudentRequestBuilder().Build();
        var savedStudent = Student.Create(request.Name, request.BirthDate);
        _studentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Student>()))
            .ReturnsAsync(savedStudent);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<RegisterStudentDto>(result); // Asserting result type
        Assert.Equal(savedStudent.Id, result.Id);
        Assert.Equal(savedStudent.GetAge(), result.Age); // Asserting Age is well mapped
        Assert.Equal(savedStudent.Name, result.Name); // Asserting Name is well mapped
        _studentRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Student>()), Times.Once); //Assert repository is called once
    }
    
    [Fact]
    public async Task HandleShouldRiseInvalidNameExceptionIfRequestNameIsInvalid()
    {
        // Arrange
        var invalidName = string.Empty;
        var request = new RegisterStudentRequestBuilder().WithName(invalidName).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidNameException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _studentRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Student>()), Times.Never); //Assert repository is never called
    }    
    
    [Fact]
    public async Task HandleShouldRiseInvalidAgeExceptionIfRequestAgeIsInvalid()
    {
        // Arrange
        var invalidBirthDate = DatesHelper.GetBirthDateForAge(15);
        var request = new RegisterStudentRequestBuilder().WithBirthDay(invalidBirthDate).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidAgeException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _studentRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Student>()), Times.Never); //Assert repository is called once
    }
    
        
    [Fact]
    public async Task HandleShouldRiseExceptionIfRepositoryThrowsException()
    {
        // Arrange
        var request = new RegisterStudentRequestBuilder().Build();
        _studentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Student>()))
            .ThrowsAsync(new Exception());
        
        // Act
        await Assert.ThrowsAsync<Exception>(() => Sut.Handle(request));
        
        //Assert
        _studentRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Student>()), Times.Once); //Assert repository is never called
    }
}