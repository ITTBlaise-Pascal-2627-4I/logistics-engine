# Pipeline CI e regole di merge

## Flusso di lavoro

1. **Branch dedicato** - ogni funzionalità nasce su un branch (`feature/...`).
2. **Pull Request verso `main`** - mai push diretto su `main`.
3. **Controlli automatici** - la pipeline [.github/workflows/ci.yml](../.github/workflows/ci.yml) parte ai push su `main` e all'apertura o aggiornamento delle PR verso `main`.
4. **Code review** - il reviewer legge i dati oggettivi prodotti dalla pipeline (esito step, coverage, test results).
5. **Merge consentito solo se i controlli obbligatori passano** (branch protection, vedi sotto).

## Cosa esegue la pipeline

| Step | Strumento | Cosa verifica |
|---|---|---|
| Restore | `dotnet restore` | Risoluzione dipendenze NuGet |
| Build + analisi statica | `dotnet build` + analyzer Roslyn | `TreatWarningsAsErrors`, `EnableNETAnalyzers` ed `EnforceCodeStyleInBuild` (vedi `backend/Directory.Build.props`): i warning emessi fanno fallire la build |
| Format check | `dotnet format --verify-no-changes` | Aderenza a `.editorconfig` (whitespace, naming, using inutilizzati) |
| Unit test + coverage | `dotnet test` + coverlet.msbuild | Test di dominio e application; **soglia minima 50% di line coverage sul totale di Domain + Application**, sotto la quale il job fallisce |
| Architecture test | xUnit + NetArchTest | Regole di dipendenza (vedi sotto) |
| Integration test | xUnit + `WebApplicationFactory` | API HTTP end-to-end (controller → MediatR → dominio → repository JSON) |
| Artefatti | upload-artifact | Report cobertura + file `.trx` scaricabili dalla run, come dati oggettivi per la review |

## Regole architetturali automatizzate

In `backend/tests/LogisticsEngine.ArchitectureTests`:

- Domain non dipende da Application, Infrastructure o WebApi.
- SharedKernel non dipende dagli altri layer.
- Application non dipende da Infrastructure o WebApi.
- Infrastructure non dipende da WebApi.
- Domain non dipende da Entity Framework o dai driver elencati nel test.

Le convenzioni di naming non sono attualmente verificate da questa suite.

## Rendere i controlli obbligatori su GitHub

Dopo il primo push del repository:

1. **Settings → Branches → Add branch ruleset** (o *Add classic branch protection rule*) su `main`.
2. Abilitare **Require a pull request before merging**.
   Richiedere almeno un'approvazione del compagno.
3. Abilitare **Require status checks to pass before merging** e selezionare il check **`Build, analyze and test`**.
4. (Consigliato) **Require branches to be up to date before merging**.

Da quel momento il merge della PR è bloccato finché la pipeline non è verde.

## Eseguire gli stessi controlli in locale

Eseguire dalla cartella `backend`:

```powershell
cd backend
dotnet restore LogisticsEngine.slnx
dotnet build LogisticsEngine.slnx --configuration Release
dotnet format LogisticsEngine.slnx --verify-no-changes
dotnet test LogisticsEngine.slnx --configuration Release
```

Questi comandi eseguono tutte le suite ma non applicano la soglia di coverage. Per il comando completo e il percorso issue → branch → PR → merge, seguire [workflow-groups.md](workflow-groups.md).
