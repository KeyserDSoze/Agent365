# AGIC Agent 365 — Governed MCP Tools

A small **real MCP stdio server** that demonstrates how to expose custom tools while keeping governance decisions explicit.

It is intentionally separate from the .NET 8 golden agent.

## Runtime

- .NET 10
- official `ModelContextProtocol` C# SDK 2.2.0
- stdio transport
- dependency injection
- MCP attribute-based tool discovery
- all logging routed to **stderr**
- no ordinary stdout writes

## Tools

### `get_governance_manifest`

Read-only governance metadata.

It reports each governed tool's:
- operation;
- risk tier;
- enabled state;
- approval requirement;
- external-side-effect flag;
- reversibility;
- owner.

It never returns the operator approval token.

### `get_tool_audit_summary`

Read-only, metadata-only audit.

It does not store full tool arguments.

### `lookup_governance_policy`

Low-risk read-only mock policy lookup.

### `create_draft_change_request`

Medium-risk, write-shaped sample.

It **does not modify any external system**. It only creates a mock draft object.

When approval is enabled, the server accepts an operator-provisioned token. No MCP tool can create or reveal that token.

## Run

Prerequisite:

```text
.NET 10 SDK
```

Then:

```bash
cd samples/mcp-governed-tools
dotnet restore
dotnet run
```

When run manually, the process waits for MCP stdio messages. Do not type arbitrary text into stdout-oriented wrappers.

## Configure a client

Copy the shape from:

```text
mcp.json.example
```

The important transport configuration is:

```json
{
  "type": "stdio",
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "samples/mcp-governed-tools/Agent365.GovernedMcpServer.csproj",
    "--no-launch-profile"
  ]
}
```

Adjust the project path for the MCP client's working directory.

## Governance configuration

Environment variables:

```text
AGIC_MCP_ENABLE_POLICY_LOOKUP=true
AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST=true
AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT=false
AGIC_MCP_APPROVAL_TOKEN=
AGIC_MCP_AUDIT_CAPACITY=200
```

### Approval mode

For a controlled lab:

```powershell
$env:AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT="true"
$env:AGIC_MCP_APPROVAL_TOKEN="lab-approval-value"
```

The model/client must supply the same value to `create_draft_change_request`.

This is a **teaching pattern**, not a production human-approval service. Production approval should use an external identity/workflow boundary rather than a shared environment secret.

## Why no approval-generation tool?

Because a model that can call the write tool must not also be able to mint its own approval.

The operator/human provisions approval outside MCP.

## Stdio safety

MCP stdio reserves stdout for protocol messages.

This sample:
- uses the official stdio transport;
- uses an empty host builder;
- sends console logs to stderr;
- never calls `Console.WriteLine`.

A stray stdout message can corrupt the protocol stream.

## Audit privacy

Audit records contain:
- timestamp;
- tool;
- operation;
- risk tier;
- allow/deny;
- success/failure;
- side-effect classification;
- reason;
- duration.

They do **not** contain complete tool arguments.

## Tests

```bash
dotnet test ./tests/Agent365.GovernedMcpServer.Tests.csproj
```

Tests cover:
- manifest secret redaction;
- read-only behavior;
- block/deny;
- approval enforcement;
- mock-only write behavior;
- bounded metadata-only audit.

## Production boundary

This sample is intentionally a local governance lab.

Before using the same pattern with real external systems:
- replace the lab approval token with an external approval workflow;
- authenticate the MCP client/server boundary where applicable;
- use scoped downstream credentials;
- persist audit to an approved evidence platform;
- add per-user/agent authorization;
- define revocation and incident response;
- classify destructive and irreversible tools explicitly;
- validate Agent 365 registry and policy integration.

## Sources

- MCP C# SDK  
  https://github.com/modelcontextprotocol/csharp-sdk

- Microsoft .NET MCP quickstart  
  https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server

- Build an MCP server  
  https://modelcontextprotocol.io/docs/develop/build-server
