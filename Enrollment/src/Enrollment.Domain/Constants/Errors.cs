namespace Enrollment.Domain.Constants;

public static class Errors
{
    //Student
    public const string StudentNotAdult = "Student must be at least 18 years old to register.";
            
    //Course
    public const string InvalidRegistrationFee = "Registration fee cannot be negative.";
    public const string StartDateBeforeEndDate = "Start date cannot be before end date.";
    public const string StartCourseInPast = "Start date cannot be before end date.";
    
    //Shared
    /// <summary>
    /// Generates a formatted error message for an invalid name, including the entity type and the minimum name length required.
    /// </summary>
    /// <param name="entityType">Receives a type and get its name.</param>
    /// <param name="minNameLenght">MinNameLenght can vary depending on the entity</param>
    /// <returns>Formatted message</returns>
    public static string InvalidName(Type entityType, int minNameLenght) => $"{nameof(entityType)} Name must have at least {minNameLenght} characters.";
}