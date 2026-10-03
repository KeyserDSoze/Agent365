# Practical delivery toolkit

Gli asset in `examples/` non sono sample casuali: sono gli **output pratici** delle diverse tappe del percorso Agent 365.

The assets under `examples/` are not random samples: they are the **practical outputs** of the Agent 365 journey.

## 02 · Prepare — assessment & readiness

Use before a POC or governance workshop.

- [Tenant readiness](checklists/tenant-readiness.md) — prerequisites, roles, licensing and access.
- [Customer discovery](checklists/customer-discovery.md) — structured assessment questions.
- [Identity Design Sheet](templates/identity-design-sheet.md) — identity, sponsor, token flow and least privilege.

## 04 · Govern — hunting & control design

Use after the initial inventory/identity baseline.

### KQL Operations Pack

Start from [kql/README.md](kql/README.md).

The intended sequence is:

1. [Agent inventory](kql/01-agent-inventory.kql)
2. [Governance gaps](kql/02-governance-gaps.kql)
3. [Tools & MCP](kql/03-agent-tools-mcp.kql)

Then convert confirmed findings into:

- [Tool Risk Register](templates/tool-risk-register.csv)
- [Data Interaction Matrix](templates/data-interaction-matrix.csv)

## 05 · Operate — evidence & incident response

Use once the POC produces runtime evidence.

- [Operations evidence sample](evidence/operations-sample.json) — metadata-only dashboard bundle.
- [Agent Incident Runbook](checklists/agent-incident-runbook.md) — triage, containment, investigation and recovery.

## How the pieces connect

```text
Tenant readiness
   ↓
Inventory baseline
   ↓
Governance gaps
   ↓
Identity / tool / data design
   ↓
POC and runtime evidence
   ↓
Operations dashboard
   ↓
Incident runbook / customer delivery
```

Always validate query fields, schemas and platform capabilities against the target tenant before using an asset in production.
