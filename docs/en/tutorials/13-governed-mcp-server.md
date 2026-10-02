# Tutorial 13 — Governed .NET MCP Server

## Goal

Build and run a real stdio MCP server with the official C# SDK, exposing classified and governed tools without connecting real external systems.

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

## Why it is separate from the Golden Agent

The golden agent remains on .NET 8 and demonstrates Agent Framework, model providers and Agent 365 observability. The MCP server is an independent process representing the external tool boundary, so MCP SDK upgrades do not force an agent-runtime upgrade.

## Start

```powershell
cd samples/mcp-governed-tools
dotnet restore
dotnet run
```

The process waits for MCP messages over stdin/stdout; it is not a normal interactive console.

## Stdio safety

Stdout is the MCP protocol stream. Application output can corrupt JSON-RPC.

The sample uses the official stdio transport, sends console logging to stderr and CI rejects `Console.WriteLine` in server source.

## Tools

- `get_governance_manifest`;
- `get_tool_audit_summary`;
- `lookup_governance_policy`;
- `create_draft_change_request`.

The manifest exposes operation, risk tier, enabled state, approval requirement, reversibility, side-effect classification and owner.

## Write-shaped tool

`create_draft_change_request` is Medium / write-draft but is deliberately a mock with `externalSideEffect=false`.

## Block policy

```text
AGIC_MCP_ENABLE_POLICY_LOOKUP=true
AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST=true
```

Disabled tools return a denied governance decision and are audited.

## Operator approval

Lab configuration:

```text
AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT=true
AGIC_MCP_APPROVAL_TOKEN=<local value>
```

No MCP tool can mint or reveal the approval token. This prevents a model from authorizing its own privileged action.

The shared token is a teaching mechanism, not a production approval system. Production approval should use an external identity/workflow boundary with subject binding, TTL, single-use semantics and audit.

## Audit privacy

`get_tool_audit_summary` stores decision metadata without full tool arguments.

## MCP client configuration

Use `mcp.json.example` as a starting point and keep secrets outside committed configuration.

## Environment inheritance

Stdio clients may inherit the parent process environment. Custom clients should pass only the minimal environment required by the MCP server to avoid leaking unrelated credentials.

## Real integration test

CI starts the server through the official `StdioClientTransport`, creates an `McpClient`, performs the handshake, lists tools and invokes `lookup_governance_policy`.

## Definition of Done

- [ ] .NET 10 build succeeds;
- [ ] stdio handshake succeeds;
- [ ] four tools are discoverable;
- [ ] read tool invocation succeeds;
- [ ] write-shaped tool has no external side effect;
- [ ] block policy validated;
- [ ] operator-only approval validated;
- [ ] approval secret not exposed;
- [ ] bounded metadata-only audit;
- [ ] no `Console.WriteLine` in the server;
- [ ] CI test and publish succeed.

## Sources

- https://github.com/modelcontextprotocol/csharp-sdk
- https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server
- https://modelcontextprotocol.io/docs/develop/build-server