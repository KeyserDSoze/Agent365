using System.ComponentModel;
using Agent365.GoldenAgent.Configuration;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;

namespace Agent365.GoldenAgent.Runtime;

public sealed class AgentRuntime
{
    private readonly AgentRuntimeOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Lazy<AIAgent> _agent;

    public AgentRuntime(
        IOptions<AgentRuntimeOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
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

    public async Task<RuntimeReadiness> CheckReadinessAsync(
        CancellationToken cancellationToken)
    {
        var provider = _options.Provider.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            return new RuntimeReadiness(
                false,
                provider,
                _options.Model,
                false,
                false,
                "Agent:Model is not configured.");
        }

        if (provider == "foundry-local")
        {
            if (!Uri.TryCreate(_options.FoundryLocalEndpoint, UriKind.Absolute, out var endpoint))
            {
                return new RuntimeReadiness(
                    false,
                    provider,
                    _options.Model,
                    false,
                    false,
                    "Agent:FoundryLocalEndpoint is invalid.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("provider-readiness");
                var origin = new Uri(endpoint.GetLeftPart(UriPartial.Authority));
                using var response = await client.GetAsync(
                    new Uri(origin, "/openai/status"),
                    cancellationToken);

                return new RuntimeReadiness(
                    response.IsSuccessStatusCode,
                    provider,
                    _options.Model,
                    true,
                    response.IsSuccessStatusCode,
                    response.IsSuccessStatusCode
                        ? null
                        : $"Foundry Local status returned HTTP {(int)response.StatusCode}.");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or TaskCanceledException)
            {
                return new RuntimeReadiness(
                    false,
                    provider,
                    _options.Model,
                    true,
                    false,
                    $"Foundry Local is not reachable: {ex.Message}");
            }
        }

        if (provider == "azure-openai")
        {
            if (!Uri.TryCreate(_options.AzureOpenAIEndpoint, UriKind.Absolute, out var endpoint))
            {
                return new RuntimeReadiness(
                    false,
                    provider,
                    _options.Model,
                    false,
                    false,
                    "Agent:AzureOpenAIEndpoint is invalid.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("provider-readiness");
                using var response = await client.GetAsync(endpoint, cancellationToken);

                // Any HTTP response proves that the configured host is reachable.
                return new RuntimeReadiness(
                    true,
                    provider,
                    _options.Model,
                    true,
                    true,
                    $"Azure endpoint reachable (HTTP {(int)response.StatusCode}); model/auth are validated on invocation.");
            }
            catch (Exception ex) when (
                ex is HttpRequestException or TaskCanceledException)
            {
                return new RuntimeReadiness(
                    false,
                    provider,
                    _options.Model,
                    true,
                    false,
                    $"Azure OpenAI endpoint is not reachable: {ex.Message}");
            }
        }

        return new RuntimeReadiness(
            false,
            provider,
            _options.Model,
            false,
            false,
            $"Unsupported Agent:Provider '{_options.Provider}'.");
    }

    private AIAgent CreateAgent()
    {
        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException(
                "Agent:Model is not configured. " +
                "For Foundry Local, run scripts/start-foundry-local.ps1 first.");
        }

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(LookupPolicy),
            AIFunctionFactory.Create(CreateDraftChangeRequest)
        };

        IChatClient chatClient = _options.Provider.Trim().ToLowerInvariant() switch
        {
            "foundry-local" => CreateFoundryLocalClient(),
            "azure-openai" => CreateAzureOpenAIClient(),
            _ => throw new InvalidOperationException(
                $"Unsupported Agent:Provider '{_options.Provider}'. " +
                "Supported values: foundry-local, azure-openai.")
        };

        return chatClient.AsAIAgent(
            instructions: _options.Instructions,
            name: _options.Name,
            tools: tools);
    }

    private IChatClient CreateFoundryLocalClient()
    {
        if (!Uri.TryCreate(_options.FoundryLocalEndpoint, UriKind.Absolute, out var endpoint))
        {
            throw new InvalidOperationException(
                "Agent:FoundryLocalEndpoint must be an absolute URI.");
        }

        return new OpenAIClient(
                new ApiKeyCredential("foundry-local"),
                new OpenAIClientOptions { Endpoint = endpoint })
            .GetChatClient(_options.Model)
            .AsIChatClient();
    }

    private IChatClient CreateAzureOpenAIClient()
    {
        if (!Uri.TryCreate(_options.AzureOpenAIEndpoint, UriKind.Absolute, out var endpoint))
        {
            throw new InvalidOperationException(
                "Agent:AzureOpenAIEndpoint must be configured with an absolute URI " +
                "when Agent:Provider is azure-openai.");
        }

        return new AzureOpenAIClient(
                endpoint,
                new DefaultAzureCredential())
            .GetChatClient(_options.Model)
            .AsIChatClient();
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
