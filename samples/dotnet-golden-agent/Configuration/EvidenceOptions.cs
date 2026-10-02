namespace Agent365.GoldenAgent.Configuration;

public sealed class EvidenceOptions
{
    public const string SectionName = "Evidence";

    public int Capacity { get; set; } = 500;
    public string ChannelName { get; set; } = "web";
    public bool IncludeTraceIdentifiers { get; set; } = true;
}
