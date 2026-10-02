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
