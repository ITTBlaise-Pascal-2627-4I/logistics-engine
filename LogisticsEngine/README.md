# LogisticsEngine

Template essenziale C# / .NET 10. La cartella `frontend/` è vuota.

## Struttura

```text
backend/
  LogisticsEngine.slnx
  Directory.Build.props
  Directory.Packages.props
  global.json
  src/
    LogisticsEngine.SharedKernel/
    LogisticsEngine.Domain/
    LogisticsEngine.Application/
    LogisticsEngine.Infrastructure/
    LogisticsEngine.WebApi/
  tests/
    LogisticsEngine.UnitTests/
    LogisticsEngine.IntegrationTests/
    LogisticsEngine.ArchitectureTests/
frontend/                           (vuota)
```

| Layer | Responsabilità | Riferimenti |
|---|---|---|
| SharedKernel | Tipi comuni Result ed Error | Nessun layer o pacchetto |
| Domain | Entità e regole di business da aggiungere | SharedKernel |
| Application | Casi d'uso, handler MediatR e validator FluentValidation | Domain |
| Infrastructure | Implementazioni delle interfacce applicative da aggiungere | Application |
| WebApi | ASP.NET Core, OpenAPI e composizione della dependency injection | Application, Infrastructure |

Application registra automaticamente handler e validator del proprio assembly.
La pipeline MediatR esegue la validazione; la Web API traduce gli errori FluentValidation in HTTP 400.
Infrastructure espone il metodo di registrazione delle future implementazioni.
Non sono inclusi moduli di business, database, cache, Docker o frontend.

## Dipendenze e test

Le versioni NuGet sono centralizzate in `backend/Directory.Packages.props`.

- Application: MediatR, FluentValidation e integrazione con dependency injection.
- WebApi: ASP.NET Core (SDK Web), Microsoft.AspNetCore.OpenApi e Microsoft.OpenApi.
- UnitTests: riferimenti a Domain/Application, xUnit, Shouldly e NSubstitute.
- IntegrationTests: riferimento a WebApi, Microsoft.AspNetCore.Mvc.Testing e WebApplicationFactory.
- ArchitectureTests: riferimento a WebApi (con i layer transitivi) e NetArchTest.Rules.
- Tutti i test: Microsoft.NET.Test.Sdk, xUnit runner e coverlet.collector.

I test di esempio verificano la pipeline di validazione, l'avvio HTTP e le dipendenze fra layer.
