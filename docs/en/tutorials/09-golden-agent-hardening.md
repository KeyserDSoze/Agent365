# Tutorial 09 — Golden Agent operational hardening

## Goal

Move the golden sample from a **lab** profile to a controlled **POC** profile using configurable guardrails.

## Health vs readiness

### `GET /health`

Confirms the ASP.NET Core process is alive.

### `GET /ready`

Checks whether the configured model provider is ready enough for inference.

For `foundry-local`, it validates model configuration and the local provider status endpoint.

For `azure-openai`, it validates configuration and endpoint reachability.

## Optional API key

Lab default:

```text
Api__RequireApiKey=false
```

POC:

```text
Api__RequireApiKey=true
Api__ApiKey=<secret>
```

Protected operational endpoints use:

```text
X-Api-Key: <secret>
```

The secret is never returned by `/api/config`.

This is a lab/POC guardrail, not a replacement for enterprise authentication and API gateways.

## Rate limiting

```text
Api__RequestsPerMinute=30
```

The chat endpoint returns HTTP 429 when the fixed-window limit is exceeded.

## Input limits

```text
Api__MaxMessageCharacters=8000
Api__MaxConversationIdCharacters=128
```

Oversized input is rejected before inference.

## Conversation store guardrails

```text
Conversations__MaxConversations=200
Conversations__IdleTimeoutMinutes=30
```

Idle in-memory sessions are removed. New conversations are rejected with HTTP 429 when capacity is reached.

## Safe diagnostics

```text
GET /api/diagnostics
```

Returns readiness and conversation counters without exposing secrets. It is protected when API-key mode is enabled.

## Automated tests

CI verifies:
- health;
- unconfigured readiness;
- secret redaction;
- API-key enforcement;
- message size limit;
- rate limiting;
- bounded conversation capacity.

## Definition of Done

- [ ] health and readiness are distinct;
- [ ] secrets are not exposed;
- [ ] API key validated where required;
- [ ] rate limit validated;
- [ ] input limits validated;
- [ ] conversation capacity defined;
- [ ] diagnostics aligned with the access model;
- [ ] automated tests pass.
