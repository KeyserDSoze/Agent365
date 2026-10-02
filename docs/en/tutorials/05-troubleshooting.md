# Tutorial 05 — Troubleshooting playbook

## No telemetry
Check in order:
1. qualifying license assigned to at least one user;
2. Agent 365 exporter enabled;
3. token resolver returns a token;
4. auth model matches runtime;
5. valid `invoke_agent` root span;
6. console telemetry works locally;
7. agent/tenant IDs;
8. network path.

## HTTP 200 but no Agent 365 data
Microsoft specifically documents two common causes: no qualifying license assigned to a user, or no valid `invoke_agent` root span.

## Registration/CLI authorization failure
Check signed-in identity, Agent ID role, required admin consent, Azure subscription permissions and selected tenant.

## Tool call denied
Check delegated vs S2S, Work IQ licensing/Preview status, admin grant, requested scope, registration and actual identity.

## Diagnostic capture
Record UTC timestamp, runtime, masked IDs, auth mode, exact command/error, trace/correlation ID, attempted remediation and result.

Never capture access tokens, client secrets, private keys or unnecessary sensitive customer prompts.
