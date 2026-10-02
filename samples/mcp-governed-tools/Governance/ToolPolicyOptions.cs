namespace Agent365.GovernedMcpServer.Governance;

public sealed record ToolPolicyOptions(
    bool EnablePolicyLookup,
    bool EnableDraftChangeRequest,
    bool RequireApprovalForDraftChangeRequest,
    string ApprovalToken,
    int AuditCapacity)
{
    public static ToolPolicyOptions FromEnvironment() =>
        new(
            EnablePolicyLookup: ReadBool(
                "AGIC_MCP_ENABLE_POLICY_LOOKUP",
                defaultValue: true),
            EnableDraftChangeRequest: ReadBool(
                "AGIC_MCP_ENABLE_DRAFT_CHANGE_REQUEST",
                defaultValue: true),
            RequireApprovalForDraftChangeRequest: ReadBool(
                "AGIC_MCP_REQUIRE_APPROVAL_FOR_DRAFT",
                defaultValue: false),
            ApprovalToken:
                Environment.GetEnvironmentVariable("AGIC_MCP_APPROVAL_TOKEN") ?? string.Empty,
            AuditCapacity: ReadInt(
                "AGIC_MCP_AUDIT_CAPACITY",
                defaultValue: 200,
                minimum: 10,
                maximum: 10_000));

    private static bool ReadBool(
        string name,
        bool defaultValue)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        return bool.TryParse(raw, out var value) ? value : defaultValue;
    }

    private static int ReadInt(
        string name,
        int defaultValue,
        int minimum,
        int maximum)
    {
        var raw = Environment.GetEnvironmentVariable(name);

        if (!int.TryParse(raw, out var value))
        {
            return defaultValue;
        }

        return Math.Clamp(value, minimum, maximum);
    }
}
