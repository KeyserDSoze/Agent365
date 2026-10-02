# Tutorial 10 — Tool Governance Runtime

## Obiettivo

Trasformare i function tool del golden agent da semplici delegate applicativi a capability governate, osservabili e revocabili.

Il lab implementa quattro concetti:

1. registry;
2. block / unblock;
3. approval one-shot;
4. audit delle invocazioni.

## Mental model Agent 365

Il rischio di un agente cresce soprattutto in funzione di ciò che i suoi tool possono toccare e fare.

Per questo ogni tool deve avere almeno:
- owner;
- operation;
- risk tier;
- stato enabled / blocked;
- approval requirement;
- reversibility;
- external side effect;
- monitoring.

Il golden sample implementa questo modello in-process per poterlo studiare e testare prima di collegare tool reali.

## Registry

```http
GET /api/tools
```

Esempio semplificato:

```json
{
  "items": [
    {
      "name": "LookupPolicy",
      "operation": "read",
      "riskTier": "Low",
      "enabled": true,
      "requiresApproval": false,
      "externalSideEffect": false,
      "reversible": true
    },
    {
      "name": "CreateDraftChangeRequest",
      "operation": "write-draft",
      "riskTier": "Medium",
      "enabled": true,
      "requiresApproval": true,
      "externalSideEffect": false,
      "reversible": true
    }
  ]
}
```

## Runtime block / unblock

Bloccare:

```http
PUT /api/tools/LookupPolicy/state
Content-Type: application/json

{
  "enabled": false
}
```

Riabilitare:

```json
{
  "enabled": true
}
```

Il comportamento è intenzionalmente simile al concetto di block/unblock del registry Agent 365.

La modifica è volatile e vale solo per il processo corrente.

Per disattivare le modifiche runtime:

```text
Tools__AllowRuntimePolicyChanges=false
```

## Approval one-shot

Abilitare l'approvazione sul tool write-shaped:

```text
Tools__RequireApprovalForDraftChangeRequest=true
```

Creare l'approvazione:

```http
POST /api/tool-approvals
Content-Type: application/json

{
  "toolName": "CreateDraftChangeRequest",
  "conversationId": "<conversation-id>",
  "reason": "Approved for the controlled lab."
}
```

L'approvazione:
- è legata al tool;
- è legata al conversation ID;
- scade;
- viene consumata una sola volta.

Default TTL:

```text
Tools__ApprovalTtlMinutes=5
```

Il tool riceve l'`approvalId` come argomento opzionale. Se la policy richiede approval e il token non è valido, l'invocazione viene negata e registrata.

## Audit

```http
GET /api/tool-audit?limit=50
```

Ogni record contiene:
- timestamp;
- conversation ID;
- tool;
- operation;
- risk tier;
- decisione allowed/denied;
- success/failure;
- external side effect;
- reason;
- approval ID;
- durata.

Il sample non registra i parametri completi del tool.

Questa scelta evita che l'evidence layer diventi automaticamente un secondo datastore di prompt e dati sensibili.

## Audit capacity

```text
Tools__AuditCapacity=500
```

Il sample usa un buffer in memoria bounded.

In produzione l'evidence va instradata verso una piattaforma di telemetry/audit coerente con retention, privacy e incident response.

## Configurazione

```text
Tools__EnablePolicyLookup=true
Tools__EnableDraftChangeRequest=true
Tools__RequireApprovalForDraftChangeRequest=false
Tools__AllowRuntimePolicyChanges=true
Tools__ApprovalTtlMinutes=5
Tools__AuditCapacity=500
```

## Endpoint protetti

Quando:

```text
Api__RequireApiKey=true
```

sono protetti anche:
- `/api/tools`;
- `/api/tool-audit`;
- `/api/tool-approvals`.

## Cosa dimostra il lab

Il sample non pretende di sostituire il Tool Registry di Agent 365.

Serve a dimostrare il pattern applicativo necessario perché un custom agent possa:
- sapere quali tool espone;
- classificare il rischio;
- revocare capability;
- richiedere human approval;
- produrre evidence locale;
- separare policy decision da business logic.

## Test automatici

I test verificano:
- registry metadata;
- runtime block;
- audit del deny;
- approval one-time;
- binding approval → conversation;
- impossibilità di replay.

## Definition of Done

- [ ] registry visualizzabile;
- [ ] risk tier presente;
- [ ] tool bloccabile;
- [ ] deny auditato;
- [ ] approval richiesta quando configurata;
- [ ] approval non riutilizzabile;
- [ ] approval non valida su un'altra conversazione;
- [ ] audit bounded;
- [ ] secret e tool arguments sensibili non salvati nel log.

## Fonti Microsoft

- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-tools-for-agent?view=o365-worldwide
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/govern-tools
- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-plugins-skills-mcp-servers?view=o365-worldwide
- https://learn.microsoft.com/microsoft-agent-365/developer/tooling?tabs=nodejs
