# KQL for Agent 365 — inventory, governance gaps and tools/MCP

## Why this pack exists

The KQL queries in this repository are not just Advanced Hunting demo snippets. They turn technical visibility into an operational flow:

**inventory → gap → investigation → remediation → evidence**

The pack is intentionally small. The goal is to teach how to start from `AgentsInfo`, validate the schema actually available in the tenant and then build more specific hunting queries.

## Where to use it

The queries are designed for **Microsoft Defender Advanced Hunting**.

Before running them:

1. verify that `AgentsInfo` is available;
2. inspect the current tenant schema;
3. validate field names and population;
4. record the platform producing the agent data;
5. treat repository queries as starter patterns, not production-ready detection rules.

## The complete flow

### 1. Build the baseline

Open [01-agent-inventory.kql](../../examples/kql/01-agent-inventory.kql).

Use it to:

- see observable agents;
- identify agent ID, name, type, platform and status;
- verify whether the dataset is complete enough to continue.

Do not jump immediately to remediation. Save a baseline first and compare it with what the team expects to find.

### 2. Find governance gaps

Open [02-governance-gaps.kql](../../examples/kql/02-governance-gaps.kql).

The starter query looks for basic gaps such as missing owners or essential metadata.

Turn findings into a remediation backlog with owner, severity, action and review date.

Connect this work to:

- [Registry & Governance](03-registry-governance.md);
- [Entra Agent ID](04-entra-agent-id.md);
- [Tenant readiness](../../examples/checklists/tenant-readiness.md).

### 3. Investigate tools and MCP exposure

Open [03-agent-tools-mcp.kql](../../examples/kql/03-agent-tools-mcp.kql).

The pattern searches `RawAgentInfo` for signals related to MCP, tools and connectors.

This is a **discovery pattern**, not proof that a particular tool is active or risky.

For every candidate:

1. inspect the full record;
2. identify platform and agent ID;
3. validate the available tool catalog/registry;
4. reconstruct permissions and approval model;
5. register the tool in the [Tool Risk Register](../../examples/templates/tool-risk-register.csv);
6. connect the analysis to [Tools & MCP](07-tools-mcp.md).

## Turning findings into controls

A useful finding should become at least one of:

- remediation;
- policy;
- ownership fix;
- approval requirement;
- monitoring rule;
- incident hypothesis;
- documented exception.

## Minimum evidence

For an assessment retain:

- executed query;
- timestamp;
- tenant/environment;
- result count;
- permitted screenshot or export;
- summarized findings;
- decisions;
- remediation/backlog reference.

Avoid copying sensitive data into unprotected documentation.

## What to read next

For assessment work:

1. [Registry & Governance](03-registry-governance.md)
2. [Defender](05-defender.md)
3. [Tools & MCP](07-tools-mcp.md)
4. [Tool governance runtime](tutorials/10-tool-governance-runtime.md)

For incident investigation:

1. [Run Evidence & Correlation](tutorials/11-run-evidence-observability.md)
2. [Reliability & Incident Operations](tutorials/14-reliability-incident-operations.md)
3. [Agent Incident Runbook](../../examples/checklists/agent-incident-runbook.md)

## Definition of Done

- [ ] tenant `AgentsInfo` schema validated;
- [ ] inventory baseline produced;
- [ ] governance gaps turned into backlog items;
- [ ] tool/MCP candidates investigated;
- [ ] query and timestamp retained as evidence;
- [ ] next action assigned to every finding.
