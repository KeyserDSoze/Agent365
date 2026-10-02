# 09 — Agent 365 SDK, CLI & Custom Agents

## SDK capabilities
Current Microsoft documentation describes four main Agent 365 SDK capability areas:
1. **Identity**
2. **Observability**
3. **Tooling**
4. **Notifications**

Adopt only the capabilities the agent actually needs.

## Preview caveat
Current guidance states that agent user account/mailbox capability is limited to tenants in the Frontier preview program. Notifications depend on the agent user account and are therefore tied to the same preview program.

## CLI
Cross-platform tool for deployment, management, automation and troubleshooting. Use the documented minimum roles per command instead of defaulting to Global Administrator.

## Current installation
```bash
dotnet tool install --global Microsoft.Agents.A365.DevTools.Cli
```

Update:
```bash
dotnet tool update --global Microsoft.Agents.A365.DevTools.Cli
```

## Onboarding
Register/discover → identity → permissions → observability → tools → inventory validation → security review → revocation test → owner/runbook → CI/CD.

## Sources
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-sdk
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-cli
- https://learn.microsoft.com/en-us/microsoft-agent-365/developer/reference/cli/
