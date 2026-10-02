# AGIC Agent 365 Golden Agent — .NET

A runnable reference implementation that keeps the **agent runtime**, **model runtime** and **Agent 365 control/observability plane** separate.

## Default stack

- **Microsoft Agent Framework** — agent runtime, sessions and function tools
- **Microsoft Foundry Local** — default local LLM runtime
- **OpenAI-compatible API** — provider boundary between Agent Framework and Foundry Local
- **Microsoft OpenTelemetry Distro** — local and Agent 365 telemetry
- **Agent 365 observability S2S** — optional export
- **ASP.NET Core** — HTTP API
- **Azure OpenAI** — optional cloud fallback/provider

The sample performs no real external write. Its change-request tool returns a mock draft only.

## Architecture

```text
HTTP client
   |
   v
ASP.NET Core /api/chat
   |
   +--> Agent 365 baggage (optional)
   |
   v
Microsoft Agent Framework
   |
   v
IChatClient
   |
   +--> Foundry Local (default)
   |      |
   |      +--> local OpenAI-compatible endpoint
   |      +--> local model cache
   |      +--> CPU / GPU / NPU variant
   |
   +--> Azure OpenAI (optional)
   |
   +--> function tools
   |
   v
Microsoft OpenTelemetry Distro
   |
   +--> Console
   +--> Agent 365 exporter (optional)
```

## Fastest Windows lab

Use Foundry Local.

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

The script:

1. detects the `foundry` CLI;
2. attempts `winget install Microsoft.FoundryLocal` when missing;
3. starts the local OpenAI-compatible service on port 39839;
4. downloads `phi-4-mini` if necessary;
5. loads the best hardware-compatible variant;
6. resolves the real loaded model ID across the current Foundry Local REST model surfaces;
7. sets `Agent__Provider`, `Agent__FoundryLocalEndpoint` and `Agent__Model`;
8. starts this API when `-RunAgent` is supplied.

First-time model/runtime downloads require internet connectivity.

## One-command end-to-end lab

To validate the complete local chain and generate evidence:

```powershell
.\scripts\run-local-lab.ps1
```

The runner validates Foundry Local bootstrap, direct model inference, Golden Agent health/config, a first Agent Framework turn and a second turn on the same conversation. It writes a timestamped JSON report and runtime logs under `lab-output/`.

Use:

```powershell
.\scripts\run-local-lab.ps1 -KeepRunning
.\scripts\run-local-lab.ps1 -StopFoundryOnExit
```

See: `docs/tutorials/08-local-lab-runner.md`.

## Why Foundry Local

For the training environment it removes unnecessary cloud dependencies:

- no Azure OpenAI resource;
- no model API key;
- no `az login`;
- inference stays on-device;
- model acquisition and hardware-aware variant selection are handled by Foundry Local.

The local provider is independent from Agent 365 observability. You can run completely local with console telemetry or enable Agent 365 telemetry separately.

## Manual Foundry Local setup

```powershell
winget install Microsoft.FoundryLocal

foundry server restart --port 39839 --idle-timeout 0
foundry model download phi-4-mini
foundry model load phi-4-mini

foundry server status
foundry model list --loaded --verbose
```

The golden agent expects:

```text
Agent__Provider=foundry-local
Agent__FoundryLocalEndpoint=http://127.0.0.1:39839/v1
Agent__Model=<concrete model id returned by /v1/models>
```

Do not hard-code a catalog variant in training scripts. Use an alias such as `phi-4-mini` to let Foundry Local select a compatible CPU/GPU/NPU model, then resolve the loaded model ID.

## Test the API

```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/config
```

First turn:

```bash
curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

Reuse the returned `conversationId` for multi-turn sessions.

## Function tools

### LookupPolicy
Read-only local mock.

### CreateDraftChangeRequest
Write-shaped example that explicitly returns:

```json
{
  "externalSideEffect": false
}
```

This is intentional: trainees must distinguish a mock tool from an approved production write integration.

> Tool calling depends on the capabilities of the local model selected from the Foundry Local catalog.

## Azure OpenAI fallback

To use the cloud provider instead:

```bash
export Agent__Provider="azure-openai"
export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="<deployment-name>"
az login

dotnet run
```

The sample uses the stable Chat Completions adapter through `IChatClient`.

## Agent 365 observability

Console export is enabled by default:

```text
Agent365__ExportToConsole=true
Agent365__ExportToAgent365=false
```

Enable custom-engine S2S export with a standard Entra app registration:

```bash
export Agent365__ExportToAgent365=true
export Agent365__AgentId="<app-client-id>"
export Agent365__TenantId="<tenant-id>"
export Agent365__ClientSecret="<secret>"
```

Required application permission:

```text
Agent365.Observability.OtelWrite
```

The exporter requests:

```text
api://9b975845-388f-4429-889e-eab1ef63949c/.default
```

Never commit the secret.

## Docker

Build and boot:

```bash
docker compose up --build
```

When Foundry Local runs on the Windows host, the container reaches it through:

```text
http://host.docker.internal:39839/v1
```

Set `Agent__Model` to the loaded model ID before invoking `/api/chat`.

The container can still boot without a configured model; health/config endpoints work and chat returns a configuration error until the runtime is ready.

## CI quality gate

The dedicated workflow validates:

1. restore;
2. compile with warnings as errors;
3. publish;
4. real HTTP boot;
5. `/health`;
6. `/api/config`;
7. Docker build;
8. container boot;
9. container health/config.

A CI runner does not download a Foundry Local model; actual inference validation belongs in the Windows lab.

## Production boundaries

Before exposing this service outside a trusted lab:

- authenticate and authorize the HTTP API;
- move secrets to a proper secret store;
- prefer Managed Identity for Azure-hosted cloud dependencies;
- replace in-memory sessions with a production persistence strategy;
- add rate limiting;
- review tool authorization and human approval;
- validate selected local-model tool-calling behavior;
- define prompt/data logging policy;
- document revocation and incident response.

## Sources

- Foundry Local on Windows  
  https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started

- Foundry Local CLI  
  https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli

- Foundry Local SDK  
  https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-sdk-current

- Agent Framework Azure OpenAI provider  
  https://learn.microsoft.com/en-us/agent-framework/agents/providers/azure-openai

- Microsoft OpenTelemetry Distro  
  https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry

- Agent 365 observability authentication  
  https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup
