using Enrollment.Domain.Exceptions;

namespace Enrollment.Domain.Entities;

public class Student : Entity
{
    private const int MinimumNameLength = 2;
    private const int MinimumAge = 18;

    public string Name { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public int GetAge() => CalculateAge(BirthDate);

    private Student(string name, DateOnly birthDate)
    {
        Name = name;
        BirthDate = birthDate;
    }

    /// <summary>
    /// Validate input and Creates a Student.
    /// </summary>
    /// <param name="name">Student name</param>
    /// <param name="birthDate">Student BirthDate</param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException">Name is null or lenght less than 2</exception>
    /// <exception cref="InvalidAgeException">Student age must be at least 18</exception>
    public static Student Create(string name, DateOnly birthDate)
    {
        var validatedName = ValidateName(name);
        ValidateAge(birthDate);

        var student = new Student(
            name: validatedName,
            birthDate: birthDate);

        return student;
    }

    private static string ValidateName(string name)
    {
        var trimmedName = name?.Trim();
        if (!IsNameValid(trimmedName))
        {
            throw new InvalidNameException(typeof(Student), MinimumNameLength);
        }

        return trimmedName;
    }
    
    private static void ValidateAge(DateOnly birthDate)
    {
        if (!IsAgeValid(birthDate))
            throw new InvalidAgeException();
    } 

    private static int CalculateAge(DateOnly birthdate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthdate.Year;

        if (birthdate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    private static bool IsAgeValid(DateOnly birthDate) => !(CalculateAge(birthDate) < MinimumAge);
    private static bool IsNameValid(string name) => !(string.IsNullOrWhiteSpace(name) || name.Length < MinimumNameLength);
}