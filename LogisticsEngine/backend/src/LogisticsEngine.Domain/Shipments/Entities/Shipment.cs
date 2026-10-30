using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.Domain.Shipments.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Shipments.Entities;

/// <summary>Entità Shipment con identità stabile e stato modificabile solo tramite comportamenti di dominio.</summary>
public sealed class Shipment : Entity
{
    private Shipment(Guid id, TrackingNumber trackingNumber) : base(id)
    {
        TrackingNumber = trackingNumber;
        Status = ShipmentStatus.Created;
    }

    public TrackingNumber TrackingNumber { get; }
    public ShipmentStatus Status { get; private set; }

    public static Result<Shipment> Create(Guid id, string? trackingNumber)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<Shipment>(ShipmentErrors.InvalidId);
        }

        Result<TrackingNumber> number = TrackingNumber.Create(trackingNumber);
        return number.IsFailure
            ? Result.Failure<Shipment>(number.Error)
            : Result.Success(new Shipment(id, number.Value));
    }

    /// <summary>Il domain service controlla le transizioni prima di invocare questo metodo.</summary>
    internal void SetStatus(ShipmentStatus status) => Status = status;
}
