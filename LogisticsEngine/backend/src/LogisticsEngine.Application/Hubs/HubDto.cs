using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Application.Hubs;

// HubDto è il modello in uscita: rappresenta i dati che l’Application restituisce al chiamante,
// per esempio alla console o alla Web API.
public sealed record HubDto(Guid Id, string Name, string Zone)
{
    public static HubDto From(Hub hub) => new(hub.Id, hub.Name, hub.Zone.Code);
}
