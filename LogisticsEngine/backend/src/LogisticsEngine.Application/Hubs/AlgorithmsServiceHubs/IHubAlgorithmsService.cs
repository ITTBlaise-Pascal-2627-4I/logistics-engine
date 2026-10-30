using LogisticsEngine.Domain.Hubs.Services;

namespace LogisticsEngine.Application.Hubs.AlgorithmsServiceHubs;

/// <summary>Contratto dei casi d'uso applicativi basati sugli algoritmi di dominio Hub.</summary>
public interface IHubAlgorithmsService
{
    IReadOnlyList<HubDto> SortById(HubSortAlgorithm algorithm);
    IReadOnlyList<IReadOnlyList<HubDto>> GroupByZone();
}
