namespace Agent365.GoldenAgent.Telemetry;

public sealed record RunEvidenceRecord(
    DateTimeOffset Timestamp,
    string RunId,
    string ConversationId,
    string TraceId,
    string Provider,
    string Model,
    string Status,
    double DurationMs,
    int RequestCharacters,
    int ResponseCharacters,
    int ToolInvocations,
    int ToolAllowed,
    int ToolDenied,
    string? ErrorType);

public sealed record RunEvidenceSummary(
    int BufferedRuns,
    int SuccessfulRuns,
    int FailedRuns,
    double AverageDurationMs,
    int ToolInvocations,
    int ToolAllowed,
    int ToolDenied);

public sealed record RunExecutionResult(
    string RunId,
    string TraceId,
    string Output);
