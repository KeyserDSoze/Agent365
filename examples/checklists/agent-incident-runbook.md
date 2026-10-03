# Agent Incident Runbook

Use this runbook when the reliability assessment is **critical**, when repeated failures affect a customer scenario, or when tool/security evidence requires investigation.

## 1. Declare

Record:

- incident ID;
- start time;
- agent / environment;
- current reliability status;
- affected provider/model;
- owner;
- incident lead;
- business impact;
- current hypothesis.

## 2. Preserve evidence

Capture before changing runtime state:

- `GET /api/reliability`;
- `GET /api/incidents/snapshot`;
- `GET /api/evidence/export`;
- readiness response;
- application stdout/stderr;
- relevant Agent 365 / OpenTelemetry trace references.

Keep:

- run IDs;
- trace IDs;
- conversation IDs;
- error taxonomy;
- tool allow/deny decisions;
- timestamps.

Do not copy full prompts or sensitive tool arguments into the incident record unless the approved investigation process explicitly requires them.

## 3. Triage

Check in order:

1. process liveness;
2. provider/model readiness;
3. failure rate;
4. consecutive failures;
5. latency shift;
6. tool deny rate;
7. error taxonomy;
8. recent configuration/deployment changes.

Classify the likely failure domain:

- runtime;
- model/provider;
- identity/authentication;
- network;
- tool policy;
- approval flow;
- capacity/rate limit;
- external dependency;
- unknown.

## 4. Contain

Prefer reversible actions:

- block the affected tool;
- disable the risky integration;
- route to a known-good provider/profile;
- stop automated retries;
- reduce scope;
- pause the affected workflow;
- revoke/rotate credentials when compromise is suspected.

Record every containment action and timestamp.

## 5. Investigate

For each suspect run:

- open the run evidence;
- correlate the trace ID;
- inspect tool audit decisions;
- compare with adjacent successful runs;
- validate configuration and readiness;
- identify the first known-bad run;
- identify the last known-good run.

## 6. Recover

Recovery criteria should be explicit.

Example:

- readiness healthy;
- no configuration error;
- five representative validation runs complete;
- failure rate below warning threshold;
- no consecutive failures;
- required tool calls allowed;
- expected deny decisions still enforced.

## 7. Close

Document:

- root cause;
- contributing factors;
- containment;
- permanent fix;
- evidence;
- validation;
- follow-up owner;
- target date.

## 8. Post-incident actions

Consider:

- threshold adjustment;
- missing telemetry;
- new automated test;
- tool policy change;
- additional readiness check;
- credential/process improvement;
- runbook improvement;
- customer communication template.

## Incident evidence package

Minimum recommended package:

```text
incident-snapshot.json
operations-export.json
latest.json
agent-stdout.log
agent-stderr.log
```

plus external trace/incident references where available.
