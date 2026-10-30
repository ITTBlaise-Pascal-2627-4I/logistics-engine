namespace LogisticsEngine.SharedKernel;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj) =>
        obj is ValueObject other && GetType() == other.GetType() &&
        GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    public override int GetHashCode() =>
        GetAtomicValues().Aggregate(GetType().GetHashCode(), HashCode.Combine);
}
