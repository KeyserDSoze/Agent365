namespace Agent365.GoldenAgent.Evidence;

public sealed record RunEvidenceRecord(
    string RunId,
    string CorrelationId,
    string ConversationId,
    string? TraceId,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    double DurationMs,
    string Status,
    string Provider,
    string Model,
    string Channel,
    int RequestCharacters,
    int ResponseCharacters,
    int ToolInvocations,
    int ToolDenied,
    string? ErrorType);

public sealed record RunEvidenceSummary(
    int StoredRuns,
    long TotalRuns,
    long SuccessfulRuns,
    long FailedRuns,
    long ToolInvocations,
    long ToolDenied,
    double AverageDurationMs,
    double SuccessRate);
