# Microsoft Agent 365 Knowledge Hub

A bilingual, practical knowledge base for Microsoft Agent 365: architecture, governance, identity, security, data protection, tool governance, observability, local/cloud model runtimes, skilling and customer delivery.

> Last verified against Microsoft documentation: **2026-10-02**.

## Repository structure

```text
.
├── src/                    # React + Vite GitHub Pages application
│   ├── app/                # UI + in-site Markdown viewer
│   └── scripts/            # build-time knowledge synchronization
├── docs/                   # Formal academy (Italian)
│   └── en/                 # English mirror
├── examples/               # KQL, checklists, templates and integration skeletons
├── samples/
│   └── dotnet-golden-agent # Runnable Agent Framework golden sample
└── .github/workflows/      # Site + .NET CI/CD
```

## Website knowledge base

The site no longer treats GitHub Markdown as an external destination. During every build it copies the repository knowledge assets into the Pages artifact, generates an index and renders them through an internal React Markdown/GFM viewer.

Run locally:

```bash
cd src
npm install
npm run dev
```

## Hands-on developer path

```text
setup → register → instrument → local/cloud model → tools/DLP → validate → operate
```

The practical track includes:
- bilingual step-by-step tutorials;
- KQL hunting packs;
- readiness/discovery templates;
- Python/Node/.NET integration skeletons;
- a runnable .NET golden agent.

## Runnable .NET golden sample

`samples/dotnet-golden-agent/` uses:

- Microsoft Agent Framework;
- **Microsoft Foundry Local as the default model runtime**;
- Azure OpenAI as an optional fallback/provider;
- Microsoft OpenTelemetry;
- optional Agent 365 S2S observability;
- multi-turn sessions and safe mock tools;
- Docker and automated runtime/container smoke tests.

Fastest Windows path:

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

The script can install Foundry Local with winget, start its OpenAI-compatible service, download/load a local model and configure the golden agent.

Start from:
- [Formal academy](docs/README.md)
- [Hands-on tutorials](docs/tutorials/README.md)
- [Foundry Local chapter](docs/16-local-model-runtime.md)
- [Golden Agent README](samples/dotnet-golden-agent/README.md)

## Editorial rule

This repository accelerates technical readiness; it does not replace Microsoft documentation. Revalidate Preview features, licensing, roles, model capabilities, SDK/CLI behavior and service limitations before production decisions.


## Operations dashboard

The GitHub Pages site includes an **Operations** section that renders privacy-safe run evidence from the golden agent.

It can:
- load the bundled sample under `examples/evidence/operations-sample.json`;
- import a real `agent365-golden-agent-evidence/v1` JSON bundle locally in the browser;
- show run KPIs, success/failure, latency, provider/model, trace correlation and tool allow/deny decisions.

The Windows local lab runner automatically writes:

```text
samples/dotnet-golden-agent/lab-output/operations-export.json
```

That file can be loaded directly into the dashboard.


## Governed MCP server

The repository includes a standalone real MCP stdio server under:

```text
samples/mcp-governed-tools/
```

It uses the official C# MCP SDK and demonstrates:
- real stdio transport and tool discovery;
- read-only and write-shaped tools;
- risk metadata and local block policy;
- operator-provisioned approval;
- metadata-only bounded audit;
- real MCP client/server handshake and invocation tests.

The MCP sample is intentionally independent from the .NET 8 golden agent.


## Reliability and incident workflow

The golden agent adds explainable reliability assessment on top of run evidence. Thresholds cover failure rate, latency, tool deny rate and consecutive failures.

The operational flow is:

```text
run evidence
  → reliability assessment
  → finding
  → suspect run / trace correlation
  → incident snapshot
  → runbook
  → recovery gate
```

The local lab writes `incident-snapshot.json`, and the site Operations view surfaces the same reliability findings for drill-down.


## Guided journey

The site and the repository now use the same end-to-end structure:

```text
Understand
   ↓
Prepare
   ↓
Build
   ↓
Govern
   ↓
Operate
```

Every major page, formal chapter and practical asset is connected to one of these stages. The Knowledge Hub exposes a dedicated `/journey` route, while document pages show their journey context, related material and curated next/previous steps.

Practical assets are organized as delivery packs rather than loose files. In particular, the KQL material is presented as a sequence:

```text
inventory → governance gaps → tools/MCP → remediation/evidence
```

See:
- `docs/README.md`
- `examples/README.md`
- `examples/kql/README.md`
- `docs/17-kql-agent-operations.md`
