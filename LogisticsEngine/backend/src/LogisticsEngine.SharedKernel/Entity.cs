namespace LogisticsEngine.SharedKernel;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        Id = id;
    }

    public Guid Id { get; }
}
