# Agent 365 Academy — from theory to delivery

This folder contains the formal technical foundation. The React site under `src/` is the navigable experience; `docs/` is the study and design reference.

**Verified:** October 2, 2026.

## How to use this knowledge base

Do not read the chapters as a flat list. The recommended journey is:

**Understand → Prepare → Build → Govern → Operate**

Follow it in sequence or enter directly at the stage matching the work you need to do.

## 01 · Understand the control plane

1. [Foundations](01-foundations.md)
2. [Architecture](02-architecture.md)
3. [Registry & Governance](03-registry-governance.md)

**Expected outcome:** a tenant map and a clear explanation of management, identity, security, data and integration planes.

## 02 · Prepare tenant, identity and assessment

1. [Entra Agent ID](04-entra-agent-id.md)
2. [Tenant Readiness](../../examples/checklists/tenant-readiness.md)
3. [Identity Design Sheet](../../examples/templates/identity-design-sheet.md)
4. [Customer Discovery](../../examples/checklists/customer-discovery.md)

**Expected outcome:** readiness baseline, ownership model, identity design and gap list.

## 03 · Build and integrate

1. [SDK & CLI](09-sdk-cli.md)
2. [Connected Platforms](15-connected-platforms.md)
3. [Local Model Runtime](16-local-model-runtime.md)
4. [Hands-on tutorials](tutorials/README.md)
5. [Golden Agent .NET](../../samples/dotnet-golden-agent/README.md)
6. [Governed MCP Server](../../samples/mcp-governed-tools/README.md)

**Expected outcome:** runnable agent, identity/onboarding model, observability and tool boundary.

## 04 · Govern identity, data, security and tools

1. [Defender](05-defender.md)
2. [KQL for Agent 365](17-kql-agent-operations.md)
3. [Purview](06-purview.md)
4. [Tools & MCP](07-tools-mcp.md)
5. [Tool Risk Register](../../examples/templates/tool-risk-register.csv)
6. [Data Interaction Matrix](../../examples/templates/data-interaction-matrix.csv)

**Expected outcome:** remediation backlog, tool risk model, data-control design and hunting evidence.

## 05 · Operate, observe and respond

1. [Observability](08-observability.md)
2. [Operating Model](10-operating-model.md)
3. [Run Evidence & Correlation](tutorials/11-run-evidence-observability.md)
4. [Operations Dashboard](tutorials/12-operations-dashboard.md)
5. [Reliability & Incident Operations](tutorials/14-reliability-incident-operations.md)
6. [Agent Incident Runbook](../../examples/checklists/agent-incident-runbook.md)
7. [Customer Delivery](13-customer-delivery.md)

**Expected outcome:** evidence package, runbook, recovery gate and 30/60/90 roadmap.

## Cross-cutting paths

- [Training Plan](11-training-plan.md)
- [Labs](12-labs.md)
- [Official Sources](14-sources.md)

## Method

**Theory → configuration → lab → artifact → review.**

A resource is ready when they can explain why a capability exists, configure or precisely locate it, produce evidence, identify caveats and translate it into a customer deliverable.
