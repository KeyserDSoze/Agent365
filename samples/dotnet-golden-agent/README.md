# AGIC Agent 365 Golden Agent — .NET

A runnable reference implementation that shows how to layer:

- **Microsoft Agent Framework** — agent runtime, sessions and function tools;
- **Azure OpenAI Chat Completions** — stable model-provider path for the baseline sample;
- **Microsoft OpenTelemetry Distro** — local and Agent 365 telemetry;
- **Agent 365 observability S2S** — optional custom-engine export;
- **ASP.NET Core** — a small HTTP surface for local/container tests.

This sample deliberately does not perform real writes to external systems. The write-style tool only creates a mock draft response.

## Architecture

```text
HTTP client
   |
   v
ASP.NET Core /api/chat
   |
   +--> BaggageBuilder (tenant / agent / conversation)
   |
   v
Microsoft Agent Framework
   |
   +--> Azure OpenAI Responses API
   |
   +--> function tools
   |
   v
Microsoft OpenTelemetry Distro
   |
   +--> Console (default)
   |
   +--> Agent 365 exporter (optional)
```

## Prerequisites

- .NET 8 SDK
- Azure OpenAI resource with a deployed chat model
- `az login` for local Azure OpenAI authentication
- for Agent 365 export: a **standard Entra app registration** with application permission `Agent365.Observability.OtelWrite` and admin consent

Current Microsoft guidance identifies the observability resource as:

```text
api://9b975845-388f-4429-889e-eab1ef63949c/.default
```

## Run locally

```bash
cd samples/dotnet-golden-agent

export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="gpt-4o-mini"

az login
dotnet restore
dotnet run
```

Then:

```bash
curl http://localhost:8080/health

curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

ASP.NET Core may choose a different local port when launched without an explicit URL. To force the same port as Docker:

```bash
ASPNETCORE_URLS=http://localhost:8080 dotnet run
```

## Conversations

Send the `conversationId` returned from the first call to continue the Agent Framework session.

The sample uses an in-memory store. It is intentionally not production persistence.

## Tool examples

The agent has two local function tools:

- `LookupPolicy` — read-only mock policy lookup;
- `CreateDraftChangeRequest` — produces a draft object with `externalSideEffect=false`.

The second tool exists to teach the team to distinguish a tool that *looks like a write* from an approved production integration.

## Local observability

Console export is on by default:

```text
Agent365__ExportToConsole=true
Agent365__ExportToAgent365=false
```

The Microsoft OpenTelemetry Distro automatically instruments supported Agent Framework and Azure OpenAI operations. The HTTP endpoint also creates Agent 365 baggage when tenant/agent IDs are configured.

## Enable Agent 365 export — custom-engine S2S

Set:

```bash
export Agent365__ExportToAgent365=true
export Agent365__AgentId="<standard-app-registration-client-id>"
export Agent365__TenantId="<tenant-id>"
export Agent365__ClientSecret="<secret>"
```

The exporter uses the Agent 365 S2S path and requests:

```text
api://9b975845-388f-4429-889e-eab1ef63949c/.default
```

The `AgentId` in baggage must match the app registration Client ID used to authenticate; the sample fails fast if they differ.

### Required Entra permission

On the app registration, add:

```text
Agent365.Observability.OtelWrite — Application
```

Grant tenant admin consent.

## Security notes

- Never commit the client secret.
- Use a secret store in real deployments.
- `DefaultAzureCredential` is used for Azure OpenAI local development. In production, prefer a specific credential such as Managed Identity.
- The custom-engine S2S observability path intentionally uses a standard app registration; Agent 365-enabled blueprint identities use a different S2S exchange flow.
- Add authentication/authorization in front of `/api/chat` before exposing this service outside a trusted lab.
- Replace the in-memory conversation store before production.

## Docker

```bash
docker compose up --build
```

The image can start without Azure OpenAI configuration; `/health` and `/api/config` still work. `/api/chat` returns 503 until the model endpoint is configured.

## Validate

1. `GET /health`
2. `GET /api/config` — confirm no secret is exposed
3. POST a first chat turn
4. reuse `conversationId`
5. trigger the policy tool
6. inspect console telemetry
7. only then enable Agent 365 export
8. verify tenant/agent baggage and expected telemetry surfaces

## Source of truth

- Agent Framework Azure OpenAI provider  
  https://learn.microsoft.com/en-us/agent-framework/agents/providers/azure-openai

- Microsoft OpenTelemetry Distro  
  https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry

- Agent 365 observability concepts  
  https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-concepts

- Observability authentication setup  
  https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup


## Why the baseline uses Chat Completions

Microsoft Agent Framework supports both Azure OpenAI Responses and Chat Completions. Responses is the richer recommended API for hosted tools, but the current .NET Responses surface still carries evaluation/prerelease diagnostics in the Azure/OpenAI client stack. The golden baseline intentionally uses the stable Chat Completions path so the reference project builds cleanly with warnings-as-errors. A separate Responses variant can be added without changing the Agent 365 architecture.
