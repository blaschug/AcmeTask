using Enrollment.Domain.Constants;
using Enrollment.Domain.Entities;
using Enrollment.Domain.Exceptions;
using Enrollment.UnitTests.Helpers;

namespace Enrollment.UnitTests.Domain;

public class CourseTests
{
    private const string ValidName = "Course";
    private const decimal ValidFee = 10;
    private static readonly DateTimeOffset ValidStartDate = DateTimeOffset.UtcNow.AddMinutes(1);
    private static readonly DateTimeOffset ValidEndDate = DateTimeOffset.UtcNow.AddDays(3);
    
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    [InlineData("     ")]
    [InlineData("  a   ")]
    [InlineData("  aq   ")]
    public void CreateShouldThrowInvalidNameExceptionIfNameIsInvalid(string invalidName)
    {
        // Act
        var result = Assert.Throws<InvalidNameException>(() => 
            Course.Create(invalidName, ValidFee, ValidStartDate, ValidEndDate)); //Asserting exception is the right exception type

        //Assert
        Assert.NotNull(result);
        // Maybe MinNameLenght could vary in the future.
        Assert.Equal(Errors.InvalidName(typeof(Course), 3), result.Message); // Asserting exception message is the correct one
    }
    
    [Theory]
    [InlineData("Val")]
    [InlineData("Valid")]
    [InlineData("Valid Name")]
    [InlineData("Yes, another valid name")]
    public void CreateCourseWithValidNameShouldReturnCourse(string validName)
    {
        // Act
        var result = Course.Create(validName, ValidFee, ValidStartDate, ValidEndDate);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(validName, result.Name); // Asserting name is equal
    }
    
    [Fact]
    public void CreateShouldReturnExceptionIfRegistrationFeeIsNegative()
    {
        // Arrange
        var invalidFee = -10m;
        
        // Act
        var result = Assert.Throws<InvalidRegistrationFeeException>(() =>
            Course.Create(ValidName, invalidFee, ValidStartDate, ValidEndDate)); //Asserting exception is the right exception type
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(Errors.InvalidRegistrationFee, result.Message); // Asserting exception message is the correct one
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void CreateCourseWithValidRgistrationFeeShouldReturnCourse(decimal validFee)
    {
        // Act
        var result =  Course.Create(ValidName, validFee, ValidStartDate, ValidEndDate);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(validFee, result.RegistrationFee); // Asserting RegistrationFee is equal
    }
    
        
    [Fact]
    public void CreateCourseWithStartDateInPastShouldReturnError()
    {
        // Arrange
        var invalidStartDate = DatesHelper.SubstractInMinutes(ValidStartDate, 10);
        
        // Act
        var result = Assert.Throws<InvalidCourseDateException>(() =>
            Course.Create(ValidName, ValidFee, invalidStartDate, ValidEndDate));
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(Errors.StartCourseInPast, result.Message); // Asserting that error message is the correct one.
    }
    
    [Fact]
    public void CreateCourseWithEndDateBeforeStartDateShouldReturnError()
    {
        // Arrange
        var invalidEndDate = DatesHelper.SubstractInMinutes(ValidStartDate, 1);
        
        // Act
        var result = Assert.Throws<InvalidCourseDateException>(() =>
            Course.Create(ValidName, ValidFee, ValidStartDate, invalidEndDate));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Errors.StartDateBeforeEndDate, result.Message); // Asserting that error message is the correct one.
    }
    
    [Fact]
    public void CreateTwoCoursesWithSameNameShouldHaveDifferentIds()
    {
        // Act
        var result1 =  Course.Create(ValidName, ValidFee, ValidStartDate, ValidEndDate);
        var result2 =  Course.Create(ValidName, ValidFee, ValidStartDate, ValidEndDate);
        
        // Assert
        Assert.NotEqual(result1.Id, result2.Id); // Asserting Ids are unique
    }
}