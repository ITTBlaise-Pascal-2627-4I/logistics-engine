using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Common;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Application.Hubs.AlgorithmsServiceHubs;

namespace LogisticsEngine.Console.Controllers;

/// <summary>Presenta il menu Hub e mantiene l'interazione testuale fuori dai livelli di dominio.</summary>
public sealed class HubController(
    ICommandHandler<GetAllCommand, IReadOnlyList<HubDto>> getAll,
    ICommandHandler<RegisterHubCommand, Result<HubDto>> registerHub,
    ICommandHandler<GetByIdCommand, Result<HubDto>> getById,
    ICommandHandler<UpdateHubCommand, Result> updateHub,
    ICommandHandler<DeleteHubCommand, Result> deleteHub,
    IHubAlgorithmsService algorithms)
{
    public void Run()
    {
        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n--- Gestione Hub ---\n1. Elenca\n2. Crea\n3. Dettaglio\n4. Modifica\n5. Elimina\n6. Ordina per ID\n7. Raggruppa per zona\n0. Esci");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": List(); break;
                case "2": Create(); break;
                case "3": Show(); break;
                case "4": Update(); break;
                case "5": Delete(); break;
                case "6": Sort(); break;
                case "7": GroupByZone(); break;
                case "0": running = false; break;
                default: System.Console.WriteLine("Scelta non valida."); break;
            }
        }
    }

    private void List()
    {
        IReadOnlyList<HubDto> hubs = getAll.Handle(new GetAllCommand());
        if (hubs.Count == 0)
        {
            System.Console.WriteLine("Nessun Hub presente.");
            return;
        }

        foreach (HubDto hub in hubs)
        {
            System.Console.WriteLine($"{hub.Id} | {hub.Name} | {hub.Zone}");
        }
    }

    private void Create()
    {
        (string? name, string? zone) = ReadHubDetails();
        Result<HubDto> result = registerHub.Handle(new RegisterHubCommand(name, zone));
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"Hub creato: {result.Value.Id}");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Show()
    {
        Result<HubDto> result = getById.Handle(new GetByIdCommand(ReadGuid()));
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"{result.Value.Id} | {result.Value.Name} | {result.Value.Zone}");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Update()
    {
        Guid id = ReadGuid();
        (string? name, string? zone) = ReadHubDetails();
        Result result = updateHub.Handle(new UpdateHubCommand(id, name, zone));
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub aggiornato.");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Delete()
    {
        Result result = deleteHub.Handle(new DeleteHubCommand(ReadGuid()));
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub eliminato.");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Sort()
    {
        System.Console.WriteLine("Ordinamento: 1. Selection Sort imperativo  2. Selection Sort funzionale  3. Sort C# integrato");
        HubSortAlgorithm? algorithm = System.Console.ReadLine()?.Trim() switch
        {
            "1" => HubSortAlgorithm.SelectionSortImperative,
            "2" => HubSortAlgorithm.SelectionSortFunctional,
            "3" => HubSortAlgorithm.BuiltInSort,
            _ => null
        };

        if (algorithm is null)
        {
            System.Console.WriteLine("Scelta non valida.");
            return;
        }

        foreach (HubDto hub in algorithms.SortById(algorithm.Value))
        {
            System.Console.WriteLine($"{hub.Id} | {hub.Name} | {hub.Zone}");
        }
    }

    private void GroupByZone()
    {
        foreach (IReadOnlyList<HubDto> group in algorithms.GroupByZone())
        {
            System.Console.WriteLine($"Zona {group[0].Zone}:");
            foreach (HubDto hub in group)
            {
                System.Console.WriteLine($"  {hub.Id} | {hub.Name}");
            }
        }
    }

    private static (string? Name, string? Zone) ReadHubDetails()
    {
        System.Console.Write("Nome: ");
        string? name = System.Console.ReadLine();
        System.Console.Write("Zona (2-10 lettere): ");
        string? zone = System.Console.ReadLine();
        return (name, zone);
    }

    private static Guid ReadGuid()
    {
        System.Console.Write("ID Hub: ");
        return Guid.TryParse(System.Console.ReadLine(), out Guid id) ? id : Guid.Empty;
    }

    private static void ShowError(Error error) => System.Console.WriteLine($"Errore [{error.Code}]: {error.Description}");
}
