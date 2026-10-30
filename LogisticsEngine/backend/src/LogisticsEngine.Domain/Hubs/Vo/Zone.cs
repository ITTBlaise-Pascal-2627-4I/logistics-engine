using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Vo;

/// <summary>Codice geografico immutabile: da 2 a 10 lettere ASCII (es. NORD).</summary>
public sealed class Zone : ValueObject
{
    private Zone(string code) => Code = code;

    public string Code { get; init; }

    public static Result<Zone> Create(string? code)
    {
        string normalized = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalized.Length is < 2 or > 10 || normalized.Any(c => c is < 'A' or > 'Z'))
        {
            return Result.Failure<Zone>(HubErrors.InvalidZone);
        }

        return Result.Success(new Zone(normalized));
    }

    protected override IEnumerable<object> GetAtomicValues() => [Code];
    public override string ToString() => Code;
}
