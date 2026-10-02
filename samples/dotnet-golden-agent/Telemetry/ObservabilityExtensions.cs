using Agent365.GoldenAgent.Configuration;
using Azure.Core;
using Azure.Identity;
using Microsoft.OpenTelemetry;

namespace Agent365.GoldenAgent.Telemetry;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder ConfigureAgent365Observability(
        this WebApplicationBuilder builder)
    {
        var options = builder.Configuration
            .GetSection(Agent365ObservabilityOptions.SectionName)
            .Get<Agent365ObservabilityOptions>() ?? new();

        ClientSecretCredential? credential = null;

        if (options.ExportToAgent365)
        {
            if (string.IsNullOrWhiteSpace(options.TenantId) ||
                string.IsNullOrWhiteSpace(options.AgentId) ||
                string.IsNullOrWhiteSpace(options.ClientSecret))
            {
                throw new InvalidOperationException(
                    "Agent365 export requires TenantId, AgentId and ClientSecret.");
            }

            credential = new ClientSecretCredential(
                options.TenantId,
                options.AgentId,
                options.ClientSecret);
        }

        builder.UseMicrosoftOpenTelemetry(otel =>
        {
            ExportTarget targets = 0;

            if (options.ExportToConsole)
            {
                targets |= ExportTarget.Console;
            }

            if (options.ExportToAgent365)
            {
                targets |= ExportTarget.Agent365;
                otel.Agent365.UseS2SEndpoint = true;
                otel.Agent365.TokenResolver = async (agentId, tenantId) =>
                {
                    if (!string.Equals(agentId, options.AgentId, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Baggage agent ID does not match the configured app registration Client ID.");
                    }

                    if (!string.Equals(tenantId, options.TenantId, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Baggage tenant ID does not match the configured Tenant ID.");
                    }

                    var token = await credential!.GetTokenAsync(
                        new TokenRequestContext([Agent365ObservabilityOptions.ObservabilityScope]));

                    return token.Token;
                };
            }

            otel.Exporters = targets == 0 ? ExportTarget.Console : targets;
        });

        return builder;
    }
}
