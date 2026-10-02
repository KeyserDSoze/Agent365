using Agent365.GovernedMcpServer.Governance;
using Agent365.GovernedMcpServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// MCP stdio reserves stdout for protocol traffic.
// All operational logs must therefore go to stderr.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

var policy = ToolPolicyOptions.FromEnvironment();

builder.Services.AddSingleton(policy);
builder.Services.AddSingleton<ToolAuditStore>();
builder.Services.AddSingleton<ToolGovernanceService>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<GovernedTools>();

await builder.Build().RunAsync();
