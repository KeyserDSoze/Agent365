# 05 — Microsoft Defender

## Goal
Provide security visibility, posture, hunting, detection and response for agents.

## Advanced Hunting
For Agent 365, Microsoft directs customers to **`AgentsInfo`**. The older `AIAgentsInfo` schema was replaced during the 2026 transition. Always inspect the live tenant schema before reusing queries.

## Hunt for
Missing owners, unexpected status/platforms, excessive permissions, high-impact tools/MCP, recent changes and out-of-standard configurations.

Starter queries live in `../../examples/kql/`.

## Incident model
Identity compromise; instruction/prompt manipulation; tool misuse; data exfiltration/oversharing; anomalous behavior; connected-platform risk; misconfiguration.

## Triage
Identify agent/owner/runtime/identity/tools/data; reconstruct timeline; restrict access; preserve evidence; correct configuration; validate recovery.

## 2026 transition
Microsoft consolidated multiple Copilot Studio / Foundry agent security capabilities into Agent 365 and moved inventory to `AgentsInfo`, with operational changes taking effect from July 1, 2026.

## Sources
- https://learn.microsoft.com/en-us/security/security-for-ai/agent-365-security
- https://learn.microsoft.com/en-us/defender-xdr/security-for-ai/transition-agent-security-to-agent-365
- https://learn.microsoft.com/en-us/microsoft-365/security/defender/advanced-hunting-schema-changes?view=o365-worldwide
