# Tutorial 07 — Foundry Local as the model runtime

## Why

Agent 365 labs do not need Azure OpenAI. Microsoft Foundry Local runs LLMs directly on the device with a managed catalog, hardware-aware model selection and an OpenAI-compatible local API.

This is the **default** model-provider path for the golden sample.

## Foundry Local handles

- a catalog of 20+ OSS models;
- model download and cache;
- hardware-compatible CPU/GPU/NPU variants;
- execution providers;
- model load/unload;
- a local OpenAI-compatible endpoint.

## Windows setup

The repository script automates the setup where possible:

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

If the CLI is missing, it attempts:

```powershell
winget install Microsoft.FoundryLocal
```

The CLI is currently Preview. The native SDK is the preferred approach when embedding Foundry Local directly into an application.

## Default model

The lab uses the alias:

```text
phi-4-mini
```

Using an alias lets Foundry Local select the best model variant for the hardware.

## Bootstrap flow

The script:
1. checks/installs Foundry Local;
2. restarts the local server on port 39839;
3. downloads the model if needed;
4. loads it;
5. queries `/v1/models` for the concrete model ID;
6. configures the golden agent provider/endpoint/model;
7. optionally starts the API.

## No Azure model dependency

Foundry Local mode requires no Azure OpenAI resource, cloud API key or `az login`. Agent 365 observability remains independent of the model provider.

## Tool calling

Tool-calling support depends on the chosen local model. Verify model capabilities before validating the golden-agent function tools.

## Sources

- https://learn.microsoft.com/en-us/windows/ai/apis/local-llms
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-sdk-current
