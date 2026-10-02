# Agent 365 Academy — formal knowledge base

Questa cartella contiene la base teorica e formale. Il sito React in `src/` è la vetrina navigabile; `docs/` è il riferimento di studio e progettazione.

**Data di verifica:** 2 ottobre 2026.

## Percorso

| # | Capitolo | Obiettivo |
|---|---|---|
| 01 | [Foundations](01-foundations.md) | Mental model corretto |
| 02 | [Architecture](02-architecture.md) | Management, identity, security, data e integration plane |
| 03 | [Registry & Governance](03-registry-governance.md) | Inventory, owner e lifecycle |
| 04 | [Entra Agent ID](04-entra-agent-id.md) | Identità e access |
| 05 | [Defender](05-defender.md) | Posture, hunting e response |
| 06 | [Purview](06-purview.md) | Data security e compliance |
| 07 | [Tools & MCP](07-tools-mcp.md) | Tool governance |
| 08 | [Observability](08-observability.md) | Telemetria, OpenTelemetry, retention e residency |
| 09 | [SDK & CLI](09-sdk-cli.md) | Custom-agent onboarding |
| 10 | [Operating Model](10-operating-model.md) | RACI, runbook e KPI |
| 11 | [Training Plan](11-training-plan.md) | Academy 3 settimane |
| 12 | [Labs](12-labs.md) | Hands-on e Definition of Done |
| 13 | [Customer Delivery](13-customer-delivery.md) | Assessment, POC e roadmap |
| 14 | [Official Sources](14-sources.md) | Source of truth |
| 15 | [Connected Platforms](15-connected-platforms.md) | Integrazione e sync di piattaforme terze |

## Metodo

**Teoria → configurazione → laboratorio → artefatto → review.**

Una risorsa è pronta quando sa spiegare il perché di una capability, configurarla o localizzarla, produrre evidenze, individuare caveat e tradurre il tutto in un deliverable cliente.

## Nota importante

Agent 365 è un **control plane trasversale**, non una feature “dentro Defender XDR”. Microsoft 365 admin center, Entra, Defender, Purview e i componenti di integrazione cooperano su piani distinti.

## Materiale pratico

Gli asset operativi sono in `../examples/`:
- query KQL starter;
- tenant readiness;
- customer discovery;
- inventory template;
- Identity Design Sheet;
- Tool Risk Register;
- Data Interaction Matrix;
- Incident Playbook.

Tutti gli esempi vanno validati contro lo schema e le capability disponibili nel tenant target.
