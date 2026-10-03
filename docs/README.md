# Agent 365 Academy — dalla teoria alla delivery

Questa cartella contiene la base teorica e formale. Il sito React in `src/` è la vetrina navigabile; `docs/` è il riferimento di studio e progettazione.

**Data di verifica:** 2 ottobre 2026.

## Come usare questa knowledge base

Non leggere i capitoli come una lista piatta. Il percorso consigliato è:

**Capire → Preparare → Costruire → Governare → Operare**

Puoi seguirlo in sequenza oppure entrare direttamente dalla fase che corrisponde al lavoro che devi fare.

## 01 · Capire il control plane

Obiettivo: costruire il mental model corretto e capire responsabilità e confini dei vari piani Microsoft.

1. [Foundations](01-foundations.md)
2. [Architecture](02-architecture.md)
3. [Registry & Governance](03-registry-governance.md)

**Output atteso:** mappa del tenant e spiegazione chiara di management, identity, security, data e integration plane.

## 02 · Preparare tenant, identità e assessment

Obiettivo: verificare prerequisiti e trasformare una prima interlocuzione in una baseline concreta.

1. [Entra Agent ID](04-entra-agent-id.md)
2. [Tenant Readiness](../examples/checklists/tenant-readiness.md)
3. [Identity Design Sheet](../examples/templates/identity-design-sheet.md)
4. [Customer Discovery](../examples/checklists/customer-discovery.md)

**Output atteso:** readiness baseline, ownership model, identity design e gap list.

## 03 · Costruire e integrare

Obiettivo: portare un agente reale dentro un runtime verificabile e predisporlo all'integrazione con Agent 365.

1. [SDK & CLI](09-sdk-cli.md)
2. [Connected Platforms](15-connected-platforms.md)
3. [Local Model Runtime](16-local-model-runtime.md)
4. [Tutorial pratici](tutorials/README.md)
5. [Golden Agent .NET](../samples/dotnet-golden-agent/README.md)
6. [Governed MCP Server](../samples/mcp-governed-tools/README.md)

**Output atteso:** agente eseguibile, identity/onboarding model, observability e tool boundary.

## 04 · Governare identità, dati, security e tool

Obiettivo: trasformare inventory e telemetry in controlli, finding e remediation.

1. [Defender](05-defender.md)
2. [KQL per Agent 365](17-kql-agent-operations.md)
3. [Purview](06-purview.md)
4. [Tools & MCP](07-tools-mcp.md)
5. [Tool Risk Register](../examples/templates/tool-risk-register.csv)
6. [Data Interaction Matrix](../examples/templates/data-interaction-matrix.csv)

**Output atteso:** remediation backlog, tool risk model, data-control design e hunting evidence.

## 05 · Operare, osservare e rispondere

Obiettivo: portare il POC verso un modello operativo con evidence, KPI, reliability e incident response.

1. [Observability](08-observability.md)
2. [Operating Model](10-operating-model.md)
3. [Run Evidence & Correlation](tutorials/11-run-evidence-observability.md)
4. [Operations Dashboard](tutorials/12-operations-dashboard.md)
5. [Reliability & Incident Operations](tutorials/14-reliability-incident-operations.md)
6. [Agent Incident Runbook](../examples/checklists/agent-incident-runbook.md)
7. [Customer Delivery](13-customer-delivery.md)

**Output atteso:** evidence package, runbook, recovery gate e roadmap 30/60/90.

## Percorsi trasversali

- [Training Plan](11-training-plan.md) — academy completa.
- [Labs](12-labs.md) — prove pratiche e Definition of Done.
- [Official Sources](14-sources.md) — source of truth Microsoft.

## Metodo

**Teoria → configurazione → laboratorio → artefatto → review.**

Una risorsa è pronta quando sa spiegare il perché di una capability, configurarla o localizzarla, produrre evidenze, individuare caveat e tradurre il tutto in un deliverable cliente.

## Principio guida

Agent 365 è un **control plane trasversale**, non una feature “dentro Defender XDR”. Microsoft 365 admin center, Entra, Defender, Purview e i componenti di integrazione cooperano su piani distinti.
