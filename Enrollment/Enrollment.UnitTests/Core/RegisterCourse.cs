using Enrollment.Application.Common.Repositories;
using Enrollment.Application.Features.RegisterCourse;
using Enrollment.Domain.Entities;
using Enrollment.Domain.Exceptions;
using Enrollment.UnitTests.Builders;
using Moq;

namespace Enrollment.UnitTests.Core;

public class RegisterCourse
{
    private RegisterCourseHandler Sut;
    private readonly Mock<ICourseRepository> _courseRepository;

    public RegisterCourse()
    {
        _courseRepository = new Mock<ICourseRepository>();
        Sut = new RegisterCourseHandler(_courseRepository.Object);
    }

    [Fact]
    public async Task HandleShouldReturnDtoIfRequestIsValid()
    {
        // Arrange
        var request = new RegisterCourseRequestBuilder().Build();
        var savedCourse = Course.Create(request.Name, request.RegistrationFee, request.StartDate, request.EndDate);
        _courseRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Course>()))
            .ReturnsAsync(savedCourse);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<RegisterCourseDto>(result); // Asserting result type
        Assert.Equal(savedCourse.Id, result.Id); // Asserting Id is well mapped
        Assert.Equal(savedCourse.Name, result.Name); // Asserting Name is well mapped
        Assert.Equal(savedCourse.StartDate, result.StartDate); // Asserting StartDate is well mapped
        Assert.Equal(savedCourse.EndDate, result.EndDate); // Asserting EndDate is well mapped
        Assert.Equal(savedCourse.RegistrationFee, result.RegistrationFee); // Asserting Name is well mapped
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Once); //Assert repository is called once
    }
    
    [Fact]
    public async Task HandleShouldRiseInvalidNameExceptionIfRequestNameIsInvalid()
    {
        // Arrange
        var invalidName = "bl";
        var request = new RegisterCourseRequestBuilder().WithName(invalidName).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidNameException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Never); //Assert repository is never called
    }    
    
    [Fact]
    public async Task HandleShouldRiseInvalidRegistrationFeeExceptionIfRequestFeeIsNegative()
    {
        // Arrange
        var invalidRegistrationFee = -1m;
        var request = new RegisterCourseRequestBuilder().WithRegistrationFee(invalidRegistrationFee).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidRegistrationFeeException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Never); //Assert repository is never called
    }
    
    [Fact]
    public async Task HandleShouldRiseInvalidCourseDateExceptionIfStarDateIsOlderThanNow()
    {
        // Arrange
        var invalidStartDate = DateTimeOffset.UtcNow.AddMinutes(-1);
        var request = new RegisterCourseRequestBuilder().WithStartDate(invalidStartDate).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidCourseDateException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Never); //Assert repository is never called
    }
    
    [Fact]
    public async Task HandleShouldRiseInvalidCourseDateExceptionIfEndDateIsOlderThanStarDate()
    {
        // Arrange
        var validStartDate = DateTimeOffset.UtcNow.AddHours(5);
        var olderEndDate = DateTimeOffset.UtcNow;
        var request = new RegisterCourseRequestBuilder()
            .WithStartDate(validStartDate)
            .WithEndDate(olderEndDate).Build();
        
        // Act
        var act = await Assert.ThrowsAsync<InvalidCourseDateException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Never); //Assert repository is called once
    }
    
        
    [Fact]
    public async Task HandleShouldRiseExceptionIfRepositoryThrowsException()
    {
        // Arrange
        var request = new RegisterCourseRequestBuilder().Build();
        _courseRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Course>()))
            .ThrowsAsync(new Exception());
        
        // Act
        await Assert.ThrowsAsync<Exception>(() => Sut.Handle(request));
        
        //Assert
        _courseRepository.Verify(x => 
            x.AddAndSaveAsync(It.IsAny<Course>()), Times.Once); //Assert repository is never called
    }
}