# Tutorial 11 — Run Evidence & Observability Correlation

## Obiettivo

Produrre evidenza tecnica per ogni run del golden agent senza trasformare l'observability layer in un secondo datastore di prompt e risposte.

Il pattern implementa:

1. un `runId` applicativo;
2. un W3C `traceId`;
3. correlation con il `conversationId`;
4. correlation con il tool audit;
5. metriche locali bounded;
6. zero prompt/response content nel run evidence store.

## Root run

Ogni chiamata:

```text
POST /api/chat
```

crea un root activity:

```text
invoke_agent
```

con tag:

```text
gen_ai.operation.name=invoke_agent
gen_ai.agent.name=<agent name>
gen_ai.conversation.id=<conversation id>
gen_ai.request.model=<model>
agent365.run.id=<run id>
```

Questo root diventa il parent context per Agent Framework e per le invocazioni tool.

## Risposta chat

La risposta espone:

```json
{
  "conversationId": "...",
  "runId": "...",
  "traceId": "...",
  "output": "...",
  "timestamp": "..."
}
```

`runId` è l'identificatore applicativo del singolo run.

`traceId` è la chiave W3C usata per correlare l'esecuzione con gli span OpenTelemetry.

## Evidence store

Endpoint:

```http
GET /api/evidence/runs
```

Filtro per conversazione:

```http
GET /api/evidence/runs?conversationId=<id>&limit=20
```

Ogni record contiene:

- timestamp;
- run ID;
- conversation ID;
- trace ID;
- provider;
- model;
- status;
- duration;
- request character count;
- response character count;
- tool invocation count;
- allow count;
- deny count;
- error type.

## Privacy boundary

Il run evidence store **non salva**:

- prompt;
- risposta;
- tool arguments;
- secret;
- token;
- API key.

L'obiettivo è avere evidence operativa sufficiente per troubleshooting e governance senza duplicare automaticamente il contenuto.

L'endpoint dichiara:

```json
{
  "capturesContent": false
}
```

## Summary

```http
GET /api/evidence/summary
```

Restituisce:

- buffered runs;
- completed runs;
- failed runs;
- average duration;
- tool invocations;
- tool allowed;
- tool denied.

## Correlation con Tool Governance

Il `ToolInvocationContext` propaga:

```text
conversationId
runId
traceId
```

Ogni record del tool audit contiene le stesse chiavi.

Quindi la catena diventa:

```text
conversation
   |
   +--> run 1 ---- trace A ---- tool audit A1/A2
   |
   +--> run 2 ---- trace B ---- tool audit B1
```

## Capacity

Default:

```text
Evidence__Enabled=true
Evidence__Capacity=500
```

Lo store è bounded e in-memory.

Non è un archivio di produzione.

Per produzione l'evidence deve confluire in una piattaforma di telemetry/audit con retention, access control, residency e incident-response policy coerenti.

## API key

Quando:

```text
Api__RequireApiKey=true
```

sono protetti anche:

- `/api/evidence/runs`;
- `/api/evidence/summary`.

## Windows lab runner

Il runner:

```powershell
.\scripts\run-local-lab.ps1
```

ora verifica anche che:

- i due turni abbiano run ID distinti;
- siano presenti entrambi nell'evidence store;
- condividano il conversation ID;
- `capturesContent=false`.

## OpenTelemetry e Agent 365

L'evidence store locale non sostituisce OpenTelemetry.

Serve come livello didattico e operativo immediatamente leggibile.

La pipeline concettuale è:

```text
HTTP /api/chat
   |
   +--> Run Evidence metadata
   |
   +--> W3C Activity / trace context
   |
   +--> Agent Framework OTel spans
   |
   +--> Tool audit correlation
   |
   +--> Console / Agent 365 exporter
```

Per Agent 365, mantenere coerente il `gen_ai.conversation.id` è fondamentale per correlare i run.

## Definition of Done

- [ ] ogni chat restituisce runId;
- [ ] ogni chat restituisce traceId;
- [ ] evidence record creato per success/failure;
- [ ] nessun prompt/response salvato nell'evidence store;
- [ ] tool audit contiene runId e traceId;
- [ ] summary disponibile;
- [ ] capacity bounded;
- [ ] endpoint protetti nel profilo POC;
- [ ] local lab runner valida la correlation.

## Fonti Microsoft

- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/agent-framework/agents/observability
