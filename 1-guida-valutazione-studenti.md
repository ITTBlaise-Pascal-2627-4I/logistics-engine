# Guida alla valutazione del progetto

Questa guida spiega come verrà valutato il progetto (e come si collega al colloquio orale) durante l'anno. La valutazione si basa su **dati oggettivi prodotti automaticamente da una pipeline** che gira ad ogni Pull Request. Il progetto di riferimento (`LogisticsEngine`, nel nostro caso) è l'esempio concreto da cui ogni gruppo può copiare la struttura.

## 1. Perché funziona così

In un progetto software vero nessuno valuta (o dovrebbe valutare) "a occhio" se il codice è buono. Si usano strumenti automatici che controllano, ad ogni modifica, che il codice compili, rispetti lo stile concordato, non abbia regressioni (grazie ai test) e rispetti l'architettura scelta. Lo stesso approccio qui serve a due scopi:

1. **Darvi un feedback immediato e onesto** - se un controllo fallisce lo sapete subito.
2. **Darci una base oggettiva di valutazione** - il voto del progetto non dipende solo dal giudizio del docente, ma da evidenze verificabili (build verde, percentuale di coverage, test che passano, regole architetturali rispettate).

## 2. Il flusso di lavoro obbligatorio

Ogni modifica al codice segue sempre questi passaggi, mai un push diretto su `main`:

1. **Branch dedicato**: per ogni funzionalità si crea un branch (es. `feature/gestione-spedizioni`). Non si lavora mai direttamente su `main`.
2. **Pull Request verso `main`**: quando la funzionalità è pronta si apre una PR. La PR descrive cosa è stato fatto e perché.
3. **Esecuzione automatica dei controlli**: GitHub Actions lancia automaticamente la pipeline (build, test, analisi) ad ogni push sulla PR.
4. **Code review**: un compagno di gruppo (o il docente) legge il codice e i risultati della pipeline prima di approvare.
5. **Merge consentito solo se i controlli obbligatori passano**: su GitHub si configura una *branch protection rule* che blocca il pulsante "Merge" finché la pipeline non è verde. Se un controllo fallisce, la PR resta bloccata: si corregge il codice, si fa un nuovo push, la pipeline riparte.

Questo flusso (branch → PR → CI → review → merge) è lo stesso usato nella maggior parte delle aziende software reali.

## 3. Cosa deve esserci nella repository di ogni gruppo

Ogni gruppo deve mantenere nella propria repository, come minimo:

- **Codice sorgente organizzato per layer** secondo Clean Architecture / DDD: `Domain`, `Application`, `Infrastructure`, `Api` (o nomi equivalenti), come nel progetto di riferimento in `backend/src/`.
- **Tre progetti di test separati**: test unitari, test di integrazione, test di architettura (vedi sezione 4). Nel progetto di riferimento sono in `backend/tests/`.
- **Un workflow CI** in `.github/workflows/` che esegua automaticamente restore, build, format check, static analysis, unit test, integration test, architecture test e coverage (esempio completo in [`.github/workflows/ci.yml`](../.github/workflows/ci.yml) di questa repository).
- **Un `.editorconfig`** con le regole di stile del gruppo (indentazione, naming, ecc.), come quello nella root di questa repository.
- **Un `Directory.Build.props`** (o equivalente) che attivi gli analyzer Roslyn e trasformi i warning in errori di build (vedi sezione 5). Esempio nella root di questa repository.
- **Branch protection su `main`** attiva, con i check della pipeline resi obbligatori (vedi sezione 7).
- **Documentazione minima**: un `README.md` che spieghi come avviare il progetto e come eseguire i controlli in locale.

Il docente che guarda la repository nel suo complesso - non solo l'ultimo commit - quindi la storia delle PR, dei branch e degli esiti della pipeline nel tempo è parte della valutazione tanto quanto il codice finale.

## 4. I test da eseguire

La pipeline esegue tre tipi di test, ciascuno verifica una cosa diversa. Non sono intercambiabili: un progetto con solo unit test, o solo test di integrazione, non è considerato completo.

### 4.1 Unit test (test unitari)

Verificano una singola unità di codice in isolamento (una entità, un metodo di dominio, un handler), senza database, rete o file system. Sono veloci e permettono di individuare subito se una regola di business è stata implementata male. Nel progetto di riferimento coprono `Domain` e `Application` (`backend/tests/Dhexby.LogisticsEngine.UnitTests`).

**Perché**: dimostrano che la logica di dominio - il cuore del progetto, secondo DDD - è corretta e continua a esserlo anche dopo modifiche future.

### 4.2 Integration test (test di integrazione)

Verificano che i vari pezzi del sistema funzionino insieme correttamente: nel progetto di riferimento, una vera chiamata HTTP end-to-end attraverso `WebApplicationFactory` (controller → Application → dominio → repository), vedi `backend/tests/Dhexby.LogisticsEngine.IntegrationTests`.

**Perché**: gli unit test da soli non garantiscono che i layer siano collegati correttamente (es. un controller che chiama il repository sbagliato). Gli integration test lo verificano.

### 4.3 Architecture test (test di architettura)

Non testano il comportamento del programma, ma le **regole strutturali** del progetto, tramite reflection sugli assembly compilati. Nel progetto di riferimento sono in `backend/tests/Dhexby.LogisticsEngine.ArchitectureTests` e verificano automaticamente, ad esempio:

- `Domain` non dipende da `Application`
- `Domain` non dipende da `Infrastructure`
- `Application` non dipende da `Infrastructure`
- i Controller dipendono da `Application`, mai direttamente dai repository
- i repository concreti si trovano solo in `Infrastructure`
- le classi handler terminano con `Handler`
- le classi command terminano con `Command`
- le classi query terminano con `Query`

**Perché**: in Clean Architecture la regola più importante (DIP) è la direzione delle dipendenze (tutto punta verso il Domain, mai il contrario).

### 4.4 Code coverage (copertura del codice)

Non è un tipo di test a sé, ma una misura: quale percentuale di codice viene effettivamente eseguita dai test. Nel progetto di riferimento la soglia minima è **80% di line coverage su Domain + Application**; sotto questa soglia il job fallisce.

**Perché**: una coverage bassa significa che gran parte del codice non è mai stata verificata da un test - potrebbe contenere bug che nessuno ha ancora scoperto. La coverage da sola non basta a garantire qualità (si può scrivere un test inutile solo per "toccare" una riga), ma una coverage troppo bassa è un segnale d'allarme oggettivo.

## 5. Analisi statica: cosa controllano gli analyzer Roslyn

Oltre ai test, la build stessa esegue analisi statica tramite gli analyzer Roslyn di .NET, configurati per bloccare la build (`TreatWarningsAsErrors`) se emettono un warning. Controllano automaticamente:

- errori potenziali (es. possibili `NullReferenceException`)
- uso scorretto delle API
- nullabilità (uso corretto di `Nullable` reference types)
- gestione delle risorse (es. `IDisposable` non rilasciati)
- prestazioni (es. allocazioni inutili)
- manutenibilità
- convenzioni e stile di codice (in base a `.editorconfig`, verificate anche dal comando `dotnet format --verify-no-changes`)


## 6. Altre metriche di qualità osservate

Oltre a coverage e warning, durante la code review si tiene conto di:

- **numero di warning**: idealmente zero, dato che la build li tratta come errori
- **complessità ciclomatica**: quanti percorsi logici diversi attraversa un metodo - un valore alto indica un metodo difficile da capire e da testare
- **lunghezza e responsabilità dei metodi e delle classi**: un metodo oltre le 30–40 righe o una classe oltre le 300–400 righe è quasi sempre un segnale che sta facendo troppe cose (viola il Single Responsibility Principle)
- **numero di parametri di un metodo**: più di 4–5 parametri suggerisce che andrebbero raggruppati in un oggetto
- **livelli di nesting**: oltre 3 livelli di `if`/`for` annidati rendono il codice difficile da seguire
- **numero di dipendenze nel costruttore**: un costruttore con troppe dipendenze indica una classe che fa troppo e andrebbe scomposta

Queste metriche non bloccano automaticamente il merge come i controlli della pipeline, ma sono criteri espliciti usati in code review e nel colloquio orale: se emergono, dovete saper spiegare la scelta.

## 7. Rendere i controlli obbligatori su GitHub

Ogni gruppo deve configurare la propria repository così:

1. **Settings → Branches → Add branch ruleset** (o *Add classic branch protection rule*) su `main`.
2. Abilitare **Require a pull request before merging**.
3. Abilitare **Require status checks to pass before merging** e selezionare il check della pipeline CI.
4. (Consigliato) **Require branches to be up to date before merging**.

Da quel momento il pulsante di merge resta disabilitato finché la pipeline non è verde: nessuno, può bypassare i controlli.


## 8. Colloquio orale

- Domande sugli argomenti svolti con uno sguardo critico e aperto a nuove applicazioni in altri ambiti;
- Live Coding: implementazione di una feature nuova o modifica di una feature esistente in modalità live;
- Lettura di codice implementato o nuovo.


## 9. Riepilogo: checklist minima per gruppo

- [ ] Struttura del codice in layer (`Domain`, `Application`, `Infrastructure`, `Api`)
- [ ] Progetto di unit test su Domain + Application
- [ ] Progetto di integration test end-to-end
- [ ] Progetto di architecture test con le regole di dipendenza e naming
- [ ] Workflow CI in `.github/workflows/` con restore, build, format check, analisi statica, unit test, integration test, architecture test, coverage
- [ ] `.editorconfig` e `Directory.Build.props` (o equivalenti) configurati
- [ ] Branch protection su `main` attiva e obbligatoria
- [ ] Nessun commit diretto su `main`: tutto passa da branch + PR + review
- [ ] README con istruzioni per avviare il progetto ed eseguire i controlli in locale

Per un esempio completo e funzionante di tutti questi elementi, potete guardare direttamente questa repository (`LogisticsEngine`): struttura dei layer in `backend/src/`, test in `backend/tests/`, pipeline in `.github/workflows/ci.yml`, dettagli tecnici aggiuntivi in [`docs/ci-pipeline.md`](./ci-pipeline.md).
