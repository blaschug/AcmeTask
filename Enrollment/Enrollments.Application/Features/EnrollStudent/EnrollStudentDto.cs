using Enrollments.Application.Models;

namespace Enrollments.Application.Features.EnrollStudent;

public record EnrollStudentDto
{
    public Guid EnrollmentId { get; init; }
    public CourseDto Course { get; init; }
    public StudentDto Student { get; init; }
    public string PaymentStatus { get; init; }
}
