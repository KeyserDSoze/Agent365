# 04 — Microsoft Entra Agent ID

## Goal
Make agents identifiable, authenticatable, authorizable and governable.

## Core concepts
- **Agent identity blueprint:** model used to create and govern agent identities.
- **Agent identity:** concrete identity used by an agent.
- **Sponsor/owner:** accountability and lifecycle responsibility.

## Patterns
- Interactive/on-behalf-of: user and agent context.
- Autonomous: dedicated identity and authorization.

## Design principles
Unique identity, least privilege, no shared credentials, explicit ownership, environment separation, access review, credential lifecycle and tested revocation.

## Conditional Access
Design policies based on agent type, resource, risk, origin, authentication flow and autonomy. Do not blindly clone user policies.

## Identity Design Sheet
Document purpose, operating pattern, blueprint, identity, sponsor, credential model, token flow, resource scope, permissions, CA, logging and revocation.

Template: `../../examples/templates/identity-design-sheet.md`.

## Sources
- https://learn.microsoft.com/en-us/entra/agent-id/
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/entra-agent-365
