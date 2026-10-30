using LogisticsEngine.Application;
using LogisticsEngine.Console.Controllers;
using LogisticsEngine.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

/// <summary>Composition root: punto unico in cui l'applicazione collega interfacce e implementazioni.</summary>
public class Program
{
    public static void Main()
    {
        // I dati JSON vengono conservati nella cartella data accanto all'applicazione avviata. In Contesti reali su utilizza un DB su un'altro server (o generalemnte altra macchina).
        string dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");

        // ServiceCollection è il contenitore DI: gli oggetti verranno costruiti e collegati da esso.
        ServiceCollection services = new();

        // registro i casi d'uso e le implementazioni dei servizi applicativi tramite le loro interfacce.
        services.AddApplication();

        // registro le tipologie di servizi esterni e di persistenza previsti.
        services.AddInfrastructure(dataDirectory);

        // Il controller riceve le proprie dipendenze dal container, senza creare servizi concreti.
        services.AddTransient<HubController>();

        // Il provider risolve l'intero grafo di dipendenze e viene rilasciato alla fine del Main.
        using ServiceProvider provider = services.BuildServiceProvider();

        // Avvia il menu console dopo che DI ha costruito HubController e i suoi command handler.
        provider.GetRequiredService<HubController>().Run();
    }
}
