namespace Agent365.GoldenAgent.Configuration;

public sealed class RunEvidenceOptions
{
    public const string SectionName = "Evidence";

    public bool Enabled { get; set; } = true;
    public int Capacity { get; set; } = 500;
}
