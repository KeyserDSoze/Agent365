# Microsoft Agent 365 Knowledge Hub

A bilingual, practical knowledge base for Microsoft Agent 365: architecture, governance, identity, security, data protection, tool governance, observability, integration, skilling and customer delivery.

> Last verified against Microsoft documentation: **2026-10-02**.

## Repository structure

```text
.
├── src/                    # React + Vite application published on GitHub Pages
│   ├── app/                # UI, content model, styles
│   └── public/
├── docs/                   # Formal technical study material
├── examples/
│   ├── kql/                # Defender Advanced Hunting examples
│   ├── checklists/         # Readiness and discovery checklists
│   └── templates/          # Reusable delivery artifacts
└── .github/workflows/      # GitHub Pages deployment
```

## Scope

The project covers Agent 365 architecture; Microsoft 365 admin center and Agent Registry; Entra Agent ID; Defender; Purview; MCP/tool governance; observability; SDK/CLI; operating model; training; labs and customer delivery.

## Run locally

```bash
cd src
npm install
npm run dev
```

## GitHub Pages

The workflow `.github/workflows/deploy-pages.yml` builds `src/` and deploys `src/dist` on every push to `main`.

If Pages was never enabled, open **Settings → Pages** and set **Source** to **GitHub Actions** once.

## Editorial rule

This repository accelerates technical readiness; it does not replace Microsoft documentation. Revalidate Preview features, licensing, role requirements and service limitations before production decisions.

Start from [docs/README.md](docs/README.md).


## Hands-on developer path

The repository now includes a practical developer track:

- `docs/tutorials/` — Italian step-by-step tutorials
- `docs/en/tutorials/` — English mirror
- `examples/reference-agent/` — Python, Node.js and .NET observability integration skeletons
- `examples/kql/advanced/` — additional Agent 365 hunting queries

The recommended flow is:

```text
setup → register → instrument → tools/DLP → validate → operate
```

For new observability integrations, the project follows Microsoft's current recommendation to use **Microsoft OpenTelemetry Distro** rather than the deprecated Agent 365 Observability SDK.


## Runnable .NET golden sample

The repository includes a buildable and containerized reference implementation under:

```text
samples/dotnet-golden-agent/
```

It demonstrates Microsoft Agent Framework + Azure OpenAI + Microsoft OpenTelemetry + optional Agent 365 S2S observability, with multi-turn sessions, safe mock tools, Docker and automated smoke tests.

Start here:
- [Golden Agent README](samples/dotnet-golden-agent/README.md)
- [Italian end-to-end tutorial](docs/tutorials/06-dotnet-golden-agent.md)
- [English end-to-end tutorial](docs/en/tutorials/06-dotnet-golden-agent.md)

The sample is a **lab/reference implementation**, not a production security boundary. Add API authentication, managed secrets, persistent session storage and deployment-specific controls before exposing it outside a trusted environment.
