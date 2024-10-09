using Enrollments.Application.Extensions;
using Enrollments.Domain.Entities;
using Enrollments.UnitTests.Helpers;

namespace Enrollments.UnitTests.Extensions;

public class MappersTests
{
    [Fact]
    public void StudentToDtoShouldMapStudentToStudentDto()
    {
        //Arrange
        var name = "Blas";
        var age = 25;
        var birthDate = DatesHelper.GetBirthDateForAge(age);
        var student = Student.Create(name, birthDate);
        
        //Act
        var result = student.ToDto();
        
        //Assert
        Assert.Equal(name, result.Name);
        Assert.Equal(age, result.Age);
    }
    
    [Fact]
    public void CourseToDtoShouldMapCourseToCourseDto()
    {
        //Arrange
        var name = "Course";
        var registrationFee = 0m;
        var startDate = DateTimeOffset.UtcNow.AddHours(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(3);
        var course = Course.Create(name, registrationFee, startDate, endDate);
        
        //Act
        var result = course.ToDto();
        
        //Assert
        Assert.Equal(name, result.Name);
    }
}