using Enrollments.Application.Common.Repositories;
using Enrollments.Application.Features.GetCourses;
using Enrollments.Domain.Entities;
using Enrollments.UnitTests.Helpers;
using Moq;

namespace Enrollments.UnitTests.Core;

public class GetCoursesTests
{
    private GetCoursesBetweenDatesBetweenDatesHandler Sut;
    private readonly Mock<ICourseRepository> _courseRepository;
    private List<Course> _fakeCourses = new List<Course>
    {
        Course.Create("Course", 10, DateTimeOffset.UtcNow.AddDays(10), DateTimeOffset.UtcNow.AddDays(20)),
        Course.Create("Course1", 0, DateTimeOffset.UtcNow.AddDays(20), DateTimeOffset.UtcNow.AddDays(30)),
        Course.Create("Course2", 10, DateTimeOffset.UtcNow.AddDays(30), DateTimeOffset.UtcNow.AddDays(40)),
        Course.Create("Course3", 10, DateTimeOffset.UtcNow.AddDays(40), DateTimeOffset.UtcNow.AddDays(50)),
        Course.Create("Course4", 10, DateTimeOffset.UtcNow.AddDays(50), DateTimeOffset.UtcNow.AddDays(60)),
        Course.Create("Course5", 10, DateTimeOffset.UtcNow.AddDays(60), DateTimeOffset.UtcNow.AddDays(70)),
        Course.Create("Course6", 10, DateTimeOffset.UtcNow.AddDays(70), DateTimeOffset.UtcNow.AddDays(80)),
        Course.Create("Course7", 10, DateTimeOffset.UtcNow.AddDays(80), DateTimeOffset.UtcNow.AddDays(90)),
        Course.Create("Course8", 10, DateTimeOffset.UtcNow.AddDays(90), DateTimeOffset.UtcNow.AddDays(100)),
        Course.Create("Course9", 10, DateTimeOffset.UtcNow.AddDays(100), DateTimeOffset.UtcNow.AddDays(110)),
    };

    public GetCoursesTests()
    {
        _courseRepository = new Mock<ICourseRepository>();
        Sut = new GetCoursesBetweenDatesBetweenDatesHandler(_courseRepository.Object);
        _fakeCourses.ForEach(x => x.AddEnrollment(
            Enrollment.Create(x, Student.Create("ValidStudent", DatesHelper.GetBirthDateForAge(20)))));
    }

    [Fact]
    public async Task HandleShouldRiseExceptionIfRepositoryThrowsException()
    {
        //Arrage
        var request = new GetCoursesBetweenDates(
            DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        _courseRepository.Setup(x => x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate))
            .ThrowsAsync(new Exception());
        
        //Act
        var result = await Assert.ThrowsAsync<Exception>(() => Sut.Handle(request));
        
        //Assert
        _courseRepository.Verify(x =>
            x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldReturnMultipleCoursesIfAreFoundBetweenDates()
    {
        //Arrage
        var request = new GetCoursesBetweenDates(
            DateTimeOffset.UtcNow.AddDays(11), DateTimeOffset.UtcNow.AddDays(50));
        _courseRepository.Setup(x => x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate))
            .ReturnsAsync(_fakeCourses
                .Where(x => x.StartDate > request.FromDate && x.EndDate < request.ToDate).ToList());
        
        //Act
        var result = await Sut.Handle(request);
        
        //Assert
        Assert.NotNull(request);
        Assert.Equal(3, result.Courses.Count);
        _courseRepository.Verify(x =>
            x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldReturnEmptyListIfNoCoursesAreFoundBetweenDates()
    {
        //Arrange
        var request = new GetCoursesBetweenDates(
            DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow.AddDays(5));
        _courseRepository.Setup(x => x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate))
            .ReturnsAsync(_fakeCourses
                .Where(x => x.StartDate > request.FromDate && x.EndDate < request.ToDate).ToList());
        
        //Act
        var result = await Sut.Handle(request);
        
        //Assert
        Assert.NotNull(request);
        Assert.Empty(result.Courses);
        _courseRepository.Verify(x =>
            x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldReturnEmptyForInvalidDateRange()
    {
        // Arrange
        var request = new GetCoursesBetweenDates(
            DateTimeOffset.UtcNow.AddDays(10), DateTimeOffset.UtcNow.AddDays(5));// EndDate is Older than StartDate
        _courseRepository.Setup(x =>
                x.GetCoursesWithEnrollmentsBetweenDatesAsync(request.FromDate, request.ToDate))
            .ReturnsAsync([]);
        
        // Act
        var result = await Sut.Handle(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Courses);
    }
}