using Agent365.GoldenAgent.Configuration;
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
                otel.Agent365.Exporter.UseS2SEndpoint = true;
                otel.Agent365.Exporter.TokenResolver = async (agentId, tenantId) =>
                {
                    using var scope = builder.Services.BuildServiceProvider().CreateScope();
                    var tokenProvider = scope.ServiceProvider
                        .GetRequiredService<Agent365TokenProvider>();

                    return await tokenProvider.GetTokenAsync(agentId, tenantId);
                };
            }

            otel.Exporters = targets == 0 ? ExportTarget.Console : targets;
        });

        return builder;
    }
}
