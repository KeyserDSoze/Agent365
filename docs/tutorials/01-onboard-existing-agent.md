# Tutorial 01 — Connect an existing agent to Agent 365

## Goal

Portare un agente Python, Node.js o .NET già esistente nel percorso Agent 365 senza riscriverne runtime, modello o orchestrazione.

## Prerequisiti

Secondo il quickstart Microsoft corrente:

- agente esistente su Python, Node.js o .NET;
- tenant con Agent 365 abilitato;
- Global Administrator, oppure Agent ID Developer con un Global Administrator disponibile per i grant OAuth;
- Azure subscription con permesso di creare risorse;
- coding assistant supportato;
- GitHub CLI.

## 1. Installare Agent 365 Skills

```bash
gh skill add microsoft/agent365-skills
```

Riavviare il coding assistant dopo l'installazione.

## 2. Setup del progetto

Nel progetto dell'agente:

> set up this project for Agent 365

Lo skill `a365-setup`:
- controlla prerequisiti;
- rileva lo stack;
- produce `a365.generated.config.json`;
- instrada verso standard agent o AI teammate.

## 3. Registrare uno standard agent

> register this agent with Agent 365

Il percorso `make-a365-agent` crea:
- agent identity blueprint;
- agent identity;
- permission necessarie;
- registrazione/catalog visibility.

## 4. Aggiungere observability

> add observability to this agent

Scegliere l'auth mode coerente con il runtime:
- OBO;
- Agentic-User;
- S2S.

### Licensing caveat

Per l'observability, Microsoft richiede che **almeno un utente nel tenant** abbia assegnata una licenza Microsoft 365 E7 o Microsoft Agent 365. La sola presenza dello SKU non è sufficiente.

## 5. Aggiungere Work IQ tools — se serve

> wire up Work IQ Mail and Calendar

**Caveat:** Work IQ MCP è attualmente Preview e richiede Microsoft 365 Copilot. Un Global Administrator deve concedere i permessi OAuth necessari.

## 6. Purview DLP — opzionale

> add Purview DLP to this agent

Vedi il tutorial dedicato.

## 7. Validazione

L'agente deve:
- avviarsi e rispondere come prima;
- comparire nel contesto Agent 365;
- emettere telemetry;
- mostrare l'identità attesa;
- accedere solo ai tool autorizzati.

Se disponibile nel coding assistant, eseguire anche la validation/diagnose capability Agent 365.

## Evidence pack

Salvare:
- `a365.generated.config.json` **senza secret**;
- screenshot/nota del blueprint;
- agent identity ID;
- test timestamp;
- evidenza telemetry;
- tool access matrix;
- eventuali errori e remediation.

## Source

https://learn.microsoft.com/en-us/microsoft-agent-365/developer/get-started
