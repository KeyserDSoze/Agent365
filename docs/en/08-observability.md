# 08 — Observability

## Goal
Create evidence useful for operations, security and governance and make agent runs explainable.

## Signals
Inventory state, agent runs, tool usage, model calls, errors, latency, identity context, data interaction, policy events, security alerts and lifecycle changes.

## Recommended path for new integrations
Current Microsoft guidance recommends **Microsoft OpenTelemetry Distro** for new Agent 365 integrations.

Three paths are documented:
1. Microsoft OpenTelemetry Distro — recommended for new integrations.
2. Agent 365 Observability SDK — still works, but is no longer the preferred path for new integrations.
3. Direct OTel — for existing OpenTelemetry pipelines or unsupported languages/frameworks.

## Data model
A run is represented as a tree of OpenTelemetry spans. Spans can represent agent invocation, tool execution, model/inference calls and final output.

## Data handling — verified 2026-10-02
Microsoft documentation updated September 30, 2026 states:
- customer content is stored in the tenant's default geography;
- EU Data Boundary commitments are honored;
- Advanced Data Residency is **not currently supported** for Agent 365 observability;
- observability data is retained for **30 days**, then automatically deleted;
- customers/developers control what custom and third-party agents send through instrumentation.

## Output
Telemetry Coverage Matrix: event, source, destination, consumer, retention, alerting, sensitive-data flag and correlation key.

## Sources
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
- https://learn.microsoft.com/en-us/microsoft-agent-365/admin/data-residency-protection-compliance
