# Tutorial 06 — Golden Agent .NET end-to-end

## Obiettivo

Eseguire un agente reference reale separando runtime agentico, model provider, tool, sessione e Agent 365 observability.

Progetto:

```text
samples/dotnet-golden-agent/
```

## Stack

- ASP.NET Core
- Microsoft Agent Framework
- **Microsoft Foundry Local — default**
- Azure OpenAI — provider opzionale
- Microsoft OpenTelemetry Distro
- Agent 365 observability custom-engine S2S

## 1. Percorso raccomandato: Foundry Local

Su Windows:

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

Lo script prepara Foundry Local, scarica/carica il modello e avvia l'API con model ID ed endpoint locali già configurati.

Approfondimento: [Foundry Local come model runtime](07-foundry-local.md).

## 2. Health e configurazione

```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/config
```

`/api/config` mostra provider e stato della configurazione ma non espone secret.

## 3. Primo turno

```bash
curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

Riutilizzare il `conversationId` restituito per i turni successivi.

## 4. Session safety

Il `ConversationStore` serializza i turni per conversation ID, evitando accesso concorrente alla stessa `AgentSession`.

## 5. Tool

- `LookupPolicy`: read-only mock.
- `CreateDraftChangeRequest`: write-shaped mock con `externalSideEffect=false`.

Il tool calling dipende dalle capability del modello locale scelto.

## 6. Azure OpenAI opzionale

```bash
export Agent__Provider="azure-openai"
export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="<deployment-name>"
az login
dotnet run
```

Il boundary resta `IChatClient`, quindi Agent Framework e Agent 365 non cambiano.

## 7. Observability

Default:

```text
Agent365__ExportToConsole=true
Agent365__ExportToAgent365=false
```

Per Agent 365 S2S:

```bash
export Agent365__ExportToAgent365=true
export Agent365__TenantId="<tenant>"
export Agent365__AgentId="<app-client-id>"
export Agent365__ClientSecret="<secret>"
```

Permission:

```text
Agent365.Observability.OtelWrite — Application
```

## 8. Docker

Con Foundry Local sull'host Windows, il container usa:

```text
http://host.docker.internal:39839/v1
```

Avvio:

```bash
docker compose up --build
```

## 9. Definition of Done

- [ ] restore/build/publish verdi;
- [ ] app boot e `/health` verdi;
- [ ] Docker build e container boot verdi;
- [ ] Foundry Local model scaricato e caricato;
- [ ] chiamata agente funzionante con model provider locale;
- [ ] multi-turn verificato;
- [ ] tool verificati con un modello che supporta tool calling;
- [ ] console telemetry visibile;
- [ ] Agent 365 export verificato se abilitato.

## Fonti

- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/agent-framework/concepts/agents
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup
