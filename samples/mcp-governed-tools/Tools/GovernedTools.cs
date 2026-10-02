using Agent365.GovernedMcpServer.Governance;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Agent365.GovernedMcpServer.Tools;

[McpServerToolType]
public sealed class GovernedTools
{
    [McpServerTool(Name = "get_governance_manifest"), Description(
        "Return the MCP server governance manifest. Read-only. " +
        "Shows tool operation, risk tier, enabled state and approval requirement. " +
        "Never returns the operator approval token.")]
    public static string GetGovernanceManifest(
        ToolGovernanceService governance) =>
        governance.GetManifestJson();

    [McpServerTool(Name = "get_tool_audit_summary"), Description(
        "Return a privacy-safe governance audit summary for this MCP server. Read-only. " +
        "The audit stores decisions and metadata, not full tool arguments.")]
    public static string GetToolAuditSummary(
        ToolGovernanceService governance) =>
        governance.GetAuditSummaryJson();

    [McpServerTool(Name = ToolGovernanceService.PolicyLookupTool), Description(
        "Read a mock internal governance policy. Low-risk, read-only tool. " +
        "No external side effect.")]
    public static string LookupGovernancePolicy(
        ToolGovernanceService governance,
        [Description(
            "Policy code. Examples: AGENT-IDENTITY, TOOL-GOVERNANCE, DATA-GOVERNANCE.")]
        string policyCode) =>
        governance.LookupPolicy(policyCode);

    [McpServerTool(Name = ToolGovernanceService.DraftChangeRequestTool), Description(
        "Create a MOCK draft change request. Medium-risk, write-shaped tool. " +
        "It never modifies an external system. When server policy requires approval, " +
        "approvalToken must be provisioned by a human/operator outside MCP.")]
    public static string CreateDraftChangeRequest(
        ToolGovernanceService governance,
        [Description("Short title of the proposed mock change.")]
        string title,
        [Description("Why the mock change is needed.")]
        string rationale,
        [Description(
            "Optional operator-provisioned approval token. " +
            "The MCP server cannot generate or reveal this token.")]
        string? approvalToken = null) =>
        governance.CreateDraftChangeRequest(
            title,
            rationale,
            approvalToken);
}
