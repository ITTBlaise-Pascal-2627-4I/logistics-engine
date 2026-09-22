# Workflow di gruppo: dalla issue al merge

Procedura da ripetere per ogni feature. La CI è già configurata in [ci.yml](../.github/workflows/ci.yml): non dovete creare il workflow né aggiungere pacchetti al progetto.

Lavorate in gruppo, con account GitHub e copie locali distinti: A sviluppa, B revisiona. Alla feature successiva scambiatevi i ruoli.

```text
Issue → main aggiornato → branch → modifica e test locali → commit → push
→ PR collegata alla issue → CI verde → revisione → merge
→ issue chiusa → aggiornamento locale di entrambi
```

## 1. Una sola volta: clonare il progetto

Servono Git, SDK .NET 10 e accesso al repository GitHub del gruppo. I comandi locali sono per PowerShell. Sostituite URL, nome ed email con i vostri dati:

```powershell
git --version
dotnet --list-sdks
git clone https://github.com/ORGANIZZAZIONE/REPOSITORY-GRUPPO.git laboratorio-gruppo
cd laboratorio-gruppo
git config user.name "Nome Cognome"
git config user.email "EMAIL_DEL_PROPRIO_ACCOUNT"
git remote -v
```

Se avete già una copia, entrate nella sua cartella senza clonare di nuovo. Autenticatevi su GitHub quando richiesto.

## 2. Aprire una issue prima di scrivere codice

A seleziona **Issues → New issue** nel repository GitHub.

Titolo di esempio: **Consentire nomi degli hub fino a 120 caratteri**.

Descrizione:

```markdown
## Richiesta
Portare da 100 a 120 il limite del nome degli hub, mantenendo
la rimozione degli spazi iniziali e finali.

## Criteri di accettazione
- [ ] Un nome di 120 caratteri viene accettato.
- [ ] Un nome di 121 caratteri viene rifiutato.
- [ ] I nomi mancanti o vuoti restano invalidi.
- [ ] Il messaggio di errore indica il nuovo limite.
- [ ] La documentazione è aggiornata.
- [ ] Test locali e CI passano.
```

Assegnare la issue ad A e concordare il requisito con B. Annotare il numero assegnato da GitHub: **negli esempi è 12, da sostituire ovunque con il vostro numero reale**.

## 3. Aggiornare main e creare un branch

Dalla cartella principale del repository:

```powershell
git status
git switch main
git pull --ff-only origin main
git switch -c feature/12-hub-name-limit
git branch --show-current
```

Prima di cambiare branch non devono esserci modifiche pendenti: salvate eventuale lavoro sul branch corretto, senza cancellarlo. Non sviluppate direttamente su `main`.

Ogni feature nasce da un nuovo branch del `main` aggiornato. Usate nomi riconoscibili, per esempio `feature/18-search-hubs`.

## 4. Modificare codice, test e documentazione

L'esempio parte dal limite iniziale di 100. Se è già stato modificato da un esercizio precedente, concordate una nuova feature invece di ripetere una modifica già integrata.

In `backend/src/LogisticsEngine.Domain/Hubs/Hub.cs`, cambiare:

```csharp
if (normalizedName.Length is < 1 or > 100)
```

in:

```csharp
if (normalizedName.Length is < 1 or > 120)
```

In `backend/src/LogisticsEngine.Domain/Hubs/HubErrors.cs`, aggiornare il testo di `InvalidName` da `1 to 100 characters` a `1 to 120 characters`.

In `backend/tests/LogisticsEngine.UnitTests/HubTests.cs`, aggiornare il test:

```csharp
[Fact]
public void Create_checks_id_and_name_length()
{
    Hub.Create(Guid.Empty, "Milano", "NORD").Error.ShouldBe(HubErrors.InvalidId);
    Hub.Create(Guid.NewGuid(), new string('a', 121), "NORD").Error.ShouldBe(HubErrors.InvalidName);
    Hub.Create(Guid.NewGuid(), new string('a', 120), "NORD").IsSuccess.ShouldBeTrue();
    Hub.Create(Guid.NewGuid(), "A", "NORD").IsSuccess.ShouldBeTrue();
}
```

In `backend/README.md`, aggiornare `1–100 caratteri` a `1–120 caratteri`.

Controllare il lavoro:

```powershell
git diff
```

Per ogni feature aggiornate i test secondo i criteri della issue, includendo casi validi, errori e limiti. Non eliminate test falliti per ottenere il verde.

## 5. Eseguire i controlli locali prima del commit

**La soluzione è in `backend`: entrare in quella cartella.** Eseguire un comando alla volta e, in caso di errore, fermarsi e correggerlo prima di continuare.

```powershell
cd backend
dotnet restore LogisticsEngine.slnx
dotnet build LogisticsEngine.slnx --configuration Release
dotnet format LogisticsEngine.slnx --verify-no-changes
dotnet test LogisticsEngine.slnx --configuration Release
```

| Comando | Scopo |
| --- | --- |
| `restore` | Risolve e scarica le dipendenze NuGet. |
| `build` | Compila ed esegue gli analyzer; i warning sono errori. |
| `format --verify-no-changes` | Controlla il formato senza modificare i file. |
| `test` | Esegue test unitari, architetturali e di integrazione. |

Se il formato fallisce:

```powershell
dotnet format LogisticsEngine.slnx
git diff
```

Revisionare le modifiche automatiche e ripetere build, verifica del formato e test. Se fallisce un test, confrontare comportamento atteso, risultato effettivo e requisito.

### Controllare anche la coverage della CI

I quattro comandi precedenti non applicano da soli la soglia di coverage. Per verificare anche questa, sempre da `backend`, dopo una build Release riuscita, incollare tutta la riga:

```powershell
dotnet test tests/LogisticsEngine.UnitTests --no-build --configuration Release --logger "trx;LogFileName=unit-tests.trx" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../../../coverage/ /p:Threshold=50 /p:ThresholdType=line /p:ThresholdStat=total /p:Include="[LogisticsEngine.Domain]*%2c[LogisticsEngine.Application]*"
```

Serve almeno il **50% di line coverage sul totale combinato di Domain e Application**, misurato dai test unitari. Test tutti verdi e coverage sufficiente sono condizioni diverse. Se non raggiungete la soglia, aggiungete test significativi; non abbassatela per aggirare il controllo.

Il report è nella cartella `coverage/` alla radice del repository. Dopo modifiche al codice, ricompilare prima di usare `--no-build`, per evitare di testare una versione vecchia.

Tornare alla radice:

```powershell
cd ..
```

## 6. Fare commit sul proprio branch

Dalla radice:

```powershell
git branch --show-current
git status
git diff
git add backend/src/LogisticsEngine.Domain/Hubs/Hub.cs backend/src/LogisticsEngine.Domain/Hubs/HubErrors.cs backend/tests/LogisticsEngine.UnitTests/HubTests.cs backend/README.md
git diff --cached
git commit -m "feat: consenti nomi hub fino a 120 caratteri (#12)"
git log -1 --oneline
```

Per una feature diversa aggiungere esplicitamente i relativi file. Se il formatter ha modificato altri file necessari, revisionarli e aggiungerli. Non includere report, `bin`, `obj` o dati locali.

`git add` seleziona il contenuto; `git diff --cached` mostra ciò che entrerà nel commit. **Il commit salva solo localmente: GitHub e il compagno non vedono ancora la modifica.**

## 7. Pubblicare con push

```powershell
git push -u origin feature/12-hub-name-limit
```

Il push pubblica il branch. `-u` collega il branch locale a quello remoto: nei successivi invii basta `git push`.

**Con questa configurazione, un push sul branch senza PR aperta non avvia la CI.** Aprire la PR come descritto di seguito. Ogni successivo push sul branch della PR riavvierà i controlli.

## 8. Aprire una PR collegata alla issue

Su GitHub scegliere **Compare & pull request**, oppure **Pull requests → New pull request**:

- **base:** `main`;
- **compare:** `feature/12-hub-name-limit`;
- **titolo:** `Consenti nomi hub fino a 120 caratteri`;
- **reviewer:** B.

Descrizione da adattare:

```markdown
## Modifica
Il limite del nome hub passa da 100 a 120 caratteri.
Aggiornati test, messaggio di errore e documentazione.

## Verifica locale
- dotnet restore LogisticsEngine.slnx
- dotnet build LogisticsEngine.slnx --configuration Release
- dotnet format LogisticsEngine.slnx --verify-no-changes
- dotnet test LogisticsEngine.slnx --configuration Release
- Test unitari con coverage e soglia 50%

Closes #12
```

Indicare come superati soltanto i controlli realmente eseguiti. `Closes #12` collega la PR alla issue e la chiude automaticamente al merge nel branch predefinito, che per il laboratorio deve essere `main`.

## 9. Vedere la pipeline su GitHub

Aprire **Checks** nella PR oppure **Actions → CI → esecuzione più recente → Build, analyze and test**. Verificare che l'esecuzione riguardi l'ultimo commit.

| Step | Cosa deve passare |
| --- | --- |
| Checkout / Setup .NET | Codice scaricato e SDK .NET 10 pronto. |
| Restore | Dipendenze ripristinate. |
| Build (warnings as errors, Roslyn analyzers) | Compilazione senza errori o warning. |
| Format check | Formattazione corretta. |
| Unit tests + coverage gate | Unit test verdi e coverage almeno 50%. |
| Architecture tests | Regole di dipendenza rispettate. |
| Integration tests | Test HTTP e persistenza superati. |
| Upload coverage report / Upload test results | Caricamento dei report prodotti. |

Se uno step fallisce, quelli ordinari successivi vengono saltati. Gli upload con `always()` tentano comunque di conservare i report disponibili. Un upload verde senza file non dimostra che ci sia un report: verificare gli artefatti effettivi.

Nella pagina dell'esecuzione in **Actions**, scaricare dalla sezione **Artifacts**:

- **coverage-report**: report XML Cobertura;
- **test-results**: esiti dei test in formato TRX.

Il log unitario mostra la percentuale di coverage. La CI esegue questi controlli su una macchina GitHub pulita, senza dipendere dai file compilati sul vostro computer.

### Se la CI è rossa

1. Leggere il primo errore nel primo step fallito.
2. Riprodurlo con il comando locale corrispondente.
3. Correggere sullo stesso branch e ripetere tutti i controlli della sezione 5.
4. Dalla radice, aggiungere i file corretti e pubblicare un nuovo commit. Esempio per una correzione al test:

```powershell
git status
git diff
git add backend/tests/LogisticsEngine.UnitTests/HubTests.cs
git diff --cached
git commit -m "fix: correggi i test dei limiti hub (#12)"
git push
```

La stessa PR si aggiorna e la CI riparte: non aprire una nuova PR. Attendere il verde del commit più recente.

## 10. Revisione del compagno

B legge la issue e **Files changed**, poi verifica:

- [ ] Criteri di accettazione soddisfatti.
- [ ] Test con casi validi, invalidi e valori limite.
- [ ] Messaggi e documentazione coerenti.
- [ ] Nessun file generato o modifica estranea.
- [ ] Ultima CI verde e report disponibili.
- [ ] Collegamento `Closes #NUMERO_CORRETTO` nella PR.

Se serve una correzione, B usa **Request changes** e spiega cosa cambiare. A corregge, ripete i controlli, fa commit e push sullo stesso branch. Quando tutto è corretto, B usa **Approve**.

La revisione umana valuta il requisito e la soluzione; la CI esegue le verifiche automatiche configurate.

## 11. Merge e aggiornamento di entrambi

Dopo approvazione e CI verde, A seleziona **Squash and merge** e conferma. Non fare push diretto su `main` per aggirare la PR.

Verificare su GitHub:

1. PR in stato **Merged**;
2. issue chiusa automaticamente;
3. nuova esecuzione CI su `main`, avviata dal merge, conclusa in verde.

Il merge non aggiorna automaticamente i computer. Entrambi, dalla radice e senza modifiche pendenti:

```powershell
git switch main
git pull --ff-only origin main
git log -3 --oneline
git status
```

Il branch remoto può essere eliminato con **Delete branch** nella PR. La feature successiva riparte dal punto 2: B sviluppa e A revisiona. Un secondo esercizio possibile è portare il limite a 150, con nuova issue e test per 150 accettato e 151 rifiutato.

## 12. Se main cambia durante il lavoro

Dal branch della feature e con copia locale pulita:

```powershell
git fetch origin
git merge origin/main
```

Se ci sono conflitti, aprire i file segnalati, scegliere il contenuto corretto ed eliminare i marcatori `<<<<<<<`, `=======`, `>>>>>>>`. Aggiungere i singoli file risolti con `git add` e completare con `git commit`. Se il merge termina automaticamente, il commit aggiuntivo non serve.

Ripetere i controlli della sezione 5 e poi `git push`. Non usare force push per risolvere conflitti.

## 13. Dimostrazione facoltativa: da CI rossa a verde

Il flusso ordinario prevede controlli locali verdi prima del push. Per una dimostrazione concordata con il docente, nella prima feature modificare inizialmente solo il limite in `Hub.cs`, lasciando il vecchio test che rifiuta 101 caratteri.

Eseguire i controlli locali e osservare il fallimento di `Create_checks_id_and_name_length`. Solo per questo esperimento, pubblicare il commit incompleto e aprire la PR indicando che è una prova didattica. Osservare lo stesso errore in CI e non fare merge.

Completare quindi la sezione 4, eseguire tutti i controlli, fare un nuovo commit e push sullo stesso branch. La PR passa da rosso a verde: il requisito era cambiato, ma il test verificava ancora il limite precedente.

## 14. Checklist per ogni feature

- [ ] Issue con criteri di accettazione.
- [ ] Branch da main aggiornato.
- [ ] Codice, test e documentazione aggiornati.
- [ ] Restore, build, format, test e coverage locali superati.
- [ ] Diff verificato, commit e push completati.
- [ ] PR verso main collegata alla issue.
- [ ] CI verde sull'ultimo commit e approvazione del compagno.
- [ ] Merge, chiusura issue e CI verde su main.
- [ ] Copie locali aggiornate con pull.

Consegnare al docente i link di issue, PR ed esecuzione CI e la coverage osservata. Una pipeline verde dimostra che sono passati i controlli configurati, non che il software sia privo di qualsiasi bug.
