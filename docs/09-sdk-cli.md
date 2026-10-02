# 09 — Agent 365 SDK, CLI & Custom Agents

## SDK

La documentazione Microsoft corrente descrive quattro capability principali aggiunte dal Microsoft Agent 365 SDK:

1. **Identity** — identità agente basata su Microsoft Entra.
2. **Observability** — telemetry OpenTelemetry auditabile e correlabile.
3. **Tooling** — accesso governato a Work IQ MCP server e workload Microsoft 365 supportati.
4. **Notifications** — ricezione/risposta a messaggi in workload Microsoft 365; alcune capability legate all'agent user account sono indicate come Frontier preview.

Non è necessario adottare tutto lo SDK: scegliere solo le capability necessarie.

## Attenzione Preview

La documentazione Microsoft corrente specifica che:
- agent user account / mailbox è disponibile solo per tenant che partecipano al programma Frontier preview;
- Notifications richiede l'agent user account ed è quindi anch'essa legata al programma Frontier preview.

Prima di proporre queste capability in produzione verificare il tenant e lo stato della feature.

## CLI

Tool cross-platform per deployment, management, automation e troubleshooting. Microsoft documenta ruoli minimi per comando: non usare Global Administrator come default operativo.

### Installazione corrente

```bash
dotnet tool install --global Microsoft.Agents.A365.DevTools.Cli
```

Update:

```bash
dotnet tool update --global Microsoft.Agents.A365.DevTools.Cli
```

Verificare la pagina ufficiale prima di fissare versioni o comandi in CI/CD.

## Custom-agent onboarding

Register/discover → identity → permission → observability → tools → inventory validation → security review → revocation test → owner/runbook → CI/CD.

## CI/CD design

Separare:
- build;
- security checks;
- identity/permission setup;
- deploy;
- post-deploy validation;
- Registry verification;
- telemetry verification.

Mai inserire secret o tenant credential nel repository.

## Fonti
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-sdk
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-cli
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/reference/cli/
