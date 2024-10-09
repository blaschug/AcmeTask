namespace Enrollments.UnitTests.Helpers;

public static class DatesHelper
{
    public static DateOnly GetBirthDateForAge(int age) => 
        DateOnly.FromDateTime(DateTime.Today.AddYears(-age));

    public static DateTimeOffset SubstractInMinutes(DateTimeOffset date, int minutes) => 
        date.Subtract(TimeSpan.FromMinutes(minutes));
}