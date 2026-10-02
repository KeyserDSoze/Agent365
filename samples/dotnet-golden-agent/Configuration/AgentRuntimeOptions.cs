namespace Agent365.GoldenAgent.Configuration;

public sealed class AgentRuntimeOptions
{
    public const string SectionName = "Agent";

    public string Name { get; set; } = "AGIC-Agent365-GoldenAgent";
    public string Model { get; set; } = "gpt-4o-mini";
    public string AzureOpenAIEndpoint { get; set; } = string.Empty;
    public string Instructions { get; set; } =
        """
        You are the AGIC Agent 365 golden sample.
        Be concise, factual and operational.
        Use the available tools when they materially improve the answer.
        The change-request tool creates a mock draft only: never claim that a real external system was modified.
        """;
}
