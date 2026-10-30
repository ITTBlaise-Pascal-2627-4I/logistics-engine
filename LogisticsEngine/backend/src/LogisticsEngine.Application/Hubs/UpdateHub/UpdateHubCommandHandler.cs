using LogisticsEngine.Application.Common;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.UpdateHub;

/// <summary>Valida e applica una modifica, preservando i dati quando una regola fallisce.</summary>
public sealed class UpdateHubCommandHandler(IHubRepository repository, IHubDomainService domainService) : ICommandHandler<UpdateHubCommand, Result>
{
    public Result Handle(UpdateHubCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure(HubErrors.InvalidId);
        }

        Hub? hub = repository.GetById(command.Id);
        if (hub is null)
        {
            return Result.Failure(HubErrors.NotFound(command.Id));
        }

        Result uniqueName = domainService.EnsureNameIsUnique(command.Name, command.Id);
        if (uniqueName.IsFailure)
        {
            return uniqueName;
        }

        Result updated = hub.Update(command.Name, command.Zone);
        if (updated.IsFailure)
        {
            return updated;
        }

        return repository.Update(hub) ? Result.Success() : Result.Failure(HubErrors.NotFound(command.Id));
    }
}
