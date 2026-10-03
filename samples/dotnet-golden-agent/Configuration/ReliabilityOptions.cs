namespace Agent365.GoldenAgent.Configuration;

public sealed class ReliabilityOptions
{
    public const string SectionName = "Reliability";

    public int WindowRuns { get; set; } = 50;
    public int MinimumRuns { get; set; } = 5;

    public double FailureRateWarning { get; set; } = 0.10;
    public double FailureRateCritical { get; set; } = 0.25;

    public double AverageLatencyWarningMs { get; set; } = 2500;
    public double AverageLatencyCriticalMs { get; set; } = 5000;

    public double ToolDenyRateWarning { get; set; } = 0.15;
    public double ToolDenyRateCritical { get; set; } = 0.30;

    public int ConsecutiveFailuresCritical { get; set; } = 3;
}
