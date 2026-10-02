# Tutorial 10 — Tool Governance Runtime

## Goal

Turn the golden agent's function tools from opaque application delegates into governed, observable and revocable capabilities.

The lab demonstrates:

1. registry;
2. block / unblock;
3. one-time approval;
4. invocation audit.

## Agent 365 mental model

Agent risk is strongly shaped by what its tools can touch and what actions they can perform.

Each tool should therefore carry:
- owner;
- operation;
- risk tier;
- enabled / blocked state;
- approval requirement;
- reversibility;
- external-side-effect flag;
- monitoring.

The golden sample implements this model in-process so the pattern can be tested before connecting real systems.

## Registry

```http
GET /api/tools
```

The registry exposes risk, operation, approval and state metadata for every built-in tool.

## Runtime block / unblock

```http
PUT /api/tools/LookupPolicy/state
Content-Type: application/json

{
  "enabled": false
}
```

Runtime changes can be disabled with:

```text
Tools__AllowRuntimePolicyChanges=false
```

The in-memory override is intentionally non-persistent.

## One-time approval

Enable approval for the write-shaped tool:

```text
Tools__RequireApprovalForDraftChangeRequest=true
```

Create an approval:

```http
POST /api/tool-approvals
Content-Type: application/json

{
  "toolName": "CreateDraftChangeRequest",
  "conversationId": "<conversation-id>",
  "reason": "Approved for the controlled lab."
}
```

An approval:
- belongs to one tool;
- belongs to one conversation;
- expires;
- can only be consumed once.

## Audit

```http
GET /api/tool-audit?limit=50
```

Audit records contain governance metadata and decisions without storing full tool arguments.

## Configuration

```text
Tools__EnablePolicyLookup=true
Tools__EnableDraftChangeRequest=true
Tools__RequireApprovalForDraftChangeRequest=false
Tools__AllowRuntimePolicyChanges=true
Tools__ApprovalTtlMinutes=5
Tools__AuditCapacity=500
```

## What this lab proves

This does not replace the Agent 365 Tool Registry.

It demonstrates the application-side control pattern required for a custom agent to:
- inventory its capabilities;
- classify risk;
- revoke tools;
- require human approval;
- retain local evidence;
- separate policy decisions from business logic.

## Automated tests

The repository validates:
- registry metadata;
- runtime blocking;
- deny audit;
- one-time approvals;
- conversation binding;
- replay prevention.

## Definition of Done

- [ ] tool registry visible;
- [ ] risk tiers present;
- [ ] tool can be blocked;
- [ ] denied invocation audited;
- [ ] approval enforced when configured;
- [ ] approval cannot be replayed;
- [ ] approval cannot cross conversations;
- [ ] audit storage bounded;
- [ ] sensitive tool arguments not persisted in the audit.

## Microsoft sources

- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-tools-for-agent?view=o365-worldwide
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/govern-tools
- https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-plugins-skills-mcp-servers?view=o365-worldwide
- https://learn.microsoft.com/microsoft-agent-365/developer/tooling?tabs=nodejs
