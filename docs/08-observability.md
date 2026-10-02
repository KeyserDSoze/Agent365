# 08 — Observability

## Obiettivo
Creare evidence utile a operations, security e governance.

## Segnali
Inventory state, activity, tool usage, error, latency, identity context, data interaction, policy event, security alert, lifecycle change.

## OpenTelemetry
Per custom agent progettare trace, metric, log, correlation ID, privacy, sampling e retention fin dall'inizio.

## Checklist
- Quale domanda operativa deve rispondere il log?
- Quale evento serve al SOC?
- Quale dato non deve essere loggato?
- Come si correlano runtime, agent identity e tool?
- Retention/residency?
- Chi accede?

## Caveat
Retention e residency possono cambiare per capability. Revalidare la documentazione aggiornata prima del progetto.

## Output
Telemetry Coverage Matrix: event, source, destination, consumer, retention, alerting, sensitive flag, correlation key.
