using Enrollments.Application.Common;
using Enrollments.Application.Common.Exceptions;
using Enrollments.Application.Common.Repositories;
using Enrollments.Application.Constants;
using Enrollments.Application.Extensions;
using Enrollments.Application.Features.EnrollStudent;
using Enrollments.Domain.Entities;
using Enrollments.Domain.Enums;
using Enrollments.UnitTests.Helpers;
using Moq;

namespace Enrollments.UnitTests.Core;

public class EnrollStudentTests
{
    private readonly Mock<IStudentRepository> _studentRepository;
    private readonly Mock<ICourseRepository> _courseRepository;
    private readonly Mock<IEnrollmentRepository> _enrollmentRepository;
    private readonly Mock<IPaymentGateway> _paymentGateway;
    private EnrollStudentHandler Sut;

    public EnrollStudentTests()
    {
        _studentRepository = new Mock<IStudentRepository>();
        _courseRepository = new Mock<ICourseRepository>();
        _enrollmentRepository = new Mock<IEnrollmentRepository>();
        _paymentGateway = new Mock<IPaymentGateway>();
        Sut = new EnrollStudentHandler(
            _courseRepository.Object,
            _studentRepository.Object,
            _enrollmentRepository.Object,
            _paymentGateway.Object);
    }
        
    [Fact]
    public async Task HandleShouldThrowEntityNotFoundExceptionIfCourseIsNotFoundById()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(null as Course);
        
        // Act
        var act = await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        Assert.Equal(ApplicationErrors.EntityNotFound(typeof(Course), courseId), act.Message);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _enrollmentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
    
    [Fact]
    public async Task HandleShouldThrowEntityNotFoundExceptionIfStudentIsNotFoundById()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 0, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(null as Student);
        
        // Act
        var act = await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.Handle(request));
        
        // Assert
        Assert.NotNull(act);
        Assert.Equal(ApplicationErrors.EntityNotFound(typeof(Student), studentId), act.Message);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
        
    [Fact]
    public async Task HandleShouldReturnEnrollDtoWithPaidStatusFailedIfPaymentGatewayFails()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        var createdEnrollment = Enrollment.Create(returnedCourse, returnedStudent);
        createdEnrollment.UpdatePaymentStatus(PaymentStatus.Failed);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _paymentGateway.Setup(x => x.ProcessPaymentAsync(returnedStudent, returnedCourse))
            .ReturnsAsync(false);
        _enrollmentRepository.Setup(x => x.IsStudentEnrolled(returnedStudent, returnedCourse))
            .ReturnsAsync(false);
        _enrollmentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(createdEnrollment);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<EnrollStudentDto>(result);
        Assert.Equal(PaymentStatus.Failed.ToString(), result.PaymentStatus.ToString());
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Once);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldReturnEnrollDtoWithPaidStatusPaidIfPaymentGatewaySuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        var createdEnrollment = Enrollment.Create(returnedCourse, returnedStudent);
        createdEnrollment.UpdatePaymentStatus(PaymentStatus.Paid);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _paymentGateway.Setup(x => x.ProcessPaymentAsync(returnedStudent, returnedCourse))
            .ReturnsAsync(true);
        _enrollmentRepository.Setup(x => x.IsStudentEnrolled(returnedStudent, returnedCourse))
            .ReturnsAsync(false);
        _enrollmentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(createdEnrollment);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<EnrollStudentDto>(result);
        Assert.Equal(PaymentStatus.Paid.ToString(), result.PaymentStatus.ToString());
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Once);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldReturnEnrollDtoWithPaidStatusNotRequiredIfFeeIs0()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 0, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        var createdEnrollment = Enrollment.Create(returnedCourse, returnedStudent);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _enrollmentRepository.Setup(x => x.IsStudentEnrolled(returnedStudent, returnedCourse))
            .ReturnsAsync(false);
        _enrollmentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(createdEnrollment);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<EnrollStudentDto>(result);
        Assert.Equal(PaymentStatus.NotRequired.ToString(), result.PaymentStatus.ToString());
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Once);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
    
    [Fact]
    public async Task HandleShouldRiseExceptionIfEnrollmentRepositoryThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _paymentGateway.Setup(x => x.ProcessPaymentAsync(returnedStudent, returnedCourse))
            .ReturnsAsync(true);
        _enrollmentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await Assert.ThrowsAsync<Exception>(async () => await Sut.Handle(request));
        
        // Assert
        Assert.NotNull(result);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Once);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldRiseExceptionIfPaymentGatewayThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _paymentGateway.Setup(x => x.ProcessPaymentAsync(returnedStudent, returnedCourse))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await Assert.ThrowsAsync<Exception>(async () => await Sut.Handle(request));
        
        // Assert
        Assert.NotNull(result);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldRiseExceptionIfStudentRepositoryThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await Assert.ThrowsAsync<Exception>(async () => await Sut.Handle(request));
        
        // Assert
        Assert.NotNull(result);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
        
    [Fact]
    public async Task HandleShouldRiseExceptionIfCourseRepositoryThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ThrowsAsync(new Exception());
        
        // Act
        var result = await Assert.ThrowsAsync<Exception>(async () => await Sut.Handle(request));
        
        // Assert
        Assert.NotNull(result);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
    
    [Fact]
    public async Task HandleShouldReturnValidResponseIfRequestIsValid()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        var returnedEnrollment = Enrollment.Create(returnedCourse, returnedStudent);
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _paymentGateway.Setup(x => x.ProcessPaymentAsync(returnedStudent, returnedCourse))
            .ReturnsAsync(true);
        _enrollmentRepository.Setup(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(returnedEnrollment);
        
        // Act
        var result = await Sut.Handle(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(returnedEnrollment.PaymentStatus.ToString(), result.PaymentStatus);
        Assert.Equal(returnedEnrollment.Id, result.EnrollmentId);
        Assert.Equivalent(returnedEnrollment.Course.ToDto(), result.Course);
        Assert.Equivalent(returnedEnrollment.Student.ToDto(), result.Student);
        Assert.Equal(returnedEnrollment.Student.GetAge(), result.Student.Age);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Once);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleShouldThrowAlreadyEnrolledExceptionIfStudentIsAlreadyEnrolled()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var request = new EnrollStudentRequest(courseId, studentId);
        var returnedCourse = Course.Create("Course", 10, DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(10));
        var returnedStudent = Student.Create("Student", DatesHelper.GetBirthDateForAge(20));
        _courseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(returnedCourse);
        _studentRepository.Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(returnedStudent);
        _enrollmentRepository.Setup(x => x.IsStudentEnrolled(returnedStudent, returnedCourse))
            .ReturnsAsync(true);
        
        // Act
        var result = await Assert.ThrowsAsync<AlreadyEnrolledException>(async () => await Sut.Handle(request));
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ApplicationErrors.AlreadyEnrolled, result.Message);
        _courseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _enrollmentRepository.Verify(x => x.AddAndSaveAsync(It.IsAny<Enrollment>()), Times.Never);
        _enrollmentRepository.Verify(x => 
            x.IsStudentEnrolled(It.IsAny<Student>(), It.IsAny<Course>()), Times.Once);
        _paymentGateway.Verify(x =>
            x.ProcessPaymentAsync(It.IsAny<Student>(), It.IsAny<Course>()), Times.Never);
    }
}