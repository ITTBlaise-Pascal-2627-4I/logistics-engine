using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Contratto per le regole di dominio che coordinano più Hub.</summary>
public interface IHubDomainService
{
    Result EnsureNameIsUnique(string? name, Guid? excludingId = null);
}
