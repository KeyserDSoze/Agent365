# Tutorial 02 — Agent 365 Observability with Microsoft OpenTelemetry

## Goal
Instrument an agent using the Microsoft-recommended path for new integrations.

## Architecture choice
Use **Microsoft OpenTelemetry Distro** for new integrations. The legacy Agent 365 Observability SDK continues to work but is not recommended for new projects.

## Python
Python 3.10+:

```bash
pip install microsoft-opentelemetry
```

Use `use_microsoft_opentelemetry()`, enable Agent 365 export only when the token resolver and tenant setup are ready, and use console export for local validation.

## Node.js

```bash
npm install @microsoft/opentelemetry
```

For ESM:

```bash
node --import @microsoft/opentelemetry/loader ./src/index.js
```

Initialize `useMicrosoftOpenTelemetry()` before instrumented modules are loaded.

## .NET

```bash
dotnet add package Microsoft.OpenTelemetry
```

Configure `UseMicrosoftOpenTelemetry` according to the actual OBO/S2S identity model.

## Critical validation
A run needs a valid **`invoke_agent` root span** to surface correctly across the Agent 365 experiences. Without it, spans can still be queryable in Defender hunting while not showing up in the admin surfaces.

## Validation order
1. Console telemetry locally.
2. Correlation identifiers.
3. Licensing.
4. Exporter enabled.
5. Non-null token resolver.
6. Correct auth mode.
7. Valid root span.
8. Correct agent/tenant identity.
9. Evidence in the expected surface.

Samples: `examples/reference-agent/`.

Sources:
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
