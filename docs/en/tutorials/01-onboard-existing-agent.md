# Tutorial 01 — Connect an existing agent to Agent 365

## Goal
Connect an existing Python, Node.js or .NET agent to Agent 365 without rebuilding its runtime, model or orchestration.

## Prerequisites
Current Microsoft guidance requires an existing supported agent, an Agent 365-enabled tenant, suitable Entra/Azure permissions, a supported coding assistant and GitHub CLI.

## Install Agent 365 Skills

```bash
gh skill add microsoft/agent365-skills
```

Restart the coding assistant.

## Project setup

Ask:

> set up this project for Agent 365

The `a365-setup` skill validates prerequisites, detects the stack and creates `a365.generated.config.json`.

## Register a standard agent

Ask:

> register this agent with Agent 365

The registration path creates the agent identity blueprint, agent identity and required permissions.

## Add observability

Ask:

> add observability to this agent

Choose the authentication model that matches the real runtime: OBO, Agentic-User or S2S.

### Licensing caveat
For Agent 365 observability, current Microsoft guidance requires at least one user in the tenant to have a Microsoft 365 E7 or Microsoft Agent 365 license assigned.

## Work IQ tools
If needed:

> wire up Work IQ Mail and Calendar

Work IQ MCP is currently Preview and requires Microsoft 365 Copilot plus the required OAuth grant.

## Validation
The agent should still run as before, appear under its registered identity, emit telemetry and access only authorized tools.

## Evidence pack
Keep generated config without secrets, blueprint/identity evidence, test timestamp, telemetry evidence, tool access matrix and remediation notes.

Source: https://learn.microsoft.com/en-us/microsoft-agent-365/developer/get-started
