# Tutorial 04 — Purview DLP integration

## Goal

Inserire un controllo DLP nel flusso applicativo dell'agente prima che contenuto non consentito raggiunga il modello.

## Stato corrente

Il quickstart Agent 365 documenta lo skill:

> add Purview DLP to this agent

Lo skill integra controlli Microsoft Graph `processContent` per verificare l'input prima dell'invio al modello quando una policy di input blocking è applicabile.

## Important caveats

La documentazione Microsoft corrente indica che il percorso richiede:
- Purview DLP for AI abilitato;
- licensing appropriato;
- pay-as-you-go billing;
- onboarding DSPM for AI.

Il supporto e il comportamento dipendono anche dallo stack/versione SDK.

## Pattern logico

```text
User/Input
   |
   v
DLP processContent check
   |
   +---- Blocked ---> Stop turn + audit
   |
   v
Agent / LLM
   |
   v
Optional response audit
   |
   v
User
```

## Design decisions

Documentare:
- cosa bloccare;
- cosa solo auditare;
- fail-open vs fail-closed;
- timeout behavior;
- user message;
- telemetry;
- incident escalation.

Il quickstart Microsoft descrive un comportamento predefinito fail-closed per errori/timeouts nel percorso generato dallo skill.

## Test cases

1. contenuto normale;
2. dato sensibile simulato;
3. timeout;
4. errore autorizzazione;
5. response audit failure;
6. bypass attempt.

## Evidence pack

- policy ID/name;
- test case;
- expected result;
- actual result;
- audit evidence;
- timestamp;
- remediation.

## Source

https://learn.microsoft.com/en-us/microsoft-agent-365/developer/get-started
