using System.ComponentModel;
using Agent365.GoldenAgent.Configuration;
using Agent365.GoldenAgent.Mcp;
using Agent365.GoldenAgent.Tools;
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
    private readonly ToolGovernanceService _tools;
    private readonly ToolInvocationContext _toolContext;
    private readonly GovernedMcpToolProvider _mcp;
    private readonly McpBridgeOptions _mcpOptions;
    private readonly Lazy<Task<AIAgent>> _agent;

    public AgentRuntime(
        IOptions<AgentRuntimeOptions> options,
        IHttpClientFactory httpClientFactory,
        ToolGovernanceService tools,
        ToolInvocationContext toolContext,
        GovernedMcpToolProvider mcp,
        IOptions<McpBridgeOptions> mcpOptions)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
        _tools = tools;
        _toolContext = toolContext;
        _mcp = mcp;
        _mcpOptions = mcpOptions.Value;
        _agent = new Lazy<Task<AIAgent>>(
            () => CreateAgentAsync(CancellationToken.None),
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<AgentSession> CreateSessionAsync(
        CancellationToken cancellationToken)
    {
        var agent = await _agent.Value.WaitAsync(cancellationToken);
        return await agent.CreateSessionAsync(cancellationToken);
    }

    public async Task<string> RunAsync(
        string message,
        AgentSession session,
        string conversationId,
        string runId,
        string traceId,
        CancellationToken cancellationToken)
    {
        using var toolScope = _toolContext.Begin(
            conversationId,
            runId,
            traceId);

        var agent = await _agent.Value.WaitAsync(cancellationToken);

        AgentResponse response = await agent.RunAsync(
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

    private async Task<AIAgent> CreateAgentAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException(
                "Agent:Model is not configured. " +
                "For Foundry Local, run scripts/start-foundry-local.ps1 first.");
        }

        var tools = new List<AITool>();

        if (!_mcpOptions.Enabled || !_mcpOptions.ReplaceBuiltInTools)
        {
            tools.Add(AIFunctionFactory.Create(LookupPolicy));
            tools.Add(AIFunctionFactory.Create(CreateDraftChangeRequest));
        }

        if (_mcpOptions.Enabled)
        {
            var mcpTools = await _mcp.GetAgentToolsAsync(cancellationToken);
            tools.AddRange(mcpTools);
        }

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

    [Description("Returns a governed mock internal policy lookup result.")]
    private ToolExecutionResult LookupPolicy(
        [Description("Policy code, for example AGENT-IDENTITY or TOOL-GOVERNANCE.")]
        string policyCode) =>
        _tools.LookupPolicy(policyCode);

    [Description("Creates a governed mock draft change request. No external system is modified.")]
    private ToolExecutionResult CreateDraftChangeRequest(
        [Description("Short title of the proposed change.")] string title,
        [Description("Why the change is needed.")] string rationale,
        [Description("Optional one-time approval ID when policy requires human approval.")]
        string? approvalId = null) =>
        _tools.CreateDraftChangeRequest(title, rationale, approvalId);
}
