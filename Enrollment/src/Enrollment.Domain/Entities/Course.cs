using Enrollment.Domain.Constants;
using Enrollment.Domain.Exceptions;

namespace Enrollment.Domain.Entities;

public class Course : Entity
{
    private const int MinimumNameLength = 3;
    
    public string Name { get; private set; }
    public decimal RegistrationFee { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    
    private Course(string name, decimal registrationFee, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        Name = name;
        RegistrationFee = registrationFee;
        StartDate = startDate;
        EndDate = endDate;
    }
    
    public static Course Create(string name, decimal registrationFee, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        // Validate name
        var validateName = ValidateName(name);
        ValidateRegistrationFee(registrationFee: registrationFee);
        ValidateDates(startDate:startDate, endDate: endDate);

        var course = new Course(
            name: validateName,
            registrationFee: registrationFee,
            startDate: startDate,
            endDate: endDate);

        return course;
    }
    
    private static string ValidateName(string name)
    {
        var trimmedName = name?.Trim();
        if (!IsNameValid(trimmedName))
        {
            throw new InvalidNameException(typeof(Course), MinimumNameLength);
        }

        return trimmedName;
    }
    
    private static void ValidateRegistrationFee(decimal registrationFee)
    {
        if (registrationFee < 0)
            throw new InvalidRegistrationFeeException();
    }

    private static void ValidateDates(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        // Validate end date cannot be older than start date
        if (startDate > endDate)
            throw new InvalidCourseDateException(Errors.StartDateBeforeEndDate);

        // Validate course cannot start in past
        if (startDate < DateTimeOffset.Now)
            throw new InvalidCourseDateException(Errors.StartCourseInPast);
    }
    
    private static bool IsNameValid(string name) => !(string.IsNullOrWhiteSpace(name) || name.Length < MinimumNameLength);
}