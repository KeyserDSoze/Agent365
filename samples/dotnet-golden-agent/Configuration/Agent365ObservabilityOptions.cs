namespace Agent365.GoldenAgent.Configuration;

public sealed class Agent365ObservabilityOptions
{
    public const string SectionName = "Agent365";

    public bool ExportToConsole { get; set; } = true;
    public bool ExportToAgent365 { get; set; }

    // For this custom-engine sample, AgentId is the standard Entra app registration Client ID.
    public string AgentId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    public const string ObservabilityScope =
        "api://9b975845-388f-4429-889e-eab1ef63949c/.default";
}
