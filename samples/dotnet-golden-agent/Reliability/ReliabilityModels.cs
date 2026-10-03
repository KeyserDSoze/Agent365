namespace Agent365.GoldenAgent.Reliability;

public sealed record ReliabilityFinding(
    string Code,
    string Severity,
    string Title,
    string Detail,
    double ObservedValue,
    double Threshold,
    string Unit,
    string RecommendedAction,
    IReadOnlyList<string> RunIds);

public sealed record ReliabilityAssessment(
    DateTimeOffset GeneratedAt,
    string Status,
    int? Score,
    int EvaluatedRuns,
    double FailureRate,
    double AverageLatencyMs,
    double ToolDenyRate,
    int ConsecutiveFailures,
    IReadOnlyDictionary<string, int> ErrorTypes,
    IReadOnlyList<ReliabilityFinding> Findings);

public sealed record IncidentSnapshot(
    string Schema,
    DateTimeOffset GeneratedAt,
    bool CapturesContent,
    ReliabilityAssessment Reliability,
    IReadOnlyList<object> SuspectRuns,
    IReadOnlyList<object> ToolDecisions);
