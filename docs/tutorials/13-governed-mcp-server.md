# Tutorial 13 — Governed MCP Server .NET

## Obiettivo

Costruire ed eseguire un MCP server stdio reale con l'SDK C# ufficiale, esponendo tool classificati e governati senza collegare sistemi esterni reali.

Sample:

`samples/mcp-governed-tools/`

## Stack

- .NET 10;
- ModelContextProtocol 2.2.0;
- Microsoft.Extensions.Hosting 10.0.12;
- stdio transport;
- attribute-based tool discovery;
- dependency injection;
- metadata-only audit.

## Perché separato dal Golden Agent

Il golden agent resta su .NET 8 e dimostra Agent Framework + model provider + Agent 365 observability. Il server MCP è un processo indipendente e mostra la tool integration boundary. Separarli evita di trasformare ogni upgrade dell'SDK MCP in un upgrade obbligatorio del runtime agentico.

## 1. Avvio

Prerequisito: .NET 10 SDK.

```powershell
cd samples/mcp-governed-tools
dotnet restore
dotnet run
```

Il processo attende messaggi MCP su stdin/stdout. Non è una console interattiva tradizionale.

## 2. Stdio safety

Con stdio, stdout è il protocol stream. Un `Console.WriteLine` applicativo può corrompere JSON-RPC.

Il sample:

- usa `WithStdioServerTransport()`;
- usa un host minimale;
- invia i log console a stderr;
- vieta `Console.WriteLine` in CI.

## 3. Tool discovery

I tool usano:

```csharp
[McpServerToolType]
[McpServerTool]
[Description(...)]
```

Tool esposti:

- `get_governance_manifest`;
- `get_tool_audit_summary`;
- `lookup_governance_policy`;
- `create_draft_change_request`.

## 4. Governance manifest

`get_governance_manifest` restituisce operation, risk tier, enabled state, approval requirement, reversibility, external-side-effect flag e owner.

Il manifest dichiara esplicitamente che l'approval token non viene esposto.

## 5. Read-only tool

`lookup_governance_policy` è classificato Low / read-only e non ha side effect.

## 6. Write-shaped tool

`create_draft_change_request` è Medium / write-draft.

È intenzionalmente un mock:

```json
{
  "state": "draft",
  "externalSideEffect": false
}
```

Nessun CRM, ticketing system o altro sistema viene modificato.

## 7. Block / enable

Configurazione:

```text
AGIC_MCP_ENABLE_POLICY_LOOKUP=true
AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST=true
```

Quando un tool è disabled, il server restituisce una decisione `denied` e la registra nell'audit.

## 8. Approval

Per il lab:

```powershell
$env:AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT="true"
$env:AGIC_MCP_APPROVAL_TOKEN="lab-approval-value"
```

Il client deve fornire il token al tool write-shaped.

### Perché il server non genera approval

Un agente che può invocare un tool non deve poter creare autonomamente l'autorizzazione necessaria a quel tool.

Per questo nessun MCP tool del sample può leggere o generare `AGIC_MCP_APPROVAL_TOKEN`.

### Limite del pattern lab

Questo token condiviso non è un approval service production-grade e non è one-shot. In produzione usare un workflow esterno con identity, subject binding, TTL, single-use e audit.

## 9. Audit

`get_tool_audit_summary` espone metadata di allow/deny senza memorizzare gli argomenti completi dei tool.

Campi principali:

- timestamp;
- tool;
- operation;
- risk tier;
- decision;
- success;
- external side effect;
- reason;
- duration.

Capacity:

```text
AGIC_MCP_AUDIT_CAPACITY=200
```

## 10. Configurazione MCP client

Usare `mcp.json.example` come base. Il server viene avviato come child process:

```text
dotnet run --project samples/mcp-governed-tools/Agent365.GovernedMcpServer.csproj
```

Non inserire secret nel file committato. Usare configurazione locale/secret management del client.

## 11. Environment inheritance

I client stdio possono ereditare le environment variables del processo parent. Questo può propagare credenziali non necessarie.

Per client custom, preferire una allowlist minima delle variabili richieste dal server, come fa il test d'integrazione della repository.

## 12. Test reale MCP

La CI non si limita a compilare.

Avvia il server tramite `StdioClientTransport`, crea un `McpClient`, esegue handshake, `ListToolsAsync()` e invoca realmente `lookup_governance_policy`.

## Definition of Done

- [ ] server buildabile su .NET 10;
- [ ] handshake stdio riuscito;
- [ ] quattro tool discoverable;
- [ ] tool read invocabile;
- [ ] write-shaped tool senza side effect esterno;
- [ ] block policy verificata;
- [ ] approval operator-only verificata;
- [ ] approval secret non esposto;
- [ ] audit bounded metadata-only;
- [ ] nessun `Console.WriteLine` nel server;
- [ ] test e publish CI verdi.

## Fonti

- https://github.com/modelcontextprotocol/csharp-sdk
- https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server
- https://modelcontextprotocol.io/docs/develop/build-server