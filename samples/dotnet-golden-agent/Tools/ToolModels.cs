namespace Agent365.GoldenAgent.Tools;

public sealed record ToolDescriptor(
    string Name,
    string Description,
    string Operation,
    string RiskTier,
    bool Enabled,
    bool RequiresApproval,
    bool ExternalSideEffect,
    bool Reversible,
    string Owner,
    string Source);

public sealed record ToolAuditRecord(
    DateTimeOffset Timestamp,
    string RunId,
    string ConversationId,
    string TraceId,
    string ToolName,
    string Operation,
    string RiskTier,
    string Decision,
    bool Success,
    bool ExternalSideEffect,
    string? Reason,
    string? ApprovalId,
    double DurationMs);

public sealed record ToolApproval(
    string Id,
    string ToolName,
    string ConversationId,
    string Reason,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt);

public sealed record ToolExecutionResult(
    bool Allowed,
    string Status,
    object? Result,
    string? Reason,
    bool ExternalSideEffect);

public sealed record DraftChangeRequest(
    string Id,
    string State,
    string Title,
    string Rationale,
    bool ExternalSideEffect);

public sealed record ToolStateUpdate(bool Enabled);

public sealed record ToolApprovalRequest(
    string ToolName,
    string ConversationId,
    string Reason);

public sealed record ToolRunStats(
    int Total,
    int Allowed,
    int Denied);
