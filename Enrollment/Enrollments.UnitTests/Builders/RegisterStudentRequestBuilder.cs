using Enrollments.Application.Features.RegisterStudent;
using Enrollments.UnitTests.Helpers;

namespace Enrollments.UnitTests.Builders;

public class RegisterStudentRequestBuilder
{
    private const string ValidName = "Blas";
    private static readonly DateOnly ValidBirthDate = DatesHelper.GetBirthDateForAge(20);
    
    private RegisterStudentRequest _registerStudentRequest;

    /// <summary>
    ///  By default creates a valid entity.
    /// </summary>
    public RegisterStudentRequestBuilder()
    {
        _registerStudentRequest = new RegisterStudentRequest(ValidName, ValidBirthDate);
    }

    public RegisterStudentRequestBuilder WithName(string name)
    {
        _registerStudentRequest = _registerStudentRequest with { Name = name};
        return this;
    }
    
    public RegisterStudentRequestBuilder WithBirthDay(DateOnly birthDate)
    {
        _registerStudentRequest = _registerStudentRequest with { BirthDate = birthDate};
        return this;
    }    

    public RegisterStudentRequest Build() => _registerStudentRequest;
}