using LogisticsEngine.Application.Common;
using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;

namespace LogisticsEngine.Application.Hubs.GetAll;

/// <summary>Carica gli Hub tramite il repository e li espone come DTO applicativi.</summary>
public sealed class GetAllCommandHandler(IHubRepository repository) : ICommandHandler<GetAllCommand, IReadOnlyList<HubDto>>
{
    public IReadOnlyList<HubDto> Handle(GetAllCommand command)
    {
        return repository.GetAll().Select(HubDto.From).ToArray();
    }
}
