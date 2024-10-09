namespace Enrollments.Application.Features.EnrollStudent;

public record EnrollStudentRequest(
    Guid CourseId,
    Guid StudentId);