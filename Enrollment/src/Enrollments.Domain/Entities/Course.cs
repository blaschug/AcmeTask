using Enrollments.Domain.Constants;
using Enrollments.Domain.Enums;
using Enrollments.Domain.Exceptions;

namespace Enrollments.Domain.Entities;

public class Course : Entity
{
    private const int MinimumNameLength = 3;
    
    public string Name { get; private set; }
    public decimal RegistrationFee { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    private ICollection<Enrollment> _enrolments = new List<Enrollment>();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrolments.ToList().AsReadOnly();
    
    public bool IsPaymentRequired() => RegistrationFee > 0;

    private Course(string name, decimal registrationFee, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        Name = name;
        RegistrationFee = registrationFee;
        StartDate = startDate;
        EndDate = endDate;
    }
    
    /// <summary>
    /// Validate input and Creates a Course.
    /// </summary>
    /// <param name="name">Course name</param>
    /// <param name="registrationFee">Course Fee</param>
    /// <param name="startDate">Course start date, must be future.</param>
    /// <param name="endDate">Course end date, must be higher than startDate</param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException">Name is null or lenght less than 3</exception>
    /// <exception cref="InvalidRegistrationFeeException">Registration fee is negative</exception>
    /// <exception cref="InvalidCourseDateException">StartDate is before now or EndDate older than StartDate</exception>
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

    /// <summary>
    /// Checks if Course already started and new Student can be Enrolled
    /// </summary>
    /// <returns>If course already started new Student cannot be added.</returns>
    public bool HasCourseStarted()
    {
        return DateTimeOffset.UtcNow >= StartDate;
    }

    public void AddEnrollment(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrolments.Add(enrollment);
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
            throw new InvalidCourseDateException(DomainErrors.StartDateBeforeEndDate);

        // Validate course cannot start in past
        if (startDate < DateTimeOffset.UtcNow)
            throw new InvalidCourseDateException(DomainErrors.StartCourseInPast);
    }
    
    private static bool IsNameValid(string name) => !(string.IsNullOrWhiteSpace(name) || name.Length < MinimumNameLength);
}