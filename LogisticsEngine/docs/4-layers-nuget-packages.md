# NuGet Packages per layer

## SharedKernel
MediatR — per IDomainEvent : INotification


## Domain
Nessun pacchetto NuGet esterno. Solo riferimento a SharedKernel.


## Application
MediatR — commands, queries, pipeline behaviors
FluentValidation — validators
FluentValidation.DependencyInjectionExtensions — registrazione automatica validators
Microsoft.Extensions.Logging.Abstractions — ILogger nel LoggingBehavior
Microsoft.Extensions.DependencyInjection.Abstractions — IServiceCollection in DependencyInjection.cs


## Infrastructure
MongoDB.Driver — MongoOrderRepository / MongoCourierRepository
Microsoft.Extensions.Caching.Memory — InMemoryRouteCache
Microsoft.Extensions.Configuration.Abstractions — lettura appsettings in DependencyInjection.cs
Microsoft.Extensions.DependencyInjection.Abstractions — IServiceCollection
Microsoft.Extensions.Configuration.Binder
Microsoft.Extensions.Options.ConfigurationExtensions

## Api
Swashbuckle.AspNetCore — Swagger UI + OpenAPI spec
Microsoft.AspNetCore.OpenApi — supporto OpenAPI nativo .NET 9
Swashbuckle.AspNetCore.Annotations — [SwaggerOperation], [ProducesResponseType] arricchiti
