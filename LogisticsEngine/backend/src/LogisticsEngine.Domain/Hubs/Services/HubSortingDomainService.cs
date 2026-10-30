using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Implementa ordinamenti puri sul dominio: non cambia la collezione ricevuta né gli Hub.</summary>
public sealed class HubSortingDomainService : IHubSortingDomainService
{
    public IReadOnlyList<Hub> SortById(IEnumerable<Hub> hubs, HubSortAlgorithm algorithm)
    {
        Hub[] input = hubs.ToArray();
        return algorithm switch
        {
            HubSortAlgorithm.SelectionSortImperative => SelectionSortImperative(input),
            HubSortAlgorithm.SelectionSortFunctional => SelectionSortFunctional(input),
            HubSortAlgorithm.BuiltInSort => BuiltInSort(input),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Algoritmo di ordinamento non supportato.")
        };
    }

    /// <summary>Usa Array.Sort come confronto con gli algoritmi didattici implementati manualmente.</summary>
    private static IReadOnlyList<Hub> BuiltInSort(Hub[] input)
    {
        Array.Sort(input, (left, right) => left.Id.CompareTo(right.Id));
        return input;
    }

    /// <summary>Selection sort classico: cerca il minimo e lo scambia nella posizione corretta.</summary>
    private static IReadOnlyList<Hub> SelectionSortImperative(Hub[] input)
    {
        for (int start = 0; start < input.Length - 1; start++)
        {
            int min = start;
            for (int candidate = start + 1; candidate < input.Length; candidate++)
            {
                if (input[candidate].Id.CompareTo(input[min].Id) < 0)
                {
                    min = candidate;
                }
            }

            // Tuple, average Python-maxing data structure
            (input[start], input[min]) = (input[min], input[start]);
        }

        return input;
    }

    /// <summary>Selection sort funzionale: seleziona minimo e ricostruisce una nuova sequenza ricorsivamente.</summary>
    private static IReadOnlyList<Hub> SelectionSortFunctional(Hub[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }
        int minimumIndex = Array.FindIndex(input, hub => hub.Id == input.MinBy(candidate => candidate.Id)!.Id);
        Hub minimum = input[minimumIndex];
        Hub[] rest = input.Where((_, index) => index != minimumIndex).ToArray();
        return new[] { minimum }.Concat(SelectionSortFunctional(rest)).ToArray();
    }
}
