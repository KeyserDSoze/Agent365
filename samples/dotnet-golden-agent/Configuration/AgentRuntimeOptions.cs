namespace Agent365.GoldenAgent.Configuration;

public sealed class AgentRuntimeOptions
{
    public const string SectionName = "Agent";

    public string Name { get; set; } = "AGIC-Agent365-GoldenAgent";

    // foundry-local | azure-openai
    public string Provider { get; set; } = "foundry-local";

    // Foundry Local bootstrap resolves the alias to the concrete model ID and sets this value.
    public string Model { get; set; } = string.Empty;

    public string FoundryLocalEndpoint { get; set; } = "http://127.0.0.1:39839/v1";
    public string AzureOpenAIEndpoint { get; set; } = string.Empty;

    public string Instructions { get; set; } =
        """
        You are the AGIC Agent 365 golden sample.
        Be concise, factual and operational.
        Use the available tools when they materially improve the answer.
        The change-request tool creates a mock draft only: never claim that a real external system was modified.
        """;
}
