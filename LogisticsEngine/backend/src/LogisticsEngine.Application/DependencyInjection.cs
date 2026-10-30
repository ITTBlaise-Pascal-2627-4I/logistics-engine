using LogisticsEngine.Application.Common;
using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Hubs.AlgorithmsServiceHubs;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Application;

/// <summary>Registra i servizi applicativi e i casi d'uso nel container di dependency injection.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IHubDomainService, HubDomainService>();
        services.AddSingleton<IHubSortingDomainService, HubSortingDomainService>();
        services.AddSingleton<IHubGroupingDomainService, HubGroupingDomainService>();
        services.AddSingleton<IHubAlgorithmsService, HubAlgorithmsService>();

        // I command handler sono registrati tramite il contratto sincrono della demo.
        services.AddTransient<ICommandHandler<GetAllCommand, IReadOnlyList<HubDto>>, GetAllCommandHandler>();
        services.AddTransient<ICommandHandler<RegisterHubCommand, Result<HubDto>>, RegisterHubCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteHubCommand, Result>, DeleteHubCommandHandler>();
        services.AddTransient<ICommandHandler<UpdateHubCommand, Result>, UpdateHubCommandHandler>();
        services.AddTransient<ICommandHandler<GetByIdCommand, Result<HubDto>>, GetByIdCommandHandler>();
        return services;
    }
}
