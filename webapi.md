# Web API LogisticsEngine

La Web API è un adapter HTTP sottile sopra l'Application. I controller non implementano regole di business: costruiscono command o query e li inviano a MediatR tramite `ISender`. Gli handler coordinano dominio e repository; i controller traducono i `Result` in status HTTP e `ProblemDetails`.

## Requisiti

- .NET SDK 10 installato.
- Esecuzione dalla cartella `backend` del repository.

## Avvio

```sh
cd backend
dotnet run --project src/LogisticsEngine.Api --launch-profile http
```

Il profilo `http` avvia l'API su `http://localhost:5003`. Per il profilo HTTPS usa `https://localhost:7259` (e HTTP su `5003`).

All'avvio, il container DI verifica scope e costruibilità dei servizi. `AddApplication()` registra handler MediatR, validator e Domain Service; `AddInfrastructure()` collega i repository JSON. I dati si trovano in `backend/data`, accanto a `src` e `tests`.

## Swagger UI e OpenAPI

In ambiente `Development` la documentazione interattiva è disponibile qui:

```text
http://localhost:5003/swagger
```

La specifica JSON della versione corrente è:

```text
http://localhost:5003/swagger/v1/swagger.json
```

Swagger UI è esposta solo in sviluppo. Il file [LogisticsEngine.Api.http](../backend/src/LogisticsEngine.Api/LogisticsEngine.Api.http) contiene chiamate di esempio utilizzabili con REST Client per VS Code o editor compatibili.

In sviluppo, CORS consente solo gli origin dei due profili locali (`http://localhost:5003` e `https://localhost:7259`). Questo copre anche il caso in cui una pagina Swagger aperta su HTTP segua la redirezione dell'API verso HTTPS, cambiando porta/origin. In produzione questa policy non è attiva: configura gli origin reali dei client browser prima di abilitarli.

## Versionamento API

Gli endpoint pubblici usano il prefisso `/api/v1`. La versione è parte dell'URL per rendere chiaro quale contratto usa il client e consentire l'introduzione di una versione incompatibile senza cambiare subito i client esistenti.

Per aggiungere una versione incompatibile:

1. mantieni `/api/v1` disponibile per i client esistenti;
2. crea controller e contratti v2 sotto `/api/v2`;
3. registra un documento OpenAPI `v2` e aggiungilo al selettore di Swagger UI;
4. comunica la deprecazione di v1 e una data di rimozione prima di ritirarla.

Per modifiche compatibili, preferisci aggiungere campi opzionali o nuovi endpoint alla versione attuale. Evita di cambiare il significato o il tipo di campi già pubblicati.

## Endpoint Hub v1

| Metodo | Percorso | Funzione |
|---|---|---|
| `GET` | `/api/v1/hubs` | Elenca gli Hub |
| `GET` | `/api/v1/hubs/{id}` | Cerca per ID |
| `GET` | `/api/v1/hubs/by-name/{name}` | Cerca per nome |
| `POST` | `/api/v1/hubs` | Crea un Hub |
| `PUT` | `/api/v1/hubs/{id}` | Aggiorna nome e zona |
| `DELETE` | `/api/v1/hubs/{id}` | Elimina un Hub |
| `GET` | `/api/v1/hubs/algorithms/sort?property=Name` | Ordina per `Id`, `Name` o `Zone` |
| `GET` | `/api/v1/hubs/algorithms/groups-by-zone` | Raggruppa per zona |
| `GET` | `/api/v1/hubs/algorithms/collections` | Mostra queue, stack, hashmap e set |
| `GET` | `/api/v1/hubs/algorithms/binary-tree` | Visita in-order dell'albero binario |
| `GET` | `/api/v1/hubs/algorithms/trie?prefix=Mi` | Cerca per prefisso del nome |
| `POST` | `/api/v1/hubs/connections` | Aggiunge un collegamento bidirezionale pesato |
| `GET` | `/api/v1/hubs/connections` | Elenca i collegamenti |
| `GET` | `/api/v1/hubs/algorithms/shortest-path?originId={id}&destinationId={id}` | Dijkstra |
| `GET` | `/api/v1/hubs/algorithms/traversal?originId={id}&traversal=BreadthFirst` | BFS o DFS |
| `GET` | `/api/v1/hubs/algorithms/simple-paths?originId={id}&destinationId={id}&maximumPaths=10` | Backtracking, con limite al numero di risultati |

Per usare gli algoritmi sul grafo, crea almeno due Hub e collegali. La distanza deve essere positiva.

```http
POST http://localhost:5003/api/v1/hubs/connections
Content-Type: application/json

{
  "firstHubId": "00000000-0000-0000-0000-000000000001",
  "secondHubId": "00000000-0000-0000-0000-000000000002",
  "distance": 12.5
}
```

Usa gli ID realmente restituiti dalla creazione degli Hub.

## Endpoint Shipment v1

| Metodo | Percorso | Funzione |
|---|---|---|
| `GET` | `/api/v1/shipments` | Elenca le spedizioni |
| `GET` | `/api/v1/shipments/{id}` | Cerca per ID |
| `POST` | `/api/v1/shipments` | Crea una spedizione |
| `PUT` | `/api/v1/shipments/{id}/status` | Richiede una transizione di stato |
| `DELETE` | `/api/v1/shipments/{id}` | Elimina una spedizione |

```http
POST http://localhost:5003/api/v1/shipments
Content-Type: application/json

{
  "trackingNumber": "ABCD1234"
}
```

Gli enum sono serializzati come stringhe. Per lo stato sono disponibili `Created`, `InTransit`, `Delivered` e `Cancelled`; le transizioni consentite sono verificate dall'entità Shipment.

## Convenzioni e buone pratiche

Queste convenzioni sono applicate agli endpoint v1:

- **Risorse e verbi HTTP:** nomi plurali (`hubs`, `shipments`) e semantica standard: `GET` legge, `POST` crea, `PUT` sostituisce o aggiorna, `DELETE` elimina.
- **Contratti HTTP separati dal dominio:** le request HTTP sono record in `Api/Contracts`; le risposte sono DTO dell'Application. Non si espongono direttamente entità e value object.
- **Validazione nel posto giusto:** gli input passano ai validator FluentValidation e alle invarianti del Domain. Il controller si limita a inoltrare la richiesta.
- **Status coerenti:** `201 Created` per la creazione, con `Location`; `204 No Content` per aggiornamenti/eliminazioni; `400` per input o regole non validi; `404` per risorse mancanti.
- **Errori uniformi:** i fallimenti applicativi sono restituiti come `ProblemDetails`, non come messaggi testuali ad hoc.
- **Cancellazione:** i controller inoltrano il `CancellationToken` a MediatR, così i client possono interrompere una richiesta non più necessaria.
- **Enum JSON leggibili:** gli enum sono serializzati con nomi stringa invece di valori numerici.
- **HTTPS e documentazione:** HTTPS redirection è attiva; Swagger UI e la specifica sono esposte in sviluppo.
- **Health check:** `GET /health` consente un controllo minimo che il processo API risponda.

## Da fare prima di un uso produttivo

Questa API è una demo didattica con persistenza su file JSON. Prima di esporla in un ambiente condiviso o pubblico:

- aggiungi autenticazione, autorizzazione per ruolo e rate limiting; al momento gli endpoint applicativi sono pubblici;
- applica paginazione e limiti alle query che restituiscono collezioni, soprattutto `GET /hubs`, `GET /shipments` e i cammini semplici;
- definisci un handler globale delle eccezioni e logging strutturato, evitando di esporre dettagli interni;
- aggiungi test HTTP d'integrazione e controlli di compatibilità per lo schema OpenAPI;
- sposta la persistenza JSON su un archivio adatto alla concorrenza e configura backup e gestione dei dati;
- valuta CORS solo quando è noto il dominio dei client browser; evita policy permissive non necessarie.

Swagger UI in sviluppo non è una misura di sicurezza: l'accesso ai dati deve essere protetto dall'autenticazione e dall'autorizzazione dell'API.
