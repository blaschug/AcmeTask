using Enrollments.Domain.Entities;

namespace Enrollments.Application.Common;

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(Student student, Course course);
}