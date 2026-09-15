# Pipeline CI e regole di merge

## Flusso di lavoro

1. **Branch dedicato** - ogni funzionalità nasce su un branch (`feature/...`).
2. **Pull Request verso `main`** - mai push diretto su `main`.
3. **Controlli automatici** - la pipeline [.github/workflows/ci.yml](../.github/workflows/ci.yml) parte a ogni push e PR.
4. **Code review** - il reviewer legge i dati oggettivi prodotti dalla pipeline (esito step, coverage, test results).
5. **Merge consentito solo se i controlli obbligatori passano** (branch protection, vedi sotto).

## Cosa esegue la pipeline

| Step | Strumento | Cosa verifica |
|---|---|---|
| Restore | `dotnet restore` | Risoluzione dipendenze NuGet |
| Build + analisi statica | `dotnet build` + analyzer Roslyn | Compilazione; `TreatWarningsAsErrors` + `AnalysisLevel latest-recommended` + `EnforceCodeStyleInBuild` (vedi `Directory.Build.props`): ogni warning (nullabilità, API scorrette, prestazioni, convenzioni) fa fallire la build |
| Format check | `dotnet format --verify-no-changes` | Aderenza a `.editorconfig` (whitespace, naming, using inutilizzati) |
| Unit test + coverage | `dotnet test` + coverlet | Test di dominio e application; **soglia minima 80% di line coverage su Domain + Application**, sotto la quale il job fallisce |
| Architecture test | xUnit + reflection | Regole di dipendenza e convenzioni (vedi sotto) |
| Integration test | xUnit + `WebApplicationFactory` | API HTTP end-to-end (controller → MediatR → dominio → repository JSON) |
| Artefatti | upload-artifact | Report cobertura + file `.trx` scaricabili dalla run, come dati oggettivi per la review |

## Regole architetturali automatizzate

In `backend/tests/Dhexby.LogisticsEngine.ArchitectureTests`:

- Domain non dipende da Application, Infrastructure, Api
- Application non dipende da Infrastructure, Api
- Infrastructure non dipende da Api
- I controller dipendono da Application (MediatR), mai dai repository
- I repository concreti vivono solo in Infrastructure
- Gli handler terminano con `Handler`
- I command terminano con `Command`, le query con `Query`
- I controller terminano con `Controller`

## Rendere i controlli obbligatori su GitHub

Dopo il primo push del repository:

1. **Settings → Branches → Add branch ruleset** (o *Add classic branch protection rule*) su `main`.
2. Abilitare **Require a pull request before merging**.
3. Abilitare **Require status checks to pass before merging** e selezionare il check **`Build, analyze and test`**.
4. (Consigliato) **Require branches to be up to date before merging**.

Da quel momento il merge della PR è bloccato finché la pipeline non è verde.

## Eseguire gli stessi controlli in locale

```bash
dotnet restore LogisticsEngine.slnx
dotnet build LogisticsEngine.slnx --configuration Release
dotnet format LogisticsEngine.slnx --verify-no-changes
dotnet test LogisticsEngine.slnx --configuration Release
```
