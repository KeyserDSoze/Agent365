# Tutorial 06 — Golden Agent .NET end-to-end

## Obiettivo

Eseguire un agente reference reale che separa chiaramente:

- runtime agentico;
- model provider;
- tool;
- sessione;
- Agent 365 observability;
- configurazione e secret;
- API/hosting.

Il progetto è in:

```text
samples/dotnet-golden-agent/
```

## Stack

- ASP.NET Core
- Microsoft Agent Framework 1.23
- Azure OpenAI
- Microsoft OpenTelemetry Distro
- Agent 365 observability custom-engine S2S

## 1. Avvio local-only

Il servizio può partire anche senza endpoint Azure OpenAI. In questa modalità si possono validare:

- bootstrap;
- health endpoint;
- configurazione pubblica;
- pipeline OpenTelemetry locale.

```bash
cd samples/dotnet-golden-agent
ASPNETCORE_URLS=http://localhost:8080 dotnet run
```

Test:

```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/config
```

## 2. Configurare Azure OpenAI

```bash
export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="gpt-4o-mini"
az login
```

L'implementazione baseline usa Chat Completions tramite `IChatClient`, poi costruisce un `AIAgent`.

Questo mantiene il runtime Agent Framework indipendente dal provider concreto.

## 3. Primo turno

```bash
curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

La risposta restituisce un `conversationId`.

## 4. Multi-turn

Riutilizzare il conversation ID:

```json
{
  "conversationId": "<id>",
  "message": "Create a mock draft change request to align an agent to that policy."
}
```

Il `ConversationStore` serializza i turni per conversation ID per evitare accesso concorrente alla stessa `AgentSession`.

## 5. Tool

Il sample espone due function tool:

### LookupPolicy
Read-only. Restituisce una policy mock.

### CreateDraftChangeRequest
Simula una write action ma restituisce esclusivamente un draft con:

```json
{
  "externalSideEffect": false
}
```

Serve per insegnare che un tool apparentemente “operativo” deve essere esplicitamente classificato come mock, approval-required o production-write.

## 6. Observability locale

Default:

```text
Agent365__ExportToConsole=true
Agent365__ExportToAgent365=false
```

Microsoft OpenTelemetry auto-instrumenta Agent Framework/Azure OpenAI supportati.

## 7. Agent 365 S2S

Il golden sample implementa il pattern **custom engine + standard Entra app registration + client credentials**.

Prerequisiti:

- app registration standard;
- application permission `Agent365.Observability.OtelWrite`;
- admin consent;
- secret disponibile solo a runtime.

Configurazione:

```bash
export Agent365__ExportToAgent365=true
export Agent365__TenantId="<tenant>"
export Agent365__AgentId="<app-client-id>"
export Agent365__ClientSecret="<secret>"
```

Scope usato:

```text
api://9b975845-388f-4429-889e-eab1ef63949c/.default
```

Il sample verifica che `AgentId` e `TenantId` passati nel baggage coincidano con l'identità usata dal token resolver.

## 8. Baggage

Ogni turno crea un contesto:

- tenant ID;
- agent ID;
- conversation ID.

Questo è necessario perché l'exporter Agent 365 partiziona e valida la telemetry per tenant/agente.

## 9. Docker

```bash
docker compose up --build
```

## 10. Definition of Done

- [ ] project restore/build/publish verdi;
- [ ] container build verde;
- [ ] `/health` risponde 200;
- [ ] `/api/config` non espone secret;
- [ ] Azure OpenAI invocation funzionante;
- [ ] tool read-only invocato;
- [ ] draft write tool invocato senza side effect;
- [ ] conversation ID riusato correttamente;
- [ ] console telemetry visibile;
- [ ] baggage tenant/agent valorizzato;
- [ ] export Agent 365 verificato in tenant, se abilitato.

## Fonti

- https://learn.microsoft.com/en-us/agent-framework/agents/providers/azure-openai
- https://learn.microsoft.com/en-us/agent-framework/concepts/agents
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup
