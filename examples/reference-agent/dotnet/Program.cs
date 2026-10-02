using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Microsoft OpenTelemetry Distro is installed in this sample.
// Exact Agent 365 authentication configuration depends on whether your agent
// uses OBO or S2S. Follow the current Microsoft guidance before enabling
// tenant export.
//
// Start by validating your real runtime locally with console/OTel export,
// then add the Agent 365 token resolver and identifiers.
//
// Official guidance:
// https://learn.microsoft.com/en-us/microsoft-agent-365/developer/microsoft-opentelemetry

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();

using var host = builder.Build();

var prompt = Environment.GetEnvironmentVariable("SAMPLE_PROMPT") ?? "hello Agent 365";
Console.WriteLine($"Reference agent received: {prompt}");

// Replace the line above with your actual Agent Framework / Semantic Kernel /
// OpenAI / custom agent runtime, then add builder.UseMicrosoftOpenTelemetry(...)
// following the current Microsoft sample for your authentication model.
