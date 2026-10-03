# Tutorial 13 — Reliability & Incident Operations

## Obiettivo

Passare da evidence passiva a un meccanismo operativo ripetibile:

1. valuta un window di run;
2. applica soglie trasparenti;
3. genera finding;
4. correla i run sospetti;
5. produce uno snapshot incident-ready;
6. segue un runbook.

## Endpoint

Reliability:

```http
GET /api/reliability
```

Incident snapshot:

```http
GET /api/incidents/snapshot?limit=50
```

Entrambi sono metadata-only e, nel profilo con API key, sono endpoint protetti.

## Soglie

Default del sample:

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

Queste soglie sono **lab defaults**, non SLO universali.

Per un cliente vanno calibrate sul workload, sul modello, sui tool e sulla criticità del processo.

## Reliability assessment

L'assessment restituisce:

- status;
- score;
- numero di run valutati;
- failure rate;
- latency media;
- tool deny rate;
- failure consecutive;
- error taxonomy;
- finding.

Status:

- `insufficient-data`;
- `healthy`;
- `degraded`;
- `critical`.

## Score

Lo score è volutamente semplice e spiegabile:

- base 100;
- -15 per finding warning;
- -30 per finding critical;
- minimo 0.

Non è un modello ML e non sostituisce un SLO/SLA.

Serve a sintetizzare il lab mantenendo visibili le regole che hanno prodotto il risultato.

## Finding

Ogni finding espone:

- code;
- severity;
- title;
- detail;
- observed value;
- threshold;
- unit;
- recommended action;
- run IDs coinvolti.

La dashboard usa i run ID per portare il tester direttamente ai run sospetti.

## Incident snapshot

Schema:

```text
agent365-golden-agent-incident/v1
```

Contiene:

- reliability assessment;
- suspect runs;
- tool decisions correlate;
- `capturesContent=false`.

Non contiene prompt o risposte complete.

## Local lab

```powershell
.\scripts\run-local-lab.ps1
```

produce ora anche:

```text
lab-output/incident-snapshot.json
```

oltre all'operations export.

## Runbook

Usare:

```text
examples/checklists/agent-incident-runbook.md
```

Fasi:

1. declare;
2. preserve evidence;
3. triage;
4. contain;
5. investigate;
6. recover;
7. close;
8. post-incident action.

## Recovery gate

Un esempio di gate:

- provider ready;
- configurazione valida;
- 5 run rappresentativi completati;
- failure rate sotto warning;
- zero failure consecutive;
- tool richiesti allowed;
- deny attesi ancora enforced.

## Definition of Done

- [ ] soglie configurate;
- [ ] assessment disponibile;
- [ ] finding spiegabili;
- [ ] incident snapshot metadata-only;
- [ ] run sospetti correlabili;
- [ ] dashboard mostra reliability;
- [ ] runbook compilabile;
- [ ] recovery gate definito.
