# Tutorial 05 — Troubleshooting playbook

## Symptom: no telemetry

Check in this order:

1. At least one user has Microsoft 365 E7 or Agent 365 license assigned.
2. Agent 365 exporter is enabled.
3. Token resolver returns a token.
4. Auth mode matches the actual agent flow.
5. A valid `invoke_agent` span is present at root.
6. Console export produces local spans.
7. Agent/tenant IDs are correct.
8. Network path is not blocking export.

## Symptom: HTTP 200 but nothing appears

Microsoft explicitly documents two common causes:
- no qualifying license assigned to a user;
- missing valid `invoke_agent` root span.

## Symptom: registration/CLI authorization error

Validate:
- signed-in identity;
- Agent ID Developer / admin role;
- required Global Admin OAuth consent;
- Azure subscription permissions;
- tenant chosen in CLI.

## Symptom: tool call denied

Validate:
- delegated vs S2S flow;
- Work IQ MCP licensing;
- admin grant;
- requested scope;
- tool registration;
- actual agent identity.

## Symptom: DLP blocks unexpectedly

Validate:
- policy targeting;
- data classification;
- request payload;
- timeout/fail-closed configuration;
- Graph permission;
- test with non-sensitive control input.

## Diagnostic capture template

For every issue record:
- timestamp UTC;
- stack/runtime;
- agent ID;
- tenant ID (masked in shared reports);
- auth mode;
- exact command;
- exact error;
- correlation/trace ID;
- remediation attempted;
- result.

## Never capture

- access tokens;
- client secrets;
- private keys;
- customer prompts containing sensitive data;
- production PII unless explicitly required and protected.
