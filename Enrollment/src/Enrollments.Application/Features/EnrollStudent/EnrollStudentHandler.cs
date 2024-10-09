using Enrollments.Application.Common;
using Enrollments.Application.Common.Exceptions;
using Enrollments.Application.Common.Repositories;
using Enrollments.Application.Constants;
using Enrollments.Application.Extensions;
using Enrollments.Domain.Entities;
using Enrollments.Domain.Enums;

namespace Enrollments.Application.Features.EnrollStudent;

public class EnrollStudentHandler : IEnrollStudentHandler
{
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public EnrollStudentHandler(
        ICourseRepository courseRepository, 
        IStudentRepository studentRepository, 
        IEnrollmentRepository enrollmentRepository, IPaymentGateway paymentGateway)
    {
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<EnrollStudentDto> Handle(EnrollStudentRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            throw new EntityNotFoundException(typeof(Course), request.CourseId);
        }

        var student = await _studentRepository.GetByIdAsync(request.StudentId);
        if (student == null)
        {
            throw new EntityNotFoundException(typeof(Student), request.StudentId);
        }
        
        if (await _enrollmentRepository.IsStudentEnrolled(student, course))
        {
            throw new AlreadyEnrolledException();
        }

        var enrollment = Enrollment.Create(course, student);

        if (course.IsPaymentRequired())
        {
            var paymentResult = await _paymentGateway.ProcessPaymentAsync(student, course);
           
            enrollment.UpdatePaymentStatus(paymentResult ? PaymentStatus.Paid : PaymentStatus.Failed);
        }
        
        var savedEnrollment = await _enrollmentRepository.AddAndSaveAsync(enrollment);
        
        return new EnrollStudentDto
        {
            EnrollmentId = savedEnrollment.Id,
            Course = savedEnrollment.Course.ToDto(),
            Student = savedEnrollment.Student.ToDto(),
            PaymentStatus = savedEnrollment.PaymentStatus.ToString()
        };
    }
}