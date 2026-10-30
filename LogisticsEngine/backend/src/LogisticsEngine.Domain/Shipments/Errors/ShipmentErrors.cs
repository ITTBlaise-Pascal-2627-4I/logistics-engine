using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Shipments.Errors;

/// <summary>Error catalog dedicato: le violazioni Shipment non riusano codici Hub.</summary>
public static class ShipmentErrors
{
    public static readonly Error InvalidId = Error.Validation("Shipment.InvalidId", "Lo shipment ID non può essere vuoto.");
    public static readonly Error InvalidTrackingNumber = Error.Validation("Shipment.InvalidTrackingNumber", "Il tracking deve contenere da 8 a 24 caratteri alfanumerici.");
    public static readonly Error InvalidTransition = Error.Validation("Shipment.InvalidTransition", "La transizione di stato richiesta non è consentita.");
    public static Error NotFound(Guid id) => Error.NotFound("Shipment.NotFound", $"La spedizione '{id}' non esiste.");
}
