# 08 — Observability

## Obiettivo
Creare evidence utile a operations, security e governance e rendere leggibile ciò che accade durante un run agentico.

## Segnali
Inventory state, agent run, tool usage, inference/model call, error, latency, identity context, data interaction, policy event, security alert e lifecycle change.

## Percorso raccomandato per nuove integrazioni

La documentazione Microsoft corrente raccomanda **Microsoft OpenTelemetry Distro** per le nuove integrazioni Agent 365. La distro unifica trace, metric e log e supporta .NET, Node.js e Python, oltre a backend OTLP compatibili.

Sono documentati tre percorsi:
1. Microsoft OpenTelemetry Distro — raccomandato per nuove integrazioni.
2. Agent 365 Observability SDK — continua a funzionare, ma non è più il percorso preferito per nuove integrazioni.
3. Direct OTel — utile quando esiste già una pipeline OpenTelemetry o il framework/linguaggio non è coperto dagli SDK.

## Data model

Un run viene rappresentato come una struttura di span OpenTelemetry. Gli span possono rappresentare invocazione agente, tool call, inference/model call e risposta finale. Per correlare correttamente la telemetry servono attributi di identità e contesto coerenti.

## Data handling e retention — stato verificato 2026-10-02

La pagina Microsoft aggiornata il 30 settembre 2026 indica:
- contenuto customer archiviato nella geografia predefinita del tenant;
- rispetto degli impegni **EU Data Boundary**;
- **Advanced Data Residency non attualmente supportata** per Agent 365 observability;
- **retention di 30 giorni** per i dati di observability, con cancellazione automatica dopo tale periodo;
- per custom/third-party agents, cliente e sviluppatore controllano quali dati vengono inviati tramite instrumentation.

Questi punti vanno verificati nuovamente prima di un progetto soggetto a requisiti normativi o di residency stringenti.

## Telemetry design checklist
- Quale domanda operativa deve rispondere il log?
- Quale evento serve al SOC?
- Quale evento serve a compliance?
- Quali input/output non devono essere raccolti?
- Come si correlano runtime, agent identity, user session e tool?
- Quali correlation key servono?
- Chi può vedere la telemetry?
- Cosa serve conservare oltre i 30 giorni e dove?

## Output
**Telemetry Coverage Matrix:** event, source, destination, consumer, retention, alerting, sensitive flag, correlation key.

## Fonti
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
- https://learn.microsoft.com/en-us/microsoft-agent-365/admin/data-residency-protection-compliance
