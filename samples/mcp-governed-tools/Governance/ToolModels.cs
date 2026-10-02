namespace Agent365.GovernedMcpServer.Governance;

public sealed record ToolDescriptor(
    string Name,
    string Operation,
    string RiskTier,
    bool Enabled,
    bool RequiresApproval,
    bool ExternalSideEffect,
    bool Reversible,
    string Owner);

public sealed record ToolAuditRecord(
    DateTimeOffset Timestamp,
    string ToolName,
    string Operation,
    string RiskTier,
    string Decision,
    bool Success,
    bool ExternalSideEffect,
    string? Reason,
    double DurationMs);

public sealed record ToolAuditSummary(
    int BufferedEvents,
    int Allowed,
    int Denied,
    int Failed);

public sealed record ToolExecutionResult(
    bool Allowed,
    string Status,
    object? Result,
    string? Reason,
    bool ExternalSideEffect);
