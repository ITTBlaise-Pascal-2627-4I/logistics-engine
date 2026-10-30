using LogisticsEngine.Application.Common;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.DeleteHub;

/// <summary>Elimina un Hub o restituisce il suo errore di not found.</summary>
public sealed class DeleteHubCommandHandler(IHubRepository repository) : ICommandHandler<DeleteHubCommand, Result>
{
    public Result Handle(DeleteHubCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure(HubErrors.InvalidId);
        }

        return repository.Delete(command.Id) ? Result.Success() : Result.Failure(HubErrors.NotFound(command.Id));
    }
}
