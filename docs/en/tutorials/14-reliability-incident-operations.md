# Tutorial 14 — Reliability & Incident Operations

## Goal

Move from passive evidence to a repeatable operational loop:

1. evaluate a run window;
2. apply transparent thresholds;
3. produce findings;
4. correlate suspect runs;
5. create an incident-ready snapshot;
6. execute a runbook.

## Endpoints

```http
GET /api/reliability
GET /api/incidents/snapshot?limit=50
```

Both are metadata-only and participate in API-key protection when enabled.

## Default lab thresholds

```text
Reliability__WindowRuns=50
Reliability__MinimumRuns=5
Reliability__FailureRateWarning=0.10
Reliability__FailureRateCritical=0.25
Reliability__AverageLatencyWarningMs=2500
Reliability__AverageLatencyCriticalMs=5000
Reliability__ToolDenyRateWarning=0.15
Reliability__ToolDenyRateCritical=0.30
Reliability__ConsecutiveFailuresCritical=3
```

These are lab defaults, not universal SLOs.

Tune them for the workload, model, tools and process criticality.

## Assessment

The response includes:

- status;
- transparent score;
- evaluated runs;
- failure rate;
- average latency;
- tool deny rate;
- consecutive failures;
- error taxonomy;
- findings.

Status values:

- `insufficient-data`;
- `healthy`;
- `degraded`;
- `critical`.

## Transparent score

The lab uses a deliberately simple rule:

- start at 100;
- subtract 15 for each warning;
- subtract 30 for each critical finding;
- floor at zero.

It is not an ML model and does not replace an SLO/SLA.

## Incident snapshot

Schema:

```text
agent365-golden-agent-incident/v1
```

It contains the assessment, suspect runs and correlated tool decisions with `capturesContent=false`.

## Local lab

The Windows runner now writes:

```text
lab-output/incident-snapshot.json
```

## Runbook

Use:

```text
examples/checklists/agent-incident-runbook.md
```

The flow covers declaration, evidence preservation, triage, containment, investigation, recovery, closure and post-incident actions.

## Definition of Done

- [ ] thresholds configured;
- [ ] assessment available;
- [ ] findings are explainable;
- [ ] incident snapshot remains metadata-only;
- [ ] suspect runs are correlatable;
- [ ] dashboard shows reliability;
- [ ] runbook is usable;
- [ ] recovery gate is defined.
