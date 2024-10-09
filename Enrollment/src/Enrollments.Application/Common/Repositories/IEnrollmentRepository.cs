using Enrollments.Domain.Entities;

namespace Enrollments.Application.Common.Repositories;

public interface IEnrollmentRepository : IRepositoryBase<Enrollment>
{
    Task<bool> IsStudentEnrolled(Student student, Course enrollment);
}