namespace Agent365.GoldenAgent.Mcp;

public sealed record McpBridgeStatus(
    bool Enabled,
    bool Required,
    bool Initialized,
    bool Connected,
    bool ReplaceBuiltInTools,
    bool ExposeAdministrativeTools,
    bool ProjectConfigured,
    bool ApprovalRequired,
    bool ApprovalTokenConfigured,
    int DiscoveredToolCount,
    IReadOnlyList<string> ExposedToolNames,
    string? Error);
