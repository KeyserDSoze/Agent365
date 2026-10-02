# Tutorial 02 — Agent 365 Observability with Microsoft OpenTelemetry

## Goal

Strumentare un agente con il percorso Microsoft raccomandato per nuove integrazioni.

## Scelta architetturale

Per nuove integrazioni usare **Microsoft OpenTelemetry Distro**.

Microsoft documenta tre percorsi:
1. Microsoft OpenTelemetry Distro — raccomandato;
2. legacy Agent 365 Observability SDK — continua a funzionare ma non usarlo per nuove integrazioni;
3. Direct OTel — per pipeline OTel già esistenti o stack non coperti.

## Python

Prerequisito: Python 3.10+.

```bash
pip install microsoft-opentelemetry
```

Skeleton:

```python
from microsoft.opentelemetry import use_microsoft_opentelemetry

def token_resolver(agent_id, tenant_id):
    # Replace with real OBO or S2S acquisition.
    return None

use_microsoft_opentelemetry(
    enable_a365=True,
    a365_token_resolver=token_resolver,
    enable_console=True,
)
```

## Node.js

```bash
npm install @microsoft/opentelemetry
```

Per ESM, registrare il loader prima dei moduli instrumented:

```bash
node --import @microsoft/opentelemetry/loader ./src/index.js
```

Skeleton:

```javascript
import { useMicrosoftOpenTelemetry } from "@microsoft/opentelemetry";

useMicrosoftOpenTelemetry({
  a365: {
    enabled: true,
    tokenResolver: async (agentId, tenantId) => null
  },
  enableConsoleExporters: true
});
```

## .NET

```bash
dotnet add package Microsoft.OpenTelemetry
```

Integrare `UseMicrosoftOpenTelemetry` nell'host e configurare il token resolver coerente con OBO/S2S.

## Critical validation

Per apparire correttamente nelle superfici Agent 365, il run deve avere un valido **`invoke_agent` root span**. Se manca, gli span possono restare interrogabili in Defender Advanced Hunting ma non apparire nelle superfici amministrative.

## Local-first validation

Prima di coinvolgere Agent 365:
- abilitare console export;
- verificare che trace/span vengano emessi;
- verificare correlation ID;
- non loggare prompt/dati sensibili senza una decisione esplicita.

## Production validation

Verificare:
- licenza assegnata a un utente;
- exporter Agent 365 abilitato;
- token resolver non nullo;
- auth mode coerente;
- root span valido;
- agent/tenant identifiers corretti;
- timestamp del test;
- presenza nelle superfici attese.

## Repository samples

Vedi:
- `examples/reference-agent/python/`
- `examples/reference-agent/node/`
- `examples/reference-agent/dotnet/`

## Sources

- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup
