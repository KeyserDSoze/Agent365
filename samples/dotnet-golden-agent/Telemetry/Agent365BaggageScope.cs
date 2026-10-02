using Agent365.GoldenAgent.Configuration;
using Microsoft.Agents.A365.Observability.Runtime.Common;

namespace Agent365.GoldenAgent.Telemetry;

public static class Agent365BaggageScope
{
    public static IDisposable? Create(
        Agent365ObservabilityOptions options,
        string conversationId)
    {
        if (string.IsNullOrWhiteSpace(options.TenantId) ||
            string.IsNullOrWhiteSpace(options.AgentId))
        {
            // Local-only mode: Agent Framework/OpenAI telemetry can still be exported to console.
            return null;
        }

        return new BaggageBuilder()
            .TenantId(options.TenantId)
            .AgentId(options.AgentId)
            .ConversationId(conversationId)
            .Build();
    }
}
