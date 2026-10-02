# 02 — Logical Architecture

## Management plane — Microsoft 365 admin center
Agent Overview, Agent Registry, tenant-wide visibility, owners, requests, analytics and governance gaps.

## Identity plane — Microsoft Entra
Agent ID, blueprints, identities, sponsors, authentication, authorization, Conditional Access and lifecycle governance.

## Security plane — Microsoft Defender
Posture, Advanced Hunting, `AgentsInfo`, threat detection, investigation and runtime protection where supported.

## Data plane — Microsoft Purview
Audit, DLP, classification, data security, eDiscovery and compliance.

## Tool & integration plane
Agent 365 SDK, CLI, connected platforms, MCP, custom agents and observability.

## Control plane vs runtime
The **control plane** governs inventory, identity, policy, security, data and evidence. The **runtime** is where the agent executes, such as Copilot Studio, Foundry or a third-party platform.

## Reference flow
Create agent → discover/register → assign owner/metadata → govern identity/access → monitor security → protect data → govern tools → collect telemetry.

## Artifact
Produce an Architecture Context Diagram including runtime, Agent 365, Entra, Defender, Purview, data sources, tools/MCP, logging and owners.
