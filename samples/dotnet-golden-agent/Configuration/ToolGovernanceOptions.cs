namespace Agent365.GoldenAgent.Configuration;

public sealed class ToolGovernanceOptions
{
    public const string SectionName = "Tools";

    public bool EnablePolicyLookup { get; set; } = true;
    public bool EnableDraftChangeRequest { get; set; } = true;
    public bool RequireApprovalForDraftChangeRequest { get; set; }
    public bool AllowRuntimePolicyChanges { get; set; } = true;
    public int ApprovalTtlMinutes { get; set; } = 5;
    public int AuditCapacity { get; set; } = 500;
}
