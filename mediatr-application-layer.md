# MediatR nell'Application layer

Questa guida spiega perché i casi d'uso passano da MediatR e come una richiesta arriva al relativo handler nella soluzione.

## Che cos'è MediatR

MediatR è un mediator **in-process**: il chiamante invia una richiesta (`IRequest<TResponse>`) tramite `ISender`, senza dipendere direttamente dalla classe handler. MediatR trova l'handler registrato e restituisce la risposta. Non è un message broker e non invia messaggi tra processi o servizi remoti.

Nel progetto:

- un'azione che modifica dati è un `Command`;
- una lettura è una `Query`;
- ogni richiesta implementa `IRequest<TResponse>`;
- ogni caso d'uso è implementato da `IRequestHandler<TRequest, TResponse>`;
- la Console usa `ISender.Send` e non costruisce gli handler.

## Esempio: registrare un Hub

Il comando descrive i dati richiesti dal caso d'uso e il tipo della risposta:

```csharp
public sealed record RegisterHubCommand(string? Name, string? Zone)
    : IRequest<Result<HubDto>>;
```

L'handler coordina validazione, regole di dominio e persistenza:

```csharp
public sealed class RegisterHubCommandHandler(
    IHubRepository repository,
    IHubDomainService domainService,
    IValidator<RegisterHubCommand> validator)
    : IRequestHandler<RegisterHubCommand, Result<HubDto>>
{
    public Task<Result<HubDto>> Handle(
        RegisterHubCommand command,
        CancellationToken cancellationToken)
    {
        var validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Task.FromResult(
                Result.Failure<HubDto>(HubValidation.ToError(validation)));
        }

        Result uniqueName = domainService.EnsureNameIsUnique(command.Name);
        if (uniqueName.IsFailure)
        {
            return Task.FromResult(Result.Failure<HubDto>(uniqueName.Error));
        }

        Result<Hub> hub = Hub.Create(Guid.NewGuid(), command.Name, command.Zone);
        if (hub.IsFailure)
        {
            return Task.FromResult(Result.Failure<HubDto>(hub.Error));
        }

        repository.Add(hub.Value);
        return Task.FromResult(Result.Success(HubDto.From(hub.Value)));
    }
}
```

Il chiamante conosce solo il contratto MediatR:

```csharp
Result<HubDto> result = await sender.Send(
    new RegisterHubCommand("Milano", "NORD"), cancellationToken);
```

La registrazione dei handler viene fatta una volta, nella composizione dell'Application:

```csharp
services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
```

Il progetto registra separatamente anche i validator FluentValidation e i servizi richiesti dagli handler.

## Perché usarlo

- **Basso accoppiamento tra chiamante e handler.** La Console dipende da `ISender`, non da un handler concreto o da un'interfaccia handler per ogni caso d'uso.
- **Un caso d'uso per richiesta.** La responsabilità di ogni handler è circoscritta a un comando o a una query.
- **Composizione uniforme.** È possibile aggiungere comportamenti trasversali, come logging, metriche o validazione, tramite pipeline behavior senza inserirli in ogni handler.
- **Test mirati.** Un handler si può testare con dipendenze sostituite; un test d'integrazione può inviare il request attraverso `ISender` e verificare che il container trovi l'handler.
- **Evoluzione della Console o di un'API.** Più interfacce possono inviare gli stessi request senza dipendere l'una dall'altra.

## Costi e svantaggi

- **Più indirezione.** Per seguire una chiamata occorre passare da `Send` al request e poi all'handler; navigare il codice richiede un passaggio in più.
- **Più tipi e configurazione.** Ogni operazione introduce request e handler, oltre alla registrazione dei servizi. Per operazioni minime può essere più codice del necessario.
- **Errori a runtime nel container.** Un handler o una sua dipendenza non registrata può emergere quando il request viene inviato, se il grafo DI non viene verificato all'avvio o nei test.
- **Non sostituisce l'architettura.** MediatR non decide le regole di dominio, non rende automaticamente asincrona la persistenza e non trasforma il sistema in CQRS distribuito.
- **Possibile abuso dei behavior.** Una pipeline behavior dovrebbe gestire preoccupazioni trasversali; regole specifiche di un singolo caso d'uso restano più chiare nell'handler o nel dominio.

## Validazione e pipeline

FluentValidation e MediatR sono strumenti distinti. Il validator dichiara e valuta le regole sui dati del request; l'handler interpreta l'esito e restituisce il `Result` usato dall'applicazione. Quando più casi d'uso condividono lo stesso flusso di validazione, si può introdurre una pipeline behavior. Il progetto mantiene al momento l'invocazione esplicita del validator negli handler Hub, così il mapping degli errori rimane visibile nel caso d'uso.

## Quando è adatto

MediatR è utile quando l'applicazione ha diversi casi d'uso, più chiamanti o esigenze trasversali che beneficiano di un punto d'invio comune. Per un'applicazione piccola con pochi metodi, l'indirezione può superare il vantaggio: la scelta va fatta in base alla complessità reale, non per applicare un pattern a ogni costo.
