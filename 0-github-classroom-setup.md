# Gestione di progetti di laboratorio con GitHub Organization

## Obiettivo

L'obiettivo è creare un ambiente di lavoro il più possibile simile a quello utilizzato nelle aziende di sviluppo software, mantenendo però una gestione semplice per il docente.

Ogni gruppo di studenti dispone di:

* una repository Git privata;
* una board GitHub Projects dedicata;
* una gestione delle attività tramite Issue;
* una pipeline di Continuous Integration (CI);
* un flusso di sviluppo basato su Pull Request e Code Review.

Il docente mantiene il controllo centralizzato di tutte le repository della classe.

---

# Struttura delle Organization

Si consiglia di creare **una GitHub Organization per ogni classe**.

Esempio:

```text
ITT-Blaise-Pascal-2626-4I
ITT-Blaise-Pascal-2626-4E
...
```

In questo modo:

* gli studenti vedono solamente le repository a cui sono stati invitati;
* il docente gestisce tutte le repository della classe da un'unica dashboard;
* è possibile riutilizzare facilmente template, workflow e impostazioni negli anni successivi.

---

# Creazione delle repository

Per ogni gruppo viene creata una repository privata.

Si suggerisce il seguente formato:

```text
<nome-progetto>-<cognome1>-<cognome2>
```

Ad esempio:

```text
logistics-tappi-pulga
logistics-rossi-bianchi
...
```

---


# Gestione dei membri

Ad ogni repository vengono assegnati:

* i componenti del gruppo;
* il docente (Owner o Maintainer).

Gli altri studenti della classe non hanno alcun accesso alla repository.

---

# Organizzazione del lavoro

Ogni gruppo utilizza **GitHub Issues** per rappresentare le attività da svolgere.

Ogni nuova funzionalità viene descritta mediante una Issue.

Esempio:

```text
#1 Implementare domain Shipments
```

Le Issue rappresentano il backlog del progetto.

---

# GitHub Projects

Ad ogni repository viene associato un **GitHub Project**.

La board viene utilizzata per gestire lo stato di avanzamento del progetto.

Una possibile configurazione è la seguente:

```text
Backlog
↓
Da fare
↓
In lavorazione
↓
Code Review
↓
Completato
```

Ogni Issue compare automaticamente nella board.

Gli studenti possono spostare autonomamente le attività durante lo sviluppo.

---

# Flusso di lavoro degli studenti

Quando il docente introduce una nuova funzionalità, il gruppo segue il seguente processo.

## 1. Creazione della Issue

Uno studente crea una nuova Issue.

Esempio:

```text
#1 Implementare domain Shipments
```

---

## 2. Pianificazione

La nuova Issue viene inserita nella colonna:

```text
Backlog
```

---

## 3. Creazione del branch

Uno studente crea un branch dedicato.

Ad esempio:

```text
features/shipments
```
---

## 4. Sviluppo

Durante lo sviluppo:

* vengono effettuati commit frequenti;
* il codice viene testato;
* vengono aggiunti eventuali Unit Test.

**Eseguire controlli in locale preliminari per CI Github Actions:**

```bash
dotnet restore LogisticsEngine.slnx
dotnet build LogisticsEngine.slnx --configuration Release
dotnet format LogisticsEngine.slnx --verify-no-changes
dotnet test LogisticsEngine.slnx --configuration Release
```


---

## 5. Pull Request

Terminato lo sviluppo, viene aperta una Pull Request verso il branch `main`.

La pipeline CI viene eseguita automaticamente.

---

## 6. Code Review

Il gruppo verifica:

* correttezza del codice;
* leggibilità;
* naming;
* rispetto della Clean Architecture;
* eventuali suggerimenti di miglioramento.

Successivamente il docente effettua la propria revisione.

---

## 7. Merge

La Pull Request viene approvata solamente se:

* la pipeline è completata con successo;
* i test risultano superati;
* la review è stata completata.

La Issue viene chiusa.

La relativa card passa automaticamente in:

```text
Completato
```

---


# Ruolo del docente

Il docente utilizza la GitHub Organization come punto di accesso centralizzato.

Può:

* visualizzare tutte le repository;
* monitorare Pull Request;
* controllare GitHub Actions;
* consultare la cronologia dei commit;
* verificare le statistiche di contribuzione;
* effettuare la Code Review;
* accedere alla board di ciascun gruppo.

---

# Valutazione

La board GitHub Projects costituisce uno strumento di supporto alla valutazione.

Permette di verificare:

* organizzazione del lavoro;
* pianificazione delle attività;
* avanzamento del progetto;
* suddivisione dei compiti;
* gestione delle funzionalità richieste.

Naturalmente non sostituisce la valutazione tecnica del codice.

Quest'ultima viene effettuata tramite:

* pipeline CI;
* analisi statica;
* Unit Test;
* Integration Test;
* Architecture Test;
* Code Review;
* colloquio orale individuale.

---

# Vantaggi didattici

L'utilizzo di GitHub Organization, GitHub Issues e GitHub Projects consente di introdurre gli studenti a un processo di sviluppo moderno e collaborativo.

In particolare permette di:

* simulare un flusso di lavoro professionale;
* responsabilizzare ogni componente del gruppo;
* pianificare e monitorare le attività;
* mantenere traccia delle decisioni progettuali;
* facilitare la collaborazione;
* documentare l'evoluzione del progetto;
* rendere più trasparente il processo di valutazione.

Inoltre il docente dispone di una visione completa dell'attività svolta da ogni gruppo e può intervenire tempestivamente in caso di ritardi, problemi organizzativi o difficoltà tecniche.
