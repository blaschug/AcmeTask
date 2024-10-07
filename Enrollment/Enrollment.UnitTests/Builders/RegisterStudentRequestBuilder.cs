using Enrollment.Application.Features.RegisterStudent;
using Enrollment.UnitTests.Helpers;

namespace Enrollment.UnitTests.Builders;

public class RegisterStudentRequestBuilder
{
    private const string ValidName = "Blas";
    private const string InValidName = "B";
    private static readonly DateOnly ValidBirthDate = DatesHelper.GetBirthDateForAge(20);
    private static readonly DateOnly InvalidBirthDate = DatesHelper.GetBirthDateForAge(15);
    
    private RegisterStudentRequest _registerStudentRequest;

    public RegisterStudentRequestBuilder()
    {
        _registerStudentRequest = new RegisterStudentRequest("", new DateOnly());
    }

    public RegisterStudentRequestBuilder WithValidName()
    {
        _registerStudentRequest = _registerStudentRequest with { Name = ValidName};
        return this;
    }
    
    public RegisterStudentRequestBuilder WithInvalidName()
    {
        _registerStudentRequest = _registerStudentRequest with { Name = InValidName};
        return this;
    }
    
    public RegisterStudentRequestBuilder WithValidBirthDay()
    {
        _registerStudentRequest = _registerStudentRequest with { BirthDate = ValidBirthDate};
        return this;
    }    
    
    public RegisterStudentRequestBuilder WithInvalidBirthDay()
    {
        _registerStudentRequest = _registerStudentRequest with { BirthDate = InvalidBirthDate };
        return this;
    }

    public RegisterStudentRequest Build()
    {
        return _registerStudentRequest;
    }
}