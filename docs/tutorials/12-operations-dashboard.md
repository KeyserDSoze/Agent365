# Tutorial 12 — Operations Dashboard

## Obiettivo

Usare il run evidence export del golden agent per ottenere una vista operativa leggibile senza costruire un backend dedicato per il sito. La dashboard vive nella GitHub Pages React app e lavora completamente nel browser.

## Evidence export

```http
GET /api/evidence/export?limit=200
```

Il bundle contiene schema version, timestamp, `capturesContent=false`, summary, runs, tool audit e tool registry.

Schema corrente:

```text
agent365-golden-agent-evidence/v1
```

## Sample incluso

Il sample è in `examples/evidence/operations-sample.json` e viene copiato automaticamente nel bundle Pages.

## Import di un export reale

Nella sezione **Operations**: cliccare **Carica export JSON**, selezionare il file generato dal golden agent e analizzarlo direttamente nel browser. Il sito statico non invia il file a un backend.

## KPI e drill-down

La vista mostra run count, success rate, latency media, failure, tool invocation/allow/deny. Il dettaglio run mostra conversation ID, trace ID, provider/model, durata, dimensioni input/output e tool decisions correlate tramite run ID.

## Privacy

Il formato export dichiara `capturesContent=false`. La dashboard è progettata per evidence metadata-only e non richiede prompt, response o tool arguments completi.

## Esempio PowerShell

```powershell
$evidence = Invoke-RestMethod -Uri "http://localhost:5080/api/evidence/export?limit=200" -Method Get
$evidence | ConvertTo-Json -Depth 20 | Set-Content ".\lab-output\operations-export.json"
```

Con API key abilitata aggiungere l'header `X-Api-Key`.

## Cosa non è

La dashboard non sostituisce Agent 365 observability, Defender, Purview Audit, SIEM/SOC tooling o una retention platform di produzione. Serve per training, demo, POC e troubleshooting locale.

## Definition of Done

- evidence export v1 generato;
- `capturesContent=false`;
- sample incluso caricato;
- export reale importabile;
- KPI coerenti;
- filtri status/provider funzionanti;
- run drill-down funzionante;
- tool decisions correlate tramite run ID.