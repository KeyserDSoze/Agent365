# Tutorial 08 — End-to-end Local Lab Runner

## Goal

Run a repeatable Windows validation of the golden agent with the model executing locally and produce a JSON evidence report.

The runner validates:

1. Foundry Local bootstrap;
2. direct OpenAI-compatible local inference;
3. Microsoft Agent Framework API boot;
4. a multi-turn conversation using the same session.

## Run

```powershell
cd samples/dotnet-golden-agent
.\scripts\run-local-lab.ps1
```

Defaults:

- model alias: `phi-4-mini`;
- Foundry Local port: `39839`;
- Golden Agent port: `5080`;
- Agent 365 cloud export disabled;
- console telemetry enabled.

## Validation flow

### 1. Foundry Local

The runner reuses `start-foundry-local.ps1` to install/start the CLI, download/load the model, resolve the concrete model ID and configure the golden agent environment.

### 2. Direct model inference

It calls:

```text
POST http://127.0.0.1:39839/v1/chat/completions
```

This separates model-runtime failures from agent-runtime failures.

### 3. Golden Agent health

The runner starts the .NET API and validates:

```text
GET /health
GET /api/config
```

The safe config must report `foundry-local` and the same model ID prepared by the bootstrap.

### 4. First agent turn

A first `POST /api/chat` must produce non-empty output.

### 5. Multi-turn

The second request reuses the returned `conversationId`. The test fails if the ID changes or the output is empty.

## Evidence

Output is written to:

```text
samples/dotnet-golden-agent/lab-output/
```

including:

- `latest.json`;
- timestamped JSON reports;
- `operations-export.json`;
- stdout/stderr logs.

`operations-export.json` uses the `agent365-golden-agent-evidence/v1` schema and can be loaded directly into the site's **Operations** dashboard.

The folder is ignored by Git.

## Useful switches

```powershell
.\scripts\run-local-lab.ps1 -ModelAlias qwen2.5-0.5b
.\scripts\run-local-lab.ps1 -FoundryPort 39900 -AgentPort 5090
.\scripts\run-local-lab.ps1 -KeepRunning
.\scripts\run-local-lab.ps1 -StopFoundryOnExit
```

## Definition of Done

- [ ] JSON report has `success=true`;
- [ ] direct local inference succeeds;
- [ ] API health succeeds;
- [ ] provider is `foundry-local`;
- [ ] first Agent Framework turn succeeds;
- [ ] second turn reuses the same conversation ID;
- [ ] evidence report is retained.

## Sources

- https://learn.microsoft.com/en-us/azure/foundry-local/how-to/how-to-use-foundry-local-cli
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-rest
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
