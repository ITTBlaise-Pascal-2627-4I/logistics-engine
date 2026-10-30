# ROADMAP DSA in ddd-logistics-engine

## **FASE 1 — Data Structures CORE**

## 1. Stack → Undo Dispatch

### Algoritmo

* push/pop
* LIFO

### Use case

```text
Undo assegnazione ordine
Redo assegnazione
```

### Integrazione

* SOLO in Application layer (no domain yet)

```csharp
DispatchHistoryStack
```

---

## 2. Queue → Order Processing

### Algoritmo

* FIFO

### Use case

```text
OrderQueue → Dispatcher → Courier
```

### Integrazione

* Application Service

---

## 3. Priority Queue → Express Delivery

### Algoritmo

* heap

### Use case

```text
SLA priority orders
```

### Integrazione

```csharp
OrderDispatchPriorityQueue
```

---

# **FASE 2 — TREES + STRUCTURING**

## 4. Trees → Delivery Zones

### Algoritmo

* tree traversal

### Use case

```text
Zone → Subzone → Streets
```

### Integrazione

* Domain model extension:

```csharp
DeliveryZone (aggregate)
```

---

# **FASE 3 — GRAPH CORE**

## 6. Graph Basics → Road Network

### Use case

* nodes = hubs
* edges = roads

### Integrazione Domain

```csharp
RoadNetwork (Domain Service)
```

---

## 7. BFS / DFS → Reachability

### Use case

```text
posso raggiungere questo hub?
```

### Integrazione

```csharp
NetworkAnalysisService
```

---

# **FASE 4 — ROUTING ENGINE**

## 8. Dijkstra → Shortest Path

### Use case

```text
best route between hubs
```

### Integrazione

```csharp
IRouteCalculationService
```

primo vero Domain Service importante

---

## 9. A* → Smart Routing

### Use case

* heuristics (distance + traffic)

### Integrazione

```csharp
SmartRoutePlanner : IRouteCalculationService
```

Strategy Pattern

---

# **FASE 5 — OPTIMIZATION**

## 10. Greedy Algorithms → Courier Assignment

### Use case

```text
assign nearest courier
```

### Integrazione

```csharp
CourierAssignmentStrategy
```

---

## 11. Dynamic Programming → Route Optimization

### Use case

* multi-stop optimization
* constraint routing

---

# **FASE 6 — ADVANCED STRUCTURES**

---

## 13. LRU Cache → Route caching

### Use case

```text
cache computed routes
```

### Integrazione Infrastructure

```csharp
CachedRouteRepository
```

---

## 14. Heaps avanzati → SLA system

### Use case

* multi-criteria priority

---

# **FASE 7 — SEARCH + MATCHING**

## 15. String Algorithms → Address correction

### Use case

* typo correction
* fuzzy search

---

## 16. Binary Search / Indexing → Order lookup

### Use case

* fast retrieval

---

# **FASE 8 — RECOMMENDATION SYSTEMS**

## 17. Content-based Recommendation

### Use case

```text
similar orders / products
```

### Integrazione

```csharp
ProductRecommendationService
```

---

## 18. Graph-based Recommendation

### Use case

```text
users → orders → products graph
```

BFS su graph sociale/logistico

---

## 19. Collaborative Filtering (light version)

### Use case

```text
users like you also ordered
```

---

# **FASE 9 — REAL-TIME SYSTEM**

## 20. Event-driven tracking system

### Use case

* tracking order lifecycle

### Integrazione

```csharp
OrderDomainEvents
TrackingProjection
```

---

## 21. Queue + Event stream

### Use case

* dispatch pipeline

---

# **FASE 10 — SYSTEM INTEGRATION**

Qui tutto converge:

---

## Flow finale

```text
Order created
   ↓
Queue (FIFO / Priority)
   ↓
Greedy assignment (courier)
   ↓
Graph routing (Dijkstra / A*)
   ↓
Tracking events
   ↓
Cache (LRU routes)
   ↓
Recommendation system updates
```

---