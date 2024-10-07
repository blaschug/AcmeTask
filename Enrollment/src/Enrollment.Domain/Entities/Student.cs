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

    public static Student Create(string name, DateOnly birthDate)
    {
        // Validate Name
        var trimmedName = name?.Trim();
        if (!IsNameValid(trimmedName))
        {
            throw new InvalidNameException(typeof(Student), MinimumNameLength);
        }

        // Validate age
        if (!IsAgeValid(birthDate))
        {
            throw new InvalidAgeException();
        }

        var student = new Student(
            name: trimmedName,
            birthDate: birthDate);

        return student;
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