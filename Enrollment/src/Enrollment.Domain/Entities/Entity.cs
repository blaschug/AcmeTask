namespace Enrollment.Domain.Entities;

public class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}