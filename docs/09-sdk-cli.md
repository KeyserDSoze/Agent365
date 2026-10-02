# 09 — Agent 365 SDK, CLI & Custom Agents

## SDK
Usato per estendere agenti custom con capability compatibili con Agent 365, inclusi pattern di identity, observability e integrazione governata.

## CLI
Tool cross-platform per deployment/management e automation. Microsoft documenta ruoli minimi per comando: non usare Global Administrator come default.

## Installazione corrente
```bash
dotnet tool install --global Microsoft.Agents.A365.DevTools.Cli
```

Update:
```bash
dotnet tool update --global Microsoft.Agents.A365.DevTools.Cli
```

Verificare sempre la pagina ufficiale prima di fissare il comando in CI/CD.

## Onboarding checklist
Register → identity → permission → observability → tools → inventory validation → security review → revocation test → owner/runbook → CI/CD.

## Fonti
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-cli
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/reference/cli/
