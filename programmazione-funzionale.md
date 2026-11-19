# Programmazione funzionale in C# — concetti essenziali

Questa guida introduce alcune idee della programmazione funzionale.

## Funzioni come valori

In C# una funzione può essere conservata in una variabile o passata a un altro metodo. I delegate generici più comuni sono:

- `Func<TInput, TResult>`: riceve un valore e restituisce un risultato;
- `Action<TInput>`: riceve un valore e non restituisce un risultato;
- `Predicate<T>`: riceve un valore e restituisce `bool`.

```csharp
Func<int, bool> isEven = number => number % 2 == 0;
Action<string> print = message => Console.WriteLine(message);
Predicate<string> hasText = text => !string.IsNullOrWhiteSpace(text);

bool result = isEven(8); // true
```

Una lambda è una forma breve per esprimere una funzione. In `number => number % 2 == 0`, `number` è il parametro e l’espressione a destra è il valore restituito.

## Funzioni che ricevono altre funzioni

Un metodo che accetta una funzione può lasciare al chiamante la scelta del comportamento. Per esempio, `Where` riceve una condizione e seleziona gli elementi per cui la condizione restituisce `true`:

```csharp
IEnumerable<int> evenNumbers = numbers.Where(number => number % 2 == 0);
```

`Where` non deve conoscere in anticipo quale condizione verrà usata: la riceve come parametro. Questo stile è chiamato *funzione di ordine superiore*.

Anche `OrderBy` riceve una funzione, che indica quale chiave estrarre da ogni elemento:

```csharp
IEnumerable<Hub> byName = hubs.OrderBy(hub => hub.Name.Value);
```

## Il sorter del progetto

`ISorter<T>` rende esplicita questa idea usando `Func<T, TKey>`: chi usa il sorter sceglie la proprietà che determina l’ordine.

```csharp
public interface ISorter<T>
{
    IReadOnlyList<T> Sort<TKey>(
        IEnumerable<T> items,
        Func<T, TKey> keySelector,
        IComparer<TKey>? comparer = null);
}
```

Un domain service può così esprimere ordinamenti specifici di Hub senza duplicare l’algoritmo generico:

```csharp
sorter.Sort(hubs, hub => hub.Id);
sorter.Sort(hubs, hub => hub.Name.Value, StringComparer.Ordinal);
sorter.Sort(hubs, hub => hub.Zone.Code, StringComparer.Ordinal);
```

La lambda `hub => hub.Name.Value` è il selettore di chiave. `Sorter<T>` applica l’ordinamento; il domain service decide quale informazione rappresenta l’ordinamento richiesto.

## Funzioni pure e dati immutabili

Una funzione pura dipende solo dai suoi argomenti, restituisce lo stesso risultato per gli stessi argomenti e non modifica dati esterni. È facile da verificare perché non dipende dallo stato dell’applicazione.

```csharp
static int Double(int value) => value * 2;
```

`Double(4)` restituisce sempre `8` e non cambia `value` o altre variabili. Al contrario, una funzione che aggiorna un repository ha un effetto osservabile e non è pura.

Preferire dati immutabili e funzioni senza effetti collaterali dove possibile rende le regole più semplici da leggere e testare. Gli effetti, come persistenza e chiamate esterne, restano normalmente nei casi d’uso e nei repository.

## Comporre trasformazioni con LINQ

Le operazioni LINQ possono essere concatenate: ogni passaggio trasforma o seleziona i dati per il passaggio successivo.

```csharp
IReadOnlyList<string> names = hubs
    .Where(hub => hub.Zone.Code == "NORD")
    .Select(hub => hub.Name.Value)
    .OrderBy(name => name, StringComparer.Ordinal)
    .ToArray();
```

Qui si filtrano gli Hub per zona, si estrae il nome e si ordinano i nomi. Ogni lambda descrive una regola locale; la sequenza descrive il flusso complessivo.

## Come si legge FluentValidation

FluentValidation usa una catena di chiamate per dichiarare regole sul comando. `RuleFor` seleziona una proprietà con una lambda; i validator successivi specificano i vincoli.

```csharp
RuleFor(command => command.Id)
    .NotEmpty()
    .WithErrorCode(HubErrors.InvalidId.Code);

RuleFor(command => command.Zone)
    .Must(value => Zone.Create(value).IsSuccess)
    .WithErrorCode(HubErrors.InvalidZone.Code);
```

`command => command.Id` seleziona il campo da validare. `Must(value => ...)` riceve una funzione che restituisce `true` quando il valore è valido. La validazione produce un esito che l’handler controlla prima di eseguire il caso d’uso.

Nel progetto il value object resta la fonte delle regole di dominio: la lambda `value => Zone.Create(value).IsSuccess` riusa la stessa validazione di `Zone`, mentre FluentValidation organizza la verifica del comando e i relativi errori.
