using System.ComponentModel;
using Agent365.GoldenAgent.Configuration;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace Agent365.GoldenAgent.Runtime;

public sealed class AgentRuntime
{
    private readonly AgentRuntimeOptions _options;
    private readonly Lazy<AIAgent> _agent;

    public AgentRuntime(IOptions<AgentRuntimeOptions> options)
    {
        _options = options.Value;
        _agent = new Lazy<AIAgent>(CreateAgent, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Task<AgentSession> CreateSessionAsync(CancellationToken cancellationToken) =>
        _agent.Value.CreateSessionAsync(cancellationToken).AsTask();

    public async Task<string> RunAsync(
        string message,
        AgentSession session,
        CancellationToken cancellationToken)
    {
        AgentResponse response = await _agent.Value.RunAsync(
            message,
            session,
            cancellationToken: cancellationToken);

        return response.ToString();
    }

    private AIAgent CreateAgent()
    {
        if (string.IsNullOrWhiteSpace(_options.AzureOpenAIEndpoint))
        {
            throw new InvalidOperationException(
                "Agent:AzureOpenAIEndpoint is not configured. " +
                "Set Agent__AzureOpenAIEndpoint before calling /api/chat.");
        }

        if (!Uri.TryCreate(_options.AzureOpenAIEndpoint, UriKind.Absolute, out var endpoint))
        {
            throw new InvalidOperationException(
                "Agent:AzureOpenAIEndpoint must be an absolute URI.");
        }

        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException("Agent:Model is not configured.");
        }

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(LookupPolicy),
            AIFunctionFactory.Create(CreateDraftChangeRequest)
        };

        IChatClient chatClient = new AzureOpenAIClient(
                endpoint,
                new DefaultAzureCredential())
            .GetChatClient(_options.Model)
            .AsIChatClient();

        return chatClient.AsAIAgent(
            instructions: _options.Instructions,
            name: _options.Name,
            tools: tools);
    }

    [Description("Returns a mock internal governance policy summary for the requested policy code.")]
    private static string LookupPolicy(
        [Description("Policy code, for example AGENT-IDENTITY or TOOL-GOVERNANCE.")]
        string policyCode)
    {
        return policyCode.Trim().ToUpperInvariant() switch
        {
            "AGENT-IDENTITY" =>
                "Every production agent must have an explicit owner, a documented identity model, least-privilege permissions, and a tested revocation path.",
            "TOOL-GOVERNANCE" =>
                "Every write-capable or externally hosted tool must have an owner, risk tier, approved use case, logging requirement, and revocation path.",
            "DATA-GOVERNANCE" =>
                "Every production agent must document its data sources, classifications, allowed operations, applicable controls, and audit evidence.",
            _ =>
                "No mock policy was found for that code. Available examples: AGENT-IDENTITY, TOOL-GOVERNANCE, DATA-GOVERNANCE."
        };
    }

    [Description("Creates a mock draft change request. It does not modify any external system.")]
    private static object CreateDraftChangeRequest(
        [Description("Short title of the proposed change.")] string title,
        [Description("Why the change is needed.")] string rationale)
    {
        var seed = $"{title}|{rationale}".GetHashCode(StringComparison.Ordinal);
        var id = $"DRAFT-CR-{Math.Abs(seed % 100000):D5}";

        return new
        {
            id,
            state = "draft",
            title,
            rationale,
            externalSideEffect = false
        };
    }
}
