using LogisticsEngine.Application.Common;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.GetById;

/// <summary>Recupera un Hub tramite repository e restituisce un DTO o un errore tipizzato.</summary>
public sealed class GetByIdCommandHandler(IHubRepository repository) : ICommandHandler<GetByIdCommand, Result<HubDto>>
{
    public Result<HubDto> Handle(GetByIdCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure<HubDto>(HubErrors.InvalidId);
        }

        Hub? hub = repository.GetById(command.Id);
        return hub is null ? Result.Failure<HubDto>(HubErrors.NotFound(command.Id)) : Result.Success(HubDto.From(hub));
    }
}
