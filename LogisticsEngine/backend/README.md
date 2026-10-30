# Esempio minimale: hub logistici

Un hub ha un ID, un nome (1–100 caratteri) e una zona (2–10 lettere ASCII).
Nome e zona vengono ripuliti dagli spazi esterni; la zona viene convertita in maiuscolo.

## Percorso di lettura

1. **SharedKernel**: Entity rappresenta l'identità; ValueObject confronta tipo e valori; Error e Result rappresentano gli esiti attesi senza eccezioni.
2. **Domain/Hubs**: Hub protegge lo stato con costruttore privato e factory; Zone è un value object immutabile; HubErrors raccoglie gli errori; IHubRepository descrive la persistenza senza conoscere JSON.
3. **Application/Hubs**: richieste e handler MediatR per GetById, GetAll, Create, Update e Delete. HubDto espone i dati senza esportare l'entità.
4. **Infrastructure**: JsonHubRepository implementa il contratto usando un file per hub. Rilegge e valida i dati del disco; un documento corrotto causa un errore, non un elenco apparentemente vuoto.
5. **WebApi**: HubsController invia richieste a MediatR e traduce Result in HTTP.

Un aggiornamento invalido non modifica neppure parzialmente l'hub.
Gli errori attesi diventano Result; errori imprevisti di I/O sono eccezioni e la Web API restituisce un ProblemDetails 500.

## API

Avviare dalla cartella backend:

    dotnet run --project src/LogisticsEngine.WebApi --urls http://localhost:5000

| Metodo | Percorso | Esito |
| --- | --- | --- |
| POST | /api/hubs | 201 + Location, oppure 400 |
| GET | /api/hubs | 200, anche con elenco vuoto |
| GET | /api/hubs/{id} | 200 oppure 404 |
| PUT | /api/hubs/{id} | 204, 400 oppure 404 |
| DELETE | /api/hubs/{id} | 204 oppure 404 |

POST e PUT accettano:

    { "name": "Milano", "zone": "NORD" }

I file vengono creati in src/LogisticsEngine.WebApi/data, relativamente al content root
della Web API. La configurazione DataDirectory permette di cambiare il percorso.
Gli ID sono generati dal server; i nomi duplicati sono consentiti.

Il repository singleton serializza l'accesso nel singolo processo e scrive prima un
file temporaneo, poi lo rinomina. È una soluzione didattica: non coordina più processi;
gli aggiornamenti concorrenti seguono la regola dell'ultima scrittura.

## Verifica

Eseguire dalla cartella backend (dove si trova la soluzione):

    dotnet restore LogisticsEngine.slnx
    dotnet build LogisticsEngine.slnx --configuration Release
    dotnet format LogisticsEngine.slnx --verify-no-changes
    dotnet test LogisticsEngine.slnx --configuration Release

I test unitari coprono vincoli e limiti, normalizzazione, uguaglianza dei value object,
aggiornamenti atomici e Result. I test di integrazione usano cartelle temporanee isolate
per verificare API, MediatR e persistenza reale. I test architetturali verificano le dipendenze.
