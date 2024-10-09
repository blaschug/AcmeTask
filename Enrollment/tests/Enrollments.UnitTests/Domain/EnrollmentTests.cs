using Enrollments.Domain.Entities;
using Enrollments.Domain.Enums;
using Enrollments.UnitTests.Helpers;

namespace Enrollments.UnitTests.Domain;

public class EnrollmentTests
{
    [Fact]
    public void CreateShouldThrowExceptionIfStudentIsNull()
    {
        // Arrange
        var student = null as Student;
        var course = Course.Create("Test", 0, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
        
        // Act
        var result = Assert.Throws<ArgumentNullException>(() => Enrollment.Create(course, student)); //Asserting exception is the right exception type
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public void CreateShouldThrowExceptionIfCoursetIsNull()
    {
        // Arrange
        var student = Student.Create("Test", DatesHelper.GetBirthDateForAge(18));
        var course = null as Course;
        
        // Act
        var result = Assert.Throws<ArgumentNullException>(() => Enrollment.Create(course, student)); //Asserting exception is the right exception type
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public void CreateShouldReturnEnrollmentWithPaymentStatusWaitingIfCourseRequiresPayment()
    {
        // Arrange
        var student = Student.Create("Test", DatesHelper.GetBirthDateForAge(18));
        var course = Course.Create("Test", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
        
        // Act
        var result = Enrollment.Create(course, student);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(course.IsPaymentRequired());
        Assert.Equal(PaymentStatus.WaitingForPayment, result.PaymentStatus);
    }
    
    [Fact]
    public void CreateShouldReturnEnrollmentWithPaymentStatusNotRequiredIfCourseNoRequiresPayment()
    {
        // Arrange
        var student = Student.Create("Test", DatesHelper.GetBirthDateForAge(18));
        var course = Course.Create("Test", 0, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
        
        // Act
        var result = Enrollment.Create(course, student);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(course.IsPaymentRequired());
        Assert.Equal(PaymentStatus.NotRequired, result.PaymentStatus);
    }
}