namespace Agent365.GoldenAgent.Configuration;

public sealed class McpBridgeOptions
{
    public const string SectionName = "Mcp";

    public bool Enabled { get; set; }
    public bool Required { get; set; }
    public bool ReplaceBuiltInTools { get; set; } = true;
    public bool ExposeAdministrativeTools { get; set; }

    public string Command { get; set; } = "dotnet";
    public string ProjectPath { get; set; } =
        "../mcp-governed-tools/Agent365.GovernedMcpServer.csproj";
    public string Configuration { get; set; } = "Release";
    public bool NoBuild { get; set; }
    public int InitializationTimeoutSeconds { get; set; } = 30;
    public int ShutdownTimeoutSeconds { get; set; } = 10;

    public bool EnablePolicyLookup { get; set; } = true;
    public bool EnableDraftChangeRequest { get; set; } = true;
    public bool RequireApprovalForDraft { get; set; }
    public string ApprovalToken { get; set; } = string.Empty;
    public int AuditCapacity { get; set; } = 200;
}
