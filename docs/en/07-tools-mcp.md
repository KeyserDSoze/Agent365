# 07 — Tool & MCP Governance

## Principle

An agent with tools can **act**, not only generate text. Risk is strongly determined by what a tool can touch and what it can do.

Agent 365 brings agentic tools into the Microsoft 365 admin center with a dedicated registry, approval, block/unblock and observability capabilities. Granular control of individual tools inside supported MCP servers is rolling out and depends on server/tool-discovery support.

## What counts as a tool

The governance surface includes:
- MCP servers;
- connectors;
- skills;
- plugins;
- custom function tools.

Governing only MCP servers therefore leaves gaps.

## Registry

Collect at least:
- name;
- publisher/owner;
- source/platform;
- auth model;
- data access;
- operation;
- permission scope;
- reversibility;
- external side effect;
- business impact;
- risk tier;
- approval requirement;
- enabled/blocked state;
- monitoring;
- revocation path.

Template: `../../examples/templates/tool-risk-register.csv`.

## Block / unblock

A capability should be revocable quickly.

Agent 365 can block tools/MCP servers centrally. Where supported, granular control can allow or block individual tools inside an MCP server.

Custom-agent runtimes should also have an application-side enforcement path when immediate local revocation is required.

## Human approval

Increase review for:
- write capability;
- delete;
- payments/transactions;
- privileged operations;
- sensitive data;
- irreversibility;
- third-party exposure;
- autonomy.

A robust approval should be:
- tool-bound;
- subject/conversation-bound;
- time-limited;
- one-shot;
- audited.

## Evidence

Record at least:
- tool;
- operation;
- risk tier;
- allow/deny;
- success/failure;
- timestamp;
- correlation key;
- approval reference;
- side-effect classification.

Avoid automatically duplicating full prompts or sensitive tool arguments into the governance log.

## Golden sample

The golden sample implements **in-process** governance:
- registry;
- risk metadata;
- runtime block/unblock;
- one-time approval;
- bounded audit;
- conversation binding;
- replay prevention.

Tutorial: `tutorials/10-tool-governance-runtime.md`.

## External MCP server

The repository also contains a real stdio MCP server:

```text
samples/mcp-governed-tools/
```

It moves the tool boundary outside the agent runtime and demonstrates:
- real MCP tool discovery;
- operation/risk metadata;
- local block policy;
- a write-shaped tool with no external side effect;
- operator-provisioned approval;
- metadata-only audit;
- real client/server handshake and invocation tests.

Client-side agent policy, server-side MCP policy and Agent 365 tenant governance are complementary enforcement layers.

Tutorial: `tutorials/13-governed-mcp-server.md`.

## Output

**Tool Risk Register + Policy Evidence**:
- catalog;
- risk tier;
- approved/denied use case;
- block state;
- approval requirement;
- compensating control;
- monitoring requirement;
- revocation path.

## Sources

- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-tools-for-agent?view=o365-worldwide
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/govern-tools
- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-plugins-skills-mcp-servers?view=o365-worldwide
- https://learn.microsoft.com/microsoft-agent-365/developer/tooling?tabs=nodejs
