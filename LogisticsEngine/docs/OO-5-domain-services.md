RoutePlanningService
Calcola un percorso logistico rispettando vincoli di distanza, tempi, fermate e finestre di consegna.
Coinvolge: Shipment, Stop, Route, DeliveryWindow.

CarrierAssignmentService
Seleziona il vettore più appropriato in base a capacità, compatibilità, costo.
Coinvolge: Shipment, Carrier, VehicleCapacity, ServiceLevel.

LoadConsolidationService
Determina quali spedizioni possono essere raggruppate nello stesso carico rispettando peso, volume, destinazione e compatibilità delle merci.
Coinvolge: Shipment, Load, CargoConstraints.

TransportPricingService
Calcola il prezzo di trasporto applicando tariffe, supplementi, fasce di peso e regole commerciali.

Coinvolge: Shipment, Tariff, PricingPolicy.
DeliveryFeasibilityService
Verifica se una consegna è fattibile rispetto a capacità operativa, orari, tempi di transito e finestre temporali.
Coinvolge: Shipment, DeliveryWindow, TransportPlan.