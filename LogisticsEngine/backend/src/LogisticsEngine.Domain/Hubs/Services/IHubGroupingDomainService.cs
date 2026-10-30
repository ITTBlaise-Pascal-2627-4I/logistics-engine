using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Contratto per il raggruppamento degli Hub per zona.</summary>
public interface IHubGroupingDomainService
{
    IReadOnlyList<IReadOnlyList<Hub>> GroupByZone(IEnumerable<Hub> hubs);
}
