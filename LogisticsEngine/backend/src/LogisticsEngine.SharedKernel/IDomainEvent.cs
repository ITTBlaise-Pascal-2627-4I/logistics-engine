namespace LogisticsEngine.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
