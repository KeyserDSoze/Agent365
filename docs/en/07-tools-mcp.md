# 07 — Tool & MCP Governance

## Principle
An agent with tools can **act**, not just generate text. Tool governance is therefore part of the security design.

## MCP
Govern servers, authentication, exposed tools, input/output, privileges, network path, ownership, logging and versioning.

## Tool Risk Register
Capture tool, owner, platform/MCP server, auth, data access, operation, permissions, reversibility, impact, risk tier, monitoring and mitigation.

Template: `../../examples/templates/tool-risk-register.csv`.

## Risk heuristic
Increase review depth as write capability, privilege, sensitive data, irreversibility, third-party exposure and autonomy increase.

## Decision questions
Human approval? Can it be read-only? Is scope constrained? How are credentials rotated? How is use logged? How is access revoked?

## Output
Tool catalog + risk tier + approved/denied use cases + compensating controls + monitoring requirements.
