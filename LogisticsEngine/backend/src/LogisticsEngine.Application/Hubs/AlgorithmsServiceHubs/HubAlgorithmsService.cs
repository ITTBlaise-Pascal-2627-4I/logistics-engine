using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Services;

namespace LogisticsEngine.Application.Hubs.AlgorithmsServiceHubs;

/// <summary>
/// Casi d'uso: raccolta dati e invocazione dei servizi di dominio sugli Hub.
/// </summary>
public sealed class HubAlgorithmsService(IHubRepository repository, IHubSortingDomainService sorting, IHubGroupingDomainService grouping) : IHubAlgorithmsService
{
    public IReadOnlyList<HubDto> SortById(HubSortAlgorithm algorithm)
    {
        IReadOnlyList<Hub> hubs = sorting.SortById(repository.GetAll(), algorithm);

        var result = new List<HubDto>();

        foreach (Hub hub in hubs)
        {
            result.Add(HubDto.From(hub));
        }

        return result.ToArray();
    }

    public IReadOnlyList<IReadOnlyList<HubDto>> GroupByZone()
    {
        IReadOnlyList<IReadOnlyList<Hub>> groups = grouping.GroupByZone(repository.GetAll());

        var result = new List<IReadOnlyList<HubDto>>();

        foreach (IReadOnlyList<Hub> group in groups)
        {
            var dtoGroup = new List<HubDto>();

            foreach (Hub hub in group)
            {
                dtoGroup.Add(HubDto.From(hub));
            }

            result.Add(dtoGroup.ToArray());
        }

        return result.ToArray();
    }
}
