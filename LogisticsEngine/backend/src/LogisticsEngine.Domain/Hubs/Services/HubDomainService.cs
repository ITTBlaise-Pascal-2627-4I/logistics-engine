using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Verifica la regola di unicità che richiede di confrontare più Hub.</summary>
public sealed class HubDomainService(IHubRepository repository) : IHubDomainService
{
    public Result EnsureNameIsUnique(string? name, Guid? excludingId = null)
    {
        Result<HubName> candidate = HubName.Create(name);
        if (candidate.IsFailure)
        {
            return Result.Failure(candidate.Error);
        }

        bool alreadyUsed = repository.GetAll().Any(hub =>
            hub.Id != excludingId && string.Equals(hub.Name, candidate.Value.Value, StringComparison.OrdinalIgnoreCase));
        return alreadyUsed
            ? Result.Failure(HubErrors.DuplicateName(candidate.Value.Value))
            : Result.Success();
    }
}
