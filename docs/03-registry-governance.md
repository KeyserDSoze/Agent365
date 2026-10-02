# 03 — Agent Registry, Inventory & Governance

## Principio
Non si governa ciò che non si vede. Ogni assessment deve partire da una baseline.

## Agent Overview
La documentazione Microsoft corrente descrive visibilità su agent count, attività recente, trend, governance gap, richieste e agenti senza owner.

## Agent Registry
Source of truth logica per gli agenti osservabili nel tenant. Copilot Studio registra automaticamente i propri agenti nel Registry secondo la documentazione corrente.

## Campi minimi
Nome/ID, piattaforma, tipo, owner, sponsor, stato, identity, audience, data source, tool/MCP, permessi, ambiente, risk tier, lifecycle, telemetry, review date.

Template: `examples/templates/agent-inventory.csv`.

## Lifecycle minimo
**Discover → Classify → Assign ownership → Approve → Publish → Monitor → Review → Retire**

## Governance gap
- owner assente;
- identity non definita;
- tool ad alto impatto;
- data access non documentato;
- audience eccessiva;
- agente stale;
- shadow/third-party non classificato;
- telemetry insufficiente.

## Risk tier interno
- Low: read-only, dati non sensibili.
- Medium: dati business, tool operativi, impatto reversibile.
- High: write, dati sensibili, sistemi critici, elevata autonomia.

Questa è una classificazione operativa del progetto, non uno standard Microsoft.
