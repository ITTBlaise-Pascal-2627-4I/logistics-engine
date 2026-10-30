namespace LogisticsEngine.Application.Hubs.UpdateHub;

/// <summary>Dati identificativi e nuovi valori dell'Hub da aggiornare.</summary>
public sealed record UpdateHubCommand(Guid Id, string? Name, string? Zone);
