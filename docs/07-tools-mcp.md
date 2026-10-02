# 07 — Tool & MCP Governance

## Principio
Un agente con tool può **agire**, non solo generare testo. La tool governance è quindi parte del security design.

## MCP
Governare server, autenticazione, tool esposti, input/output, privilegi, network path, owner, logging e versioning.

## Tool Risk Register
Campi: tool, owner, piattaforma/MCP, auth, data access, operation, permissions, reversibility, impact, risk tier, monitoring e mitigation.

Template: `examples/templates/tool-risk-register.csv`.

## Heuristic
Aumentare la review quando crescono write capability, privilegi, dati sensibili, irreversibilità, third-party exposure e autonomia.

## Decision questions
Human approval? Read-only possibile? Scope limitabile? Credential rotation? Logging? Revocation? Vendor trust?

## Output
Catalogo tool + risk tier + approved/denied use case + compensating control + monitoring requirement.
