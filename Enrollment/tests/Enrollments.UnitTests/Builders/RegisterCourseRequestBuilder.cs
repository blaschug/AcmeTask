using Enrollments.Application.Features.RegisterCourse;

namespace Enrollments.UnitTests.Builders;

public class RegisterCourseRequestBuilder
{
    private const string ValidName = "Course";
    private const decimal RegistrationFee = 100m;
    private static DateTimeOffset ValidStartDate = DateTimeOffset.UtcNow.AddMinutes(1);
    private static DateTimeOffset ValidEndDate = DateTimeOffset.UtcNow.AddMinutes(30);
    private RegisterCourseRequest _registerCourseRequest;
    
    public RegisterCourseRequestBuilder()
    {
        _registerCourseRequest = new RegisterCourseRequest(ValidName, RegistrationFee, ValidStartDate, ValidEndDate);
    }

    public RegisterCourseRequestBuilder WithName(string name)
    {
        _registerCourseRequest = _registerCourseRequest with { Name = name };
        return this;
    }
    
    public RegisterCourseRequestBuilder WithRegistrationFee(decimal registrationFee)
    {
        _registerCourseRequest = _registerCourseRequest with { RegistrationFee = registrationFee };
        return this;
    }
    
    public RegisterCourseRequestBuilder WithStartDate(DateTimeOffset startDate)
    {
        _registerCourseRequest = _registerCourseRequest with { StartDate = startDate };
        return this;
    }
    
    public RegisterCourseRequestBuilder WithEndDate(DateTimeOffset endDate)
    {
        _registerCourseRequest = _registerCourseRequest with { EndDate = endDate };
        return this;
    }
    
    public RegisterCourseRequest Build() => _registerCourseRequest;
}