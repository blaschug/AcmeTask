using Enrollment.Domain.Constants;
using Enrollment.Domain.Entities;
using Enrollment.Domain.Exceptions;
using Enrollment.UnitTests.Helpers;

namespace Enrollment.UnitTests.Domain;

public class StudentTests
{
    private const string ValidName = "Blas";
    private const int ValidAge = 20;
    private const int MinNameLenght = 2;
    
    [Theory]
    [InlineData(17)]
    [InlineData(15)]
    [InlineData(9)]
    public void CreateShouldThrowInvalidAgeExceptionIfUnder18(int age)
    {
        //Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(age);
        
        //Act n Assert
        var result = Assert.Throws<InvalidAgeException>(() => Student.Create(ValidName, birthDate));  //Asserting exception is the right exception type

        //Assert
        Assert.NotNull(result);
        Assert.Equal(Errors.StudentNotAdult, result.Message); // Asserting exception message is the correct one
    }
    
    [Theory]
    [InlineData(18)]
    [InlineData(20)]
    [InlineData(25)]
    public void CreateWithValidAgeShouldReturnStudent(int age)
    {
        // Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(age);
        
        // Act
        var result = Student.Create(ValidName, birthDate);

        // Assert
        Assert.NotNull(result); // Asserting student exists.
        Assert.IsType<Student>(result); // Asserting result type
        Assert.Equal(age, result.GetAge()); // Asserting current age is the same as the given one.
    }
    
    [Theory]
    [InlineData("iv")]
    [InlineData("Juan")]
    [InlineData("Name SecondName Surname")] 
    public void CreateStudentWithValidNameShouldReturnStudent(string validName)
    {
        // Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(ValidAge);
        
        // Act
        var result = Student.Create(validName, birthDate);

        // Assert
        Assert.NotNull(result); // Asserting student exists.
        Assert.IsType<Student>(result); // Asserting result type
        Assert.Equal(validName, result.Name); // Asserting current name is the same as the given one.
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    [InlineData("     ")]
    [InlineData("  a   ")]
    public void CreateShouldThrowInvalidNameExceptionIfNameIsInvalid(string invalidName)
    {
        // Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(ValidAge);
        
        // Act
        var result = Assert.Throws<InvalidNameException>(() => Student.Create(invalidName, birthDate)); //Asserting exception is the right exception type
        
        // Assert
        Assert.NotNull(result);
        // Maybe MinNameLenght could vary in the future.
        Assert.Equal(Errors.InvalidName(typeof(Student), MinNameLenght), result.Message); // Asserting exception message is the correct one
    }
    
    [Fact]
    public void CreateTwoStudentsWithSameNameShouldHaveDifferentIds()
    {
        // Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(ValidAge);
        
        // Act
        var student1 = Student.Create(ValidName, birthDate);
        var student2 = Student.Create(ValidName, birthDate);
        
        // Assert
        Assert.NotEqual(student1.Id, student2.Id); // Asserting Ids are unique
    }
    
    [Fact]
    public void AgeShouldBeCalculatedCorrectly()
    {
        // Arrange
        var birthDate = DatesHelper.GetBirthDateForAge(ValidAge);
        
        // Act
        var student = Student.Create(ValidName, birthDate);
        
        // Assert
        Assert.Equal(ValidAge, student.GetAge());
    }
}