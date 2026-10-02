# Tutorial 06 — .NET Golden Agent end-to-end

## Goal

Run a real reference agent that clearly separates the agent runtime, model provider, tools, sessions, Agent 365 observability, configuration/secrets and HTTP hosting.

Project:

```text
samples/dotnet-golden-agent/
```

## Stack

- ASP.NET Core
- Microsoft Agent Framework 1.23
- Azure OpenAI
- Microsoft OpenTelemetry Distro
- Agent 365 custom-engine S2S observability

## 1. Local-only boot

The service can boot without Azure OpenAI configuration. This validates bootstrap, health, safe configuration and the local telemetry pipeline.

```bash
cd samples/dotnet-golden-agent
ASPNETCORE_URLS=http://localhost:8080 dotnet run
```

```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/config
```

## 2. Configure Azure OpenAI

```bash
export Agent__AzureOpenAIEndpoint="https://YOUR-RESOURCE.openai.azure.com/"
export Agent__Model="gpt-4o-mini"
az login
```

The baseline uses Azure OpenAI Chat Completions through `IChatClient`, then creates an Agent Framework `AIAgent`.

## 3. First turn

```bash
curl -X POST http://localhost:8080/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"What does policy AGENT-IDENTITY require?"}'
```

Reuse the returned `conversationId` for subsequent turns.

## 4. Tools

The agent exposes:

- `LookupPolicy` — read-only mock lookup;
- `CreateDraftChangeRequest` — write-shaped tool that creates a mock draft only and explicitly returns `externalSideEffect=false`.

## 5. Conversation safety

The in-memory store serializes turns per conversation ID so concurrent requests do not operate on the same `AgentSession` at the same time.

## 6. Observability

Default:

```text
Agent365__ExportToConsole=true
Agent365__ExportToAgent365=false
```

Microsoft OpenTelemetry automatically instruments supported Agent Framework and Azure OpenAI activity.

## 7. Agent 365 S2S

The sample implements the custom-engine path using a standard Entra app registration and client credentials.

Required permission:

```text
Agent365.Observability.OtelWrite — Application
```

Runtime configuration:

```bash
export Agent365__ExportToAgent365=true
export Agent365__TenantId="<tenant>"
export Agent365__AgentId="<app-client-id>"
export Agent365__ClientSecret="<secret>"
```

Scope:

```text
api://9b975845-388f-4429-889e-eab1ef63949c/.default
```

## 8. Definition of Done

- [ ] restore/build/publish succeeds;
- [ ] Docker build succeeds;
- [ ] health returns 200;
- [ ] safe config exposes no secret;
- [ ] Azure OpenAI invocation works;
- [ ] tools work as designed;
- [ ] multi-turn conversation works;
- [ ] console telemetry is visible;
- [ ] tenant/agent baggage is present;
- [ ] Agent 365 export is verified when enabled.

## Sources

- https://learn.microsoft.com/en-us/agent-framework/agents/providers/azure-openai
- https://learn.microsoft.com/en-us/agent-framework/concepts/agents
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/observability-authentication-setup
