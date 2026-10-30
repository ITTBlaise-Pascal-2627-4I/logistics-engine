using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>
/// Raggruppa Hub per zona con un Dictionary, concetto simile a Group Anagrams (#49).
/// </summary>
public sealed class HubGroupingDomainService : IHubGroupingDomainService
{
    public IReadOnlyList<IReadOnlyList<Hub>> GroupByZone(IEnumerable<Hub> hubs)
    {
        var groups = new Dictionary<string, List<Hub>>();

        foreach (Hub hub in hubs)
        {
            if (!groups.TryGetValue(hub.Zone.Code, out List<Hub>? group))
            {
                group = new List<Hub>();
                groups.Add(hub.Zone.Code, group);
            }

            group.Add(hub);
        }

        var result = new List<IReadOnlyList<Hub>>();

        var zoneCodes = new List<string>(groups.Keys);
        zoneCodes.Sort();

        foreach (string zoneCode in zoneCodes)
        {
            List<Hub> group = groups[zoneCode];
            group.Sort(CompareHubsById);

            result.Add(group.ToArray());
        }

        return result;
    }

    private static int CompareHubsById(Hub first, Hub second)
    {
        return first.Id.CompareTo(second.Id);
    }
}
