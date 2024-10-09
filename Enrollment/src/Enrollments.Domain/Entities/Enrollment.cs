using Enrollments.Domain.Enums;

namespace Enrollments.Domain.Entities;

public class Enrollment : Entity
{
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; }
    
    public Guid StudentId { get; private set; }
    public Student Student { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }

    private Enrollment(Course course, Student student)
    {
        CourseId = course.Id;
        Course = course;
        StudentId = student.Id;
        Student = student;
        PaymentStatus = course.IsPaymentRequired() ? 
            PaymentStatus.WaitingForPayment : PaymentStatus.NotRequired;
    }

    /// <summary>
    /// Validate input and Creates an Enrollment with PaymentStatus: WaitingForPayment or NotRequired.
    /// </summary>
    /// <param name="course"></param>
    /// <param name="student"></param>
    /// <returns>New Enrollment with PaymentStatus: WaitingForPayment or NotRequired depending on the Course.</returns>
    public static Enrollment Create(Course course, Student student)
    {
        ArgumentNullException.ThrowIfNull(course);
        ArgumentNullException.ThrowIfNull(student);
        
        if (course.HasCourseStarted())
        {
            throw new Exception();
        }
        
        var enrollment =  new Enrollment(
            course: course,
            student: student);
        
        return enrollment;
    }
    
    public void UpdatePaymentStatus(PaymentStatus paymentStatus) => PaymentStatus = paymentStatus;
}