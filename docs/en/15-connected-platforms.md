# 15 — Connected Platforms

## Goal
Bring agents running on external platforms into the Microsoft Agent 365 Registry for centralized visibility and governance.

## Current admin flow
**Microsoft 365 admin center → Agents → All Agents → Connected platforms → Manage → Connect a platform**.

Administrators authenticate to the external environment, synchronize agents and can inspect sync status, errors and counts.

## Supported platforms — verified 2026-10-02
Current Microsoft documentation lists:
- Amazon Bedrock
- Google Vertex AI
- Salesforce Agentforce
- Databricks Genie
- Anthropic Claude Managed Agents
- Oracle Generative AI Agents
- Snowflake Cortex

Anthropic Claude Managed Agents integration is marked **Preview** because the relevant APIs are beta.

## Security considerations
Use dedicated credentials, least privilege, documented region/scope, rotation/revocation, explicit API permissions, dedicated workspaces/accounts where recommended, and monitor sync failures or stale connections.

## Discovery output
Provider, workspace/environment, region, auth method, credential owner, permissions, sync mode, agent count, supported actions, revocation path and preview limitations.

## Source
https://learn.microsoft.com/en-us/microsoft-agent-365/admin/connected-platforms
