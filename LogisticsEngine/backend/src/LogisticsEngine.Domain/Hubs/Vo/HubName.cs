using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Vo;

/// <summary>Value object che garantisce un nome Hub normalizzato e valido.</summary>
public sealed class HubName : ValueObject
{
    private HubName(string value) => Value = value;
    public string Value { get; }

    public static Result<HubName> Create(string? value)
    {
        string normalized = value?.Trim() ?? string.Empty;
        return normalized.Length is < 1 or > 120
            ? Result.Failure<HubName>(HubErrors.InvalidName)
            : Result.Success(new HubName(normalized));
    }

    protected override IEnumerable<object> GetAtomicValues() => [Value];
    public override string ToString() => Value;
}
