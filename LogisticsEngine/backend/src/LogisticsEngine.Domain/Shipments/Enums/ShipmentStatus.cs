namespace LogisticsEngine.Domain.Shipments.Enums;

/// <summary>Stati ammessi nel ciclo di vita di una spedizione.</summary>
public enum ShipmentStatus
{
    Created,
    InTransit,
    Delivered,
    Cancelled
}
