# 07 — Tool & MCP Governance

## Principio

Un agente con tool può **agire**, non solo generare testo. Il rischio dipende in modo sostanziale da ciò che il tool può toccare e fare.

Agent 365 porta i tool nel Microsoft 365 admin center con un registry dedicato e capability di approvazione, block/unblock e osservabilità. Il controllo granulare dei singoli tool dentro alcuni MCP server è in rollout e dipende dal tipo di server/supporto alla tool discovery.

## Cosa conta come tool

Nel mental model Agent 365 rientrano:
- MCP server;
- connector;
- skill;
- plugin;
- function tool custom.

Quindi governare solo gli MCP server non copre l'intera superficie agentica.

## Registry

Per ogni capability raccogliere almeno:

- name;
- publisher/owner;
- source/platform;
- auth model;
- data access;
- operation;
- permission scope;
- reversible;
- external side effect;
- business impact;
- risk tier;
- approval requirement;
- enabled/blocked state;
- monitoring;
- revocation path.

Template: `examples/templates/tool-risk-register.csv`.

## Block / unblock

Una capability deve poter essere revocata rapidamente.

Nel control plane Agent 365 il block può essere applicato al tool/MCP server; dove supportato, il controllo granulare può consentire o bloccare singoli tool all'interno di un server.

Nel custom agent la stessa logica deve esistere anche nel runtime applicativo quando serve un enforcement locale immediato.

## Human approval

Aumentare la review quando crescono:
- write capability;
- delete;
- pagamento/transazione;
- privilegi;
- dati sensibili;
- irreversibilità;
- third-party exposure;
- autonomia.

Approval utile quando la capability è ammessa ma l'azione specifica richiede conferma umana.

Un approval token robusto dovrebbe essere:
- tool-bound;
- subject/conversation-bound;
- time-limited;
- one-shot;
- auditato.

## Evidence

Registrare almeno:
- tool;
- operation;
- risk tier;
- allow/deny;
- success/failure;
- timestamp;
- correlation key;
- approval reference;
- side-effect classification.

Evitare di duplicare automaticamente prompt o argomenti sensibili nel log di governance.

## Golden sample

Il golden sample implementa:
- registry in-process;
- risk metadata;
- runtime block/unblock;
- approval one-shot;
- audit bounded;
- conversation binding;
- replay prevention.

Tutorial: `tutorials/10-tool-governance-runtime.md`.

## Decision questions

- Human approval?
- Read-only possibile?
- Scope limitabile?
- Credential rotation?
- Logging?
- Revocation?
- Vendor trust?
- Tool discovery disponibile?
- Il block va applicato al server intero o alla singola function?

## Output

**Tool Risk Register + Policy Evidence**:
- catalogo;
- risk tier;
- approved/denied use case;
- block state;
- approval requirement;
- compensating control;
- monitoring requirement;
- revocation path.

## Fonti

- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-tools-for-agent?view=o365-worldwide
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/govern-tools
- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-plugins-skills-mcp-servers?view=o365-worldwide
- https://learn.microsoft.com/microsoft-agent-365/developer/tooling?tabs=nodejs
