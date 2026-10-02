# Agent 365 reference integration skeleton

This is an **integration skeleton**, not a full AI agent framework.

Purpose:
- show where Agent 365 observability is initialized;
- keep runtime/model logic replaceable;
- provide the same structure in Python, Node.js and .NET;
- support local console validation before tenant export.

## Important

The samples intentionally do **not** contain:
- tenant IDs;
- client IDs;
- secrets;
- real token acquisition;
- model-provider credentials.

Implement the token resolver using the OBO or S2S pattern appropriate to your agent.

## Recommended onboarding

1. Start with an existing agent.
2. Install Agent 365 Skills:
   `gh skill add microsoft/agent365-skills`
3. Run setup and register the agent.
4. Add observability.
5. Use these samples to understand the instrumentation boundary.
6. Validate console telemetry first.
7. Enable Agent 365 export.
8. Verify the run has a valid `invoke_agent` root span.

Official quickstart:
https://learn.microsoft.com/en-us/microsoft-agent-365/developer/get-started
