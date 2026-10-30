using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Entities;

public sealed class Hub : Entity
{
    private Hub(Guid id, string name, Zone zone) : base(id)
    {
        NameValue = HubName.Create(name).Value;
        Zone = zone;
    }

    /// <summary>Il valore grezzo resta comodo da mostrare; NameValue conserva l'invariante del value object.</summary>
    public string Name => NameValue.Value;
    public HubName NameValue { get; private set; }
    public Zone Zone { get; private set; }

    public static Result<Hub> Create(Guid id, string? name, string? zoneCode)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<Hub>(HubErrors.InvalidId);
        }

        Result<HubName> validName = HubName.Create(name);
        if (validName.IsFailure)
        {
            return Result.Failure<Hub>(validName.Error);
        }

        Result<Zone> zone = Zone.Create(zoneCode);
        return zone.IsFailure
            ? Result.Failure<Hub>(zone.Error)
            : Result.Success(new Hub(id, validName.Value.Value, zone.Value));
    }

    public Result Update(string? name, string? zoneCode)
    {
        Result<Hub> candidate = Create(Id, name, zoneCode);
        if (candidate.IsFailure)
        {
            return Result.Failure(candidate.Error);
        }

        // Applica entrambe le modifiche solo dopo aver validato tutti i dati.
        NameValue = candidate.Value.NameValue;
        Zone = candidate.Value.Zone;
        return Result.Success();
    }
}
