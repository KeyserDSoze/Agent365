# 02 — Logical Architecture

## Management plane — Microsoft 365 admin center
Agent Overview, Agent Registry, visibilità tenant-wide, owner, richieste, analytics e governance gap.

## Identity plane — Microsoft Entra
Agent ID, blueprint, identities, sponsor, authentication, authorization, Conditional Access e lifecycle governance.

## Security plane — Microsoft Defender
Posture, Advanced Hunting, `AgentsInfo`, threat detection, investigation e runtime protection dove supportata.

## Data plane — Microsoft Purview
Audit, DLP, sensitivity/classification, data security, eDiscovery e compliance.

## Tool & integration plane
Agent 365 SDK, CLI, connected platforms, MCP, custom agents e observability.

## Control plane vs runtime
Il **control plane** governa inventory, identity, policy, security, data e evidence. Il **runtime** è dove l'agente viene eseguito (per esempio Copilot Studio, Foundry o una piattaforma terza).

## Flusso
1. L'agente nasce in un runtime.
2. Viene scoperto/registrato.
3. Riceve owner e metadata.
4. Entra governa identity/access.
5. Defender osserva security.
6. Purview governa data interaction.
7. Tool e connettori vengono classificati.
8. Telemetry alimenta operations e response.

## Artefatto
Architecture Context Diagram con runtime, Agent 365, Entra, Defender, Purview, data source, tool/MCP, logging e owner.
