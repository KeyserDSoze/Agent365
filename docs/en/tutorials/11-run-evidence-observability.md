# Tutorial 11 — Run Evidence & Observability Correlation

## Goal

Produce technical evidence for every golden-agent run without turning the observability layer into a second prompt/response datastore.

The pattern provides:

1. application `runId`;
2. W3C `traceId`;
3. conversation correlation;
4. tool-audit correlation;
5. bounded local metrics;
6. no prompt/response content in the run evidence store.

## Root run

Every:

```text
POST /api/chat
```

creates a root `invoke_agent` activity carrying:

```text
gen_ai.operation.name
gen_ai.agent.name
gen_ai.conversation.id
gen_ai.request.model
agent365.run.id
```

Agent Framework and governed tools execute under that context.

## Chat response

```json
{
  "conversationId": "...",
  "runId": "...",
  "traceId": "...",
  "output": "...",
  "timestamp": "..."
}
```

The run ID identifies one application execution; the W3C trace ID correlates it with telemetry spans.

## Evidence API

```http
GET /api/evidence/runs
GET /api/evidence/runs?conversationId=<id>&limit=20
GET /api/evidence/summary
```

Evidence records contain metadata such as provider/model, duration, status, character counts and tool allow/deny counters.

## Privacy boundary

The run evidence store does **not** retain:

- prompts;
- responses;
- tool arguments;
- secrets;
- tokens;
- API keys.

The API explicitly returns:

```json
{
  "capturesContent": false
}
```

## Tool correlation

Tool audit records now carry:

```text
conversationId
runId
traceId
```

This makes it possible to reconstruct a run's governance decisions without persisting full content.

## Capacity

```text
Evidence__Enabled=true
Evidence__Capacity=500
```

The store is bounded and in-memory. It is not a production retention solution.

## Windows lab runner

The local runner verifies that both multi-turn calls appear in evidence with distinct run IDs and the same conversation ID.

## OpenTelemetry and Agent 365

Local run evidence complements rather than replaces OpenTelemetry:

```text
HTTP chat
  +--> local run evidence
  +--> W3C trace context
  +--> Agent Framework OTel
  +--> governed tool audit
  +--> console / Agent 365 export
```

Keep `gen_ai.conversation.id` consistent for Agent 365 run correlation.

## Definition of Done

- [ ] run ID returned for every chat;
- [ ] trace ID returned;
- [ ] success/failure evidence retained;
- [ ] no prompt/response content stored;
- [ ] tool audit carries run/trace IDs;
- [ ] summary endpoint available;
- [ ] bounded capacity;
- [ ] endpoints protected in the POC profile;
- [ ] local lab validates correlation.

## Microsoft sources

- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/agent-framework/agents/observability
