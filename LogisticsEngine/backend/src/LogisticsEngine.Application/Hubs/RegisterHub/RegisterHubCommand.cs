namespace LogisticsEngine.Application.Hubs.RegisterHub;

/// <summary>Dati necessari a registrare un Hub.</summary>
public sealed record RegisterHubCommand(string? Name, string? Zone);
