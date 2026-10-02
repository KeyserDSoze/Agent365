# 16 — Local Model Runtime: Microsoft Foundry Local

## Goal

Separate the **model runtime** from the Agent 365 control plane and make labs runnable without an Azure OpenAI dependency.

## Architecture

```text
Agent Framework
     |
     v
Model Provider Boundary
     |
     +--> Foundry Local        ← default lab
     +--> Azure OpenAI         ← optional cloud provider
     +--> other compatible     ← future
```

Agent 365 continues to handle identity, observability, governance and security independently from the model provider.

## Why Foundry Local

Microsoft Foundry Local provides:
- 20+ ready-to-use local models;
- a managed catalog;
- hardware-aware aliases;
- model download/cache;
- CPU/GPU/NPU variant selection;
- execution-provider management;
- OpenAI-compatible APIs;
- C#, Python, JavaScript and Rust SDKs;
- an optional local REST server.

## Foundry Local vs Windows AI APIs vs Windows ML

**Windows AI APIs** provide higher-level ready-to-use Windows AI capabilities.

**Foundry Local** is the repository default for managed local LLMs and OpenAI-compatible inference.

**Windows ML** is the lower-level option when bringing and controlling your own ONNX models.

## Agent Framework .NET: OpenAI-compatible boundary

Current Agent Framework documentation provides a dedicated `FoundryLocalClient` on the Python path; the equivalent integration is not currently documented for .NET.

The .NET golden sample therefore uses:

```text
Agent Framework
    |
    v
Microsoft.Extensions.AI IChatClient
    |
    v
OpenAI SDK
    |
    v
Foundry Local /v1/chat/completions
```

This keeps the model-provider boundary explicit and lets the same agent runtime switch between Foundry Local and Azure OpenAI without changing its sessions, tools or operating model.

## Native SDK

Windows package:

```bash
dotnet add package Microsoft.AI.Foundry.Local.WinML
```

The native SDK can initialize in-process, resolve a model by alias, download/cache it, load it and expose native chat or an optional local REST service.

Current Windows quickstart guidance targets Windows 11 24H2+ and .NET 9+.

## CLI

```powershell
winget install Microsoft.FoundryLocal

foundry model list
foundry model info phi-4-mini
foundry model download phi-4-mini
foundry model load phi-4-mini
foundry server restart --port 39839 --idle-timeout 0
foundry server status
```

The CLI is currently Preview.

## Alias vs model ID

Use a catalog alias such as `phi-4-mini` for hardware-aware model selection. Resolve the concrete loaded model ID from `/v1/models` before calling the OpenAI-compatible endpoint.

The golden sample automates that flow.

## Privacy boundary

Local inference stays on-device, but enabling Agent 365 telemetry, remote tools or cloud integrations can still move data off-device. Treat those flows independently.

## Tool calling

Function-tool support depends on the selected model. Validate catalog capability before running tool-oriented labs.

## Golden sample

See:
- `../../samples/dotnet-golden-agent/`
- `tutorials/07-foundry-local.md`

The default provider is `foundry-local`; Azure OpenAI remains optional.

## Sources

- https://learn.microsoft.com/en-us/windows/ai/apis/local-llms
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/windows/ai/windows-ai-comparison
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-sdk-current
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/agent-framework/integrations/by-component/model-providers/foundry-local
