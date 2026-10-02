# Tutorial 06 — .NET Golden Agent end-to-end

## Goal

Run a real reference agent while separating agent runtime, model provider, tools, sessions and Agent 365 observability.

Project:

```text
samples/dotnet-golden-agent/
```

## Stack

- ASP.NET Core
- Microsoft Agent Framework
- **Microsoft Foundry Local — default**
- Azure OpenAI — optional provider
- Microsoft OpenTelemetry Distro
- Agent 365 custom-engine S2S observability

## 1. Recommended path: Foundry Local

On Windows:

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

The script prepares Foundry Local, downloads/loads the model and starts the API with the resolved local model ID and endpoint.

See [Foundry Local as the model runtime](07-foundry-local.md).

## 2. Health and safe config

```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/config
```

## 3. First turn

```bash
curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

Reuse the returned `conversationId`.

## 4. Session safety

The store serializes turns per conversation ID to avoid concurrent access to the same `AgentSession`.

## 5. Tools

- `LookupPolicy`: read-only mock.
- `CreateDraftChangeRequest`: write-shaped mock with `externalSideEffect=false`.

Tool calling depends on the selected local model capability.

## 6. Optional Azure OpenAI provider

```bash
export Agent__Provider="azure-openai"
export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="<deployment-name>"
az login
dotnet run
```

The provider boundary remains `IChatClient`.

## 7. Observability

Console is default. Agent 365 S2S export is optional and requires `Agent365.Observability.OtelWrite`.

## 8. Docker

When Foundry Local runs on the Windows host, the container reaches it through:

```text
http://host.docker.internal:39839/v1
```

## 9. Definition of Done

- [ ] restore/build/publish succeeds;
- [ ] app health succeeds;
- [ ] Docker build and container health succeed;
- [ ] Foundry Local model is downloaded/loaded;
- [ ] local model invocation works;
- [ ] multi-turn works;
- [ ] tools validated with a tool-capable model;
- [ ] console telemetry visible;
- [ ] Agent 365 export verified when enabled.

## Sources

- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/agent-framework/concepts/agents
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
