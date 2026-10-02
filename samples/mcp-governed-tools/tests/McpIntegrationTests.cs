using Agent365.GovernedMcpServer.Governance;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Xunit;

namespace Agent365.GovernedMcpServer.Tests;

public sealed class McpIntegrationTests
{
    [Fact]
    public async Task StdioServer_Handshakes_ListsTools_AndInvokesReadTool()
    {
        var project = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../Agent365.GovernedMcpServer.csproj"));

        Assert.True(File.Exists(project), $"MCP server project not found: {project}");

        var environment = StdioClientTransportOptions.GetDefaultEnvironmentVariables();

        foreach (var key in new[] { "DOTNET_ROOT", "NUGET_PACKAGES" })
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                environment[key] = value;
            }
        }

        environment["AGIC_MCP_ENABLE_POLICY_LOOKUP"] = "true";
        environment["AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST"] = "true";
        environment["AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT"] = "false";
        environment["AGIC_MCP_AUDIT_CAPACITY"] = "50";

        var stderr = new List<string>();

        var transport = new StdioClientTransport(
            new StdioClientTransportOptions
            {
                Name = "AGIC Agent 365 Governed MCP CI",
                Command = "dotnet",
                Arguments =
                [
                    "run",
                    "--project",
                    project,
                    "--configuration",
                    "Release",
                    "--no-build",
                    "--no-launch-profile"
                ],
                InheritEnvironmentVariables = false,
                EnvironmentVariables = environment,
                ShutdownTimeout = TimeSpan.FromSeconds(10),
                StandardErrorLines = line =>
                {
                    lock (stderr)
                    {
                        stderr.Add(line);
                    }
                }
            });

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await using var client = await McpClient.CreateAsync(
            transport,
            cancellationToken: timeout.Token);

        var tools = await client.ListToolsAsync(
            cancellationToken: timeout.Token);

        Assert.Contains(
            tools,
            tool => tool.Name == "get_governance_manifest");
        Assert.Contains(
            tools,
            tool => tool.Name == "get_tool_audit_summary");
        Assert.Contains(
            tools,
            tool => tool.Name == ToolGovernanceService.PolicyLookupTool);
        Assert.Contains(
            tools,
            tool => tool.Name == ToolGovernanceService.DraftChangeRequestTool);

        var lookup = tools.Single(
            tool => tool.Name == ToolGovernanceService.PolicyLookupTool);

        var arguments = new AIFunctionArguments
        {
            ["policyCode"] = "AGENT-IDENTITY"
        };

        var result = await lookup.InvokeAsync(
            arguments,
            cancellationToken: timeout.Token);

        var text = result?.ToString()
            ?? throw new InvalidOperationException(
                "MCP tool invocation returned a null result.");

        Assert.Contains(
            "AGENT-IDENTITY",
            text,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "completed",
            text,
            StringComparison.OrdinalIgnoreCase);
    }
}
